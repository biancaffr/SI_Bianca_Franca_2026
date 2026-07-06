using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Pages.Localizacao.Estado;

public partial class Index
{
    [Inject] private EstadosService EstadosService { get; set; } = default!;

    private List<Estados>? _estados;
    private FiltroBaseDto _filtro = new();
    private EstadosModal _modalEstado = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarEstados();
    }

    private async Task CarregarEstados()
    {
        _estados = await EstadosService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarEstados();
    }

    private async Task AbrirCadastro() => await _modalEstado.Abrir();

    private async Task AbrirEdicao(Estados est) => await _modalEstado.Abrir(new EstadosDTO
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
        await CarregarEstados();
    }
}