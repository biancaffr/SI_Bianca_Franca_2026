using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Pages.Localizacao.Cidade;

public partial class Index
{
    [Inject] private CidadesService CidadesService { get; set; } = default!;

    private List<Cidades>? cidades;
    private string filtroStatus = "ativo";
    private string termoPesquisa = string.Empty;
    private CidadesModal _modalCidade = default!;

    private IEnumerable<Cidades> CidadesFiltradas =>
        (cidades ?? [])
        .Where(c => filtroStatus switch
        {
            "ativo" => c.Ativo,
            "inativo" => !c.Ativo,
            _ => true
        })
        .Where(c => string.IsNullOrWhiteSpace(termoPesquisa) ||
                    c.Cidade.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    c.CodigoIbge.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    c.Ddd.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (c.OEstado != null && c.OEstado.Estado.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        cidades = await CidadesService.ListarTudo();
    }

    private void AbrirCadastro() => _modalCidade.Abrir();

    private void AbrirEdicao(Cidades cid) => _modalCidade.Abrir(new CidadesDTO
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
        await CarregarDados();
    }
}
