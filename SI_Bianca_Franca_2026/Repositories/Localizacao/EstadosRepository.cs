using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.Models.Localizacao;

namespace SI_Bianca_Franca_2026.Repositories.Localizacao
{
    public class EstadosRepository
    {
        private readonly string _stringConexao;

        public EstadosRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                e.id,
                e.estado,
                e.uf,
                e.id_pais               AS IdPais,
                e.data_criacao          AS DataCriacao,
                e.data_ultima_alteracao AS DataUltimaAlteracao,
                e.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                e.ativo,
                u.nome                  AS NomeUsuarioAlteracao,
                p.id,
                p.pais,
                p.sigla
            FROM estados e
            INNER JOIN paises p ON e.id_pais = p.id
            LEFT JOIN usuarios u ON e.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE e.id = @Id";
        private static string SqlListarPorPais => @"
            SELECT
                e.id,
                e.estado,
                e.uf,
                e.id_pais AS IdPais,
                e.data_criacao AS DataCriacao,
                e.data_ultima_alteracao AS DataUltimaAlteracao,
                e.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                e.ativo
            FROM estados e
            WHERE e.id_pais = @IdPais
            AND e.ativo = 1
            ORDER BY e.estado";

        private static Estados MapearEstado(Estados estado, Paises pais)
        {
            estado.OPais = pais;
            return estado;
        }

        public async Task<List<Estados>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Estados, Paises, Estados>(
                SqlBase,
                MapearEstado,
                splitOn: "id"
            );
            return resultado.ToList();
        }

        public async Task<List<Estados>> ListarPorPaisAsync(int idPais)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<Estados>(
                SqlListarPorPais,
                new { IdPais = idPais })).ToList();
        }

        public async Task<Estados?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Estados, Paises, Estados>(
                SqlPesquisar,
                MapearEstado,
                splitOn: "id",
                param: new { Id = id }
            );
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Estados entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO estados 
                           (estado, uf, id_pais, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES 
                           (@Estado, @Uf, @IdPais, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Estados entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE estados SET 
                           estado = @Estado,
                           uf = @Uf,
                           id_pais = @IdPais,
                           data_ultima_alteracao = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao = @IdUsuarioUltimaAlteracao,
                           ativo = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE estados SET 
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
            await conexao.ExecuteAsync("DELETE FROM estados WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteEstadoCadastradoAsync(string estado, string uf, int idPais, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM estados 
                           WHERE ((estado = @Estado AND id_pais = @IdPais) 
                              OR (uf = @Uf AND id_pais = @IdPais))
                           AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Estado = estado, Uf = uf, IdPais = idPais, Id = ignorarId });
            return qtde > 0;
        }
    }
}