using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class UnidadesMedidaRepository
    {
        private readonly string _stringConexao;

        public UnidadesMedidaRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                u.id,
                u.sigla,
                u.unidade_medida            AS UnidadeMedida,
                u.data_criacao              AS DataCriacao,
                u.data_ultima_alteracao     AS DataUltimaAlteracao,
                u.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                u.ativo,
                us.nome                     AS NomeUsuarioAlteracao
            FROM unidades_medida u
            LEFT JOIN usuarios us ON u.id_usuario_ultima_alteracao = us.id";

        private static string SqlPesquisar => SqlBase + " WHERE u.id = @Id";

        public async Task<List<UnidadesMedida>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("u.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("u.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add(@"(u.sigla LIKE @Busca
                  OR u.unidade_medida LIKE @Busca)");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY u.id";

            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<UnidadesMedida>(sql, param)).ToList();
        }

        public async Task<UnidadesMedida?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<UnidadesMedida>(SqlPesquisar, new { Id = id });
        }

        public async Task InserirAsync(UnidadesMedida entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO unidades_medida
                           (sigla, unidade_medida, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Sigla, @UnidadeMedida, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(UnidadesMedida entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE unidades_medida SET
                           sigla                         = @Sigla,
                           unidade_medida                = @UnidadeMedida,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE unidades_medida SET
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

        public async Task<bool> EstaEmUsoAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM (
                SELECT id_unidade_medida FROM produtos   WHERE id_unidade_medida = @Id
                UNION ALL
                SELECT id_unidade_medida FROM nfes_itens WHERE id_unidade_medida = @Id
            ) AS referencias";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql, new { Id = id });
            return qtde > 0;
        }
        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync("DELETE FROM unidades_medida WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteSiglaCadastradaAsync(string sigla, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM unidades_medida
                           WHERE sigla = @Sigla AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Sigla = sigla, Id = ignorarId });
            return qtde > 0;
        }
    }
}
