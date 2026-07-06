using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Transportadora;

public partial class Index
{
    [Inject] private TransportadorasService TransportadorasService { get; set; } = default!;

    private List<Transportadoras>? _transportadoras;
    private FiltroBaseDto _filtro = new();
    private TransportadorasModal _modalTransportadora = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarTransportadoras();
    }

    private async Task CarregarTransportadoras()
    {
        _transportadoras = await TransportadorasService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarTransportadoras();
    }

    private async Task AbrirCadastro() => await _modalTransportadora.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalTransportadora.AbrirEditar(id);

    private async Task OnTransportadoraSalva(Transportadoras t)
    {
        await CarregarTransportadoras();
    }
}