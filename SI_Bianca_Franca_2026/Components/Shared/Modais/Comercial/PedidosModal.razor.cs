using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Comercial;
using SI_Bianca_Franca_2026.Models.Comercial;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Comercial;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Services.Produto;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Comercial;

public partial class PedidosModal
{
    [Inject] private PedidosService PedidosService { get; set; } = default!;
    [Inject] private ClientesService ClientesService { get; set; } = default!;
    [Inject] private EmitentesService EmitentesService { get; set; } = default!;
    [Inject] private ProdutosService ProdutosService { get; set; } = default!;
    [Inject] private FichasTecnicasService FichasTecnicasService { get; set; } = default!;
    [Inject] private SkusService SkusService { get; set; } = default!;

    [Parameter] public EventCallback<Pedidos> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;

    private PedidosDTO _dto = new();
    private List<Clientes> _clientes = new();
    private List<Emitentes> _emitentes = new();
    private List<Produtos> _produtosFinais = new();
    private List<Produtos> _materiaisPrimas = new();
    private Dictionary<int, List<Skus>> _skusPorMaterial = new();

    private List<ItemPedidoEdicao> _itens = new();

    public static Dictionary<string, string> StatusDisponiveis => new()
    {
        { "AGUARDANDO_APROVACAO_ARTE", "Aguardando Aprovação de Arte" },
        { "ARTE_APROVADA", "Arte Aprovada" },
        { "EM_PRODUCAO", "Em Produção" },
        { "PRONTO_RETIRADA", "Pronto para Retirada" },
        { "ENVIADO", "Enviado" },
        { "ENTREGUE", "Entregue" },
        { "CANCELADO", "Cancelado" }
    };

    private decimal ValorTotalPedido => _itens.Sum(i => i.ValorTotal);

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new PedidosDTO { Ativo = true, DataPedido = DateTime.Now, Status = "AGUARDANDO_APROVACAO_ARTE" };
        _itens = new();
        await CarregarListas();
        _aberto = true;
        StateHasChanged();
    }

    public async Task AbrirEditar(int id)
    {
        _erro = string.Empty;
        _carregando = true;
        _aberto = true;
        StateHasChanged();

        await CarregarListas();

        var pedido = await PedidosService.Pesquisar(id);
        if (pedido is null)
        {
            _aberto = false;
            StateHasChanged();
            return;
        }

        _dto = new PedidosDTO
        {
            Id = pedido.Id,
            IdCliente = pedido.IdCliente,
            IdEmitente = pedido.IdEmitente,
            DataPedido = pedido.DataPedido,
            DataPrevisaoEntrega = pedido.DataPrevisaoEntrega,
            DataEntrega = pedido.DataEntrega,
            Status = pedido.Status,
            IdVenda = pedido.IdVenda,
            ValorTotal = pedido.ValorTotal,
            Observacao = pedido.Observacao,
            Ativo = pedido.Ativo,
            DataCriacao = pedido.DataCriacao,
            DataUltimaAlteracao = pedido.DataUltimaAlteracao,
            NomeUsuarioAlteracao = pedido.NomeUsuarioAlteracao
        };

        _itens = pedido.Itens.Select(i => new ItemPedidoEdicao
        {
            Id = i.Id,
            IdProduto = i.IdProduto,
            NomeProduto = i.OProduto?.Produto,
            UnidadeMedida = i.OProduto?.OUnidadeMedida?.Sigla,
            DescricaoPersonalizacao = i.DescricaoPersonalizacao,
            Quantidade = i.Quantidade,
            ValorUnitario = i.ValorUnitario,
            ValorDesconto = i.ValorDesconto,
            Materiais = i.Materiais.Select(m => new MaterialPedidoEdicao
            {
                Id = m.Id,
                Sku = m.Sku,
                IdProdutoMaterial = m.OSku?.IdProduto ?? 0,
                NomeProdutoMaterial = m.OSku?.OProduto?.Produto,
                Quantidade = m.Quantidade,
                Observacao = m.Observacao,
                VeioDaFicha = true
            }).ToList()
        }).ToList();

        foreach (var item in _itens)
            foreach (var material in item.Materiais)
                await GarantirSkusCarregados(material.IdProdutoMaterial);

        _carregando = false;
        StateHasChanged();
    }

    private async Task CarregarListas()
    {
        var clientes = await ClientesService.ListarTudo();
        _clientes = clientes.Where(c => c.Ativo).ToList();

        var emitentes = await EmitentesService.ListarTudo();
        _emitentes = emitentes.Where(e => e.Ativo).ToList();

        var produtos = await ProdutosService.ListarPorTipo("PRODUTO_FINAL");
        _produtosFinais = produtos.Where(p => p.Ativo).ToList();

        var materiais = await ProdutosService.ListarPorTipo("MATERIA_PRIMA");
        _materiaisPrimas = materiais.Where(m => m.Ativo).ToList();
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private void AdicionarItem() => _itens.Add(new ItemPedidoEdicao());

    private void RemoverItem(int idx) => _itens.RemoveAt(idx);

    private async Task AoTrocarProduto(ItemPedidoEdicao item, string? texto)
    {
        if (!int.TryParse(texto, out var idProduto)) return;

        item.IdProduto = idProduto;
        var produto = _produtosFinais.FirstOrDefault(p => p.Id == idProduto);
        item.NomeProduto = produto?.Produto;
        item.UnidadeMedida = produto?.OUnidadeMedida?.Sigla;

        item.Materiais = new();

        if (idProduto > 0)
        {
            var fichas = await FichasTecnicasService.ListarPorProduto(idProduto);
            foreach (var ficha in fichas)
            {
                item.Materiais.Add(new MaterialPedidoEdicao
                {
                    IdProdutoMaterial = ficha.IdProdutoMaterial,
                    NomeProdutoMaterial = ficha.OProdutoMaterial?.Produto,
                    Quantidade = ficha.Quantidade * (item.Quantidade > 0 ? item.Quantidade : 1),
                    VeioDaFicha = true
                });
                await GarantirSkusCarregados(ficha.IdProdutoMaterial);
            }
        }

        StateHasChanged();
    }

    private void AdicionarMaterialManual(ItemPedidoEdicao item) =>
        item.Materiais.Add(new MaterialPedidoEdicao { VeioDaFicha = false });

    private void RemoverMaterial(ItemPedidoEdicao item, MaterialPedidoEdicao material) =>
        item.Materiais.Remove(material);

    private async Task AoTrocarMaterialGenerico(MaterialPedidoEdicao material, string? texto)
    {
        if (!int.TryParse(texto, out var idProduto)) return;

        material.IdProdutoMaterial = idProduto;
        material.NomeProdutoMaterial = _materiaisPrimas.FirstOrDefault(m => m.Id == idProduto)?.Produto;
        material.Sku = string.Empty;

        await GarantirSkusCarregados(idProduto);
        StateHasChanged();
    }

    private async Task GarantirSkusCarregados(int idProdutoMaterial)
    {
        if (idProdutoMaterial == 0 || _skusPorMaterial.ContainsKey(idProdutoMaterial))
            return;

        var skus = await SkusService.ListarPorProduto(idProdutoMaterial);
        _skusPorMaterial[idProdutoMaterial] = skus.Where(s => s.Ativo).ToList();
    }

    private List<Skus> ObterSkusDoMaterial(int idProdutoMaterial) =>
        _skusPorMaterial.TryGetValue(idProdutoMaterial, out var skus) ? skus : new();

    private void AtualizarQuantidadeItem(ItemPedidoEdicao item, string? texto)
    {
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            item.Quantidade = r;
    }

    private void AtualizarValorUnitario(ItemPedidoEdicao item, string? texto)
    {
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            item.ValorUnitario = r;
    }

    private void AtualizarDesconto(ItemPedidoEdicao item, string? texto)
    {
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            item.ValorDesconto = r;
    }

    private void AtualizarQuantidadeMaterial(MaterialPedidoEdicao material, string? texto)
    {
        if (decimal.TryParse(texto, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.GetCultureInfo("pt-BR"), out var r))
            material.Quantidade = r;
    }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;

            if (!_itens.Any())
            {
                _erro = "O pedido deve ter ao menos um item.";
                return;
            }

            if (_itens.Any(i => i.IdProduto == 0))
            {
                _erro = "Todos os itens precisam de um produto selecionado.";
                return;
            }

            if (_itens.Any(i => i.Materiais.Any(m => string.IsNullOrWhiteSpace(m.Sku))))
            {
                _erro = "Todos os materiais precisam de um SKU selecionado.";
                return;
            }

            var pedido = new Pedidos
            {
                Id = _dto.Id,
                IdCliente = _dto.IdCliente,
                IdEmitente = _dto.IdEmitente,
                DataPedido = _dto.DataPedido,
                DataPrevisaoEntrega = _dto.DataPrevisaoEntrega,
                DataEntrega = _dto.DataEntrega,
                Status = _dto.Status,
                IdVenda = _dto.IdVenda,
                Observacao = _dto.Observacao,
                Ativo = _dto.Ativo,
                DataCriacao = _dto.DataCriacao
            };

            var itensModel = _itens.Select(i => new PedidosItens
            {
                Id = i.Id,
                IdProduto = i.IdProduto,
                DescricaoPersonalizacao = i.DescricaoPersonalizacao,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                ValorDesconto = i.ValorDesconto,
                Materiais = i.Materiais.Select(m => new PedidosFichasTecnicas
                {
                    Id = m.Id,
                    Sku = m.Sku,
                    Quantidade = m.Quantidade,
                    Observacao = m.Observacao
                }).ToList()
            }).ToList();

            if (pedido.Id == 0)
            {
                pedido.Id = await PedidosService.Inserir(pedido, itensModel);
            }
            else
            {
                await PedidosService.Atualizar(pedido, itensModel);
            }

            await AoSalvarComSucesso.InvokeAsync(pedido);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}