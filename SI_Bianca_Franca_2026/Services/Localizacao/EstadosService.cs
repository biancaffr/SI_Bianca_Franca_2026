using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Localizacao;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Localizacao
{
    public class EstadosService : PaiService<Estados>
    {
        private readonly EstadosRepository _repository;

        public EstadosService(EstadosRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Estados>> ListarTudo(FiltroBaseDto filtro)
            => await _repository.ListarAsync(filtro);
        public async Task<List<Estados>> ListarPorPais(int idPais)
            => await _repository.ListarPorPaisAsync(idPais);

        public async Task<Estados?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(Estados entity)
        {
            NormalizarTextos(entity);

            bool jaExiste = await _repository.ExisteEstadoCadastradoAsync(entity.Estado, entity.Uf, entity.IdPais);
            if (jaExiste)
                throw new Exception("Já existe um estado cadastrado com este Nome ou UF para o país selecionado.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Estados entity)
        {
            NormalizarTextos(entity);

            bool jaExiste = await _repository.ExisteEstadoCadastradoAsync(entity.Estado, entity.Uf, entity.IdPais, entity.Id);
            if (jaExiste)
                throw new Exception("Já existe outro estado cadastrado com este Nome ou UF neste país.");

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
                throw new Exception("Este estado não pode ser excluído pois está em uso.");

            await _repository.RemoverAsync(id);
        }

        private static void NormalizarTextos(Estados e)
        {
            e.Estado = e.Estado.Trim().ToUpperInvariant();
            e.Uf = e.Uf.Trim().ToUpperInvariant();
        }
    }
}