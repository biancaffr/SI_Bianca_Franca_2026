using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Transportadora;

public partial class Index
{
    [Inject] private TransportadorasService TransportadorasService { get; set; } = default!;

    private List<Transportadoras>? transportadoras;
    private string filtroStatus = "ativo";
    private string termoPesquisa = string.Empty;

    private TransportadorasModal _modalTransportadora = default!;

    private IEnumerable<Transportadoras> TransportadorasFiltrados =>
        (transportadoras ?? [])
        .Where(t => filtroStatus switch
        {
            "ativo" => t.Ativo,
            "inativo" => !t.Ativo,
            _ => true
        })
        .Where(t => string.IsNullOrWhiteSpace(termoPesquisa) ||
                    t.NomeRazaoSocial.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (t.CpfCnpj != null && t.CpfCnpj.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    (t.ApelidoNomeFantasia != null && t.ApelidoNomeFantasia.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarTransportadoras();
    }

    private async Task CarregarTransportadoras()
    {
        transportadoras = await TransportadorasService.ListarTudo();
    }

    private async Task AbrirCadastro() => await _modalTransportadora.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalTransportadora.AbrirEditar(id);

    private async Task OnTransportadoraSalva(Transportadoras t)
    {
        await CarregarTransportadoras();
    }
}
