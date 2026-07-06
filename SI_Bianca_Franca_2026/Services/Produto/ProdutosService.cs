using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Pessoa;
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

        public async Task<List<Produtos>> ListarTudo(FiltroProdutosDto filtro)
            => await _repository.ListarAsync(filtro);

        public async Task<Produtos?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task<int> Inserir(Produtos entity)
        {
            NormalizarTextos(entity);

            bool produtoExiste = await _repository.ExisteProdutoCadastradoAsync(entity.Produto);
            if (produtoExiste)
                throw new Exception("Já existe um produto cadastrado com este nome.");

            PreencherInsercao(entity);
            return await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Produtos entity)
        {
            NormalizarTextos(entity);

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
        {
            bool emUso = await _repository.EstaEmUsoAsync(id);
            if (emUso)
                throw new Exception("Este produto não pode ser excluído pois está em uso.");
            await _repository.RemoverAsync(id);
        }

        private static void NormalizarTextos(Produtos e)
        {
            e.Produto = e.Produto.Trim().ToUpperInvariant();
            e.Referencia = e.Referencia?.Trim().ToUpperInvariant();
            e.Ncm = e.Ncm?.Trim();
            e.Cest = e.Cest?.Trim();
            e.Observacao = e.Observacao?.Trim().ToUpperInvariant();
        }
    }
}