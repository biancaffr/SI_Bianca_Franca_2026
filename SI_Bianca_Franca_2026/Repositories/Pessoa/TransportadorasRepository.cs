using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class TransportadorasRepository
    {
        private readonly string _stringConexao;

        public TransportadorasRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                t.id,
                t.tipo_pessoa                   AS TipoPessoa,
                t.nome_razao_social             AS NomeRazaoSocial,
                t.cpf_cnpj                      AS CpfCnpj,
                t.rg_ie                         AS RgIe,
                t.apelido_nome_fantasia         AS ApelidoNomeFantasia,
                t.id_cidade                     AS IdCidade,
                t.bairro,
                t.logradouro,
                t.numero,
                t.complemento,
                t.cep,
                t.id_pais                       AS IdPais,
                t.telefone,
                t.email,
                t.rntrc,
                t.observacao,
                t.data_criacao                  AS DataCriacao,
                t.data_ultima_alteracao         AS DataUltimaAlteracao,
                t.id_usuario_ultima_alteracao   AS IdUsuarioUltimaAlteracao,
                t.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                p.id,
                p.pais,
                p.sigla,
                ci.id,
                ci.cidade,
                ci.id_estado                    AS IdEstado
            FROM transportadoras t
            LEFT JOIN usuarios u  ON t.id_usuario_ultima_alteracao = u.id
            LEFT JOIN paises p    ON t.id_pais = p.id
            LEFT JOIN cidades ci  ON t.id_cidade = ci.id";

        private static string SqlPesquisar => SqlBase + " WHERE t.id = @Id";

        private static Transportadoras MapearTransportadora(Transportadoras transportadora, Paises pais, Cidades cidade)
        {
            transportadora.OPais = pais;
            transportadora.OCidade = cidade;
            return transportadora;
        }

        public async Task<List<Transportadoras>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Transportadoras, Paises, Cidades, Transportadoras>(
                SqlBase,
                MapearTransportadora,
                splitOn: "id,id"
            );
            return resultado.ToList();
        }

        public async Task<Transportadoras?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Transportadoras, Paises, Cidades, Transportadoras>(
                SqlPesquisar,
                MapearTransportadora,
                splitOn: "id,id",
                param: new { Id = id }
            );
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Transportadoras entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO transportadoras
                           (tipo_pessoa, nome_razao_social, cpf_cnpj, rg_ie,
                            apelido_nome_fantasia, id_cidade, bairro, logradouro,
                            numero, complemento, cep, id_pais, telefone, email,
                            rntrc, observacao, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe,
                            @ApelidoNomeFantasia, @IdCidade, @Bairro, @Logradouro,
                            @Numero, @Complemento, @Cep, @IdPais, @Telefone, @Email,
                            @Rntrc, @Observacao, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Transportadoras entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE transportadoras SET
                           tipo_pessoa                   = @TipoPessoa,
                           nome_razao_social             = @NomeRazaoSocial,
                           cpf_cnpj                      = @CpfCnpj,
                           rg_ie                         = @RgIe,
                           apelido_nome_fantasia         = @ApelidoNomeFantasia,
                           id_cidade                     = @IdCidade,
                           bairro                        = @Bairro,
                           logradouro                    = @Logradouro,
                           numero                        = @Numero,
                           complemento                   = @Complemento,
                           cep                           = @Cep,
                           id_pais                       = @IdPais,
                           telefone                      = @Telefone,
                           email                         = @Email,
                           rntrc                         = @Rntrc,
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
            string sql = @"UPDATE transportadoras SET
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
            await conexao.ExecuteAsync(
                "DELETE FROM transportadoras WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCpfCnpjCadastradoAsync(string cpfCnpj, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM transportadoras
                           WHERE cpf_cnpj = @CpfCnpj AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { CpfCnpj = cpfCnpj, Id = ignorarId });
            return qtde > 0;
        }
    }
}