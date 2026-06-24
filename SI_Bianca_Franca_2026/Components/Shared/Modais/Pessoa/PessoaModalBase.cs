using SI_Bianca_Franca_2026.DTOs.Pessoa;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Services.Localizacao;
using SI_Bianca_Franca_2026.Components.Shared.Modais.Localizacao;
using Microsoft.AspNetCore.Components;

namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Pessoa
{
    public abstract class PessoaModalBase<TDto> : ComponentBase
        where TDto : PessoasDTO, new()
    {
        [Inject] protected PaisesService PaisesService { get; set; } = default!;
        [Inject] protected EstadosService EstadosService { get; set; } = default!;
        [Inject] protected CidadesService CidadesService { get; set; } = default!;

        protected TDto _dto = new();
        protected List<Paises> _paises = new();
        protected List<Estados> _estadosFiltrados = new();
        protected List<Cidades> _cidadesFiltradas = new();
        protected int _idEstadoSelecionado = 0;

        protected PaisesModal _modalPais = default!;
        protected EstadosModal _modalEstado = default!;
        protected CidadesModal _modalCidade = default!;

        protected bool EhBrasileiro => _paises
            .FirstOrDefault(p => p.Id == _dto.IdPais)?.Sigla == "BRA";

        protected async Task CarregarPaisesAsync()
        {
            var todos = await PaisesService.ListarTudo();
            _paises = todos.Where(p => p.Ativo).ToList();
        }

        protected async Task CarregarLocalizacaoDoRegistroAsync()
        {
            if (_dto.IdPais > 0)
            {
                var todos = await EstadosService.ListarPorPais(_dto.IdPais);
                _estadosFiltrados = todos
                    .Where(e => e.Ativo || e.Id == _idEstadoSelecionado)
                    .ToList();
            }

            if (_dto.IdCidade.HasValue)
            {
                var cidade = await CidadesService.Pesquisar(_dto.IdCidade.Value);
                if (cidade != null)
                {
                    _idEstadoSelecionado = cidade.IdEstado;

                    var todosEstados = await EstadosService.ListarPorPais(_dto.IdPais);
                    _estadosFiltrados = todosEstados
                        .Where(e => e.Ativo || e.Id == _idEstadoSelecionado)
                        .ToList();

                    var todasCidades = await CidadesService.ListarPorEstado(_idEstadoSelecionado);
                    _cidadesFiltradas = todasCidades
                        .Where(c => c.Ativo || c.Id == _dto.IdCidade)
                        .ToList();
                }
            }
        }

        protected async Task AoMudarPaisAsync()
        {
            _idEstadoSelecionado = 0;
            _dto.IdCidade = null;
            _cidadesFiltradas = new();

            _estadosFiltrados = _dto.IdPais > 0
                ? await EstadosService.ListarPorPais(_dto.IdPais)
                : new();
        }

        protected async Task AoMudarEstadoAsync()
        {
            _dto.IdCidade = null;

            _cidadesFiltradas = _idEstadoSelecionado > 0
                ? await CidadesService.ListarPorEstado(_idEstadoSelecionado)
                : new();
        }

        protected async Task OnPaisSalvoAsync(Paises pais)
        {
            await CarregarPaisesAsync();
            var novo = _paises.FirstOrDefault(p => p.Sigla == pais.Sigla);
            if (novo != null)
            {
                _dto.IdPais = novo.Id;
            }
        }

        protected async Task OnEstadoSalvoAsync(Estados estado)
        {
            _estadosFiltrados = await EstadosService.ListarPorPais(_dto.IdPais);
            var novo = _estadosFiltrados.FirstOrDefault(e => e.Uf == estado.Uf);
            if (novo != null)
            {
                _idEstadoSelecionado = novo.Id;
                await AoMudarEstadoAsync();
            }
        }

        protected async Task OnCidadeSalvaAsync(Cidades cidade)
        {
            _cidadesFiltradas = await CidadesService.ListarPorEstado(_idEstadoSelecionado);
            var nova = _cidadesFiltradas.FirstOrDefault(c => c.CodigoIbge == cidade.CodigoIbge);
            if (nova != null)
                _dto.IdCidade = nova.Id;
        }

        protected void AbrirModalNovoPais() => _modalPais.Abrir();

        protected void AbrirModalNovoEstado() => _modalEstado.Abrir(idPaisPreSelecionado: _dto.IdPais);

        protected void AbrirModalNovaCidade() => _modalCidade.Abrir(idEstadoPreSelecionado: _idEstadoSelecionado);
    }
}