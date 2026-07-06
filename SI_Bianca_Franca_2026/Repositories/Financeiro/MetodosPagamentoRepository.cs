using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;

namespace SI_Bianca_Franca_2026.Repositories.Financeiro
{
    public class MetodosPagamentoRepository
    {
        private readonly string _stringConexao;

        public MetodosPagamentoRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
        }

        private static string SqlBase => @"
            SELECT
                m.id,
                m.sigla,
                m.metodo,
                m.data_criacao              AS DataCriacao,
                m.data_ultima_alteracao     AS DataUltimaAlteracao,
                m.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                m.ativo,
                u.nome                      AS NomeUsuarioAlteracao
            FROM metodos_pagamento m
            LEFT JOIN usuarios u ON m.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE m.id = @Id";

        public async Task<List<MetodosPagamento>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("m.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("m.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add("(m.sigla LIKE @Busca OR m.metodo LIKE @Busca)");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY m.metodo";

            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<MetodosPagamento>(sql, param)).ToList();
        }

        public async Task<MetodosPagamento?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<MetodosPagamento>(SqlPesquisar, new { Id = id });
        }

        public async Task InserirAsync(MetodosPagamento entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO metodos_pagamento
                           (sigla, metodo, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Sigla, @Metodo, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(MetodosPagamento entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE metodos_pagamento SET
                           sigla                         = @Sigla,
                           metodo                        = @Metodo,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(@"UPDATE metodos_pagamento SET
                           ativo                         = @Ativo,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuario
                           WHERE id = @Id",
                new { Ativo = novoStatus, DataUltimaAlteracao = DateTime.Now, IdUsuario = idUsuario, Id = id });
        }

        public async Task<bool> EstaEmUsoAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM (
                SELECT id_metodo_pagamento FROM condicoes_pagamentos_parcelas WHERE id_metodo_pagamento = @Id
                UNION ALL
                SELECT id_metodo_pagamento FROM nfes_pagamentos WHERE id_metodo_pagamento = @Id
            ) AS referencias";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql, new { Id = id });
            return qtde > 0;
        }

        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync("DELETE FROM metodos_pagamento WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteSiglaCadastradaAsync(string sigla, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM metodos_pagamento WHERE sigla = @Sigla AND id != @Id",
                new { Sigla = sigla, Id = ignorarId });
            return qtde > 0;
        }
    }
}