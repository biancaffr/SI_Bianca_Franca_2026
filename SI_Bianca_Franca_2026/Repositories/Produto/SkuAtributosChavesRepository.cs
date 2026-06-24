using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class SkuAtributosChavesRepository
    {
        private readonly string _stringConexao;

        public SkuAtributosChavesRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                s.id,
                s.chave,
                s.data_criacao              AS DataCriacao,
                s.data_ultima_alteracao     AS DataUltimaAlteracao,
                s.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                s.ativo,
                u.nome                      AS NomeUsuarioAlteracao
            FROM sku_atributos_chaves s
            LEFT JOIN usuarios u ON s.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE s.id = @Id";

        public async Task<List<SkuAtributosChaves>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<SkuAtributosChaves>(SqlBase)).ToList();
        }

        public async Task<SkuAtributosChaves?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<SkuAtributosChaves>(SqlPesquisar, new { Id = id });
        }

        public async Task<int> InserirAsync(SkuAtributosChaves entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO sku_atributos_chaves
                   (chave, data_criacao, data_ultima_alteracao,
                    id_usuario_ultima_alteracao, ativo)
                   VALUES
                   (@Chave, @DataCriacao, @DataUltimaAlteracao,
                    @IdUsuarioUltimaAlteracao, @Ativo);
                   SELECT LAST_INSERT_ID();";
            return await conexao.ExecuteScalarAsync<int>(sql, entity);
        }

        public async Task AtualizarAsync(SkuAtributosChaves entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE sku_atributos_chaves SET
                           chave                         = @Chave,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE sku_atributos_chaves SET
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
            await conexao.ExecuteAsync("DELETE FROM sku_atributos_chaves WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteChaveCadastradaAsync(string chave, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM sku_atributos_chaves
                           WHERE chave = @Chave AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Chave = chave, Id = ignorarId });
            return qtde > 0;
        }
    }
}
