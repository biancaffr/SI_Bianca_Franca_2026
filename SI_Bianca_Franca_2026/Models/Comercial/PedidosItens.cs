using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Models.Comercial
{
    public class PedidosItens
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int IdProduto { get; set; }
        public string? DescricaoPersonalizacao { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public int IdUsuarioUltimaAlteracao { get; set; }
        public bool Ativo { get; set; }

        public Produtos? OProduto { get; set; }
        public List<PedidosFichasTecnicas> Materiais { get; set; } = new();
    }
}