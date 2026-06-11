using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class VeiculosRepository
    {
        private readonly string _stringConexao;

        public VeiculosRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                v.id,
                v.id_transportadora         AS IdTransportadora,
                v.id_estado                 AS IdEstado,
                v.placa,
                v.rntrc,
                v.renavam,
                v.tipo_veiculo              AS TipoVeiculo,
                v.marca_modelo              AS MarcaModelo,
                v.observacao,
                v.data_criacao              AS DataCriacao,
                v.data_ultima_alteracao     AS DataUltimaAlteracao,
                v.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                v.ativo,
                u.nome                      AS NomeUsuarioAlteracao,
                e.id,
                e.estado,
                e.uf,
                t.id,
                t.nome_razao_social         AS NomeRazaoSocial
            FROM veiculos v
            LEFT JOIN usuarios u        ON v.id_usuario_ultima_alteracao = u.id
            INNER JOIN estados e        ON v.id_estado = e.id
            LEFT JOIN transportadoras t ON v.id_transportadora = t.id";

        private static string SqlPesquisar => SqlBase + " WHERE v.id = @Id";
        private static string SqlPorEstado => SqlBase + " WHERE v.id_estado = @IdEstado AND v.ativo = 1";

        private static Veiculos MapearVeiculo(Veiculos veiculo, Estados estado, Transportadoras transportadora)
        {
            veiculo.OEstado = estado;
            if (transportadora?.Id > 0)
                veiculo.OTransportadora = transportadora;
            return veiculo;
        }

        public async Task<List<Veiculos>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Veiculos, Estados, Transportadoras, Veiculos>(
                SqlBase,
                MapearVeiculo,
                splitOn: "id,id"
            );
            return resultado.ToList();
        }

        public async Task<Veiculos?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Veiculos, Estados, Transportadoras, Veiculos>(
                SqlPesquisar,
                MapearVeiculo,
                splitOn: "id,id",
                param: new { Id = id }
            );
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Veiculos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO veiculos
                           (id_transportadora, id_estado, placa, rntrc, renavam,
                            tipo_veiculo, marca_modelo, observacao, data_criacao,
                            data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdTransportadora, @IdEstado, @Placa, @Rntrc, @Renavam,
                            @TipoVeiculo, @MarcaModelo, @Observacao, @DataCriacao,
                            @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Veiculos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE veiculos SET
                           id_transportadora             = @IdTransportadora,
                           id_estado                     = @IdEstado,
                           placa                         = @Placa,
                           rntrc                         = @Rntrc,
                           renavam                       = @Renavam,
                           tipo_veiculo                  = @TipoVeiculo,
                           marca_modelo                  = @MarcaModelo,
                           observacao                    = @Observacao,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AlterarStatusAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE veiculos SET
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
            await conexao.ExecuteAsync("DELETE FROM veiculos WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExistePlacaCadastradaAsync(string placa, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM veiculos
                           WHERE placa = @Placa AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Placa = placa, Id = ignorarId });
            return qtde > 0;
        }
    }
}
