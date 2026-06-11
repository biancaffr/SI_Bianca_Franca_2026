using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class FornecedoresRepository
    {
        private readonly string _stringConexao;

        public FornecedoresRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                f.id,
                f.tipo_pessoa                   AS TipoPessoa,
                f.nome_razao_social             AS NomeRazaoSocial,
                f.cpf_cnpj                      AS CpfCnpj,
                f.rg_ie                         AS RgIe,
                f.apelido_nome_fantasia         AS ApelidoNomeFantasia,
                f.id_cidade                     AS IdCidade,
                f.bairro,
                f.logradouro,
                f.numero,
                f.complemento,
                f.cep,
                f.id_pais                       AS IdPais,
                f.telefone,
                f.email,
                f.observacao,
                f.data_criacao                  AS DataCriacao,
                f.data_ultima_alteracao         AS DataUltimaAlteracao,
                f.id_usuario_ultima_alteracao   AS IdUsuarioUltimaAlteracao,
                f.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                p.id,
                p.pais,
                p.sigla,
                ci.id,
                ci.cidade,
                ci.id_estado                    AS IdEstado
            FROM fornecedores f
            LEFT JOIN usuarios u  ON f.id_usuario_ultima_alteracao = u.id
            LEFT JOIN paises p    ON f.id_pais = p.id
            LEFT JOIN cidades ci  ON f.id_cidade = ci.id";

        private static string SqlPesquisar => SqlBase + " WHERE f.id = @Id";

        private static Fornecedores MapearFornecedor(Fornecedores fornecedor, Paises pais, Cidades cidade)
        {
            fornecedor.OPais = pais;
            fornecedor.OCidade = cidade;
            return fornecedor;
        }

        public async Task<List<Fornecedores>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Fornecedores, Paises, Cidades, Fornecedores>(
                SqlBase,
                MapearFornecedor,
                splitOn: "id,id"
            );
            return resultado.ToList();
        }

        public async Task<Fornecedores?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Fornecedores, Paises, Cidades, Fornecedores>(
                SqlPesquisar,
                MapearFornecedor,
                splitOn: "id,id",
                param: new { Id = id }
            );
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Fornecedores entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO fornecedores
                           (tipo_pessoa, nome_razao_social, cpf_cnpj, rg_ie,
                            apelido_nome_fantasia, id_cidade, bairro, logradouro,
                            numero, complemento, cep, id_pais, telefone, email,
                            observacao, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe,
                            @ApelidoNomeFantasia, @IdCidade, @Bairro, @Logradouro,
                            @Numero, @Complemento, @Cep, @IdPais, @Telefone, @Email,
                            @Observacao, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Fornecedores entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE fornecedores SET
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
            string sql = @"UPDATE fornecedores SET
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
                "DELETE FROM fornecedores WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCpfCnpjCadastradoAsync(string cpfCnpj, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM fornecedores
                           WHERE cpf_cnpj = @CpfCnpj AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { CpfCnpj = cpfCnpj, Id = ignorarId });
            return qtde > 0;
        }
    }
}