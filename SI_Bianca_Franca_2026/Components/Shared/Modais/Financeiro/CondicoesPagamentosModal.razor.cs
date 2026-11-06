using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Financeiro;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Services.Financeiro;
using System.Globalization;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Financeiro;

public partial class CondicoesPagamentosModal
{
    [Inject] private CondicoesPagamentosService CondicoesPagamentosService { get; set; } = default!;
    [Inject] private MetodosPagamentoService MetodosPagamentoService { get; set; } = default!;

    [Parameter] public EventCallback<CondicoesPagamentos> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback<int> AoExcluirComSucesso { get; set; }



    private bool _aberto = false;
    private string _erro = string.Empty;
    private CondicoesPagamentosDTO _dto = new();
    private List<MetodosPagamento> _metodos = new();
    private int _qtdParcelas = 1;
    private MetodosPagamentoModal _modalMetodo = default!;
    private ModalConfirmacao _modalConfirmacao = default!;
    private void AbrirModalNovoMetodo() => _modalMetodo.Abrir();

    private static readonly CultureInfo _ptBR = CultureInfo.GetCultureInfo("pt-BR");

    private decimal TotalPercentual => _dto.Parcelas.Sum(p => p.Percentual);

    public async Task AbrirNovo()
    {
        _erro = string.Empty;
        _dto = new CondicoesPagamentosDTO { Ativo = true };
        _qtdParcelas = 1;
        await CarregarMetodos();

        _dto.Parcelas.Add(new CondicoesPagamentosParcelasDTO
        {
            NumeroParcela = 1,
            Percentual = 100,
            PrazoDias = 0,
            IdMetodoPagamento = 0
        });

        _aberto = true;
        StateHasChanged();
    }

    public async Task AbrirEditar(int id)
    {
        _erro = string.Empty;
        await CarregarMetodos();

        var condicao = await CondicoesPagamentosService.Pesquisar(id);
        if (condicao is null)
        {
            _aberto = true;
            StateHasChanged();
            return;
        }

        _dto = new CondicoesPagamentosDTO
        {
            Id = condicao.Id,
            CondicaoPagamento = condicao.CondicaoPagamento,
            DescontoPercentual = condicao.DescontoPercentual,
            MultaPercentual = condicao.MultaPercentual,
            TaxaJurosPercentual = condicao.TaxaJurosPercentual,
            Ativo = condicao.Ativo,
            DataCriacao = condicao.DataCriacao,
            DataUltimaAlteracao = condicao.DataUltimaAlteracao,
            NomeUsuarioAlteracao = condicao.NomeUsuarioAlteracao,
            Parcelas = condicao.Parcelas.Select(p => new CondicoesPagamentosParcelasDTO
            {
                Id = p.Id,
                NumeroParcela = p.NumeroParcela,
                Percentual = p.Percentual,
                PrazoDias = p.PrazoDias,
                IdMetodoPagamento = p.IdMetodoPagamento
            }).ToList()
        };

        _qtdParcelas = _dto.Parcelas.Count;
        _aberto = true;
        StateHasChanged();
    }

    private async Task CarregarMetodos()
    {
        var metodos = await MetodosPagamentoService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _metodos = metodos.ToList();
    }

    private async Task OnMetodoSalvo(MetodosPagamento metodo)
    {
        var metodos = await MetodosPagamentoService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _metodos = metodos.ToList();
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private void AtualizarQtdParcelas(ChangeEventArgs e)
    {
        if (!int.TryParse(e.Value?.ToString(), out var qtd) || qtd < 1)
        {
            _qtdParcelas = 1;
            StateHasChanged();
            return;
        }

        _qtdParcelas = qtd;

        while (_dto.Parcelas.Count < qtd)
        {
            _dto.Parcelas.Add(new CondicoesPagamentosParcelasDTO
            {
                NumeroParcela = _dto.Parcelas.Count + 1,
                Percentual = 0,
                PrazoDias = 0,
                IdMetodoPagamento = 0
            });
        }

        while (_dto.Parcelas.Count > qtd)
            _dto.Parcelas.RemoveAt(_dto.Parcelas.Count - 1);

        var percentualPorParcela = Math.Round(100m / qtd, 4);
        for (int i = 0; i < _dto.Parcelas.Count; i++)
        {
            _dto.Parcelas[i].Percentual = percentualPorParcela;
            _dto.Parcelas[i].PrazoDias = i * 30;
        }

        var totalAtual = _dto.Parcelas.Sum(p => p.Percentual);
        _dto.Parcelas[^1].Percentual += 100m - totalAtual;

        StateHasChanged();
    }

    private void AtualizarPercentualParcela(CondicoesPagamentosParcelasDTO parcela, string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto)) { parcela.Percentual = 0; return; }
        if (decimal.TryParse(texto, NumberStyles.Any, _ptBR, out var r))
            parcela.Percentual = r;
    }

    private void AtualizarDecimal(ChangeEventArgs e, Action<decimal> setter)
    {
        var texto = e.Value?.ToString();
        if (string.IsNullOrWhiteSpace(texto)) { setter(0); return; }
        if (decimal.TryParse(texto, NumberStyles.Any, _ptBR, out var r))
            setter(r);
    }
    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;

            if (!_dto.Parcelas.Any())
            {
                _erro = "Adicione ao menos uma parcela.";
                return;
            }

            if (_dto.Parcelas.Any(p => p.IdMetodoPagamento == 0))
            {
                _erro = "Selecione a forma de pagamento em todas as parcelas.";
                return;
            }

            if (TotalPercentual != 100m)
            {
                _erro = $"A soma dos percentuais deve ser exatamente 100%. Total atual: {TotalPercentual.ToString("0.####", _ptBR)}%";
                return;
            }

            var condicao = new CondicoesPagamentos
            {
                Id = _dto.Id,
                CondicaoPagamento = _dto.CondicaoPagamento,
                DescontoPercentual = _dto.DescontoPercentual,
                MultaPercentual = _dto.MultaPercentual,
                TaxaJurosPercentual = _dto.TaxaJurosPercentual,
                Ativo = _dto.Ativo,
                DataCriacao = _dto.DataCriacao
            };

            var parcelas = _dto.Parcelas.Select(p => new CondicoesPagamentosParcelas
            {
                Id = p.Id,
                NumeroParcela = p.NumeroParcela,
                Percentual = p.Percentual,
                PrazoDias = p.PrazoDias,
                IdMetodoPagamento = p.IdMetodoPagamento
            }).ToList();

            if (condicao.Id == 0)
                await CondicoesPagamentosService.Inserir(condicao, parcelas);
            else
                await CondicoesPagamentosService.Atualizar(condicao, parcelas);

            await AoSalvarComSucesso.InvokeAsync(condicao);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir($"Deseja realmente excluir a condição de pagamento \"{_dto.CondicaoPagamento}\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await CondicoesPagamentosService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}