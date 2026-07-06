using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.DTOs.Filtros;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Produto
{
    public class ProdutosRepository
    {
        private readonly string _stringConexao;

        public ProdutosRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection")!;
        }

        private static string SqlBase => @"
            SELECT
                p.id,
                p.produto,
                p.referencia,
                p.saldo,
                p.preco_custo               AS PrecoCusto,
                p.preco_venda               AS PrecoVenda,
                p.estoque_minimo            AS EstoqueMinimo,
                p.valor_ultima_compra       AS ValorUltimaCompra,
                p.tipo,
                p.id_categoria              AS IdCategoria,
                p.id_unidade_medida         AS IdUnidadeMedida,
                p.ncm,
                p.cest,
                p.origem,
                p.observacao,
                p.data_criacao              AS DataCriacao,
                p.data_ultima_alteracao     AS DataUltimaAlteracao,
                p.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                p.ativo,
                u.nome                      AS NomeUsuarioAlteracao,
                c.id                        AS CategoriaId,
                c.categoria,
                um.id                       AS UnidadeId,
                um.sigla,
                um.unidade_medida           AS UnidadeMedida
            FROM produtos p
            LEFT JOIN usuarios u        ON p.id_usuario_ultima_alteracao = u.id
            INNER JOIN categorias c     ON p.id_categoria = c.id
            INNER JOIN unidades_medida um ON p.id_unidade_medida = um.id";

        private static string SqlPesquisar => SqlBase + " WHERE p.id = @Id";
        private static string SqlPorTipo => SqlBase + " WHERE p.tipo = @Tipo";

        private static Produtos MapearProduto(Produtos produto, Categorias categoria, UnidadesMedida unidade)
        {
            produto.OCategoria = categoria;
            produto.OUnidadeMedida = unidade;
            return produto;
        }

        public async Task<List<Produtos>> ListarAsync(FiltroProdutosDto filtro)
        {
            var where = new List<string>();
            var param = new DynamicParameters();

            if (filtro.StatusFiltro == "ativo")
                where.Add("p.ativo = TRUE");
            else if (filtro.StatusFiltro == "inativo")
                where.Add("p.ativo = FALSE");

            if (filtro.TipoFiltro != "todos")
            {
                where.Add("p.tipo = @Tipo");
                param.Add("Tipo", filtro.TipoFiltro);
            }

            if (!string.IsNullOrWhiteSpace(filtro.Busca))
            {
                where.Add(@"(p.produto     LIKE @Busca
                  OR p.referencia  LIKE @Busca
                  OR c.categoria   LIKE @Busca)");
                param.Add("Busca", $"%{filtro.Busca}%");
            }

            var whereClause = where.Any()
                ? "WHERE " + string.Join(" AND ", where)
                : string.Empty;

            var sql = $"{SqlBase} {whereClause} ORDER BY p.produto";

            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Produtos, Categorias, UnidadesMedida, Produtos>(
                sql, MapearProduto,
                splitOn: "CategoriaId,UnidadeId",
                param: param);
            return resultado.ToList();
        }

        public async Task<Produtos?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Produtos, Categorias, UnidadesMedida, Produtos>(
                SqlPesquisar, MapearProduto, splitOn: "CategoriaId,UnidadeId", param: new { Id = id });
            return resultado.FirstOrDefault();
        }

        public async Task<int> InserirAsync(Produtos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO produtos
                           (produto, referencia, saldo, preco_custo, preco_venda,
                            estoque_minimo, valor_ultima_compra, tipo,
                            id_categoria, id_unidade_medida, ncm, cest, origem,
                            observacao, data_criacao, data_ultima_alteracao,
                            id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@Produto, @Referencia, @Saldo, @PrecoCusto, @PrecoVenda,
                            @EstoqueMinimo, @ValorUltimaCompra, @Tipo,
                            @IdCategoria, @IdUnidadeMedida, @Ncm, @Cest, @Origem,
                            @Observacao, @DataCriacao, @DataUltimaAlteracao,
                            @IdUsuarioUltimaAlteracao, @Ativo);
                           SELECT LAST_INSERT_ID();";
            return await conexao.ExecuteScalarAsync<int>(sql, entity);
        }

        public async Task AtualizarAsync(Produtos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE produtos SET
                           produto                       = @Produto,
                           referencia                    = @Referencia,
                           saldo                         = @Saldo,
                           preco_custo                   = @PrecoCusto,
                           preco_venda                   = @PrecoVenda,
                           estoque_minimo                = @EstoqueMinimo,
                           valor_ultima_compra           = @ValorUltimaCompra,
                           tipo                          = @Tipo,
                           id_categoria                  = @IdCategoria,
                           id_unidade_medida             = @IdUnidadeMedida,
                           ncm                           = @Ncm,
                           cest                          = @Cest,
                           origem                        = @Origem,
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
            string sql = @"UPDATE produtos SET
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
            await conexao.ExecuteAsync("DELETE FROM produtos WHERE id = @Id", new { Id = id });
        }

        public async Task<bool> ExisteProdutoCadastradoAsync(string produto, int ignorarId = 0)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = "SELECT COUNT(1) FROM produtos WHERE produto = @Produto AND id != @IgnorarId";
            var qtde = await conexao.ExecuteScalarAsync<int>(sql, new { Produto = produto, IgnorarId = ignorarId });
            return qtde > 0;
        }
    }
}