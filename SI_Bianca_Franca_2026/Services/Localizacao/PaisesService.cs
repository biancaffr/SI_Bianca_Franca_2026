using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Localizacao;
using SI_Bianca_Franca_2026.Services.App;
using System.Runtime.Intrinsics.X86;

namespace SI_Bianca_Franca_2026.Services.Localizacao
{
    public class PaisesService
    {
        private readonly PaisesRepository _repository;
        private readonly IAppContextService _appContext;

        public PaisesService(PaisesRepository repository, IAppContextService appContext)
        {
            _repository = repository;
            _appContext = appContext;
        }

        public async Task<List<Paises>> ListarTudo()
        {
            return await _repository.ListarTudoAsync();
        }

        public async Task<Paises?> Pesquisar(int cod)
        {
            return await _repository.PesquisarAsync(cod);
        }

        public async Task Inserir(Paises entity)
        {
            bool jaExiste = await _repository.ExistePaisCadastradoAsync(entity.Pais, entity.Sigla);
            if (jaExiste)
                throw new Exception("Já existe um país cadastrado com esse Nome ou Sigla.");

            entity.DataCriacao = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Paises entity)
        {
            bool jaExiste = await _repository.ExistePaisCadastradoAsync(entity.Pais, entity.Sigla, entity.Id);
            if (jaExiste)
                throw new Exception("Já existe um país cadastrado com esse Nome ou Sigla.");

            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            await _repository.AtualizarAsync(entity);
        }

        public async Task Desativar(int cod)
        {
            await _repository.AlterarStatusAsync(cod, false, _appContext.IdUsuarioAtual);
        }

        public async Task Ativar(int cod)
        {
            await _repository.AlterarStatusAsync(cod, true, _appContext.IdUsuarioAtual);
        }

        public async Task Remover(int cod)
        {
            await _repository.RemoverAsync(cod);
        }
    }
}
