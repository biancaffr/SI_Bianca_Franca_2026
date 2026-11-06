using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class FornecedoresRepository
    {
        private readonly string _stringConexao;

        public FornecedoresRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
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
                f.telefone,
                f.email,
                f.observacao,
                f.data_criacao                  AS DataCriacao,
                f.data_ultima_alteracao         AS DataUltimaAlteracao,
                f.id_usuario_ultima_alteracao   AS IdUsuarioUltimaAlteracao,
                f.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                ci.id                           AS CidadeId,
                ci.cidade,
                ci.id_estado                    AS IdEstado,
                e.id                            AS EstadoId,
                e.estado,
                e.uf
            FROM fornecedores f
            LEFT JOIN usuarios u  ON f.id_usuario_ultima_alteracao = u.id
            LEFT JOIN cidades ci  ON f.id_cidade = ci.id
            LEFT JOIN estados e   ON ci.id_estado = e.id";

        private static string SqlPesquisar => SqlBase + " WHERE f.id = @Id";

        private static Fornecedores MapearFornecedor(Fornecedores fornecedor, Cidades cidade, Estados estado)
        {
            cidade.OEstado = estado;
            fornecedor.OCidade = cidade;
            return fornecedor;
        }

        public async Task<List<Fornecedores>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("f.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("f.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add(@"(f.nome_razao_social      LIKE @Busca
                          OR f.apelido_nome_fantasia  LIKE @Busca
                          OR f.cpf_cnpj               LIKE @Busca
                          OR f.telefone               LIKE @Busca
                          OR ci.cidade                LIKE @Busca
                          OR e.uf                     LIKE @Busca)");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY f.id";

            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Fornecedores, Cidades, Estados, Fornecedores>(
                sql, MapearFornecedor,
                splitOn: "CidadeId,EstadoId",
                param: param);
            return resultado.ToList();
        }

        public async Task<Fornecedores?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Fornecedores, Cidades, Estados, Fornecedores>(
                SqlPesquisar, MapearFornecedor,
                splitOn: "CidadeId,EstadoId",
                param: new { Id = id });
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Fornecedores entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO fornecedores
                           (tipo_pessoa, nome_razao_social, cpf_cnpj, rg_ie,
                            apelido_nome_fantasia, id_cidade, bairro, logradouro,
                            numero, complemento, cep, telefone, email,
                            observacao, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe,
                            @ApelidoNomeFantasia, @IdCidade, @Bairro, @Logradouro,
                            @Numero, @Complemento, @Cep, @Telefone, @Email,
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
            await conexao.ExecuteAsync(@"UPDATE fornecedores SET
                           ativo                         = @Ativo,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuario
                           WHERE id = @Id",
                new { Ativo = novoStatus, DataUltimaAlteracao = DateTime.Now, IdUsuario = idUsuario, Id = id });
        }

        public async Task<bool> EstaEmUsoAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM (
                    SELECT id_fornecedor FROM contas_pagar WHERE id_fornecedor = @Id
                ) AS referencias";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql, new { Id = id });
            return qtde > 0;
        }

        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync("DELETE FROM fornecedores WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCpfCnpjCadastradoAsync(string cpfCnpj, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM fornecedores WHERE cpf_cnpj = @CpfCnpj AND id != @Id",
                new { CpfCnpj = cpfCnpj, Id = ignorarId });
            return qtde > 0;
        }
    }
}