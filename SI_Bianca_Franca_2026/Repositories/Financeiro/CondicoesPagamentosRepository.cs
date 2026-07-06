using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;

namespace SI_Bianca_Franca_2026.Repositories.Financeiro
{
    public class CondicoesPagamentosRepository
    {
        private readonly string _stringConexao;

        public CondicoesPagamentosRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
        }

        private static string SqlBase => @"
            SELECT
                c.id,
                c.condicao_pagamento        AS CondicaoPagamento,
                c.desconto_percentual       AS DescontoPercentual,
                c.multa_percentual          AS MultaPercentual,
                c.taxa_juros_percentual     AS TaxaJurosPercentual,
                c.data_criacao              AS DataCriacao,
                c.data_ultima_alteracao     AS DataUltimaAlteracao,
                c.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                c.ativo,
                u.nome                      AS NomeUsuarioAlteracao
            FROM condicoes_pagamentos c
            LEFT JOIN usuarios u ON c.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE c.id = @Id";

        public async Task<List<CondicoesPagamentos>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("c.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("c.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add("c.condicao_pagamento LIKE @Busca");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY c.condicao_pagamento";

            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = (await conexao.QueryAsync<CondicoesPagamentos>(sql, param)).ToList();
            foreach (var c in resultado)
                c.Parcelas = await ListarParcelasAsync(c.Id);
            return resultado;
        }

        public async Task<CondicoesPagamentos?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryFirstOrDefaultAsync<CondicoesPagamentos>(
                SqlPesquisar, new { Id = id });
            if (resultado != null)
                resultado.Parcelas = await ListarParcelasAsync(id);
            return resultado;
        }

        public async Task<List<CondicoesPagamentosParcelas>> ListarParcelasAsync(int idCondicao)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"
                SELECT
                    p.id,
                    p.id_condicao_pagamento     AS IdCondicaoPagamento,
                    p.numero_parcela            AS NumeroParcela,
                    p.percentual,
                    p.prazo_dias                AS PrazoDias,
                    p.id_metodo_pagamento       AS IdMetodoPagamento,
                    p.data_criacao              AS DataCriacao,
                    p.data_ultima_alteracao     AS DataUltimaAlteracao,
                    p.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                    p.ativo,
                    m.id                        AS MetodoId,
                    m.metodo
                FROM condicoes_pagamentos_parcelas p
                INNER JOIN metodos_pagamento m ON p.id_metodo_pagamento = m.id
                WHERE p.id_condicao_pagamento = @IdCondicao
                ORDER BY p.numero_parcela";

            var resultado = await conexao.QueryAsync<CondicoesPagamentosParcelas, MetodosPagamento, CondicoesPagamentosParcelas>(
                sql,
                (parcela, metodo) => { parcela.OMetodoPagamento = metodo; return parcela; },
                splitOn: "MetodoId",
                param: new { IdCondicao = idCondicao });
            return resultado.ToList();
        }

        public async Task<int> InserirAsync(CondicoesPagamentos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO condicoes_pagamentos
                           (condicao_pagamento, desconto_percentual,
                            multa_percentual, taxa_juros_percentual,
                            data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@CondicaoPagamento, @DescontoPercentual,
                            @MultaPercentual, @TaxaJurosPercentual,
                            @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo);
                           SELECT LAST_INSERT_ID();";
            return await conexao.ExecuteScalarAsync<int>(sql, entity);
        }

        public async Task AtualizarAsync(CondicoesPagamentos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE condicoes_pagamentos SET
                           condicao_pagamento          = @CondicaoPagamento,
                           desconto_percentual         = @DescontoPercentual,
                           multa_percentual            = @MultaPercentual,
                           taxa_juros_percentual       = @TaxaJurosPercentual,
                           data_ultima_alteracao       = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao = @IdUsuarioUltimaAlteracao,
                           ativo                       = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task InserirParcelasAsync(int idCondicao, List<CondicoesPagamentosParcelas> parcelas)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO condicoes_pagamentos_parcelas
                           (id_condicao_pagamento, numero_parcela, percentual, prazo_dias,
                            id_metodo_pagamento, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdCondicaoPagamento, @NumeroParcela, @Percentual, @PrazoDias,
                            @IdMetodoPagamento, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, parcelas);
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
            await conexao.ExecuteAsync(@"UPDATE condicoes_pagamentos SET
                           ativo                       = @Ativo,
                           data_ultima_alteracao       = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao = @IdUsuario
                           WHERE id = @Id",
                new { Ativo = novoStatus, DataUltimaAlteracao = DateTime.Now, IdUsuario = idUsuario, Id = id });
        }

        public async Task<bool> EstaEmUsoAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM (
                SELECT id_condicao_pagamento FROM clientes         WHERE id_condicao_pagamento = @Id
                UNION ALL
                SELECT id_condicao_pagamento FROM contas_pagar     WHERE id_condicao_pagamento = @Id
                UNION ALL
                SELECT id_condicao_pagamento FROM contas_receber   WHERE id_condicao_pagamento = @Id
            ) AS referencias";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql, new { Id = id });
            return qtde > 0;
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

        public async Task<bool> ExisteCondicaoCadastradaAsync(string condicao, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM condicoes_pagamentos WHERE condicao_pagamento = @Condicao AND id != @IgnorarId",
                new { Condicao = condicao, IgnorarId = ignorarId });
            return qtde > 0;
        }
    }
}