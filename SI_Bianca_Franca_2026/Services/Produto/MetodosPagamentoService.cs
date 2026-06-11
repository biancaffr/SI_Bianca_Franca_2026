using SI_Bianca_Franca_2026.Models.Produto;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Produto
{
    public class MetodosPagamentoService : PaiService<MetodosPagamento>
    {
        private readonly MetodosPagamentoRepository _repository;

        public MetodosPagamentoService(MetodosPagamentoRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<MetodosPagamento>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<MetodosPagamento?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(MetodosPagamento entity)
        {
            bool codigoExiste = await _repository.ExisteCodigoCadastradoAsync(entity.Codigo);
            if (codigoExiste)
                throw new Exception("Já existe um método de pagamento cadastrado com este código.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(MetodosPagamento entity)
        {
            bool codigoExiste = await _repository.ExisteCodigoCadastradoAsync(entity.Codigo, entity.Id);
            if (codigoExiste)
                throw new Exception("Já existe outro método de pagamento cadastrado com este código.");

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
