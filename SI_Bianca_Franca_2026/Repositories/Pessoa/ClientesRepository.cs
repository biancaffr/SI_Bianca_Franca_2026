using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class ClientesRepository
    {
        private readonly string _stringConexao;

        public ClientesRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                c.id,
                c.tipo_pessoa                   AS TipoPessoa,
                c.nome_razao_social             AS NomeRazaoSocial,
                c.cpf_cnpj                      AS CpfCnpj,
                c.rg_ie                         AS RgIe,
                c.apelido_nome_fantasia         AS ApelidoNomeFantasia,
                c.id_cidade                     AS IdCidade,
                c.bairro,
                c.logradouro,
                c.numero,
                c.complemento,
                c.cep,
                c.id_pais                       AS IdPais,
                c.telefone,
                c.email,
                c.limite_credito                AS LimiteCredito,
                c.observacao,
                c.data_criacao                  AS DataCriacao,
                c.data_ultima_alteracao         AS DataUltimaAlteracao,
                c.id_usuario_ultima_alteracao   AS IdUsuarioUltimaAlteracao,
                c.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                p.id,
                p.pais,
                p.sigla,
                ci.id,
                ci.cidade,
                ci.id_estado                    AS IdEstado
            FROM clientes c
            LEFT JOIN usuarios u  ON c.id_usuario_ultima_alteracao = u.id
            LEFT JOIN paises p    ON c.id_pais = p.id
            LEFT JOIN cidades ci  ON c.id_cidade = ci.id";

        private static string SqlPesquisar => SqlBase + " WHERE c.id = @Id";

        private static Clientes MapearCliente(Clientes cliente, Paises pais, Cidades cidade)
        {
            cliente.OPais = pais;
            cliente.OCidade = cidade;
            return cliente;
        }

        public async Task<List<Clientes>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Clientes, Paises, Cidades, Clientes>(
                SqlBase,
                MapearCliente,
                splitOn: "id,id"
            );
            return resultado.ToList();
        }

        public async Task<Clientes?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Clientes, Paises, Cidades, Clientes>(
                SqlPesquisar,
                MapearCliente,
                splitOn: "id,id",
                param: new { Id = id }
            );
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Clientes entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO clientes
                           (tipo_pessoa, nome_razao_social, cpf_cnpj, rg_ie,
                            apelido_nome_fantasia, id_cidade, bairro, logradouro,
                            numero, complemento, cep, id_pais, telefone, email,
                            limite_credito, observacao, data_criacao,
                            data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe,
                            @ApelidoNomeFantasia, @IdCidade, @Bairro, @Logradouro,
                            @Numero, @Complemento, @Cep, @IdPais, @Telefone, @Email,
                            @LimiteCredito, @Observacao, @DataCriacao,
                            @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Clientes entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE clientes SET
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
                           limite_credito                = @LimiteCredito,
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
            string sql = @"UPDATE clientes SET
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
                "DELETE FROM clientes WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCpfCnpjCadastradoAsync(string cpfCnpj, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM clientes
                           WHERE cpf_cnpj = @CpfCnpj AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { CpfCnpj = cpfCnpj, Id = ignorarId });
            return qtde > 0;
        }
    }
}