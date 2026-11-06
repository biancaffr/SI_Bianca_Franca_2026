using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

namespace SI_Bianca_Franca_2026.Components.Pages.Produto.UnidadeMedida;

public partial class Index
{
    [Inject] private UnidadesMedidaService UnidadesMedidaService { get; set; } = default!;

    private List<UnidadesMedida>? _unidades;
    private FiltroBaseDto _filtro = new();
    private UnidadesMedidaModal _modalUnidade = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        _unidades = await UnidadesMedidaService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarDados();
    }

    private void AbrirCadastro() => _modalUnidade.Abrir();

    private void AbrirEdicao(UnidadesMedida unidade)
    {
        _modalUnidade.Abrir(new UnidadesMedidaDTO
        {
            Id = unidade.Id,
            Sigla = unidade.Sigla,
            UnidadeMedida = unidade.UnidadeMedida,
            Ativo = unidade.Ativo,
            DataCriacao = unidade.DataCriacao,
            DataUltimaAlteracao = unidade.DataUltimaAlteracao,
            NomeUsuarioAlteracao = unidade.NomeUsuarioAlteracao
        });
    }

    private async Task OnUnidadeSalva(UnidadesMedida unidade)
    {
        await CarregarDados();
    }
}