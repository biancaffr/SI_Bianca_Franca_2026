using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Financeiro;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Financeiro
{
    public class MetodosPagamentoService : PaiService<MetodosPagamento>
    {
        private readonly MetodosPagamentoRepository _repository;

        public MetodosPagamentoService(MetodosPagamentoRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<MetodosPagamento>> ListarTudo(FiltroBaseDto filtro)
            => await _repository.ListarAsync(filtro);

        public async Task<MetodosPagamento?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(MetodosPagamento entity)
        {
            NormalizarTextos(entity);

            bool codigoExiste = await _repository.ExisteSiglaCadastradaAsync(entity.Sigla);
            if (codigoExiste)
                throw new Exception("Já existe um método de pagamento cadastrado com este código.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(MetodosPagamento entity)
        {
            NormalizarTextos(entity);

            bool codigoExiste = await _repository.ExisteSiglaCadastradaAsync(entity.Sigla, entity.Id);
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

        private static void NormalizarTextos(MetodosPagamento m)
        {
            m.Sigla = m.Sigla.Trim().ToUpperInvariant();
            m.Metodo = m.Metodo.Trim().ToUpperInvariant();
        }
    }
}
