using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

public partial class PaisesModal
{
    [Inject] private PaisesService PaisesService { get; set; } = default!;

    [Parameter] public EventCallback<Paises> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback<int> AoExcluirComSucesso { get; set; }


    private bool _aberto = false;
    private string _erro = string.Empty;
    private PaisesDTO _dto = new();

    private ModalConfirmacao _modalConfirmacao = default!;

    public void Abrir(PaisesDTO? dto = null)
    {
        _erro = string.Empty;
        _dto = dto ?? new PaisesDTO { Ativo = true };
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
            var pais = new Paises
            {
                Id = _dto.Id,
                Pais = _dto.Pais,
                Sigla = _dto.Sigla,
                Ddi = _dto.Ddi,
                Moeda = _dto.Moeda,
                Ativo = _dto.Ativo
            };

            if (pais.Id == 0)
                await PaisesService.Inserir(pais);
            else
                await PaisesService.Atualizar(pais);

            await AoSalvarComSucesso.InvokeAsync(pais);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir($"Deseja realmente excluir o país \"{_dto.Pais}\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await PaisesService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}
