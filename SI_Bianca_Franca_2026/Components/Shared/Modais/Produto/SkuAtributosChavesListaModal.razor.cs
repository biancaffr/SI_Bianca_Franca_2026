using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

public partial class SkuAtributosChavesListaModal
{
    [Inject] private SkuAtributosChavesService SkuAtributosChavesService { get; set; } = default!;

    [Parameter] public EventCallback AoAtualizarLista { get; set; }

    private bool _aberto = false;
    private List<SkuAtributosChaves>? _chaves;
    private string _filtroStatus = "ativo";
    private string _termoPesquisa = string.Empty;
    private SkuAtributosChavesModal _modalChave = default!;

    private IEnumerable<SkuAtributosChaves> ChavesFiltradas =>
        (_chaves ?? [])
        .Where(c => _filtroStatus switch
        {
            "ativo" => c.Ativo,
            "inativo" => !c.Ativo,
            _ => true
        })
        .Where(c => string.IsNullOrWhiteSpace(_termoPesquisa) ||
                    c.Chave.Contains(_termoPesquisa, StringComparison.OrdinalIgnoreCase));

    public async Task Abrir()
    {
        await CarregarChaves();
        _aberto = true;
        StateHasChanged();
    }

    private async Task CarregarChaves()
    {
        _chaves = await SkuAtributosChavesService.ListarTudo();
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private async Task AbrirNovo() => await _modalChave.Abrir();

    private async Task AbrirEdicao(int id)
    {
        var chave = await SkuAtributosChavesService.Pesquisar(id);
        if (chave is null) return;

        var dto = new SkuAtributosChavesDTO
        {
            Id = chave.Id,
            Chave = chave.Chave,
            Ativo = chave.Ativo,
            DataCriacao = chave.DataCriacao,
            DataUltimaAlteracao = chave.DataUltimaAlteracao,
            NomeUsuarioAlteracao = chave.NomeUsuarioAlteracao
        };

        await _modalChave.Abrir(dto);
    }

    private async Task OnChaveSalva(SkuAtributosChaves chave)
    {
        await CarregarChaves();
        await AoAtualizarLista.InvokeAsync();
    }
}