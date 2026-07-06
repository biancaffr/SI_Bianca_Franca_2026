using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Services.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

public partial class VeiculosModal
{
    [Inject] private VeiculosService VeiculosService { get; set; } = default!;
    [Inject] private EstadosService EstadosService { get; set; } = default!;
    [Inject] private TransportadorasService TransportadorasService { get; set; } = default!;

    [Parameter] public EventCallback<Veiculos> AoSalvarComSucesso { get; set; }

    private bool _aberto = false;
    private bool _carregando = false;
    private string _erro = string.Empty;
    private VeiculosDTO _dto = new();
    private List<Estados> _estados = new();
    private List<Transportadoras> _transportadoras = new();
    private EstadosModal _modalEstado = default!;

    public async Task Abrir(VeiculosDTO? dto = null)
    {
        _erro = string.Empty;
        _carregando = true;
        _aberto = true;
        StateHasChanged();

        await CarregarListas();

        _dto = dto ?? new VeiculosDTO { Ativo = true };

        _carregando = false;
        StateHasChanged();
    }

    private async Task CarregarListas()
    {
        var estados = await EstadosService.ListarTudo(new FiltroBaseDto { StatusFiltro = "todos" });
        _estados = estados.Where(e => e.Ativo || (_dto.IdEstado > 0 && e.Id == _dto.IdEstado)).ToList();

        var transportadoras = await TransportadorasService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _transportadoras = transportadoras.ToList();
    }

    private void Fechar()
    {
        _aberto = false;
        StateHasChanged();
    }

    private async Task AbrirModalNovoEstado() => await _modalEstado.Abrir();

    private async Task OnEstadoSalvoAsync(Estados estado)
    {
        var estados = await EstadosService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _estados = estados.ToList();
        _dto.IdEstado = estado.Id;
        StateHasChanged();
    }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;

            var veiculo = new Veiculos
            {
                Id = _dto.Id,
                IdTransportadora = _dto.IdTransportadora,
                IdEstado = _dto.IdEstado,
                Placa = _dto.Placa,
                Rntrc = _dto.Rntrc,
                Renavam = _dto.Renavam,
                TipoVeiculo = _dto.TipoVeiculo,
                MarcaModelo = _dto.MarcaModelo,
                Observacao = _dto.Observacao,
                Ativo = _dto.Ativo,
                DataCriacao = _dto.DataCriacao
            };

            if (veiculo.Id == 0)
                await VeiculosService.Inserir(veiculo);
            else
                await VeiculosService.Atualizar(veiculo);

            await AoSalvarComSucesso.InvokeAsync(veiculo);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}