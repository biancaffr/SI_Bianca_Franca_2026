using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Financeiro;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Services.Financeiro;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Financeiro;

namespace SI_Bianca_Franca_2026.Components.Pages.Financeiro.MetodoPagamento;

public partial class Index
{
    [Inject] private MetodosPagamentoService MetodosPagamentoService { get; set; } = default!;

    private List<MetodosPagamento>? _metodos;
    private FiltroBaseDto _filtro = new();
    private MetodosPagamentoModal _modalMetodo = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        _metodos = await MetodosPagamentoService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarDados();
    }

    private void AbrirCadastro() => _modalMetodo.Abrir();

    private void AbrirEdicao(MetodosPagamento metodo)
    {
        _modalMetodo.Abrir(new MetodosPagamentoDTO
        {
            Id = metodo.Id,
            Sigla = metodo.Sigla,
            Metodo = metodo.Metodo,
            Ativo = metodo.Ativo,
            DataCriacao = metodo.DataCriacao,
            DataUltimaAlteracao = metodo.DataUltimaAlteracao,
            NomeUsuarioAlteracao = metodo.NomeUsuarioAlteracao
        });
    }

    private async Task OnMetodoSalvo(MetodosPagamento metodo)
    {
        await CarregarDados();
    }
}