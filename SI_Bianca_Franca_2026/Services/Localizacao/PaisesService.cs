using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Localizacao;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Localizacao
{
    public class PaisesService : PaiService<Paises>
    {
        private readonly PaisesRepository _repository;

        public PaisesService(PaisesRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Paises>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<Paises?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(Paises entity)
        {
            bool jaExiste = await _repository.ExistePaisCadastradoAsync(entity.Pais, entity.Sigla);
            if (jaExiste)
                throw new Exception("Já existe um país cadastrado com esse Nome ou Sigla.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Paises entity)
        {
            bool jaExiste = await _repository.ExistePaisCadastradoAsync(entity.Pais, entity.Sigla, entity.Id);
            if (jaExiste)
                throw new Exception("Já existe um país cadastrado com esse Nome ou Sigla.");

            PreencherAtualizacao(entity);
            await _repository.AtualizarAsync(entity);
        }

        public async Task Desativar(int id)
            => await _repository.AlterarStatusAsync(id, false, _appContext.IdUsuarioAtual);

        public async Task Ativar(int id)
            => await _repository.AlterarStatusAsync(id, true, _appContext.IdUsuarioAtual);

        public async Task Remover(int id)
            => await _repository.RemoverAsync(id);
    }
}