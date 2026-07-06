using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Pages.Localizacao.Pais;

public partial class Index
{
    [Inject] private PaisesService PaisesService { get; set; } = default!;

    private List<Paises>? _paises;
    private FiltroBaseDto _filtro = new();
    private PaisesModal _modalPais = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarPaises();
    }

    private async Task CarregarPaises()
    {
        _paises = await PaisesService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarPaises();
    }

    private void AbrirCadastro() => _modalPais.Abrir();

    private void AbrirEdicao(Paises pais) => _modalPais.Abrir(new PaisesDTO
    {
        Id = pais.Id,
        Pais = pais.Pais,
        Sigla = pais.Sigla,
        Ddi = pais.Ddi,
        Moeda = pais.Moeda,
        Ativo = pais.Ativo,
        DataCriacao = pais.DataCriacao,
        DataUltimaAlteracao = pais.DataUltimaAlteracao,
        NomeUsuarioAlteracao = pais.NomeUsuarioAlteracao ?? "Sistema"
    });

    private async Task OnPaisSalvo(Paises pais)
    {
        await CarregarPaises();
    }
}