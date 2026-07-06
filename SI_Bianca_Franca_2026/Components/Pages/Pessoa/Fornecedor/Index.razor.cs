using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Fornecedor;

public partial class Index
{
    [Inject] private FornecedoresService FornecedoresService { get; set; } = default!;

    private List<Fornecedores>? _fornecedores;
    private FiltroBaseDto _filtro = new();
    private FornecedoresModal _modalFornecedor = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarFornecedores();
    }

    private async Task CarregarFornecedores()
    {
        _fornecedores = await FornecedoresService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarFornecedores();
    }

    private async Task AbrirCadastro() => await _modalFornecedor.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalFornecedor.AbrirEditar(id);

    private async Task OnFornecedorSalvo(Fornecedores f)
    {
        await CarregarFornecedores();
    }
}