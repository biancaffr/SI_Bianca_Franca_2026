// Components/Pages/NfeEntrada/Index.razor.cs
using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Components.Shared.Modais.NfeEntrada;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.NfeEntrada;
using SI_Bianca_Franca_2026.Services.NfeEntrada;

namespace SI_Bianca_Franca_2026.Components.Pages.NfeEntrada.NfeEntrada;

public partial class Index
{
    [Inject] private NfeEntradaService NfeEntradaService { get; set; } = default!;

    private List<NfeEntradas>? _notas;
    private FiltroBaseDto _filtro = new();
    private NfeEntradaModal _modal = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
        => _notas = await NfeEntradaService.ListarTudo(_filtro);

    private async Task OnFiltroAlterado()
        => await CarregarDados();

    private async Task AbrirNovo() => await _modal.AbrirNovo();

    private async Task AbrirVisualizar(NfeEntradas nota)
        => await _modal.AbrirVisualizar(nota.Numero, nota.Modelo, nota.Serie, nota.IdFornecedor);

    private async Task OnSalva(NfeEntradas _) => await CarregarDados();
    private async Task OnCancelada() => await CarregarDados();

    private static string BadgeSituacao(string s) => s switch
    {
        "PENDENTE" => "bg-warning text-dark",
        "CONFIRMADA" => "bg-success",
        "CANCELADA" => "bg-danger",
        _ => "bg-secondary"
    };

    private static string FormatarSituacao(string s) => s switch
    {
        "PENDENTE" => "Pendente",
        "CONFIRMADA" => "Confirmada",
        "CANCELADA" => "Cancelada",
        _ => s
    };
}