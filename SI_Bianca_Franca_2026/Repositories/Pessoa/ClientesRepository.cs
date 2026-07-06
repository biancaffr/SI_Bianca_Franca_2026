using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.Localizacao;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Repositories.Pessoa
{
    public class ClientesRepository
    {
        private readonly string _stringConexao;

        public ClientesRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
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
                c.telefone,
                c.email,
                c.limite_credito                AS LimiteCredito,
                c.id_condicao_pagamento         AS IdCondicaoPagamento,
                c.observacao,
                c.data_criacao                  AS DataCriacao,
                c.data_ultima_alteracao         AS DataUltimaAlteracao,
                c.id_usuario_ultima_alteracao   AS IdUsuarioUltimaAlteracao,
                c.ativo,
                u.nome                          AS NomeUsuarioAlteracao,
                ci.id                           AS CidadeId,
                ci.cidade,
                ci.id_estado                    AS IdEstado,
                e.id                            AS EstadoId,
                e.estado,
                e.uf,
                cp.id                           AS CondicaoId,
                cp.condicao_pagamento           AS CondicaoPagamento
            FROM clientes c
            LEFT JOIN usuarios u              ON c.id_usuario_ultima_alteracao = u.id
            LEFT JOIN cidades ci              ON c.id_cidade = ci.id
            LEFT JOIN estados e               ON ci.id_estado = e.id
            LEFT JOIN condicoes_pagamentos cp ON c.id_condicao_pagamento = cp.id";

        private static string SqlPesquisar => SqlBase + " WHERE c.id = @Id";

        private static Clientes MapearCliente(Clientes cliente, Cidades cidade, Estados estado, CondicoesPagamentos condicao)
        {
            cidade.OEstado = estado;
            cliente.OCidade = cidade;
            if (condicao?.Id > 0)
                cliente.OCondicaoPagamento = condicao;
            return cliente;
        }

        public async Task<List<Clientes>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("c.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("c.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add(@"(c.nome_razao_social      LIKE @Busca
                          OR c.apelido_nome_fantasia  LIKE @Busca
                          OR c.cpf_cnpj               LIKE @Busca
                          OR c.telefone               LIKE @Busca
                          OR ci.cidade                LIKE @Busca
                          OR e.uf                     LIKE @Busca)");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY c.nome_razao_social";

            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Clientes, Cidades, Estados, CondicoesPagamentos, Clientes>(
                sql, MapearCliente,
                splitOn: "CidadeId,EstadoId,CondicaoId",
                param: param);
            return resultado.ToList();
        }

        public async Task<Clientes?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Clientes, Cidades, Estados, CondicoesPagamentos, Clientes>(
                SqlPesquisar, MapearCliente,
                splitOn: "CidadeId,EstadoId,CondicaoId",
                param: new { Id = id });
            return resultado.FirstOrDefault();
        }

        public async Task InserirAsync(Clientes entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO clientes
                           (tipo_pessoa, nome_razao_social, cpf_cnpj, rg_ie,
                            apelido_nome_fantasia, id_cidade, bairro, logradouro,
                            numero, complemento, cep, telefone, email,
                            limite_credito, id_condicao_pagamento, observacao,
                            data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@TipoPessoa, @NomeRazaoSocial, @CpfCnpj, @RgIe,
                            @ApelidoNomeFantasia, @IdCidade, @Bairro, @Logradouro,
                            @Numero, @Complemento, @Cep, @Telefone, @Email,
                            @LimiteCredito, @IdCondicaoPagamento, @Observacao,
                            @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo)";
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
                           telefone                      = @Telefone,
                           email                         = @Email,
                           limite_credito                = @LimiteCredito,
                           id_condicao_pagamento         = @IdCondicaoPagamento,
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
            await conexao.ExecuteAsync(@"UPDATE clientes SET
                           ativo                         = @Ativo,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuario
                           WHERE id = @Id",
                new { Ativo = novoStatus, DataUltimaAlteracao = DateTime.Now, IdUsuario = idUsuario, Id = id });
        }

        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync("DELETE FROM clientes WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteCpfCnpjCadastradoAsync(string cpfCnpj, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM clientes WHERE cpf_cnpj = @CpfCnpj AND id != @Id",
                new { CpfCnpj = cpfCnpj, Id = ignorarId });
            return qtde > 0;
        }
    }
}