using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;
using ProdutoModel = SI_Bianca_Franca_2026.Models.Produto.Produtos;


namespace SI_Bianca_Franca_2026.Components.Pages.Produto.Sku;

public partial class Index
{
    [Inject] private SkusService SkusService { get; set; } = default!;
    [Inject] private ProdutosService ProdutosService { get; set; } = default!;

    private List<Skus>? _skus;
    private List<ProdutoModel> _produtos = new();
    private string _filtroProduto = "0";
    private string _filtroStatus = "ativo";
    private string _termoPesquisa = string.Empty;

    private IEnumerable<Skus> SkusFiltrados =>
        (_skus ?? [])
        .Where(s => _filtroStatus switch
        {
            "ativo" => s.Ativo,
            "inativo" => !s.Ativo,
            _ => true
        })
        .Where(s => _filtroProduto == "0" || s.IdProduto.ToString() == _filtroProduto)
        .Where(s => string.IsNullOrWhiteSpace(_termoPesquisa) ||
                    s.Sku.Contains(_termoPesquisa, StringComparison.OrdinalIgnoreCase) ||
                    (s.OProduto != null && s.OProduto.Produto.Contains(_termoPesquisa, StringComparison.OrdinalIgnoreCase)));

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        _skus = await SkusService.ListarTudoComProduto();

        var todosMateriais = await ProdutosService.ListarPorTipo("MATERIA_PRIMA");
        _produtos = todosMateriais.Where(p => p.Ativo).ToList();
    }
}