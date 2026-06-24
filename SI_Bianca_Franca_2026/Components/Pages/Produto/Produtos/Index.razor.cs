using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Services.Produto;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;
using ProdutoModel = SI_Bianca_Franca_2026.Models.Produto.Produtos;

namespace SI_Bianca_Franca_2026.Components.Pages.Produto.Produtos;

public partial class Index
{
    [Inject] private ProdutosService ProdutosService { get; set; } = default!;

    private List<ProdutoModel>? produtos;
    private string filtroStatus = "ativo";
    private string filtroTipo = "todos";
    private string termoPesquisa = string.Empty;

    private ProdutosModal _modalProduto = default!;

    private IEnumerable<ProdutoModel> ProdutosFiltrados =>
        (produtos ?? [])
        .Where(p => filtroStatus switch
        {
            "ativo" => p.Ativo,
            "inativo" => !p.Ativo,
            _ => true
        })
        .Where(p => filtroTipo == "todos" || p.Tipo == filtroTipo)
        .Where(p => string.IsNullOrWhiteSpace(termoPesquisa) ||
                    p.Produto.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (p.OCategoria != null && p.OCategoria.Categoria.Contains(termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarProdutos();
    }

    private async Task CarregarProdutos()
    {
        produtos = await ProdutosService.ListarTudo();
    }

    private async Task AbrirCadastro() => await _modalProduto.AbrirNovo();

    private async Task AbrirEdicao(int id) => await _modalProduto.AbrirEditar(id);

    private async Task OnProdutoSalvo(ProdutoModel produto)
    {
        await CarregarProdutos();
    }
}
