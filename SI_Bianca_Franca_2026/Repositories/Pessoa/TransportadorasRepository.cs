using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class TransportadorasRepository
    {
        private readonly string _stringConexao;

        public TransportadorasRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
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
                t.telefone,
                t.email,
                t.rntrc,
                t.observacao,
                t.data_criacao                  AS DataCriacao,
                t.data_ultima_alteracao         AS DataUltimaAlteracao,
                t.id_usuario_ultima_alteracao   AS IdUsuarioUltimaAlteracao,
                t.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                ci.id                           AS CidadeId,
                ci.cidade,
                ci.id_estado                    AS IdEstado,
                e.id                            AS EstadoId,
                e.estado,
                e.uf
            FROM transportadoras t
            LEFT JOIN usuarios u  ON t.id_usuario_ultima_alteracao = u.id
            LEFT JOIN cidades ci  ON t.id_cidade = ci.id
            LEFT JOIN estados e   ON ci.id_estado = e.id";

        private static string SqlPesquisar => SqlBase + " WHERE t.id = @Id";

        private static Transportadoras MapearTransportadora(Transportadoras transportadora, Cidades cidade, Estados estado)
        {
            cidade.OEstado = estado;
            transportadora.OCidade = cidade;
            return transportadora;
        }

        public async Task<List<Transportadoras>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("t.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("t.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add(@"(t.nome_razao_social      LIKE @Busca
                          OR t.apelido_nome_fantasia  LIKE @Busca
                          OR t.cpf_cnpj               LIKE @Busca
                          OR t.telefone               LIKE @Busca
                          OR t.rntrc                  LIKE @Busca
                          OR ci.cidade                LIKE @Busca
                          OR e.uf                     LIKE @Busca)");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY t.id";

            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Transportadoras, Cidades, Estados, Transportadoras>(
                sql, MapearTransportadora,
                splitOn: "CidadeId,EstadoId",
                param: param);
            return resultado.ToList();
        }

        public async Task<Transportadoras?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Transportadoras, Cidades, Estados, Transportadoras>(
                SqlPesquisar, MapearTransportadora,
                splitOn: "CidadeId,EstadoId",
                param: new { Id = id });
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Transportadoras entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO transportadoras
                           (tipo_pessoa, nome_razao_social, cpf_cnpj, rg_ie,
                            apelido_nome_fantasia, id_cidade, bairro, logradouro,
                            numero, complemento, cep, telefone, email,
                            rntrc, observacao, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe,
                            @ApelidoNomeFantasia, @IdCidade, @Bairro, @Logradouro,
                            @Numero, @Complemento, @Cep, @Telefone, @Email,
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
            await conexao.ExecuteAsync(@"UPDATE transportadoras SET
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
                SELECT id_transportadora FROM veiculos WHERE id_transportadora = @Id
                UNION ALL
                SELECT id_transportadora FROM nfes     WHERE id_transportadora = @Id
            ) AS referencias";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql, new { Id = id });
            return qtde > 0;
        }

        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync("DELETE FROM transportadoras WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCpfCnpjCadastradoAsync(string cpfCnpj, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM transportadoras WHERE cpf_cnpj = @CpfCnpj AND id != @Id",
                new { CpfCnpj = cpfCnpj, Id = ignorarId });
            return qtde > 0;
        }
    }
}