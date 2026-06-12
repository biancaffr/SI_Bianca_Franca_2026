using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class FichasTecnicasService
    {
        private readonly FichasTecnicasRepository _repository;
        private readonly IAppContextService _appContext;

        public FichasTecnicasService(FichasTecnicasRepository repository, IAppContextService appContext)
        {
            _repository = repository;
            _appContext = appContext;
        }

        public async Task<List<FichasTecnicas>> ListarPorProduto(int idProduto)
            => await _repository.ListarPorProdutoAsync(idProduto);

        public async Task Inserir(FichasTecnicas entity)
        {
            bool jaExiste = await _repository.ExisteMaterialNaFichaAsync(entity.IdProduto, entity.IdProdutoMaterial);
            if (jaExiste)
                throw new Exception("Este material já está cadastrado na ficha técnica deste produto.");

            entity.DataCriacao = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            entity.Ativo = true;

            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(FichasTecnicas entity)
        {
            bool jaExiste = await _repository.ExisteMaterialNaFichaAsync(
                entity.IdProduto, entity.IdProdutoMaterial, entity.Id);
            if (jaExiste)
                throw new Exception("Este material já está cadastrado na ficha técnica deste produto.");

            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;

            await _repository.AtualizarAsync(entity);
        }

        public async Task Remover(int id)
            => await _repository.RemoverAsync(id);
    }
}
