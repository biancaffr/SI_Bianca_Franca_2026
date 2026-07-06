using Microsoft.AspNetCore.Components;

namespace SI_Bianca_Franca_2026.Components.Shared;

public partial class ModalConfirmacao
{
    [Parameter] public EventCallback AoConfirmar { get; set; }

    private bool _aberto = false;
    private string _mensagem = string.Empty;

    public void Abrir(string mensagem)
    {
        _mensagem = mensagem;
        _aberto = true;
        StateHasChanged();
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private async Task Confirmar()
    {
        _aberto = false;
        StateHasChanged();
        await AoConfirmar.InvokeAsync();
    }
}