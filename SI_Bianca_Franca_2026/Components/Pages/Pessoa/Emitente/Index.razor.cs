using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Emitente;

public partial class Index
{
    [Inject] private EmitentesService EmitentesService { get; set; } = default!;

    private List<Emitentes>? _emitentes;
    private FiltroBaseDto _filtro = new();
    private EmitentesModal _modalEmitente = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarEmitentes();
    }

    private async Task CarregarEmitentes()
    {
        _emitentes = await EmitentesService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarEmitentes();
    }

    private async Task AbrirCadastro() => await _modalEmitente.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalEmitente.AbrirEditar(id);

    private async Task OnEmitenteSalvo(Emitentes e)
    {
        await CarregarEmitentes();
    }
}