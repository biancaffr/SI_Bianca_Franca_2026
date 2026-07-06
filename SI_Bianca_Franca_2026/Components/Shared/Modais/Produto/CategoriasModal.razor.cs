using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

public partial class CategoriasModal
{
    [Inject] private CategoriasService CategoriasService { get; set; } = default!;

    [Parameter] public EventCallback<Categorias> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback<int> AoExcluirComSucesso { get; set; }

    private bool _aberto = false;
    private string _erro = string.Empty;
    private CategoriasDTO _dto = new();
    private ModalConfirmacao _modalConfirmacao = default!;

    public void Abrir(CategoriasDTO? dto = null)
    {
        _erro = string.Empty;
        _dto = dto ?? new CategoriasDTO { Ativo = true };
        _aberto = true;
        StateHasChanged();
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;
            var categoria = new Categorias
            {
                Id = _dto.Id,
                Categoria = _dto.Categoria,
                Ativo = _dto.Ativo
            };

            if (categoria.Id == 0)
                await CategoriasService.Inserir(categoria);
            else
                await CategoriasService.Atualizar(categoria);

            await AoSalvarComSucesso.InvokeAsync(categoria);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir($"Deseja realmente excluir a categoria \"{_dto.Categoria}\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await CategoriasService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}