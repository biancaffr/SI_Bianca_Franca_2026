using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Services.Financeiro;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Financeiro;
using System.Globalization;

namespace SI_Bianca_Franca_2026.Components.Pages.Financeiro.CondicaoPagamento;

public partial class Index
{
    [Inject] private CondicoesPagamentosService CondicoesPagamentosService { get; set; } = default!;

    private List<CondicoesPagamentos>? _condicoes;
    private FiltroBaseDto _filtro = new();
    private CondicoesPagamentosModal _modal = default!;

    private static readonly CultureInfo _ptBR = CultureInfo.GetCultureInfo("pt-BR");

    protected override async Task OnInitializedAsync()
        => await CarregarDados();

    private async Task CarregarDados()
        => _condicoes = await CondicoesPagamentosService.ListarTudo(_filtro);

    private async Task OnFiltroAlterado()
        => await CarregarDados();

    private async Task AbrirCadastro() => await _modal.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modal.AbrirEditar(id);
    private async Task OnSalvo(CondicoesPagamentos c) => await CarregarDados();
}