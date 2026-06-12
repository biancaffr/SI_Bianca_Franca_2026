using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class SkusService
    {
        private readonly SkusRepository _repository;
        private readonly IAppContextService _appContext;

        public SkusService(SkusRepository repository, IAppContextService appContext)
        {
            _repository = repository;
            _appContext = appContext;
        }

        public async Task<List<Skus>> ListarPorProduto(int idProduto)
            => await _repository.ListarPorProdutoAsync(idProduto);

        public async Task<Skus?> Pesquisar(string sku)
            => await _repository.PesquisarAsync(sku);

        public async Task Inserir(Skus entity, List<SkusAtributosValores> atributos)
        {
            bool skuExiste = await _repository.ExisteSkuCadastradoAsync(entity.Sku);
            if (skuExiste)
                throw new Exception("Já existe um SKU cadastrado com este código.");

            if (!string.IsNullOrWhiteSpace(entity.GtinEan))
            {
                bool gtinExiste = await _repository.ExisteGtinCadastradoAsync(entity.GtinEan);
                if (gtinExiste)
                    throw new Exception("Já existe um SKU cadastrado com este GTIN/EAN.");
            }

            entity.DataCriacao = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            entity.Ativo = true;

            await _repository.InserirAsync(entity);

            if (atributos.Any())
            {
                foreach (var a in atributos) a.Sku = entity.Sku;
                await _repository.InserirAtributosAsync(entity.Sku, atributos);
            }
        }

        public async Task Atualizar(Skus entity, List<SkusAtributosValores> atributos)
        {
            if (!string.IsNullOrWhiteSpace(entity.GtinEan))
            {
                bool gtinExiste = await _repository.ExisteGtinCadastradoAsync(entity.GtinEan, entity.Sku);
                if (gtinExiste)
                    throw new Exception("Já existe outro SKU cadastrado com este GTIN/EAN.");
            }

            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;

            await _repository.AtualizarAsync(entity);
            await _repository.DeletarAtributosAsync(entity.Sku);

            if (atributos.Any())
            {
                foreach (var a in atributos) a.Sku = entity.Sku;
                await _repository.InserirAtributosAsync(entity.Sku, atributos);
            }
        }

        public async Task Desativar(string sku)
            => await _repository.AlterarStatusAsync(sku, false, _appContext.IdUsuarioAtual);

        public async Task Ativar(string sku)
            => await _repository.AlterarStatusAsync(sku, true, _appContext.IdUsuarioAtual);

        public async Task Remover(string sku)
            => await _repository.RemoverAsync(sku);
    }
}
