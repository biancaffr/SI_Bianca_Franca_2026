using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Fornecedor;

public partial class Index
{
    [Inject] private FornecedoresService FornecedoresService { get; set; } = default!;

    private List<Fornecedores>? fornecedores;
    private string filtroStatus = "ativo";
    private string termoPesquisa = string.Empty;

    private FornecedoresModal _modalFornecedor = default!;

    private IEnumerable<Fornecedores> FornecedoresFiltrados =>
        (fornecedores ?? [])
        .Where(f => filtroStatus switch
        {
            "ativo" => f.Ativo,
            "inativo" => !f.Ativo,
            _ => true
        })
        .Where(f => string.IsNullOrWhiteSpace(termoPesquisa) ||
                    f.NomeRazaoSocial.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (f.CpfCnpj != null && f.CpfCnpj.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)) ||
                    (f.ApelidoNomeFantasia != null && f.ApelidoNomeFantasia.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarFornecedores();
    }

    private async Task CarregarFornecedores()
    {
        fornecedores = await FornecedoresService.ListarTudo();
    }

    private async Task AbrirCadastro() => await _modalFornecedor.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalFornecedor.AbrirEditar(id);

    private async Task OnFornecedorSalvo(Fornecedores f)
    {
        await CarregarFornecedores();
    }
}
