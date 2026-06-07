using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Localizacao;
using SI_Bianca_Franca_2026.Services.App;

namespace SI_Bianca_Franca_2026.Services.Localizacao
{
    public class EstadosService
    {
        private readonly EstadosRepository _repository;
        private readonly IAppContextService _appContext;

        public EstadosService(EstadosRepository repository, IAppContextService appContext)
        {
            _repository = repository;
            _appContext = appContext;
        }

        public async Task<List<Estados>> ListarTudo()
        {
            return await _repository.ListarTudoAsync();
        }

        public async Task<Estados?> Pesquisar(int id)
        {
            return await _repository.PesquisarAsync(id);
        }

        public async Task Inserir(Estados entity)
        {
            bool jaExiste = await _repository.ExisteEstadoCadastradoAsync(entity.Estado, entity.Uf, entity.IdPais);
            if (jaExiste)
                throw new Exception("Já existe um estado cadastrado com este Nome ou UF para o país selecionado.");

            entity.DataCriacao = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Estados entity)
        {
            bool jaExiste = await _repository.ExisteEstadoCadastradoAsync(entity.Estado, entity.Uf, entity.IdPais, entity.Id);
            if (jaExiste)
                throw new Exception("Já existe outro estado cadastrado com este Nome ou UF neste país.");

            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            await _repository.AtualizarAsync(entity);
        }

        public async Task Desativar(int id)
        {
            await _repository.AlterarStatusAsync(id, false, _appContext.IdUsuarioAtual);
        }

        public async Task Ativar(int id)
        {
            await _repository.AlterarStatusAsync(id, true, _appContext.IdUsuarioAtual);
        }

        public async Task Remover(int id)
        {
            await _repository.RemoverAsync(id);
        }
    }
}