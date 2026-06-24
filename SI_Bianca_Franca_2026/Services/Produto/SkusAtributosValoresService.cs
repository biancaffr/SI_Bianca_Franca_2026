using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class SkusAtributosValoresService : PaiService<SkusAtributosValores>
    {
        private readonly SkusAtributosValoresRepository _repository;

        public SkusAtributosValoresService(SkusAtributosValoresRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<SkusAtributosValores>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<List<SkusAtributosValores>> ListarPorChave(int idChave)
            => await _repository.ListarPorChaveAsync(idChave);

        public async Task<SkusAtributosValores?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task<int> Inserir(SkusAtributosValores entity)
        {
            bool jaExiste = await _repository.ExisteValorCadastradoAsync(entity.IdChave, entity.Valor);
            if (jaExiste)
                throw new Exception("Este valor já está cadastrado para esta chave.");

            PreencherInsercao(entity);
            return await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(SkusAtributosValores entity)
        {
            bool jaExiste = await _repository.ExisteValorCadastradoAsync(entity.IdChave, entity.Valor, entity.Id);
            if (jaExiste)
                throw new Exception("Este valor já está cadastrado para esta chave.");

            PreencherAtualizacao(entity);
            await _repository.AtualizarAsync(entity);
        }

        public async Task Desativar(int id)
        {
            bool emUso = await _repository.EstaEmUsoAsync(id);

            if (emUso)
            {
                var valor = await _repository.PesquisarAsync(id);
                if (valor != null)
                {
                    valor.Ativo = false;
                    await _repository.AtualizarAsync(valor);
                }
            }
            else
            {
                await _repository.RemoverAsync(id);
            }
        }
        public async Task Ativar(int id)
        {
            var valor = await _repository.PesquisarAsync(id);
            if (valor != null)
            {
                valor.Ativo = true;
                await _repository.AtualizarAsync(valor);
            }
        }
    }
}