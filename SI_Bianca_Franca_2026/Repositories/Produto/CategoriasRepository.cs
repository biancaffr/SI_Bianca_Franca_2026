using MySql.Data.MySqlClient;
using Dapper;
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
                c.descricao,
                c.data_criacao              AS DataCriacao,
                c.data_ultima_alteracao     AS DataUltimaAlteracao,
                c.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                c.ativo,
                u.nome                      AS NomeUsuarioAlteracao
            FROM categorias c
            LEFT JOIN usuarios u ON c.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE c.id = @Id";

        public async Task<List<Categorias>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<Categorias>(SqlBase)).ToList();
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
                           (categoria, descricao, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Categoria, @Descricao, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Categorias entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE categorias SET
                           categoria                     = @Categoria,
                           descricao                     = @Descricao,
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
