using Dapper;
using MySql.Data.MySqlClient;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.NfeEntrada;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.NfeEntrada
{
    public class NfeEntradaRepository
    {
        private readonly string _stringConexao;

        public NfeEntradaRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
        }

        private static string SqlBase => @"
            SELECT
                n.numero,
                n.modelo,
                n.serie,
                n.id_fornecedor             AS IdFornecedor,
                n.id_transportadora         AS IdTransportadora,
                n.id_condicao_pagamento     AS IdCondicaoPagamento,
                n.data_emissao              AS DataEmissao,
                n.data_chegada              AS DataChegada,
                n.tipo_frete                AS TipoFrete,
                n.valor_produtos            AS ValorProdutos,
                n.valor_frete               AS ValorFrete,
                n.valor_seguro              AS ValorSeguro,
                n.valor_outras_despesas     AS ValorOutrasDespesas,
                n.valor_desconto            AS ValorDesconto,
                n.valor_total               AS ValorTotal,
                n.observacao,
                n.situacao,
                n.data_criacao              AS DataCriacao,
                n.data_ultima_alteracao     AS DataUltimaAlteracao,
                n.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                n.ativo,
                u.nome                      AS NomeUsuarioAlteracao,
                f.id                        AS FornecedorId,
                f.nome_razao_social         AS NomeRazaoSocial,
                t.id                        AS TransportadoraId,
                t.nome_razao_social         AS NomeRazaoSocialTransp,
                cp.id                       AS CondicaoId,
                cp.condicao_pagamento       AS CondicaoPagamento
            FROM nfes_entrada n
            INNER JOIN fornecedores f          ON n.id_fornecedor = f.id
            LEFT JOIN usuarios u               ON n.id_usuario_ultima_alteracao = u.id
            LEFT JOIN transportadoras t        ON n.id_transportadora = t.id
            LEFT JOIN condicoes_pagamentos cp  ON n.id_condicao_pagamento = cp.id";

        private static NfeEntradas MapearNota(
            NfeEntradas nota,
            Fornecedores fornecedor,
            Transportadoras transportadora,
            CondicoesPagamentos condicao)
        {
            nota.OFornecedor = fornecedor;
            if (transportadora?.Id > 0) nota.OTransportadora = transportadora;
            if (condicao?.Id > 0) nota.OCondicaoPagamento = condicao;
            return nota;
        }

        public async Task<List<NfeEntradas>> ListarAsync(FiltroBaseDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("n.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("n.ativo = FALSE");

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add(@"(n.numero             LIKE @Busca
                          OR f.nome_razao_social  LIKE @Busca
                          OR n.situacao           LIKE @Busca)");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY n.data_emissao DESC, n.numero DESC";

            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<
                NfeEntradas, Fornecedores, Transportadoras, CondicoesPagamentos, NfeEntradas>(
                sql, MapearNota,
                splitOn: "FornecedorId,TransportadoraId,CondicaoId",
                param: param);
            return resultado.ToList();
        }

        public async Task<NfeEntradas?> PesquisarAsync(
            string numero, string modelo, string serie, int idFornecedor)
        {
            var sql = SqlBase + @"
                WHERE n.numero = @Numero AND n.modelo = @Modelo
                  AND n.serie = @Serie AND n.id_fornecedor = @IdFornecedor";

            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<
                NfeEntradas, Fornecedores, Transportadoras, CondicoesPagamentos, NfeEntradas>(
                sql, MapearNota,
                splitOn: "FornecedorId,TransportadoraId,CondicaoId",
                param: new { Numero = numero, Modelo = modelo, Serie = serie, IdFornecedor = idFornecedor });

            var nota = resultado.FirstOrDefault();
            if (nota != null)
                nota.Itens = await ListarItensAsync(numero, modelo, serie, idFornecedor);
            return nota;
        }

        public async Task<bool> ChaveExisteAsync(
            string numero, string modelo, string serie, int idFornecedor)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(@"
                SELECT COUNT(1) FROM nfes_entrada
                WHERE numero = @Numero AND modelo = @Modelo
                  AND serie = @Serie AND id_fornecedor = @IdFornecedor",
                new { Numero = numero, Modelo = modelo, Serie = serie, IdFornecedor = idFornecedor });
            return qtde > 0;
        }

        public async Task<List<NfeEntradaItens>> ListarItensAsync(
            string numero, string modelo, string serie, int idFornecedor)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var sql = @"
                SELECT
                    ni.nota_numero          AS NotaNumero,
                    ni.nota_modelo          AS NotaModelo,
                    ni.nota_serie           AS NotaSerie,
                    ni.id_fornecedor        AS IdFornecedor,
                    ni.id_produto           AS IdProduto,
                    ni.sequencia,
                    ni.quantidade,
                    ni.valor_unitario       AS ValorUnitario,
                    ni.valor_desconto       AS ValorDesconto,
                    ni.percentual_desconto  AS PercentualDesconto,
                    ni.valor_total          AS ValorTotal,
                    ni.rateio_frete         AS RateioFrete,
                    ni.rateio_seguro        AS RateioSeguro,
                    ni.rateio_outras        AS RateioOutras,
                    ni.custo_final          AS CustoFinal,
                    ni.data_criacao         AS DataCriacao,
                    ni.data_ultima_alteracao AS DataUltimaAlteracao,
                    p.id                    AS ProdutoId,
                    p.produto,
                    um.sigla
                FROM nfes_entrada_itens ni
                INNER JOIN produtos p         ON ni.id_produto = p.id
                INNER JOIN unidades_medida um ON p.id_unidade_medida = um.id
                WHERE ni.nota_numero = @Numero AND ni.nota_modelo = @Modelo
                  AND ni.nota_serie = @Serie AND ni.id_fornecedor = @IdFornecedor
                ORDER BY ni.sequencia";

            var resultado = await conexao.QueryAsync<NfeEntradaItens, Produtos, NfeEntradaItens>(
                sql,
                (item, produto) => { item.OProduto = produto; return item; },
                splitOn: "ProdutoId",
                param: new { Numero = numero, Modelo = modelo, Serie = serie, IdFornecedor = idFornecedor });
            return resultado.ToList();
        }

        public async Task InserirAsync(NfeEntradas entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(@"
        INSERT INTO nfes_entrada
        (numero, modelo, serie, id_fornecedor, id_transportadora,
         id_condicao_pagamento, data_emissao, data_chegada, tipo_frete,
         valor_produtos, valor_frete, valor_seguro, valor_outras_despesas,
         valor_desconto, valor_total, observacao, situacao,
         data_criacao, data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
        VALUES
        (@Numero, @Modelo, @Serie, @IdFornecedor, @IdTransportadora,
         @IdCondicaoPagamento, @DataEmissao, @DataChegada, @TipoFrete,
         @ValorProdutos, @ValorFrete, @ValorSeguro, @ValorOutrasDespesas,
         @ValorDesconto, @ValorTotal, @Observacao, @Situacao,
         @DataCriacao, @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)",
                entity);
        }

        public async Task InserirItensAsync(List<NfeEntradaItens> itens)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(@"
                INSERT INTO nfes_entrada_itens
                (nota_numero, nota_modelo, nota_serie, id_fornecedor, id_produto,
                 sequencia, quantidade, valor_unitario, valor_desconto,
                 percentual_desconto, valor_total, rateio_frete, rateio_seguro,
                 rateio_outras, custo_final, data_criacao, data_ultima_alteracao)
                VALUES
                (@NotaNumero, @NotaModelo, @NotaSerie, @IdFornecedor, @IdProduto,
                 @Sequencia, @Quantidade, @ValorUnitario, @ValorDesconto,
                 @PercentualDesconto, @ValorTotal, @RateioFrete, @RateioSeguro,
                 @RateioOutras, @CustoFinal, @DataCriacao, @DataUltimaAlteracao)",
                itens);
        }

        public async Task CancelarAsync(
            string numero, string modelo, string serie, int idFornecedor, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(@"
                UPDATE nfes_entrada SET
                    situacao                      = 'CANCELADA',
                    ativo                         = FALSE,
                    data_ultima_alteracao         = @DataUltimaAlteracao,
                    id_usuario_ultima_alteracao   = @IdUsuario
                WHERE numero = @Numero AND modelo = @Modelo
                  AND serie = @Serie AND id_fornecedor = @IdFornecedor",
                new
                {
                    DataUltimaAlteracao = DateTime.Now,
                    IdUsuario = idUsuario,
                    Numero = numero,
                    Modelo = modelo,
                    Serie = serie,
                    IdFornecedor = idFornecedor
                });
        }
    }
}