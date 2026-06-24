using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class SkusRepository
    {
        private readonly string _stringConexao;

        public SkusRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                s.sku,
                s.id_produto            AS IdProduto,
                s.gtin_ean              AS GtinEan,
                s.preco_custo           AS PrecoCusto,
                s.estoque,
                s.estoque_minimo        AS EstoqueMinimo,
                s.data_criacao          AS DataCriacao,
                s.data_ultima_alteracao AS DataUltimaAlteracao,
                s.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                s.ativo,
                u.nome                  AS NomeUsuarioAlteracao
            FROM skus s
            LEFT JOIN usuarios u ON s.id_usuario_ultima_alteracao = u.id";

        private static string SqlPesquisar => SqlBase + " WHERE s.sku = @Sku";
        private static string SqlPorProduto => SqlBase + " WHERE s.id_produto = @IdProduto";
        private static string SqlBaseComProduto => @"
            SELECT
                s.sku,
                s.id_produto            AS IdProduto,
                s.gtin_ean              AS GtinEan,
                s.preco_custo           AS PrecoCusto,
                s.estoque,
                s.estoque_minimo        AS EstoqueMinimo,
                s.data_criacao          AS DataCriacao,
                s.data_ultima_alteracao AS DataUltimaAlteracao,
                s.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                s.ativo,
                u.nome                  AS NomeUsuarioAlteracao,
                p.id                    AS ProdutoId,
                p.produto,
                um.sigla
            FROM skus s
            LEFT JOIN usuarios u           ON s.id_usuario_ultima_alteracao = u.id
            INNER JOIN produtos p          ON s.id_produto = p.id
            INNER JOIN unidades_medida um  ON p.id_unidade_medida = um.id";

        public async Task<List<Skus>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<Skus>(SqlBase)).ToList();
        }

        public async Task<List<Skus>> ListarTudoComProdutoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Skus, Produtos, UnidadesMedida, Skus>(
                SqlBaseComProduto,
                (sku, produto, unidade) =>
                {
                    produto.OUnidadeMedida = unidade;
                    sku.OProduto = produto;
                    return sku;
                },
                splitOn: "ProdutoId,sigla"
            );

            var skusList = resultado.ToList();
            foreach (var sku in skusList)
                sku.Atributos = await ListarAtributosAsync(sku.Sku);

            return skusList;
        }

        public async Task<List<Skus>> ListarPorProdutoAsync(int idProduto)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var skus = (await conexao.QueryAsync<Skus>(SqlPorProduto, new { IdProduto = idProduto })).ToList();

            foreach (var sku in skus)
                sku.Atributos = await ListarAtributosAsync(sku.Sku);

            return skus;
        }

        public async Task<Skus?> PesquisarAsync(string sku)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryFirstOrDefaultAsync<Skus>(SqlPesquisar, new { Sku = sku });

            if (resultado != null)
                resultado.Atributos = await ListarAtributosAsync(sku);

            return resultado;
        }

        public async Task<List<SkusAtributosValoresRelacionamento>> ListarAtributosAsync(string sku)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT
                           r.sku,
                           r.id_valor      AS IdValor,
                           v.id,
                           v.id_chave      AS IdChave,
                           v.valor,
                           ch.id,
                           ch.chave
                           FROM skus_atributos_valores_relacionamento r
                           INNER JOIN sku_atributos_valores v ON r.id_valor = v.id
                           INNER JOIN sku_atributos_chaves ch ON v.id_chave = ch.id
                           WHERE r.sku = @Sku";

            var resultado = await conexao.QueryAsync<SkusAtributosValoresRelacionamento, SkusAtributosValores, SkuAtributosChaves, SkusAtributosValoresRelacionamento>(
                sql,
                (relacionamento, valor, chave) =>
                {
                    valor.OChave = chave;
                    relacionamento.OValor = valor;
                    return relacionamento;
                },
                splitOn: "id,id",
                param: new { Sku = sku }
            );
            return resultado.ToList();
        }

        public async Task InserirAsync(Skus entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO skus
                           (sku, id_produto, gtin_ean, preco_custo, estoque, estoque_minimo,
                            data_criacao, data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Sku, @IdProduto, @GtinEan, @PrecoCusto, @Estoque, @EstoqueMinimo,
                            @DataCriacao, @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task InserirAtributosAsync(string sku, List<SkusAtributosValoresRelacionamento> atributos)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO skus_atributos_valores_relacionamento (sku, id_valor)
                           VALUES (@Sku, @IdValor)";
            await conexao.ExecuteAsync(sql, atributos);
        }

        public async Task AtualizarAsync(Skus entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE skus SET
                           gtin_ean                      = @GtinEan,
                           preco_custo                   = @PrecoCusto,
                           estoque                       = @Estoque,
                           estoque_minimo                = @EstoqueMinimo,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE sku = @Sku";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task DeletarAtributosAsync(string sku)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(
                "DELETE FROM skus_atributos_valores_relacionamento WHERE sku = @Sku", new { Sku = sku });
        }

        public async Task AlterarStatusAsync(string sku, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE skus SET
                           ativo                         = @Ativo,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuario
                           WHERE sku = @Sku";
            await conexao.ExecuteAsync(sql, new
            {
                Ativo = novoStatus,
                DataUltimaAlteracao = DateTime.Now,
                IdUsuario = idUsuario,
                Sku = sku
            });
        }

        public async Task RemoverAsync(string sku)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(
                "DELETE FROM skus_atributos_valores_relacionamento WHERE sku = @Sku", new { Sku = sku });
            await conexao.ExecuteAsync(
                "DELETE FROM skus WHERE sku = @Sku", new { Sku = sku });
        }

        public async Task<bool> ExisteSkuCadastradoAsync(string sku)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM skus WHERE sku = @Sku", new { Sku = sku });
            return qtde > 0;
        }

        public async Task<bool> ExisteGtinCadastradoAsync(string gtin, string ignorarSku = "")
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM skus
                           WHERE gtin_ean = @Gtin AND sku != @Sku";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { Gtin = gtin, Sku = ignorarSku });
            return qtde > 0;
        }
    }
}