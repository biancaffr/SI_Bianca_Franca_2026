using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

public partial class UnidadesMedidaModal
{
    [Inject] private UnidadesMedidaService UnidadesMedidaService { get; set; } = default!;

    [Parameter] public EventCallback<UnidadesMedida> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback<int> AoExcluirComSucesso { get; set; }

    private bool _aberto = false;
    private string _erro = string.Empty;
    private UnidadesMedidaDTO _dto = new();
    private ModalConfirmacao _modalConfirmacao = default!;

    public void Abrir(UnidadesMedidaDTO? dto = null)
    {
        _erro = string.Empty;
        _dto = dto ?? new UnidadesMedidaDTO { Ativo = true };
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
            var unidade = new UnidadesMedida
            {
                Id = _dto.Id,
                Sigla = _dto.Sigla,
                UnidadeMedida = _dto.UnidadeMedida,
                Ativo = _dto.Ativo
            };

            if (unidade.Id == 0)
                await UnidadesMedidaService.Inserir(unidade);
            else
                await UnidadesMedidaService.Atualizar(unidade);

            await AoSalvarComSucesso.InvokeAsync(unidade);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir($"Deseja realmente excluir a unidade de medida \"{_dto.UnidadeMedida}\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await UnidadesMedidaService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}