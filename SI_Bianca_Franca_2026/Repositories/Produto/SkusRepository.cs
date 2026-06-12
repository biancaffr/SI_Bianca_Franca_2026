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

        public async Task<List<Skus>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            return (await conexao.QueryAsync<Skus>(SqlBase)).ToList();
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

        public async Task<List<SkusAtributosValores>> ListarAtributosAsync(string sku)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT
                           av.sku,
                           av.id_chave     AS IdChave,
                           av.valor,
                           ch.id,
                           ch.chave
                           FROM skus_atributos_valores av
                           INNER JOIN sku_atributos_chaves ch ON av.id_chave = ch.id
                           WHERE av.sku = @Sku";

            var resultado = await conexao.QueryAsync<SkusAtributosValores, SkuAtributosChaves, SkusAtributosValores>(
                sql,
                (atributo, chave) =>
                {
                    atributo.OChave = chave;
                    return atributo;
                },
                splitOn: "id",
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

        public async Task InserirAtributosAsync(string sku, List<SkusAtributosValores> atributos)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO skus_atributos_valores (sku, id_chave, valor)
                           VALUES (@Sku, @IdChave, @Valor)";
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
                "DELETE FROM skus_atributos_valores WHERE sku = @Sku", new { Sku = sku });
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
                "DELETE FROM skus_atributos_valores WHERE sku = @Sku", new { Sku = sku });
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
