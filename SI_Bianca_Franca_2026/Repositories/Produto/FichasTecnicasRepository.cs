using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class FichasTecnicasRepository
    {
        private readonly string _stringConexao;

        public FichasTecnicasRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<List<FichasTecnicas>> ListarPorProdutoAsync(int idProduto)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"
                SELECT
                    ft.id,
                    ft.id_produto               AS IdProduto,
                    ft.id_produto_material      AS IdProdutoMaterial,
                    ft.quantidade,
                    ft.observacao,
                    ft.data_criacao             AS DataCriacao,
                    ft.data_ultima_alteracao    AS DataUltimaAlteracao,
                    ft.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                    ft.ativo,
                    u.nome                      AS NomeUsuarioAlteracao,
                    p.id,
                    p.produto,
                    um.sigla
                FROM fichas_tecnicas ft
                LEFT JOIN usuarios u        ON ft.id_usuario_ultima_alteracao = u.id
                INNER JOIN produtos p       ON ft.id_produto_material = p.id
                INNER JOIN unidades_medida um ON p.id_unidade_medida = um.id
                WHERE ft.id_produto = @IdProduto AND ft.ativo = 1";

            var resultado = await conexao.QueryAsync<FichasTecnicas, Produtos, FichasTecnicas>(
                sql,
                (ficha, material) =>
                {
                    ficha.OProdutoMaterial = material;
                    return ficha;
                },
                splitOn: "id",
                param: new { IdProduto = idProduto }
            );
            return resultado.ToList();
        }

        public async Task InserirAsync(FichasTecnicas entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO fichas_tecnicas
                           (id_produto, id_produto_material, quantidade, observacao,
                            data_criacao, data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdProduto, @IdProdutoMaterial, @Quantidade, @Observacao,
                            @DataCriacao, @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarAsync(FichasTecnicas entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE fichas_tecnicas SET
                           quantidade                    = @Quantidade,
                           observacao                    = @Observacao,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task RemoverAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(
                "DELETE FROM fichas_tecnicas WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteMaterialNaFichaAsync(int idProduto, int idProdutoMaterial, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"SELECT COUNT(1) FROM fichas_tecnicas
                           WHERE id_produto = @IdProduto
                           AND id_produto_material = @IdProdutoMaterial
                           AND id != @Id";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql,
                new { IdProduto = idProduto, IdProdutoMaterial = idProdutoMaterial, Id = ignorarId });
            return qtde > 0;
        }
    }
}
