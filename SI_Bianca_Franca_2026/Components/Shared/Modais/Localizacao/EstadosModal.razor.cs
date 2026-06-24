using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

public partial class EstadosModal
{
    [Inject] private PaisesService PaisesService { get; set; } = default!;
    [Inject] private EstadosService EstadosService { get; set; } = default!;

    [Parameter] public EventCallback<Estados> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private string _erro = string.Empty;
    private EstadosDTO _dto = new();
    private List<Paises> _paises = new();
    private PaisesModal _modalPais = default!;

    public async Task Abrir(EstadosDTO? dto = null, int idPaisPreSelecionado = 0)
    {
        _erro = string.Empty;
        _dto = dto ?? new EstadosDTO
        {
            Ativo = true,
            IdPais = idPaisPreSelecionado
        };

        await CarregarPaisesAsync();
        _aberto = true;
        StateHasChanged();
    }

    private async Task CarregarPaisesAsync()
    {
        var todos = await PaisesService.ListarTudo();
        _paises = todos.Where(p => p.Ativo || p.Id == _dto.IdPais).ToList();
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private async Task OnPaisSalvo(Paises pais)
    {
        var todos = await PaisesService.ListarTudo();
        _paises = todos.Where(p => p.Ativo).ToList();

        var novo = _paises.FirstOrDefault(p => p.Sigla == pais.Sigla);
        if (novo != null)
            _dto.IdPais = novo.Id;

        StateHasChanged();
    }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;
            var estado = new Estados
            {
                Id = _dto.Id,
                Estado = _dto.Estado,
                Uf = _dto.Uf,
                IdPais = _dto.IdPais,
                Ativo = _dto.Ativo
            };

            if (estado.Id == 0)
                await EstadosService.Inserir(estado);
            else
                await EstadosService.Atualizar(estado);

            await AoSalvarComSucesso.InvokeAsync(estado);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}
