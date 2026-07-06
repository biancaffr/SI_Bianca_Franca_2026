using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class CategoriasRepository
    {
        private readonly string _stringConexao;

        public CategoriasRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                c.id,
                c.categoria,
                c.data_criacao              AS DataCriacao,
                c.data_ultima_alteracao     AS DataUltimaAlteracao,
                c.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                c.ativo,
                u.nome                      AS NomeUsuarioAlteracao
            FROM categorias c
            LEFT JOIN usuarios u ON c.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE c.id = @Id";

        public async Task<List<Categorias>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("c.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("c.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add("c.categoria LIKE @Busca");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY c.categoria";

            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<Categorias>(sql, param)).ToList();
        }

        public async Task<Categorias?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<Categorias>(SqlPesquisar, new { Id = id });
        }

        public async Task InserirAsync(Categorias entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO categorias
                           (categoria, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Categoria, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Categorias entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE categorias SET
                           categoria                     = @Categoria,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE categorias SET
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
            await conexao.ExecuteAsync("DELETE FROM categorias WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCategoriaCadastradaAsync(string categoria, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM categorias
                           WHERE categoria = @Categoria AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Categoria = categoria, Id = ignorarId });
            return qtde > 0;
        }
    }
}
