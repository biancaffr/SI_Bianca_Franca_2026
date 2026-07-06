using Microsoft.AspNetCore.Components;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Services.Pessoa;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa;

namespace SI_Bianca_Franca_2026.Components.Pages.Pessoa.Veiculo;

public partial class Index
{
    [Inject] private VeiculosService VeiculosService { get; set; } = default!;

    private List<Veiculos>? _veiculos;
    private FiltroBaseDto _filtro = new();
    private VeiculosModal _modalVeiculo = default!;

    protected override async Task OnInitializedAsync()
    {
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        _veiculos = await VeiculosService.ListarTudo(_filtro);
    }

    private async Task OnFiltroAlterado()
    {
        await CarregarDados();
    }

    private async Task AbrirCadastro() => await _modalVeiculo.Abrir();

    private async Task AbrirEdicao(Veiculos veiculo)
    {
        await _modalVeiculo.Abrir(new VeiculosDTO
        {
            Id = veiculo.Id,
            IdTransportadora = veiculo.IdTransportadora,
            IdEstado = veiculo.IdEstado,
            Placa = veiculo.Placa,
            Rntrc = veiculo.Rntrc,
            Renavam = veiculo.Renavam,
            TipoVeiculo = veiculo.TipoVeiculo,
            MarcaModelo = veiculo.MarcaModelo,
            Observacao = veiculo.Observacao,
            Ativo = veiculo.Ativo,
            DataCriacao = veiculo.DataCriacao,
            DataUltimaAlteracao = veiculo.DataUltimaAlteracao,
            NomeUsuarioAlteracao = veiculo.NomeUsuarioAlteracao
        });
    }

    private async Task OnVeiculoSalvo(Veiculos veiculo)
    {
        await CarregarDados();
    }
}