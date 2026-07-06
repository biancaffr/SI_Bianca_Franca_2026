using SI_Bianca_Franca_2026.DTOs.Filtros;
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
        [Inject] protected CidadesService CidadesService { get; set; } = default!;

        protected TDto _dto = new();
        protected List<Cidades> _cidades = new();
        protected CidadesModal _modalCidade = default!;

        protected bool EhBrasileiro =>
            _cidades.FirstOrDefault(c => c.Id == _dto.IdCidade)?.OEstado?.OPais?.Sigla == "BRA";

        protected async Task CarregarCidadesAsync()
        {
            var todas = await CidadesService.ListarTudo(new FiltroBaseDto { StatusFiltro = "todos" });
            _cidades = todas
                .Where(c => c.Ativo || c.Id == _dto.IdCidade)
                .OrderBy(c => c.Cidade)
                .ToList();
        }

        protected string FormatarCidade(Cidades cidade) =>
            $"{cidade.Cidade} - {cidade.OEstado?.Uf}";

        protected async Task OnCidadeSalvaAsync(Cidades cidade)
        {
            await CarregarCidadesAsync();
            var nova = _cidades.FirstOrDefault(c => c.CodigoIbge == cidade.CodigoIbge);
            if (nova != null)
                _dto.IdCidade = nova.Id;
        }

        protected async Task AbrirModalNovaCidade() => await _modalCidade.Abrir();
    }
}