using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class SkusAtributosValoresRepository
    {
        private readonly string _stringConexao;

        public SkusAtributosValoresRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                v.id,
                v.id_chave                  AS IdChave,
                v.valor,
                v.data_criacao              AS DataCriacao,
                v.data_ultima_alteracao     AS DataUltimaAlteracao,
                v.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                v.ativo,
                u.nome                      AS NomeUsuarioAlteracao
            FROM sku_atributos_valores v
            LEFT JOIN usuarios u ON v.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE v.id = @Id";
        private static string SqlPorChave => SqlBase + " WHERE v.id_chave = @IdChave";

        public async Task<List<SkusAtributosValores>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<SkusAtributosValores>(SqlBase)).ToList();
        }

        public async Task<List<SkusAtributosValores>> ListarPorChaveAsync(int idChave)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<SkusAtributosValores>(
                SqlPorChave, new { IdChave = idChave })).ToList();
        }

        public async Task<SkusAtributosValores?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<SkusAtributosValores>(SqlPesquisar, new { Id = id });
        }

        public async Task<int> InserirAsync(SkusAtributosValores entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO sku_atributos_valores
                           (id_chave, valor, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdChave, @Valor, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo);
                           SELECT LAST_INSERT_ID();";
            return await conexao.ExecuteScalarAsync<int>(sql, entity);
        }

        public async Task AtualizarAsync(SkusAtributosValores entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE sku_atributos_valores SET
                           valor                         = @Valor,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync("DELETE FROM sku_atributos_valores WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteValorCadastradoAsync(int idChave, string valor, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM sku_atributos_valores
                           WHERE id_chave = @IdChave AND valor = @Valor AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { IdChave = idChave, Valor = valor, Id = ignorarId });
            return qtde > 0;
        }

        public async Task<bool> EstaEmUsoAsync(int idValor)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM skus_atributos_valores_relacionamento WHERE id_valor = @IdValor",
                new { IdValor = idValor });
            return qtde > 0;
        }
    }
}
