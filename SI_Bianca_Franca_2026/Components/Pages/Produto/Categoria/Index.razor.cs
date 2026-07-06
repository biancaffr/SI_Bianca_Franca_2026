using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

namespace SI_Bianca_Franca_2026.Components.Pages.Produto.Categoria;

public partial class Index
{
    [Inject] private CategoriasService CategoriasService { get; set; } = default!;

    private List<Categorias>? _categorias;
    private FiltroBaseDto _filtro = new();
    private CategoriasModal _modalCategoria = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        _categorias = await CategoriasService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarDados();
    }

    private void AbrirCadastro() => _modalCategoria.Abrir();

    private void AbrirEdicao(Categorias categoria)
    {
        _modalCategoria.Abrir(new CategoriasDTO
        {
            Id = categoria.Id,
            Categoria = categoria.Categoria,
            Ativo = categoria.Ativo,
            DataCriacao = categoria.DataCriacao,
            DataUltimaAlteracao = categoria.DataUltimaAlteracao,
            NomeUsuarioAlteracao = categoria.NomeUsuarioAlteracao
        });
    }

    private async Task OnCategoriaSalva(Categorias categoria)
    {
        await CarregarDados();
    }
}