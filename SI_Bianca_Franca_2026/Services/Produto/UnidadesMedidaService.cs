using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class UnidadesMedidaService : PaiService<UnidadesMedida>
    {
        private readonly UnidadesMedidaRepository _repository;

        public UnidadesMedidaService(UnidadesMedidaRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

      
        public async Task<List<UnidadesMedida>> ListarTudo(FiltroBaseDto filtro)
            => await _repository.ListarAsync(filtro);

        public async Task<UnidadesMedida?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(UnidadesMedida entity)
        {
            NormalizarTextos(entity);

            bool siglaExiste = await _repository.ExisteSiglaCadastradaAsync(entity.Sigla);
            if (siglaExiste)
                throw new Exception("Já existe uma unidade de medida cadastrada com esta sigla.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(UnidadesMedida entity)
        {
            NormalizarTextos(entity);

            bool siglaExiste = await _repository.ExisteSiglaCadastradaAsync(entity.Sigla, entity.Id);
            if (siglaExiste)
                throw new Exception("Já existe outra unidade de medida cadastrada com esta sigla.");

            PreencherAtualizacao(entity);
            await _repository.AtualizarAsync(entity);
        }

        public async Task Desativar(int id)
            => await _repository.AlterarStatusAsync(id, false, _appContext.IdUsuarioAtual);

        public async Task Ativar(int id)
            => await _repository.AlterarStatusAsync(id, true, _appContext.IdUsuarioAtual);

        public async Task Remover(int id)
        {
            bool emUso = await _repository.EstaEmUsoAsync(id);
            if (emUso)
                throw new Exception("Esta unidade de medida não pode ser excluída pois está em uso.");
            await _repository.RemoverAsync(id);
        }

        private static void NormalizarTextos(UnidadesMedida e)
        {
            e.Sigla = e.Sigla.Trim().ToUpperInvariant();
            e.UnidadeMedida = e.UnidadeMedida.Trim().ToUpperInvariant();
        }
    }
}