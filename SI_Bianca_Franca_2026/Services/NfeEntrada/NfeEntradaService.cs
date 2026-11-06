using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.NfeEntrada;
using SI_Bianca_Franca_2026.Models.NfeEntrada;
using SI_Bianca_Franca_2026.Repositories.Financeiro;
using SI_Bianca_Franca_2026.Repositories.NfeEntrada;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;

namespace SI_Bianca_Franca_2026.Services.NfeEntrada
{
    public class NfeEntradaService
    {
        private readonly NfeEntradaRepository _repository;
        private readonly ProdutosRepository _produtosRepository;
        private readonly CondicoesPagamentosRepository _condicoesRepository;
        private readonly IAppContextService _appContext;

        public NfeEntradaService(
            NfeEntradaRepository repository,
            ProdutosRepository produtosRepository,
            CondicoesPagamentosRepository condicoesRepository,
            IAppContextService appContext)
        {
            _repository = repository;
            _produtosRepository = produtosRepository;
            _condicoesRepository = condicoesRepository;
            _appContext = appContext;
        }

        public async Task<List<NfeEntradas>> ListarTudo(FiltroBaseDto filtro)
            => await _repository.ListarAsync(filtro);

        public async Task<NfeEntradas?> Pesquisar(
            string numero, string modelo, string serie, int idFornecedor)
            => await _repository.PesquisarAsync(numero, modelo, serie, idFornecedor);

        public async Task<bool> ChaveExiste(
            string numero, string modelo, string serie, int idFornecedor)
            => await _repository.ChaveExisteAsync(numero, modelo, serie, idFornecedor);

        public async Task<List<ParcelaVisualizacaoDTO>> GerarParcelasVisualizacao(
    int idCondicao, decimal valorTotal, DateTime dataEmissao)
        {
            var condicao = await _condicoesRepository.PesquisarAsync(idCondicao);
            if (condicao is null || !condicao.Parcelas.Any()) return new();

            var parcelas = new List<ParcelaVisualizacaoDTO>();
            int total = condicao.Parcelas.Count;

            foreach (var p in condicao.Parcelas.OrderBy(x => x.NumeroParcela))
            {
                var valor = Math.Round(valorTotal * p.Percentual / 100, 2);
                var vencimento = dataEmissao.AddDays(p.PrazoDias);
                parcelas.Add(new ParcelaVisualizacaoDTO
                {
                    Numero = p.NumeroParcela,
                    Total = total,
                    Vencimento = vencimento,
                    Valor = valor,
                    MetodoPagamento = p.OMetodoPagamento?.Metodo ?? string.Empty
                });
            }
            return parcelas;
        }

        public async Task Inserir(NfeEntradas entity, List<NfeEntradaItens> itens)
        {
            if (!itens.Any())
                throw new Exception("A nota deve ter ao menos um produto.");

            if (await _repository.ChaveExisteAsync(
                entity.Numero, entity.Modelo, entity.Serie, entity.IdFornecedor))
                throw new Exception("Já existe uma nota com essa chave.");

            // Rateio de frete/seguro/outras por item proporcional ao valor total
            var totalItens = itens.Sum(i => i.ValorTotal);
            foreach (var item in itens)
            {
                var proporcao = totalItens > 0 ? item.ValorTotal / totalItens : 0;
                item.RateioFrete = Math.Round(entity.ValorFrete * proporcao, 4);
                item.RateioSeguro = Math.Round(entity.ValorSeguro * proporcao, 4);
                item.RateioOutras = Math.Round(entity.ValorOutrasDespesas * proporcao, 4);
                item.CustoFinal = Math.Round(
                    (item.ValorTotal + item.RateioFrete + item.RateioSeguro + item.RateioOutras)
                    / item.Quantidade, 4);
                item.NotaNumero = entity.Numero;
                item.NotaModelo = entity.Modelo;
                item.NotaSerie = entity.Serie;
                item.IdFornecedor = entity.IdFornecedor;
                item.DataCriacao = DateTime.Now;
                item.DataUltimaAlteracao = DateTime.Now;
            }

            entity.ValorProdutos = totalItens;
            entity.ValorTotal = totalItens + entity.ValorFrete
                + entity.ValorSeguro + entity.ValorOutrasDespesas - entity.ValorDesconto;
            entity.Observacao = entity.Observacao?.Trim().ToUpperInvariant();
            entity.Situacao = "CONFIRMADA";
            entity.DataCriacao = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            entity.Ativo = true;

            await _repository.InserirAsync(entity);
            await _repository.InserirItensAsync(itens);

            // Atualiza saldo e custo dos produtos
            foreach (var item in itens)
            {
                var produto = await _produtosRepository.PesquisarAsync(item.IdProduto);
                if (produto != null)
                {
                    produto.Saldo += item.Quantidade;
                    produto.ValorUltimaCompra = (decimal?)item.CustoFinal ?? produto.ValorUltimaCompra;
                    await _produtosRepository.AtualizarAsync(produto);
                }
            }

            // Gera contas a pagar se houver condição de pagamento
            if (entity.IdCondicaoPagamento.HasValue)
                await GerarContasPagar(entity);
        }

        public async Task Cancelar(
            string numero, string modelo, string serie, int idFornecedor)
        {
            var nota = await _repository.PesquisarAsync(numero, modelo, serie, idFornecedor)
                ?? throw new Exception("Nota não encontrada.");

            if (nota.Situacao == "CANCELADA")
                throw new Exception("Esta nota já está cancelada.");

            // Estorna saldo dos produtos
            foreach (var item in nota.Itens)
            {
                var produto = await _produtosRepository.PesquisarAsync(item.IdProduto);
                if (produto != null)
                {
                    produto.Saldo -= item.Quantidade;
                    if (produto.Saldo < 0) produto.Saldo = 0;
                    await _produtosRepository.AtualizarAsync(produto);
                }
            }

            await _repository.CancelarAsync(
                numero, modelo, serie, idFornecedor, _appContext.IdUsuarioAtual);
        }

        private async Task GerarContasPagar(NfeEntradas entity)
        {
            // TODO: implementar quando ContasPagarRepository for desenvolvido
            await Task.CompletedTask;
        }
    }
}