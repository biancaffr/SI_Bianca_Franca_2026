using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;
using System.Globalization;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

public partial class ProdutosModal
{
    [Inject] private ProdutosService ProdutosService { get; set; } = default!;
    [Inject] private CategoriasService CategoriasService { get; set; } = default!;
    [Inject] private UnidadesMedidaService UnidadesMedidaService { get; set; } = default!;

    [Parameter] public EventCallback<Produtos> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback<int> AoExcluirComSucesso { get; set; }

    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;


    private ProdutosDTO _dto = new();
    private List<Categorias> _categorias = new();
    private List<UnidadesMedida> _unidades = new();

    private CategoriasModal _modalCategoria = default!;
    private UnidadesMedidaModal _modalUnidade = default!;
    private ModalConfirmacao _modalConfirmacao = default!;

    public static Dictionary<byte, string> OrigensDisponiveis => new()
    {
        { 0, "0 — Nacional" },
        { 1, "1 — Estrangeira (importação direta)" },
        { 2, "2 — Estrangeira (adquirida no mercado interno)" },
        { 3, "3 — Nacional com conteúdo superior a 40%" },
        { 4, "4 — Nacional (produção conforme processos básicos)" },
        { 5, "5 — Nacional com conteúdo inferior a 40%" },
        { 6, "6 — Estrangeira (importação direta, sem similar nacional)" },
        { 7, "7 — Estrangeira (adquirida no mercado interno, sem similar nacional)" },
        { 8, "8 — Nacional com conteúdo superior a 70%" }
    };

    private string SaldoTexto
    {
        get => _dto.Saldo == 0 ? string.Empty : _dto.Saldo.ToString("0.####", CultureInfo.GetCultureInfo("pt-BR"));
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _dto.Saldo = 0;
            else if (decimal.TryParse(value, NumberStyles.Any,
                CultureInfo.GetCultureInfo("pt-BR"), out var r))
                _dto.Saldo = r;
        }
    }

    private string EstoqueMinimoTexto
    {
        get => _dto.EstoqueMinimo == 0 ? string.Empty : _dto.EstoqueMinimo.ToString("0.####", CultureInfo.GetCultureInfo("pt-BR"));
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _dto.EstoqueMinimo = 0;
            else if (decimal.TryParse(value, NumberStyles.Any,
                CultureInfo.GetCultureInfo("pt-BR"), out var r))
                _dto.EstoqueMinimo = r;
        }
    }

    private string PrecoCustoTexto
    {
        get => _dto.PrecoCusto?.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) ?? "";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _dto.PrecoCusto = null;
            else if (decimal.TryParse(value, NumberStyles.Any,
                CultureInfo.GetCultureInfo("pt-BR"), out var r))
                _dto.PrecoCusto = r;
        }
    }

    private string PrecoVendaTexto
    {
        get => _dto.PrecoVenda?.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) ?? "";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _dto.PrecoVenda = null;
            else if (decimal.TryParse(value, NumberStyles.Any,
                CultureInfo.GetCultureInfo("pt-BR"), out var r))
                _dto.PrecoVenda = r;
        }
    }

    private string ValorUltimaCompraTexto
    {
        get => _dto.ValorUltimaCompra?.ToString("N2", CultureInfo.GetCultureInfo("pt-BR")) ?? "";
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                _dto.ValorUltimaCompra = null;
            else if (decimal.TryParse(value, NumberStyles.Any,
                CultureInfo.GetCultureInfo("pt-BR"), out var r))
                _dto.ValorUltimaCompra = r;
        }
    }

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new ProdutosDTO { Ativo = true };
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

        var produto = await ProdutosService.Pesquisar(id);
        if (produto is null)
        {
            _aberto = false;
            StateHasChanged();
            return;
        }

        _dto = new ProdutosDTO
        {
            Id = produto.Id,
            Produto = produto.Produto,
            Referencia = produto.Referencia,
            Saldo = produto.Saldo,
            PrecoCusto = produto.PrecoCusto,
            PrecoVenda = produto.PrecoVenda,
            EstoqueMinimo = produto.EstoqueMinimo,
            ValorUltimaCompra = produto.ValorUltimaCompra,
            Tipo = produto.Tipo,
            IdCategoria = produto.IdCategoria,
            IdUnidadeMedida = produto.IdUnidadeMedida,
            Ncm = produto.Ncm,
            Cest = produto.Cest,
            Origem = produto.Origem,
            Observacao = produto.Observacao,
            Ativo = produto.Ativo,
            DataCriacao = produto.DataCriacao,
            DataUltimaAlteracao = produto.DataUltimaAlteracao,
            NomeUsuarioAlteracao = produto.NomeUsuarioAlteracao
        };

        _carregando = false;
        StateHasChanged();
    }

    private async Task CarregarListas()
    {
        _categorias = await CategoriasService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _unidades = await UnidadesMedidaService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;

            if (_dto.PrecoCusto > 99999.99m)
            {
                _erro = "Preço de Custo: valor máximo permitido é 99.999,99.";
                return;
            }
            if (_dto.PrecoVenda > 99999.99m)
            {
                _erro = "Preço de Venda: valor máximo permitido é 99.999,99.";
                return;
            }
            if (_dto.ValorUltimaCompra > 99999.99m)
            {
                _erro = "Valor da Última Compra: valor máximo permitido é 99.999,99.";
                return;
            }
            if (_dto.Saldo > 999999.9999m)
            {
                _erro = "Saldo: valor máximo permitido é 999.999,9999.";
                return;
            }
            if (_dto.EstoqueMinimo > 999999.9999m)
            {
                _erro = "Estoque Mínimo: valor máximo permitido é 999.999,9999.";
                return;
            }

            var produto = new Produtos
            {
                Id = _dto.Id,
                Produto = _dto.Produto,
                Referencia = _dto.Referencia,
                Saldo = _dto.Saldo,
                PrecoCusto = _dto.PrecoCusto,
                PrecoVenda = _dto.PrecoVenda,
                EstoqueMinimo = _dto.EstoqueMinimo,
                ValorUltimaCompra = _dto.ValorUltimaCompra,
                Tipo = _dto.Tipo,
                IdCategoria = _dto.IdCategoria,
                IdUnidadeMedida = _dto.IdUnidadeMedida,
                Ncm = _dto.Ncm,
                Cest = _dto.Cest,
                Origem = _dto.Origem,
                Observacao = _dto.Observacao,
                Ativo = _dto.Ativo,
                DataCriacao = _dto.DataCriacao
            };

            if (produto.Id == 0)
            {
                var id = await ProdutosService.Inserir(produto);
                produto.Id = id;
            }
            else
            {
                await ProdutosService.Atualizar(produto);
            }

            await AoSalvarComSucesso.InvokeAsync(produto);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private void AbrirModalNovaCategoria() => _modalCategoria.Abrir();

    private async Task OnCategoriaSalva(Categorias categoria)
    {
        _categorias = await CategoriasService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _dto.IdCategoria = categoria.Id;
    }

    private void AbrirModalNovaUnidade() => _modalUnidade.Abrir();
    private async Task OnUnidadeSalva(UnidadesMedida unidade)
    {
        _unidades = await UnidadesMedidaService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _dto.IdUnidadeMedida = unidade.Id;
    }

    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir($"Deseja realmente excluir o produto \"{_dto.Produto}\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await ProdutosService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}