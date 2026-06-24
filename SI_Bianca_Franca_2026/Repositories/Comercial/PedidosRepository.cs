using MySql.Data.MySqlClient;
using Dapper;
using SI_Bianca_Franca_2026.Models.Comercial;
using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Repositories.Comercial
{
    public class PedidosRepository
    {
        private readonly string _stringConexao;

        public PedidosRepository(IConfiguration configuration)
        {
            _stringConexao = configuration.GetConnectionString("DefaultConnection");
        }

        private static string SqlBase => @"
            SELECT
                p.id,
                p.id_cliente               AS IdCliente,
                p.id_emitente              AS IdEmitente,
                p.data_pedido              AS DataPedido,
                p.data_previsao_entrega    AS DataPrevisaoEntrega,
                p.data_entrega             AS DataEntrega,
                p.status,
                p.id_venda                 AS IdVenda,
                p.valor_total              AS ValorTotal,
                p.observacao,
                p.data_criacao             AS DataCriacao,
                p.data_ultima_alteracao    AS DataUltimaAlteracao,
                p.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                p.ativo,
                u.nome                     AS NomeUsuarioAlteracao,
                c.id,
                c.nome_razao_social        AS NomeRazaoSocial,
                e.id,
                e.nome_razao_social        AS NomeRazaoSocial
            FROM pedidos p
            LEFT JOIN usuarios u    ON p.id_usuario_ultima_alteracao = u.id
            INNER JOIN clientes c   ON p.id_cliente = c.id
            INNER JOIN emitentes e  ON p.id_emitente = e.id";

        private static string SqlPesquisar => SqlBase + " WHERE p.id = @Id";
        private static string SqlPorStatus => SqlBase + " WHERE p.status = @Status AND p.ativo = 1";

        private static Pedidos MapearPedido(Pedidos pedido, Clientes cliente, Emitentes emitente)
        {
            pedido.OCliente = cliente;
            pedido.OEmitente = emitente;
            return pedido;
        }

        public async Task<List<Pedidos>> ListarTudoAsync()
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Pedidos, Clientes, Emitentes, Pedidos>(
                SqlBase,
                MapearPedido,
                splitOn: "id,id"
            );
            return resultado.ToList();
        }

        public async Task<List<Pedidos>> ListarPorStatusAsync(string status)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Pedidos, Clientes, Emitentes, Pedidos>(
                SqlPorStatus,
                MapearPedido,
                splitOn: "id,id",
                param: new { Status = status }
            );
            return resultado.ToList();
        }

        public async Task<Pedidos?> PesquisarAsync(int id)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var resultado = await conexao.QueryAsync<Pedidos, Clientes, Emitentes, Pedidos>(
                SqlPesquisar,
                MapearPedido,
                splitOn: "id,id",
                param: new { Id = id }
            );

            var pedido = resultado.FirstOrDefault();
            if (pedido != null)
                pedido.Itens = await ListarItensAsync(pedido.Id);

            return pedido;
        }

        public async Task<List<PedidosItens>> ListarItensAsync(int idPedido)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"
                SELECT
                    pi.id,
                    pi.id_pedido                AS IdPedido,
                    pi.id_produto               AS IdProduto,
                    pi.descricao_personalizacao AS DescricaoPersonalizacao,
                    pi.quantidade,
                    pi.valor_unitario           AS ValorUnitario,
                    pi.valor_desconto           AS ValorDesconto,
                    pi.valor_total              AS ValorTotal,
                    pi.data_criacao             AS DataCriacao,
                    pi.data_ultima_alteracao    AS DataUltimaAlteracao,
                    pi.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                    pi.ativo,
                    pr.id,
                    pr.produto,
                    um.sigla
                FROM pedidos_itens pi
                INNER JOIN produtos pr ON pi.id_produto = pr.id
                INNER JOIN unidades_medida um ON pr.id_unidade_medida = um.id
                WHERE pi.id_pedido = @IdPedido AND pi.ativo = 1";

            var itens = (await conexao.QueryAsync<PedidosItens, Produtos, PedidosItens>(
                sql,
                (item, produto) =>
                {
                    item.OProduto = produto;
                    return item;
                },
                splitOn: "id",
                param: new { IdPedido = idPedido }
            )).ToList();

            foreach (var item in itens)
                item.Materiais = await ListarMateriaisDoItemAsync(item.Id);

            return itens;
        }

        public async Task<List<PedidosFichasTecnicas>> ListarMateriaisDoItemAsync(int idPedidoItem)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"
                SELECT
                    pf.id,
                    pf.id_pedido_item   AS IdPedidoItem,
                    pf.sku,
                    pf.quantidade,
                    pf.observacao,
                    pf.data_criacao             AS DataCriacao,
                    pf.data_ultima_alteracao    AS DataUltimaAlteracao,
                    pf.id_usuario_ultima_alteracao AS IdUsuarioUltimaAlteracao,
                    pf.ativo,
                    s.sku                AS SkuJoin,
                    s.id_produto         AS IdProduto,
                    pr.id                AS ProdutoId,
                    pr.produto           AS Produto
                FROM pedidos_fichas_tecnicas pf
                INNER JOIN skus s ON pf.sku = s.sku
                INNER JOIN produtos pr ON s.id_produto = pr.id
                WHERE pf.id_pedido_item = @IdPedidoItem AND pf.ativo = 1";

            var resultado = await conexao.QueryAsync<PedidosFichasTecnicas, Skus, Produtos, PedidosFichasTecnicas>(
                sql,
                (material, sku, produto) =>
                {
                    sku.OProduto = produto;
                    material.OSku = sku;
                    return material;
                },
                splitOn: "SkuJoin,ProdutoId",
                param: new { IdPedidoItem = idPedidoItem }
            );
            return resultado.ToList();
        }

        public async Task<int> InserirAsync(Pedidos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO pedidos
                           (id_cliente, id_emitente, data_pedido, data_previsao_entrega, data_entrega,
                            status, id_venda, valor_total, observacao, data_criacao,
                            data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdCliente, @IdEmitente, @DataPedido, @DataPrevisaoEntrega, @DataEntrega,
                            @Status, @IdVenda, @ValorTotal, @Observacao, @DataCriacao,
                            @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo);
                           SELECT LAST_INSERT_ID();";
            return await conexao.ExecuteScalarAsync<int>(sql, entity);
        }

        public async Task AtualizarAsync(Pedidos entity)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE pedidos SET
                           id_cliente                    = @IdCliente,
                           id_emitente                   = @IdEmitente,
                           data_pedido                   = @DataPedido,
                           data_previsao_entrega         = @DataPrevisaoEntrega,
                           data_entrega                  = @DataEntrega,
                           status                        = @Status,
                           id_venda                      = @IdVenda,
                           valor_total                   = @ValorTotal,
                           observacao                    = @Observacao,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao,
                           ativo                         = @Ativo
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, entity);
        }

        public async Task AtualizarStatusAsync(int id, string status, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE pedidos SET
                           status                        = @Status,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuario
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, new
            {
                Status = status,
                DataUltimaAlteracao = DateTime.Now,
                IdUsuario = idUsuario,
                Id = id
            });
        }

        public async Task AlterarStatusAtivoAsync(int id, bool novoStatus, int idUsuario)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE pedidos SET
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

        public async Task<int> InserirItemAsync(PedidosItens item)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO pedidos_itens
                           (id_pedido, id_produto, descricao_personalizacao, quantidade,
                            valor_unitario, valor_desconto, valor_total, data_criacao,
                            data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdPedido, @IdProduto, @DescricaoPersonalizacao, @Quantidade,
                            @ValorUnitario, @ValorDesconto, @ValorTotal, @DataCriacao,
                            @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo);
                           SELECT LAST_INSERT_ID();";
            return await conexao.ExecuteScalarAsync<int>(sql, item);
        }

        public async Task AtualizarItemAsync(PedidosItens item)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"UPDATE pedidos_itens SET
                           id_produto                    = @IdProduto,
                           descricao_personalizacao      = @DescricaoPersonalizacao,
                           quantidade                    = @Quantidade,
                           valor_unitario                = @ValorUnitario,
                           valor_desconto                = @ValorDesconto,
                           valor_total                   = @ValorTotal,
                           data_ultima_alteracao         = @DataUltimaAlteracao,
                           id_usuario_ultima_alteracao   = @IdUsuarioUltimaAlteracao
                           WHERE id = @Id";
            await conexao.ExecuteAsync(sql, item);
        }

        public async Task RemoverItemAsync(int idItem)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(
                "DELETE FROM pedidos_fichas_tecnicas WHERE id_pedido_item = @Id", new { Id = idItem });
            await conexao.ExecuteAsync(
                "DELETE FROM pedidos_itens WHERE id = @Id", new { Id = idItem });
        }

        public async Task InserirMaterialAsync(PedidosFichasTecnicas material)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            string sql = @"INSERT INTO pedidos_fichas_tecnicas
                           (id_pedido_item, sku, quantidade, observacao, data_criacao,
                            data_ultima_alteracao, id_usuario_ultima_alteracao, ativo)
                           VALUES
                           (@IdPedidoItem, @Sku, @Quantidade, @Observacao, @DataCriacao,
                            @DataUltimaAlteracao, @IdUsuarioUltimaAlteracao, @Ativo)";
            await conexao.ExecuteAsync(sql, material);
        }

        public async Task DeletarMateriaisDoItemAsync(int idPedidoItem)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            await conexao.ExecuteAsync(
                "DELETE FROM pedidos_fichas_tecnicas WHERE id_pedido_item = @Id", new { Id = idPedidoItem });
        }

        public async Task<bool> ExisteVendaVinculadaAsync(int idPedido)
        {
            using var conexao = new MySqlConnection(_stringConexao);
            var qtde = await conexao.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM pedidos WHERE id = @Id AND id_venda IS NOT NULL",
                new { Id = idPedido });
            return qtde > 0;
        }
    }
}