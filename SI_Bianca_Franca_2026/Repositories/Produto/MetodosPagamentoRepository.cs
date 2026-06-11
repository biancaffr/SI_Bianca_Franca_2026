using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class MetodosPagamentoRepository
    {
        private readonly string _stringConexao;

        public MetodosPagamentoRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                m.id,
                m.codigo,
                m.descricao,
                m.data_criacao              AS DataCriacao,
                m.data_ultima_alteracao     AS DataUltimaAlteracao,
                m.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                m.ativo,
                u.nome                      AS NomeUsuarioAlteracao
            FROM metodos_pagamento m
            LEFT JOIN usuarios u ON m.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE m.id = @Id";

        public async Task<List<MetodosPagamento>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<MetodosPagamento>(SqlBase)).ToList();
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
                           (codigo, descricao, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Codigo, @Descricao, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(MetodosPagamento entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE metodos_pagamento SET
                           codigo                        = @Codigo,
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
            string sql = @"UPDATE metodos_pagamento SET
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
            await conexao.ExecuteAsync("DELETE FROM metodos_pagamento WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCodigoCadastradoAsync(string codigo, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM metodos_pagamento
                           WHERE codigo = @Codigo AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Codigo = codigo, Id = ignorarId });
            return qtde > 0;
        }
    }
}
