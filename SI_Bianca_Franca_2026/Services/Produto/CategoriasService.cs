using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class CategoriasService : PaiService<Categorias>
    {
        private readonly CategoriasRepository _repository;

        public CategoriasService(CategoriasRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Categorias>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<Categorias?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(Categorias entity)
        {
            bool categoriaExiste = await _repository.ExisteCategoriaCadastradaAsync(entity.Categoria);
            if (categoriaExiste)
                throw new Exception("Já existe uma categoria cadastrada com este nome.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Categorias entity)
        {
            bool categoriaExiste = await _repository.ExisteCategoriaCadastradaAsync(entity.Categoria, entity.Id);
            if (categoriaExiste)
                throw new Exception("Já existe outra categoria cadastrada com este nome.");

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
