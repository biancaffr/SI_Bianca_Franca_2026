using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Financeiro;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.NfeEntrada;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.NfeEntrada;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Financeiro;
using SI_Bianca_Franca_2026.Services.NfeEntrada;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Services.Produto;
using System.Globalization;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.NfeEntrada;

public partial class NfeEntradaModal
{
    [Inject] private NfeEntradaService NfeEntradaService { get; set; } = default!;
    [Inject] private FornecedoresService FornecedoresService { get; set; } = default!;
    [Inject] private TransportadorasService TransportadorasService { get; set; } = default!;
    [Inject] private ProdutosService ProdutosService { get; set; } = default!;
    [Inject] private CondicoesPagamentosService CondicoesPagamentosService { get; set; } = default!;

    [Parameter] public EventCallback<NfeEntradas> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback AoCancelarComSucesso { get; set; }

    private bool _aberto;
    private bool _carregando;
    private bool _visualizando;
    private string _erro = string.Empty;

    private string _statusChave = "incompleta";
    private bool _chaveBloqueada;
    private bool _datasValidas;
    private string _erroDatas = string.Empty;

    private bool _parcelasGeradas;
    private bool _confirmandoGerarParcelas;

    private bool _confirmandoGerarNota;

    private bool _pedindoMotivo;
    private string _motivoCancelamento = string.Empty;
    private string _erroMotivo = string.Empty;

    private NfeEntradaDTO _dto = new();
    private List<Fornecedores> _fornecedores = new();
    private List<Transportadoras> _transportadoras = new();
    private List<Produtos> _produtos = new();
    private List<CondicoesPagamentos> _condicoes = new();

    private FornecedoresModal _modalFornecedor = default!;
    private TransportadorasModal _modalTransportadora = default!;
    private ProdutosModal _modalProduto = default!;
    private CondicoesPagamentosModal _modalCondicao = default!;

    private static readonly CultureInfo _ptBR = CultureInfo.GetCultureInfo("pt-BR");

    private bool CamposBloqueados => _statusChave != "disponivel" || !_datasValidas;
    private bool ProdutosBloqueados => _parcelasGeradas;

    private decimal TotalProdutos => _dto.Itens.Sum(i => i.ValorTotal);
    private decimal TotalNota => TotalProdutos + _dto.ValorFrete
        + _dto.ValorSeguro + _dto.ValorOutrasDespesas - _dto.ValorDesconto;

    // ── Abertura ──────────────────────────────────────────

    public async Task AbrirNovo()
    {
        _dto = new NfeEntradaDTO();
        _dto.Itens.Add(new NfeEntradaItensDTO { Sequencia = 1 });
        _statusChave = "incompleta";
        _chaveBloqueada = false;
        _datasValidas = false;
        _erroDatas = string.Empty;
        _erro = string.Empty;
        _parcelasGeradas = false;
        _confirmandoGerarParcelas = false;
        _visualizando = false;
        await CarregarListas();
        _aberto = true;
        StateHasChanged();
    }

    public async Task AbrirVisualizar(string numero, string modelo, string serie, int idFornecedor)
    {
        _carregando = true;
        _visualizando = true;
        _aberto = true;
        StateHasChanged();

        var nota = await NfeEntradaService.Pesquisar(numero, modelo, serie, idFornecedor);
        if (nota is null) { _aberto = false; StateHasChanged(); return; }

        _dto = new NfeEntradaDTO
        {
            Numero = nota.Numero,
            Modelo = nota.Modelo,
            Serie = nota.Serie,
            IdFornecedor = nota.IdFornecedor,
            NomeFornecedor = nota.OFornecedor?.NomeRazaoSocial ?? string.Empty,
            NomeFornecedorCompleto = $"#{nota.IdFornecedor} — {nota.OFornecedor?.CpfCnpj} — {nota.OFornecedor?.NomeRazaoSocial}",
            IdTransportadora = nota.IdTransportadora,
            NomeTransportadora = nota.OTransportadora?.NomeRazaoSocial,
            IdCondicaoPagamento = nota.IdCondicaoPagamento,
            NomeCondicaoPagamento = nota.OCondicaoPagamento?.CondicaoPagamento,
            DataEmissao = nota.DataEmissao,
            DataChegada = nota.DataChegada,
            TipoFrete = nota.TipoFrete,
            ValorProdutos = nota.ValorProdutos,
            ValorFrete = nota.ValorFrete,
            ValorSeguro = nota.ValorSeguro,
            ValorOutrasDespesas = nota.ValorOutrasDespesas,
            ValorDesconto = nota.ValorDesconto,
            ValorTotal = nota.ValorTotal,
            Observacao = nota.Observacao,
            Situacao = nota.Situacao,
            Ativo = nota.Ativo,
            DataCriacao = nota.DataCriacao,
            DataUltimaAlteracao = nota.DataUltimaAlteracao,
            NomeUsuarioAlteracao = nota.NomeUsuarioAlteracao,
            Itens = nota.Itens.Select((i, idx) => new NfeEntradaItensDTO
            {
                IdProduto = i.IdProduto,
                NomeProduto = i.OProduto?.Produto ?? string.Empty,
                SiglaProduto = i.OProduto?.OUnidadeMedida?.Sigla ?? string.Empty,
                Sequencia = i.Sequencia,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                PercentualDesconto = i.PercentualDesconto,
                ValorDesconto = i.ValorDesconto,
                CustoFinal = i.CustoFinal ?? 0,
                CustoMedio = i.CustoMedio ?? 0
            }).ToList()
        };

        if (nota.IdCondicaoPagamento.HasValue && nota.DataEmissao != default)
            _dto.Parcelas = await NfeEntradaService.GerarParcelasVisualizacao(
                nota.IdCondicaoPagamento.Value, nota.ValorTotal, nota.DataEmissao);

        _carregando = false;
        StateHasChanged();
    }

    // ── Lógica de bloqueio ────────────────────────────────

    private bool TemProdutoSelecionado()
        => _dto.Itens.Any(i => i.IdProduto > 0);

    private void AtualizarBloqueioChave()
    {
        _chaveBloqueada = TemProdutoSelecionado();
    }

    // ── Chave ─────────────────────────────────────────────

    private async Task FiltrarApenasNumeros(ChangeEventArgs e)
    {
        var valor = e.Value?.ToString() ?? string.Empty;
        _dto.Numero = new string(valor.Where(char.IsDigit).Take(9).ToArray());
        await VerificarChave();
    }

    private async Task OnModeloAlterado(ChangeEventArgs e)
    {
        _dto.Modelo = e.Value?.ToString() ?? string.Empty;
        await VerificarChave();
    }

    private async Task OnSerieAlterada(ChangeEventArgs e)
    {
        _dto.Serie = e.Value?.ToString() ?? string.Empty;
        await VerificarChave();
    }

    private async Task OnFornecedorAlterado(ChangeEventArgs e)
    {
        _dto.IdFornecedor = int.TryParse(e.Value?.ToString(), out var r) ? r : 0;
        _dto.Parcelas = new();
        _parcelasGeradas = false;
        await VerificarChave();
    }

    private async Task VerificarChave()
    {
        if (string.IsNullOrWhiteSpace(_dto.Numero) ||
            string.IsNullOrWhiteSpace(_dto.Modelo) ||
            string.IsNullOrWhiteSpace(_dto.Serie) ||
            _dto.IdFornecedor == 0)
        {
            _statusChave = "incompleta";
            _datasValidas = false;
            StateHasChanged();
            return;
        }

        _statusChave = "verificando";
        StateHasChanged();

        var existe = await NfeEntradaService.ChaveExiste(
            _dto.Numero, _dto.Modelo, _dto.Serie, _dto.IdFornecedor);

        _statusChave = existe ? "duplicada" : "disponivel";
        StateHasChanged();
    }

    // ── Datas ─────────────────────────────────────────────

    private void OnDataEmissaoAlterada(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var data))
            _dto.DataEmissao = data;
        else
            _dto.DataEmissao = null;
        _dto.DataChegada = null;
        _dto.Parcelas = new();
        _parcelasGeradas = false;
        ValidarDatas();
        StateHasChanged();
    }

    private void OnDataChegadaAlterada(ChangeEventArgs e)
    {
        if (DateTime.TryParse(e.Value?.ToString(), out var data))
            _dto.DataChegada = data;
        else
            _dto.DataChegada = null;
        _dto.Parcelas = new();
        _parcelasGeradas = false;
        ValidarDatas();
        StateHasChanged();
    }

    private void ValidarDatas()
    {
        _erroDatas = string.Empty;
        _datasValidas = false;

        var hoje = DateTime.Today;

        if (_dto.DataEmissao is null) { _erroDatas = "Informe a data de emissão."; return; }
        if (_dto.DataEmissao > hoje) { _erroDatas = "A data de emissão não pode ser futura."; return; }
        if (_dto.DataChegada is null) { _erroDatas = "Informe a data de chegada."; return; }
        if (_dto.DataChegada < _dto.DataEmissao) { _erroDatas = "A data de chegada não pode ser anterior à data de emissão."; return; }
        if (_dto.DataChegada > hoje) { _erroDatas = "A data de chegada não pode ser futura."; return; }

        _datasValidas = true;
    }

    // ── Itens ─────────────────────────────────────────────

    private void AdicionarItem()
    {
        _dto.Itens.Add(new NfeEntradaItensDTO { Sequencia = (short)(_dto.Itens.Count + 1) });
        StateHasChanged();
    }

    private void RemoverItem(NfeEntradaItensDTO item)
    {
        _dto.Itens.Remove(item);
        if (!TemProdutoSelecionado())
            _chaveBloqueada = false;
        for (short i = 0; i < _dto.Itens.Count; i++)
            _dto.Itens[i].Sequencia = (short)(i + 1);
        RecalcularCustoFinal();
        StateHasChanged();
    }

    private void AtualizarProdutoItem(NfeEntradaItensDTO item, string? valor)
    {
        if (!int.TryParse(valor, out var id)) return;
        item.IdProduto = id;
        var produto = _produtos.FirstOrDefault(p => p.Id == id);
        item.NomeProduto = produto?.Produto ?? string.Empty;
        item.SiglaProduto = produto?.OUnidadeMedida?.Sigla ?? string.Empty;
        AtualizarBloqueioChave();
        RecalcularCustoFinal();
        StateHasChanged();
    }

    private void AtualizarDecimalItem(ChangeEventArgs e, Action<decimal> setter)
    {
        if (decimal.TryParse(e.Value?.ToString(), NumberStyles.Any, _ptBR, out var r) && r >= 0)
            setter(r);
        RecalcularCustoFinal();
        StateHasChanged();
    }

    // Incrementado quando o texto digitado no desconto é vazio/inválido, para o campo
    // ser recriado e voltar a mostrar 0 (o Blazor não redesenha se o valor do modelo não mudou)
    private int _versaoDesconto;

    private void AtualizarPercentualDesconto(NfeEntradaItensDTO item, string? texto)
    {
        if (!decimal.TryParse(texto, NumberStyles.Any, _ptBR, out var pct))
        {
            pct = 0;
            _versaoDesconto++;
        }
        pct = Math.Max(0, Math.Min(100, pct));
        item.PercentualDesconto = pct;
        item.ValorDesconto = Math.Round(item.ValorUnitario * pct / 100, 4); // por unidade
        RecalcularCustoFinal();
        StateHasChanged();
    }

    private void AtualizarValorDesconto(NfeEntradaItensDTO item, string? texto)
    {
        if (!decimal.TryParse(texto, NumberStyles.Any, _ptBR, out var val))
        {
            val = 0;
            _versaoDesconto++;
        }
        val = Math.Max(0, val);
        val = Math.Min(val, item.ValorUnitario); // desconto por unidade não pode passar do valor unitário
        item.ValorDesconto = val;
        item.PercentualDesconto = item.ValorUnitario > 0 ? Math.Round(val / item.ValorUnitario * 100, 2) : 0;
        RecalcularCustoFinal();
        StateHasChanged();
    }

    private void AtualizarDecimal(ChangeEventArgs e, Action<decimal> setter)
    {
        if (decimal.TryParse(e.Value?.ToString(), NumberStyles.Any, _ptBR, out var r) && r >= 0)
            setter(r);
        RecalcularCustoFinal();
        StateHasChanged();
    }

    private void RecalcularCustoFinal()
    {
        var totalItens = _dto.Itens.Sum(i => i.ValorTotal);
        foreach (var item in _dto.Itens)
        {
            var proporcao = totalItens > 0 ? item.ValorTotal / totalItens : 0;
            var rateioFrete = Math.Round(_dto.ValorFrete * proporcao, 4);
            var rateioSeguro = Math.Round(_dto.ValorSeguro * proporcao, 4);
            var rateioOutras = Math.Round(_dto.ValorOutrasDespesas * proporcao, 4);
            item.CustoFinal = item.Quantidade > 0
                ? Math.Round((item.ValorTotal + rateioFrete + rateioSeguro + rateioOutras)
                    / item.Quantidade, 4)
                : 0;
        }
        RecalcularCustoMedio();
    }

    // Custo médio ponderado usando saldo e custo atuais do produto (mesmo cálculo do Service)
    private void RecalcularCustoMedio()
    {
        var estado = new Dictionary<int, (decimal Saldo, decimal Custo)>();
        foreach (var item in _dto.Itens)
        {
            if (item.IdProduto <= 0 || item.Quantidade <= 0) { item.CustoMedio = 0; continue; }
            if (!estado.TryGetValue(item.IdProduto, out var e))
            {
                var p = _produtos.FirstOrDefault(x => x.Id == item.IdProduto);
                e = (p?.Saldo ?? 0, p?.PrecoCusto ?? 0);
            }
            item.CustoMedio = NfeEntradaService.CalcularCustoMedio(
                e.Saldo, e.Custo, item.Quantidade, item.CustoFinal);
            estado[item.IdProduto] = (e.Saldo + item.Quantidade, Math.Round(item.CustoMedio, 2));
        }
    }

    // ── Frete ─────────────────────────────────────────────

    private void OnTipoFreteAlterado(ChangeEventArgs e)
    {
        _dto.TipoFrete = e.Value?.ToString() ?? "0";
        if (_dto.TipoFrete == "9")
        {
            _dto.IdTransportadora = null;
            _dto.ValorFrete = 0;
            _dto.ValorSeguro = 0;
            _dto.ValorOutrasDespesas = 0;
        }
        RecalcularCustoFinal();
        StateHasChanged();
    }

    // ── Condição de pagamento / Parcelas ──────────────────

    private void OnCondicaoAlterada(ChangeEventArgs e)
    {
        _dto.IdCondicaoPagamento = int.TryParse(e.Value?.ToString(), out var r) && r > 0 ? r : null;
        _dto.Parcelas = new();
        _parcelasGeradas = false;
        StateHasChanged();
    }

    private void PedirConfirmacaoGerarParcelas()
    {
        _erro = string.Empty;

        if (!_dto.Itens.Any(i => i.IdProduto > 0))
        { _erro = "Adicione ao menos um produto à nota antes de gerar as parcelas."; return; }
        if (string.IsNullOrEmpty(_dto.TipoFrete))
        { _erro = "Selecione o tipo de frete antes de gerar as parcelas."; return; }
        if (!_dto.IdCondicaoPagamento.HasValue)
        { _erro = "Selecione uma condição de pagamento antes de gerar as parcelas."; return; }

        _confirmandoGerarParcelas = true;
        StateHasChanged();
    }

    private async Task ConfirmarGerarParcelas()
    {
        _confirmandoGerarParcelas = false;
        _dto.Parcelas = await NfeEntradaService.GerarParcelasVisualizacao(
            _dto.IdCondicaoPagamento!.Value, TotalNota, _dto.DataEmissao!.Value);
        _parcelasGeradas = true;
        StateHasChanged();
    }

    private void CancelarGerarParcelas()
    {
        _confirmandoGerarParcelas = false;
        StateHasChanged();
    }

    // ── Salvar ────────────────────────────────────────────

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;

            if (_statusChave != "disponivel")
            { _erro = "Verifique a chave da nota antes de gerar."; return; }
            if (!_datasValidas)
            { _erro = "Verifique as datas antes de gerar."; return; }
            if (!_dto.Itens.Any(i => i.IdProduto > 0))
            { _erro = "Adicione ao menos um produto à nota."; return; }
            if (_dto.Itens.Any(i => i.IdProduto > 0 && i.Quantidade <= 0))
            { _erro = "A quantidade deve ser maior que zero em todos os itens."; return; }
            if (!_dto.IdCondicaoPagamento.HasValue)
            { _erro = "Selecione uma condição de pagamento antes de gerar a nota."; return; }
            if (!_parcelasGeradas || !_dto.Parcelas.Any())
            { _erro = "Gere as parcelas antes de finalizar a nota."; return; }

            var nota = new NfeEntradas
            {
                Numero = _dto.Numero,
                Modelo = _dto.Modelo,
                Serie = _dto.Serie,
                IdFornecedor = _dto.IdFornecedor,
                IdTransportadora = _dto.IdTransportadora,
                IdCondicaoPagamento = _dto.IdCondicaoPagamento,
                DataEmissao = _dto.DataEmissao!.Value,
                DataChegada = _dto.DataChegada,
                TipoFrete = _dto.TipoFrete,
                ValorFrete = _dto.ValorFrete,
                ValorSeguro = _dto.ValorSeguro,
                ValorOutrasDespesas = _dto.ValorOutrasDespesas,
                ValorDesconto = _dto.ValorDesconto,
                Observacao = _dto.Observacao
            };

            var itens = _dto.Itens
                .Where(i => i.IdProduto > 0)
                .Select(i => new NfeEntradaItens
                {
                    IdProduto = i.IdProduto,
                    Sequencia = i.Sequencia,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    PercentualDesconto = i.PercentualDesconto,
                    ValorDesconto = i.ValorDesconto,
                    ValorTotal = i.ValorTotal
                }).ToList();

            await NfeEntradaService.Inserir(nota, itens);
            await AoSalvarComSucesso.InvokeAsync(nota);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    // ── Cancelamento ──────────────────────────────────────

    private void PedirMotivoCancelamento()
    {
        _motivoCancelamento = string.Empty;
        _erroMotivo = string.Empty;
        _pedindoMotivo = true;
        StateHasChanged();
    }

    private async Task ConfirmarCancelamento()
    {
        if (string.IsNullOrWhiteSpace(_motivoCancelamento))
        {
            _erroMotivo = "Informe o motivo do cancelamento.";
            StateHasChanged();
            return;
        }
        try
        {
            await NfeEntradaService.Cancelar(
                _dto.Numero, _dto.Modelo, _dto.Serie, _dto.IdFornecedor);
            _pedindoMotivo = false;
            await AoCancelarComSucesso.InvokeAsync();
            Fechar();
        }
        catch (Exception ex)
        {
            _erroMotivo = ex.Message;
            StateHasChanged();
        }
    }

    // ── Listas e callbacks ────────────────────────────────

    private async Task CarregarListas()
    {
        _fornecedores = await FornecedoresService.ListarTudo(
            new FiltroPessoasDto { StatusFiltro = "ativo" });
        _transportadoras = await TransportadorasService.ListarTudo(
            new FiltroPessoasDto { StatusFiltro = "ativo" });
        _produtos = await ProdutosService.ListarTudo(
            new FiltroProdutosDto { StatusFiltro = "ativo" });
        _condicoes = await CondicoesPagamentosService.ListarTudo(
            new FiltroBaseDto { StatusFiltro = "ativo" });
    }

    private async Task OnFornecedorSalvo(Fornecedores _)
    {
        _fornecedores = await FornecedoresService.ListarTudo(
            new FiltroPessoasDto { StatusFiltro = "ativo" });
        StateHasChanged();
    }

    private async Task OnTransportadoraSalva(Transportadoras _)
    {
        _transportadoras = await TransportadorasService.ListarTudo(
            new FiltroPessoasDto { StatusFiltro = "ativo" });
        StateHasChanged();
    }

    private async Task OnProdutoSalvo(Produtos _)
    {
        _produtos = await ProdutosService.ListarTudo(
            new FiltroProdutosDto { StatusFiltro = "ativo" });
        StateHasChanged();
    }

    private async Task OnCondicaoSalva(CondicoesPagamentos _)
    {
        _condicoes = await CondicoesPagamentosService.ListarTudo(
            new FiltroBaseDto { StatusFiltro = "ativo" });
        StateHasChanged();
    }

    private void AbrirModalFornecedor() => _modalFornecedor.AbrirNovo();
    private void AbrirModalTransportadora() => _modalTransportadora.AbrirNovo();
    private void AbrirModalProduto() => _modalProduto.AbrirNovo();
    private void AbrirModalCondicao() => _modalCondicao.AbrirNovo();

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private static string FormatarSituacao(string s) => s switch
    {
        "PENDENTE" => "Pendente",
        "CONFIRMADA" => "Confirmada",
        "CANCELADA" => "Cancelada",
        _ => s
    };

    private static string LabelTipoFrete(string t) => t switch
    {
        "0" => "0 – CIF (Remetente)",
        "1" => "1 – FOB (Destinatário)",
        "9" => "9 – Sem ocorrência de transporte",
        _ => t
    };

    private void PedirConfirmacaoGerarNota()
    {
        _erro = string.Empty;

        if (_statusChave != "disponivel")
        { _erro = "Verifique a chave da nota antes de gerar."; return; }
        if (!_datasValidas)
        { _erro = "Verifique as datas antes de gerar."; return; }
        if (!_dto.Itens.Any(i => i.IdProduto > 0))
        { _erro = "Adicione ao menos um produto à nota."; return; }
        if (_dto.Itens.Any(i => i.IdProduto > 0 && i.Quantidade <= 0))
        { _erro = "A quantidade deve ser maior que zero em todos os itens."; return; }
        if (!_dto.IdCondicaoPagamento.HasValue)
        { _erro = "Selecione uma condição de pagamento antes de gerar a nota."; return; }
        if (!_parcelasGeradas || !_dto.Parcelas.Any())
        { _erro = "Gere as parcelas antes de finalizar a nota."; return; }

        _confirmandoGerarNota = true;
        StateHasChanged();
    }

    private async Task ConfirmarGerarNota()
    {
        _confirmandoGerarNota = false;
        try
        {
            var nota = new NfeEntradas
            {
                Numero = _dto.Numero,
                Modelo = _dto.Modelo,
                Serie = _dto.Serie,
                IdFornecedor = _dto.IdFornecedor,
                IdTransportadora = _dto.IdTransportadora,
                IdCondicaoPagamento = _dto.IdCondicaoPagamento,
                DataEmissao = _dto.DataEmissao!.Value,
                DataChegada = _dto.DataChegada,
                TipoFrete = _dto.TipoFrete,
                ValorFrete = _dto.ValorFrete,
                ValorSeguro = _dto.ValorSeguro,
                ValorOutrasDespesas = _dto.ValorOutrasDespesas,
                ValorDesconto = _dto.ValorDesconto,
                Observacao = _dto.Observacao
            };

            var itens = _dto.Itens
                .Where(i => i.IdProduto > 0)
                .Select(i => new NfeEntradaItens
                {
                    IdProduto = i.IdProduto,
                    Sequencia = i.Sequencia,
                    Quantidade = i.Quantidade,
                    ValorUnitario = i.ValorUnitario,
                    PercentualDesconto = i.PercentualDesconto,
                    ValorDesconto = i.ValorDesconto,
                    ValorTotal = i.ValorTotal
                }).ToList();

            await NfeEntradaService.Inserir(nota, itens);
            await AoSalvarComSucesso.InvokeAsync(nota);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
            StateHasChanged();
        }
    }

    private void CancelarGerarNota()
    {
        _confirmandoGerarNota = false;
        StateHasChanged();
    }
}