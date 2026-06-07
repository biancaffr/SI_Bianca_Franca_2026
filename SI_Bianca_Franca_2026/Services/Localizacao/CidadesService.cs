using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Localizacao;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Localizacao
{
    public class CidadesService : PaiService<Cidades>
    {
        private readonly CidadesRepository _repository;

        public CidadesService(CidadesRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Cidades>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<Cidades?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(Cidades entity)
        {
            bool cidadeExiste = await _repository.ExisteCidadeCadastradaAsync(entity.Cidade, entity.IdEstado);
            if (cidadeExiste)
                throw new Exception("Já existe uma cidade cadastrada com este nome neste estado.");

            bool ibgeExiste = await _repository.ExisteCodigoIbgeCadastradoAsync(entity.CodigoIbge);
            if (ibgeExiste)
                throw new Exception("Já existe uma cidade cadastrada com este código IBGE.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Cidades entity)
        {
            bool cidadeExiste = await _repository.ExisteCidadeCadastradaAsync(entity.Cidade, entity.IdEstado, entity.Id);
            if (cidadeExiste)
                throw new Exception("Já existe outra cidade cadastrada com este nome neste estado.");

            bool ibgeExiste = await _repository.ExisteCodigoIbgeCadastradoAsync(entity.CodigoIbge, entity.Id);
            if (ibgeExiste)
                throw new Exception("Já existe outra cidade cadastrada com este código IBGE.");

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