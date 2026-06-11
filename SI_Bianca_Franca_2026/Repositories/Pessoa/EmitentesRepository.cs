using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class EmitentesRepository
    {
        private readonly string _stringConexao;

        public EmitentesRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                e.id,
                e.tipo_pessoa                   AS TipoPessoa,
                e.nome_razao_social             AS NomeRazaoSocial,
                e.cpf_cnpj                      AS CpfCnpj,
                e.rg_ie                         AS RgIe,
                e.apelido_nome_fantasia         AS ApelidoNomeFantasia,
                e.id_cidade                     AS IdCidade,
                e.bairro,
                e.logradouro,
                e.numero,
                e.complemento,
                e.cep,
                e.id_pais                       AS IdPais,
                e.telefone,
                e.email,
                e.inscricao_municipal           AS InscricaoMunicipal,
                e.regime_tributario             AS RegimeTributario,
                e.observacao,
                e.data_criacao                  AS DataCriacao,
                e.data_ultima_alteracao         AS DataUltimaAlteracao,
                e.id_usuario_ultima_alteracao   AS IdUsuarioUltimaAlteracao,
                e.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                p.id,
                p.pais,
                p.sigla,
                ci.id,
                ci.cidade,
                ci.id_estado                    AS IdEstado
            FROM emitentes e
            LEFT JOIN usuarios u  ON e.id_usuario_ultima_alteracao = u.id
            LEFT JOIN paises p    ON e.id_pais = p.id
            LEFT JOIN cidades ci  ON e.id_cidade = ci.id";

        private static string SqlPesquisar => SqlBase + " WHERE e.id = @Id";

        private static Emitentes MapearEmitente(Emitentes emitente, Paises pais, Cidades cidade)
        {
            emitente.OPais = pais;
            emitente.OCidade = cidade;
            return emitente;
        }

        public async Task<List<Emitentes>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Emitentes, Paises, Cidades, Emitentes>(
                SqlBase,
                MapearEmitente,
                splitOn: "id,id"
            );
            return resultado.ToList();
        }

        public async Task<Emitentes?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Emitentes, Paises, Cidades, Emitentes>(
                SqlPesquisar,
                MapearEmitente,
                splitOn: "id,id",
                param: new { Id = id }
            );
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Emitentes entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO emitentes
                           (tipo_pessoa, nome_razao_social, cpf_cnpj, rg_ie,
                            apelido_nome_fantasia, id_cidade, bairro, logradouro,
                            numero, complemento, cep, id_pais, telefone, email,
                            inscricao_municipal, regime_tributario, observacao,
                            data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe,
                            @ApelidoNomeFantasia, @IdCidade, @Bairro, @Logradouro,
                            @Numero, @Complemento, @Cep, @IdPais, @Telefone, @Email,
                            @InscricaoMunicipal, @RegimeTributario, @Observacao,
                            @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(Emitentes entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE emitentes SET
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
                           inscricao_municipal           = @InscricaoMunicipal,
                           regime_tributario             = @RegimeTributario,
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
            string sql = @"UPDATE emitentes SET
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
                "DELETE FROM emitentes WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCpfCnpjCadastradoAsync(string cpfCnpj, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM emitentes
                           WHERE cpf_cnpj = @CpfCnpj AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { CpfCnpj = cpfCnpj, Id = ignorarId });
            return qtde > 0;
        }
    }
}