using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.DTOs.Estoque;
using SI_Bianca_Franca_2026.Models.Estoque;

namespace SI_Bianca_Franca_2026.Repositories.Estoque
{
    public class MovimentacoesEstoqueRepository
    {
        private readonly string _stringConexao;

        public MovimentacoesEstoqueRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
        }

        /// <summary>Saldo e custo médio do produto = os da última movimentação ativa dele.</summary>
        public async Task<EstoqueProdutoDTO> ObterEstoqueAsync(int idProduto)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var estoque = await conexao.QueryFirstOrDefaultAsync<EstoqueProdutoDTO>(@"
                SELECT saldo_apos  AS Saldo,
                       custo_medio AS CustoMedio
                FROM movimentacoes_estoque_itens
                WHERE id_produto = @IdProduto AND ativo = TRUE
                ORDER BY id DESC
                LIMIT 1",
                new { IdProduto = idProduto });
            return estoque ?? new EstoqueProdutoDTO();
        }

        /// <summary>Grava a movimentação e seus itens na mesma transação.</summary>
        public async Task<int> InserirAsync(MovimentacoesEstoque mov)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.OpenAsync();
            using var transacao = await conexao.BeginTransactionAsync();

            var idMov = await conexao.ExecuteScalarAsync<int>(@"
                INSERT INTO movimentacoes_estoque
                (data_movimentacao, tipo_movimentacao, id_usuario, id_venda, id_ordem_servico,
                 nota_numero, nota_modelo, nota_serie, id_fornecedor, observacao,
                 data_criacao, data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                VALUES
                (@DataMovimentacao, @TipoMovimentacao, @IdUsuario, @IdVenda, @IdOrdemServico,
                 @NotaNumero, @NotaModelo, @NotaSerie, @IdFornecedor, @Observacao,
                 @DataCriacao, @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo);
                SELECT LAST_INSERT_ID();",
                mov, transacao);

            foreach (var item in mov.Itens)
                item.IdMovimentacaoEstoque = idMov;

            await conexao.ExecuteAsync(@"
                INSERT INTO movimentacoes_estoque_itens
                (id_movimentacao_estoque, id_produto, quantidade, custo_unitario,
                 saldo_apos, custo_medio,
                 data_criacao, data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                VALUES
                (@IdMovimentacaoEstoque, @IdProduto, @Quantidade, @CustoUnitario,
                 @SaldoApos, @CustoMedio,
                 @DataCriacao, @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)",
                mov.Itens, transacao);

            await transacao.CommitAsync();
            return idMov;
        }
    }
}
