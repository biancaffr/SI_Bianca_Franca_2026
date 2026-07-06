using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Pages.Localizacao.Cidade;

public partial class Index
{
    [Inject] private CidadesService CidadesService { get; set; } = default!;

    private List<Cidades>? _cidades;
    private FiltroBaseDto _filtro = new();
    private CidadesModal _modalCidade = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarCidades();
    }

    private async Task CarregarCidades()
    {
        _cidades = await CidadesService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarCidades();
    }

    private async Task AbrirCadastro() => await _modalCidade.Abrir();

    private async Task AbrirEdicao(Cidades cid) => await _modalCidade.Abrir(new CidadesDTO
    {
        Id = cid.Id,
        Cidade = cid.Cidade,
        IdEstado = cid.IdEstado,
        CodigoIbge = cid.CodigoIbge,
        Ddd = cid.Ddd,
        Ativo = cid.Ativo,
        DataCriacao = cid.DataCriacao,
        DataUltimaAlteracao = cid.DataUltimaAlteracao,
        NomeUsuarioAlteracao = cid.NomeUsuarioAlteracao ?? "Sistema"
    });

    private async Task OnCidadeSalva(Cidades cidade)
    {
        await CarregarCidades();
    }
}