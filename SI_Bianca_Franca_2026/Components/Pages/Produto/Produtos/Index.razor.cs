using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Services.Produto;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;
using ProdutoModel = SI_Bianca_Franca_2026.Models.Produto.Produtos;

namespace SI_Bianca_Franca_2026.Components.Pages.Produto.Produtos;

public partial class Index
{
    [Inject] private ProdutosService ProdutosService { get; set; } = default!;

    private List<ProdutoModel>? _produtos;
    private FiltroProdutosDto _filtro = new();
    private ProdutosModal _modalProduto = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarProdutos();
    }

    private async Task CarregarProdutos()
    {
        _produtos = await ProdutosService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarProdutos();
    }

    private async Task AbrirCadastro() => await _modalProduto.AbrirNovo();
    private async Task AbrirEdicao(int id) => await _modalProduto.AbrirEditar(id);

    private async Task OnProdutoSalvo(ProdutoModel produto)
    {
        await CarregarProdutos();
    }
}