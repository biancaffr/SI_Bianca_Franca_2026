using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SI_Bianca_Franca_2026.DTOs.Produto;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Services.Produto;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto;

public partial class SkuAtributosChavesModal
{
    [Inject] private SkuAtributosChavesService SkuAtributosChavesService { get; set; } = default!;
    [Inject] private SkusAtributosValoresService SkusAtributosValoresService { get; set; } = default!;

    [Parameter] public EventCallback<SkuAtributosChaves> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private string _erro = string.Empty;
    private SkuAtributosChavesDTO _dto = new();
    private string _novoValorTexto = string.Empty;

    public async Task Abrir(SkuAtributosChavesDTO? dto = null)
    {
        _erro = string.Empty;
        _novoValorTexto = string.Empty;

        if (dto != null)
        {
            _dto = dto;
            var valores = await SkusAtributosValoresService.ListarPorChave(dto.Id);
            _dto.ValoresPossiveis = valores
                .Select(v => new ValorPossivelDTO { Id = v.Id, Valor = v.Valor, Ativo = v.Ativo })
                .ToList();
        }
        else
        {
            _dto = new SkuAtributosChavesDTO { Ativo = true };
        }

        _aberto = true;
        StateHasChanged();
    }
    private void AlternarStatusValor(ValorPossivelDTO valor) => valor.Ativo = !valor.Ativo;

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private void AoTeclarNovoValor(KeyboardEventArgs e)
    {
        if (e.Key == "Enter")
            AdicionarValor();
    }

    private void AdicionarValor()
    {
        var texto = _novoValorTexto.Trim();
        if (string.IsNullOrWhiteSpace(texto)) return;

        if (_dto.ValoresPossiveis.Any(v => v.Valor.Equals(texto, StringComparison.OrdinalIgnoreCase)))
        {
            _erro = "Este valor já foi adicionado.";
            return;
        }

        _erro = string.Empty;
        _dto.ValoresPossiveis.Add(new ValorPossivelDTO { Valor = texto });
        _novoValorTexto = string.Empty;
    }

    private void RemoverValor(ValorPossivelDTO valor) => _dto.ValoresPossiveis.Remove(valor);

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;

            var chave = new SkuAtributosChaves
            {
                Id = _dto.Id,
                Chave = _dto.Chave,
                Ativo = _dto.Ativo
            };

            int idChave;
            if (chave.Id == 0)
            {
                idChave = await SkuAtributosChavesService.Inserir(chave);
                chave.Id = idChave;
            }
            else
            {
                idChave = chave.Id;
                await SkuAtributosChavesService.Atualizar(chave);
            }

            await SincronizarValoresAsync(idChave);

            await AoSalvarComSucesso.InvokeAsync(chave);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
    private async Task SincronizarValoresAsync(int idChave)
    {
        var valoresExistentes = await SkusAtributosValoresService.ListarPorChave(idChave);

        foreach (var item in _dto.ValoresPossiveis)
        {
            if (item.Id == 0)
            {
                await SkusAtributosValoresService.Inserir(new SkusAtributosValores
                {
                    IdChave = idChave,
                    Valor = item.Valor,
                    Ativo = true
                });
            }
            else
            {
                var existente = valoresExistentes.FirstOrDefault(v => v.Id == item.Id);
                if (existente != null && existente.Ativo != item.Ativo)
                {
                    if (item.Ativo)
                        await SkusAtributosValoresService.Ativar(item.Id); 
                    else
                        await SkusAtributosValoresService.Desativar(item.Id); 
                }
            }
        }
    }
}