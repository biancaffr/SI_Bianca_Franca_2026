using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.Models.Localizacao;

namespace SI_Bianca_Franca_2026.Repositories.Localizacao
{
    public class CidadesRepository
    {
        private readonly string _stringConexao;

        public CidadesRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<Cidades>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"
                SELECT
                    c.id,
                    c.cidade,
                    c.id_estado             AS IdEstado,
                    c.codigo_ibge           AS CodigoIbge,
                    c.ddd,
                    c.data_criacao          AS DataCriacao,
                    c.data_ultima_alteracao AS DataUltimaAlteracao,
                    c.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                    c.ativo,
                    u.nome                  AS NomeUsuarioAlteracao,
                    e.id,
                    e.estado,
                    e.uf,
                    e.id_pais               AS IdPais
                FROM cidades c
                INNER JOIN estados e ON c.id_estado = e.id
                LEFT JOIN usuarios u ON c.id_usuario_ultima_alteracao = u.id";

            var resultado = await conexao.QueryAsync<Cidades, Estados, Cidades>(
                sql,
                (cidade, estado) =>
                {
                    cidade.OEstado = estado;
                    return cidade;
                },
                splitOn: "id"
            );

            return resultado.ToList();
        }

        public async Task<Cidades?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return await conexao.QueryFirstOrDefaultAsync<Cidades>(
                "SELECT * FROM cidades WHERE id = @Id", new { Id = id });
        }

        public async Task InserirAsync(Cidades entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO cidades
                           (cidade, id_estado, codigo_ibge, ddd, data_criacao,
                            data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Cidade, @IdEstado, @CodigoIbge, @Ddd, @DataCriacao,
                            @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Cidades entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE cidades SET
                           cidade = @Cidade,
                           id_estado = @IdEstado,
                           codigo_ibge = @CodigoIbge,
                           ddd = @Ddd,
                           data_ultima_alteracao = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao = @IdUsuarioUltimaAlteracao,
                           ativo = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE cidades SET
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
            await conexao.ExecuteAsync("DELETE FROM cidades WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCidadeCadastradaAsync(string cidade, int idEstado, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM cidades
                           WHERE cidade = @Cidade AND id_estado = @IdEstado AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Cidade = cidade, IdEstado = idEstado, Id = ignorarId });
            return qtde > 0;
        }

        public async Task<bool> ExisteCodigoIbgeCadastradoAsync(string codigoIbge, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM cidades
                           WHERE codigo_ibge = @CodigoIbge AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { CodigoIbge = codigoIbge, Id = ignorarId });
            return qtde > 0;
        }
    }
}