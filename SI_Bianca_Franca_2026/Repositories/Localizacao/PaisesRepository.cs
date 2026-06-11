using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.Models.Localizacao;

namespace SI_Bianca_Franca_2026.Repositories.Localizacao
{
    public class PaisesRepository
    {
        private readonly string _stringConexao;

        public PaisesRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT 
                p.id,
                p.pais,
                p.sigla,
                p.ddi,
                p.moeda,
                p.data_criacao                AS DataCriacao,
                p.data_ultima_alteracao       AS DataUltimaAlteracao,
                p.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                p.ativo,
                u.nome                        AS NomeUsuarioAlteracao
            FROM paises p
            LEFT JOIN usuarios u ON p.id_usuario_ultima_alteracao = u.id";

        private static string SqlListar => SqlBase;
        private static string SqlPesquisar => SqlBase + " WHERE p.id = @Id";

        public async Task<List<Paises>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<Paises>(SqlListar)).ToList();
        }

        public async Task<Paises?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<Paises>(SqlPesquisar, new { Id = id });
        }

        public async Task InserirAsync(Paises entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO paises 
                           (pais, sigla, ddi, moeda, data_criacao, data_ultima_alteracao, 
                            id_usuario_ultima_alteracao, ativo)
                           VALUES 
                           (@Pais, @Sigla, @Ddi, @Moeda, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Paises entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE paises SET 
                           pais = @Pais, sigla = @Sigla, ddi = @Ddi, moeda = @Moeda,
                           data_ultima_alteracao = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao = @IdUsuarioUltimaAlteracao,
                           ativo = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE paises SET 
                           ativo = @Ativo,
                           data_ultima_alteracao = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao = @IdUsuario
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
            await conexao.ExecuteAsync("DELETE FROM paises WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExistePaisCadastradoAsync(string nomePais, string sigla, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM paises 
                           WHERE (pais = @Pais OR sigla = @Sigla) AND id != @Id";
            var quantidade = await conexao.ExecuteScalarAsync<int>(sql,
                new { Pais = nomePais, Sigla = sigla, Id = ignorarId });
            return quantidade > 0;
        }
    }
}