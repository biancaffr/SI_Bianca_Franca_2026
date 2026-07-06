using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.Localizacao;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;

public partial class CidadesModal
{
    [Inject] private EstadosService EstadosService { get; set; } = default!;
    [Inject] private CidadesService CidadesService { get; set; } = default!;

    [Parameter] public EventCallback<Cidades> AoSalvarComSucesso { get; set; }
    [Parameter] public EventCallback AoExcluirComSucesso { get; set; }

    private bool _aberto = false;
    private bool _confirmandoExclusao = false;
    private string _erro = string.Empty;
    private CidadesDTO _dto = new();
    private List<Estados> _estados = new();
    private EstadosModal _modalEstado = default!;
    private ModalConfirmacao _modalConfirmacao = default!;

    public async Task Abrir(CidadesDTO? dto = null, int idEstadoPreSelecionado = 0)
    {
        _erro = string.Empty;
        _confirmandoExclusao = false;
        _dto = dto ?? new CidadesDTO
        {
            Ativo = true,
            IdEstado = idEstadoPreSelecionado
        };

        await CarregarEstadosAsync();
        _aberto = true;
        StateHasChanged();
    }

    private async Task CarregarEstadosAsync()
    {
        var todos = await EstadosService.ListarTudo(new FiltroBaseDto { StatusFiltro = "todos" });
        _estados = todos.Where(e => e.Ativo || e.Id == _dto.IdEstado).ToList();
    }

    private void Fechar()
    {
        _aberto = false;
        _confirmandoExclusao = false;
        StateHasChanged();
    }

    private async Task AbrirModalEstado() => await _modalEstado.Abrir();

    private async Task OnEstadoSalvo(Estados estado)
    {
        var todos = await EstadosService.ListarTudo(new FiltroBaseDto { StatusFiltro = "ativo" });
        _estados = todos.ToList();

        var novo = _estados.FirstOrDefault(e => e.Uf == estado.Uf && e.IdPais == estado.IdPais);
        if (novo != null)
            _dto.IdEstado = novo.Id;

        StateHasChanged();
    }

    private async Task Salvar()
    {
        try
        {
            _erro = string.Empty;
            var cidade = new Cidades
            {
                Id = _dto.Id,
                Cidade = _dto.Cidade,
                IdEstado = _dto.IdEstado,
                CodigoIbge = _dto.CodigoIbge,
                Ddd = _dto.Ddd,
                Ativo = _dto.Ativo
            };

            if (cidade.Id == 0)
                await CidadesService.Inserir(cidade);
            else
                await CidadesService.Atualizar(cidade);

            await AoSalvarComSucesso.InvokeAsync(cidade);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }

    private void PedirConfirmacaoExclusao()
    {
        _modalConfirmacao.Abrir("Deseja realmente excluir a cidade \"" + _dto.Cidade + "\"?");
    }

    private async Task ConfirmarExclusao()
    {
        try
        {
            _erro = string.Empty;
            await CidadesService.Remover(_dto.Id);
            await AoExcluirComSucesso.InvokeAsync(_dto.Id);
            Fechar();
        }
        catch (Exception ex)
        {
            _erro = ex.Message;
        }
    }
}