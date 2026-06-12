using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class ProdutosService : PaiService<Produtos>
    {
        private readonly ProdutosRepository _repository;

        public ProdutosService(ProdutosRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Produtos>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<List<Produtos>> ListarPorTipo(string tipo)
            => await _repository.ListarPorTipoAsync(tipo);

        public async Task<Produtos?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task<int> Inserir(Produtos entity)
        {
            bool produtoExiste = await _repository.ExisteProdutoCadastradoAsync(entity.Produto);
            if (produtoExiste)
                throw new Exception("Já existe um produto cadastrado com este nome.");

            PreencherInsercao(entity);
            return await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Produtos entity)
        {
            bool produtoExiste = await _repository.ExisteProdutoCadastradoAsync(entity.Produto, entity.Id);
            if (produtoExiste)
                throw new Exception("Já existe outro produto cadastrado com este nome.");

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
