using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class SkuAtributosChavesService : PaiService<SkuAtributosChaves>
    {
        private readonly SkuAtributosChavesRepository _repository;

        public SkuAtributosChavesService(SkuAtributosChavesRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<SkuAtributosChaves>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<SkuAtributosChaves?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task<int> Inserir(SkuAtributosChaves entity)
        {
            bool chaveExiste = await _repository.ExisteChaveCadastradaAsync(entity.Chave);
            if (chaveExiste)
                throw new Exception("Já existe um atributo cadastrado com esta chave.");

            PreencherInsercao(entity);
            return await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(SkuAtributosChaves entity)
        {
            bool chaveExiste = await _repository.ExisteChaveCadastradaAsync(entity.Chave, entity.Id);
            if (chaveExiste)
                throw new Exception("Já existe outro atributo cadastrado com esta chave.");

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
