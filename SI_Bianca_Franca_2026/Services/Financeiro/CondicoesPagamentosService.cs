using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Repositories.Financeiro;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Financeiro
{
    public class CondicoesPagamentosService : PaiService<CondicoesPagamentos>
    {
        private readonly CondicoesPagamentosRepository _repository;

        public CondicoesPagamentosService(CondicoesPagamentosRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<CondicoesPagamentos>> ListarTudo(FiltroBaseDto filtro)
            => await _repository.ListarAsync(filtro);

        public async Task<CondicoesPagamentos?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(CondicoesPagamentos entity, List<CondicoesPagamentosParcelas> parcelas)
        {
            NormalizarTextos(entity);

            bool existe = await _repository.ExisteCondicaoCadastradaAsync(entity.CondicaoPagamento);
            if (existe)
                throw new Exception("Já existe uma condição de pagamento cadastrada com esta descrição.");

            ValidarParcelas(parcelas);

            PreencherInsercao(entity);
            int idGerado = await _repository.InserirAsync(entity);

            PreencherInsercaoParcelas(parcelas, idGerado);
            await _repository.InserirParcelasAsync(idGerado, parcelas);
        }

        public async Task Atualizar(CondicoesPagamentos entity, List<CondicoesPagamentosParcelas> parcelas)
        {
            NormalizarTextos(entity);

            bool existe = await _repository.ExisteCondicaoCadastradaAsync(entity.CondicaoPagamento, entity.Id);
            if (existe)
                throw new Exception("Já existe outra condição de pagamento cadastrada com esta descrição.");

            ValidarParcelas(parcelas);

            PreencherAtualizacao(entity);
            await _repository.AtualizarAsync(entity);

            await _repository.DeletarParcelasAsync(entity.Id);
            PreencherInsercaoParcelas(parcelas, entity.Id);
            await _repository.InserirParcelasAsync(entity.Id, parcelas);
        }

        public async Task Desativar(int id)
            => await _repository.AlterarStatusAsync(id, false, _appContext.IdUsuarioAtual);

        public async Task Ativar(int id)
            => await _repository.AlterarStatusAsync(id, true, _appContext.IdUsuarioAtual);

        public async Task Remover(int id)
            => await _repository.RemoverAsync(id);

        private static void NormalizarTextos(CondicoesPagamentos e)
        {
            e.CondicaoPagamento = e.CondicaoPagamento.Trim().ToUpperInvariant();
        }

        private static void ValidarParcelas(List<CondicoesPagamentosParcelas> parcelas)
        {
            if (!parcelas.Any())
                throw new Exception("A condição de pagamento deve ter ao menos uma parcela.");

            var total = parcelas.Sum(p => p.Percentual);
            if (Math.Abs(total - 100) > 0.01m)
                throw new Exception($"A soma dos percentuais deve totalizar 100%. Total atual: {total:N2}%");
        }

        private void PreencherInsercaoParcelas(List<CondicoesPagamentosParcelas> parcelas, int idCondicao)
        {
            for (int i = 0; i < parcelas.Count; i++)
            {
                parcelas[i].IdCondicaoPagamento = idCondicao;
                parcelas[i].NumeroParcela = i + 1;
                parcelas[i].DataCriacao = DateTime.Now;
                parcelas[i].DataUltimaAlteracao = DateTime.Now;
                parcelas[i].IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
                parcelas[i].Ativo = true;
            }
        }
    }
}