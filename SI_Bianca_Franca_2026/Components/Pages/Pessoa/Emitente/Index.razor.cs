using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Emitente;

public partial class Index
{
    [Inject] private EmitentesService EmitentesService { get; set; } = default!;

    private List<Emitentes>? emitentes;
    private string filtroStatus = "ativo";
    private string termoPesquisa = string.Empty;

    private EmitentesModal _modalEmitente = default!;

    private IEnumerable<Emitentes> EmitentesFiltrados =>
        (emitentes ?? [])
        .Where(e => filtroStatus switch
        {
            "ativo" => e.Ativo,
            "inativo" => !e.Ativo,
            _ => true
        })
        .Where(e => string.IsNullOrWhiteSpace(termoPesquisa) ||
                    e.NomeRazaoSocial.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (e.CpfCnpj != null && e.CpfCnpj.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    (e.ApelidoNomeFantasia != null && e.ApelidoNomeFantasia.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarEmitentes();
    }

    private async Task CarregarEmitentes()
    {
        emitentes = await EmitentesService.ListarTudo();
    }

    private async Task AbrirCadastro() => await _modalEmitente.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalEmitente.AbrirEditar(id);

    private async Task OnEmitenteSalvo(Emitentes e)
    {
        await CarregarEmitentes();
    }
}
