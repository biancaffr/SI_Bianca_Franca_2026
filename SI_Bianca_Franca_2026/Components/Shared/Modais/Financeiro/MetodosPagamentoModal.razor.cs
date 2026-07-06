using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Financeiro;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Services.Financeiro;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Financeiro;

public partial class MetodosPagamentoModal
{
    [Inject] private MetodosPagamentoService MetodosPagamentoService { get; set; } = default!;

    [Parameter] public EventCallback<MetodosPagamento> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private string _erro = string.Empty;
    private MetodosPagamentoDTO _dto = new();

    public void Abrir(MetodosPagamentoDTO? dto = null)
    {
        _erro = string.Empty;
        _dto = dto ?? new MetodosPagamentoDTO { Ativo = true };
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
            var metodo = new MetodosPagamento
            {
                Id = _dto.Id,
                Sigla = _dto.Sigla,
                Metodo = _dto.Metodo,
                Ativo = _dto.Ativo
            };

            if (metodo.Id == 0)
                await MetodosPagamentoService.Inserir(metodo);
            else
                await MetodosPagamentoService.Atualizar(metodo);

            await AoSalvarComSucesso.InvokeAsync(metodo);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}