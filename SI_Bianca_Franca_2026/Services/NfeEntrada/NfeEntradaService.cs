using SI_Bianca_Franca_2026.DTOs.Estoque;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.DTOs.NfeEntrada;
using SI_Bianca_Franca_2026.Models.Estoque;
using SI_Bianca_Franca_2026.Models.NfeEntrada;
using SI_Bianca_Franca_2026.Repositories.Financeiro;
using SI_Bianca_Franca_2026.Repositories.NfeEntrada;
using SI_Bianca_Franca_2026.Repositories.Produto;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Estoque;

namespace SI_Bianca_Franca_2026.Services.NfeEntrada
{
    public class NfeEntradaService
    {
        private readonly NfeEntradaRepository _repository;
        private readonly ProdutosRepository _produtosRepository;
        private readonly CondicoesPagamentosRepository _condicoesRepository;
        private readonly MovimentacoesEstoqueService _estoqueService;
        private readonly IAppContextService _appContext;

        public NfeEntradaService(
            NfeEntradaRepository repository,
            ProdutosRepository produtosRepository,
            CondicoesPagamentosRepository condicoesRepository,
            MovimentacoesEstoqueService estoqueService,
            IAppContextService appContext)
        {
            _repository = repository;
            _produtosRepository = produtosRepository;
            _condicoesRepository = condicoesRepository;
            _estoqueService = estoqueService;
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
            var ordenadas = condicao.Parcelas.OrderBy(x => x.NumeroParcela).ToList();
            int total = ordenadas.Count;

            foreach (var p in ordenadas)
            {
                var valor = Math.Round(valorTotal * p.Percentual / 100, 2, MidpointRounding.AwayFromZero);
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

            // A parcela de MAIOR percentual (a primeira, em caso de empate) compensa a diferença
            // de centavos, para que a soma das parcelas feche exatamente com o total da nota.
            var maiorPercentual = ordenadas.Max(x => x.Percentual);
            var idxCompensa = ordenadas.FindIndex(x => x.Percentual == maiorPercentual);
            var somaDasOutras = parcelas.Where((_, i) => i != idxCompensa).Sum(x => x.Valor);
            parcelas[idxCompensa].Valor = Math.Round(valorTotal, 2, MidpointRounding.AwayFromZero) - somaDasOutras;

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
            var produtos = new Dictionary<int, Models.Produto.Produtos>();
            var estoque = new Dictionary<int, EstoqueProdutoDTO>();
            var movimentacao = new MovimentacoesEstoque
            {
                TipoMovimentacao = "ENTRADA",
                NotaNumero = entity.Numero,
                NotaModelo = entity.Modelo,
                NotaSerie = entity.Serie,
                IdFornecedor = entity.IdFornecedor,
                Observacao = $"ENTRADA PELA NF-E Nº {entity.Numero} (MODELO {entity.Modelo}, SÉRIE {entity.Serie})"
            };
            foreach (var item in itens)
            {
                var proporcao = totalItens > 0 ? item.ValorTotal / totalItens : 0;
                item.RateioFrete = Math.Round(entity.ValorFrete * proporcao, 4);
                item.RateioSeguro = Math.Round(entity.ValorSeguro * proporcao, 4);
                item.RateioOutras = Math.Round(entity.ValorOutrasDespesas * proporcao, 4);
                item.CustoFinal = Math.Round(
                    (item.ValorTotal + item.RateioFrete + item.RateioSeguro + item.RateioOutras)
                    / item.Quantidade, 4);

                if (!produtos.TryGetValue(item.IdProduto, out var produto))
                {
                    produto = await _produtosRepository.PesquisarAsync(item.IdProduto)
                        ?? throw new Exception("Produto não encontrado.");
                    produtos[item.IdProduto] = produto;
                    estoque[item.IdProduto] = await _estoqueService.ObterEstoque(item.IdProduto);
                }
                produto.ValorUltimaCompra = Math.Round(item.CustoFinal ?? 0, 2);

                // Custo médio ponderado: usa saldo e custo médio do produto ANTES desta entrada
                // (sem movimentação anterior, saldo e custo médio são 0)
                var atual = estoque[item.IdProduto];
                item.CustoMedio = MovimentacoesEstoqueService.CalcularCustoMedio(
                    atual.Saldo, atual.CustoMedio, item.Quantidade, item.CustoFinal ?? 0);
                atual.Saldo += item.Quantidade;
                atual.CustoMedio = item.CustoMedio.Value;

                movimentacao.Itens.Add(new MovimentacoesEstoqueItens
                {
                    IdProduto = item.IdProduto,
                    Quantidade = item.Quantidade,
                    CustoUnitario = item.CustoFinal ?? 0,
                    SaldoApos = atual.Saldo,
                    CustoMedio = atual.CustoMedio
                });

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

            // Entrada no estoque (kardex) e valor da última compra dos produtos
            await _estoqueService.Registrar(movimentacao);
            foreach (var produto in produtos.Values)
                await _produtosRepository.AtualizarAsync(produto);

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

            // Estorna o estoque: saída pelo custo médio atual (o custo médio não muda na saída)
            var estoque = new Dictionary<int, EstoqueProdutoDTO>();
            var estorno = new MovimentacoesEstoque
            {
                TipoMovimentacao = "SAIDA",
                NotaNumero = numero,
                NotaModelo = modelo,
                NotaSerie = serie,
                IdFornecedor = idFornecedor,
                Observacao = $"ESTORNO POR CANCELAMENTO DA NF-E Nº {numero} (MODELO {modelo}, SÉRIE {serie})"
            };
            foreach (var item in nota.Itens)
            {
                if (!estoque.TryGetValue(item.IdProduto, out var atual))
                {
                    atual = await _estoqueService.ObterEstoque(item.IdProduto);
                    estoque[item.IdProduto] = atual;
                }
                atual.Saldo = Math.Max(0, atual.Saldo - item.Quantidade);

                estorno.Itens.Add(new MovimentacoesEstoqueItens
                {
                    IdProduto = item.IdProduto,
                    Quantidade = item.Quantidade,
                    CustoUnitario = atual.CustoMedio,
                    SaldoApos = atual.Saldo,
                    CustoMedio = atual.CustoMedio
                });
            }
            await _estoqueService.Registrar(estorno);

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