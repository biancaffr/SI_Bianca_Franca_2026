using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Pages.Localizacao.Estado;

public partial class Index
{
    [Inject] private EstadosService EstadosService { get; set; } = default!;
    [Inject] private PaisesService PaisesService { get; set; } = default!;

    private List<Estados>? estados;
    private string filtroStatus = "ativo";
    private string termoPesquisa = string.Empty;
    private EstadosModal _modalEstado = default!;

    private IEnumerable<Estados> EstadosFiltrados =>
        (estados ?? [])
        .Where(e => filtroStatus switch
        {
            "ativo" => e.Ativo,
            "inativo" => !e.Ativo,
            _ => true
        })
        .Where(e => string.IsNullOrWhiteSpace(termoPesquisa) ||
                    e.Estado.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    e.Uf.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (e.OPais != null && e.OPais.Pais.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        estados = await EstadosService.ListarTudo();
        var todosPaises = await PaisesService.ListarTudo(); // mantendo a lógica de inicialização original
    }

    private void AbrirCadastro() => _modalEstado.Abrir();

    private void AbrirEdicao(Estados est) => _modalEstado.Abrir(new EstadosDTO
    {
        Id = est.Id,
        Estado = est.Estado,
        Uf = est.Uf,
        IdPais = est.IdPais,
        Ativo = est.Ativo,
        DataCriacao = est.DataCriacao,
        DataUltimaAlteracao = est.DataUltimaAlteracao,
        NomeUsuarioAlteracao = est.NomeUsuarioAlteracao ?? "Sistema"
    });

    private async Task OnEstadoSalvo(Estados estado)
    {
        await CarregarDados();
    }
}
