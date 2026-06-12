using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Financeiro
{
    public class CondicoesPagamentosRepository
    {
        private readonly string _stringConexao;

        public CondicoesPagamentosRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                cp.id,
                cp.descricao,
                cp.id_metodo_pagamento          AS IdMetodoPagamento,
                cp.entrada_minima_percentual    AS EntradaMinimaPercentual,
                cp.desconto_percentual          AS DescontoPercentual,
                cp.acrescimo_percentual         AS AcrescimoPercentual,
                cp.multa_percentual             AS MultaPercentual,
                cp.taxa_juros_percentual        AS TaxaJurosPercentual,
                cp.data_criacao                 AS DataCriacao,
                cp.data_ultima_alteracao        AS DataUltimaAlteracao,
                cp.id_usuario_ultima_alteracao  AS IdUsuarioUltimaAlteracao,
                cp.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                m.id,
                m.codigo,
                m.descricao
            FROM condicoes_pagamentos cp
            LEFT JOIN usuarios u            ON cp.id_usuario_ultima_alteracao = u.id
            INNER JOIN metodos_pagamento m  ON cp.id_metodo_pagamento = m.id";

        private static string SqlPesquisar => SqlBase + " WHERE cp.id = @Id";

        private static CondicoesPagamentos MapearCondicao(CondicoesPagamentos condicao, MetodosPagamento metodo)
        {
            condicao.OMetodoPagamento = metodo;
            return condicao;
        }

        public async Task<List<CondicoesPagamentos>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<CondicoesPagamentos, MetodosPagamento, CondicoesPagamentos>(
                SqlBase,
                MapearCondicao,
                splitOn: "id"
            );
            return resultado.ToList();
        }

        public async Task<CondicoesPagamentos?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<CondicoesPagamentos, MetodosPagamento, CondicoesPagamentos>(
                SqlPesquisar,
                MapearCondicao,
                splitOn: "id",
                param: new { Id = id }
            );

            var condicao = resultado.FirstOrDefault();
            if (condicao != null)
                condicao.Parcelas = await ListarParcelasAsync(id);

            return condicao;
        }

        public async Task<List<CondicoesPagamentosParcelas>> ListarParcelasAsync(int idCondicao)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT
                           id,
                           id_condicao_pagamento   AS IdCondicaoPagamento,
                           numero_parcela          AS NumeroParcela,
                           percentual,
                           prazo_dias              AS PrazoDias,
                           data_criacao            AS DataCriacao,
                           data_ultima_alteracao   AS DataUltimaAlteracao,
                           id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                           ativo
                           FROM condicoes_pagamentos_parcelas
                           WHERE id_condicao_pagamento = @IdCondicao
                           ORDER BY numero_parcela";
            return (await conexao.QueryAsync<CondicoesPagamentosParcelas>(sql,
                new { IdCondicao = idCondicao })).ToList();
        }

        public async Task<int> InserirAsync(CondicoesPagamentos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO condicoes_pagamentos
                           (descricao, id_metodo_pagamento, entrada_minima_percentual,
                            desconto_percentual, acrescimo_percentual, multa_percentual,
                            taxa_juros_percentual, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Descricao, @IdMetodoPagamento, @EntradaMinimaPercentual,
                            @DescontoPercentual, @AcrescimoPercentual, @MultaPercentual,
                            @TaxaJurosPercentual, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo);
                           SELECT LAST_INSERT_ID();";
            return await conexao.ExecuteScalarAsync<int>(sql, entity);
        }

        public async Task InserirParcelasAsync(int idCondicao, List<CondicoesPagamentosParcelas> parcelas)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO condicoes_pagamentos_parcelas
                           (id_condicao_pagamento, numero_parcela, percentual, prazo_dias,
                            data_criacao, data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdCondicaoPagamento, @NumeroParcela, @Percentual, @PrazoDias,
                            @DataCriacao, @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, parcelas);
        }

        public async Task AtualizarAsync(CondicoesPagamentos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE condicoes_pagamentos SET
                           descricao                     = @Descricao,
                           id_metodo_pagamento           = @IdMetodoPagamento,
                           entrada_minima_percentual     = @EntradaMinimaPercentual,
                           desconto_percentual           = @DescontoPercentual,
                           acrescimo_percentual          = @AcrescimoPercentual,
                           multa_percentual              = @MultaPercentual,
                           taxa_juros_percentual         = @TaxaJurosPercentual,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task DeletarParcelasAsync(int idCondicao)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(
                "DELETE FROM condicoes_pagamentos_parcelas WHERE id_condicao_pagamento = @Id",
                new { Id = idCondicao });
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE condicoes_pagamentos SET
                           ativo                         = @Ativo,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuario
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, new
            {
                Ativo = novoStatus,
                DataUltimaAlteracao = DateTime.Now,
                IdUsuario = idUsuario,
                Id = id
            });
        }

        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(
                "DELETE FROM condicoes_pagamentos_parcelas WHERE id_condicao_pagamento = @Id",
                new { Id = id });
            await conexao.ExecuteAsync(
                "DELETE FROM condicoes_pagamentos WHERE id = @Id",
                new { Id = id });
        }

        public async Task<bool> ExisteDescricaoCadastradaAsync(string descricao, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM condicoes_pagamentos
                           WHERE descricao = @Descricao AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Descricao = descricao, Id = ignorarId });
            return qtde > 0;
        }
    }
}
