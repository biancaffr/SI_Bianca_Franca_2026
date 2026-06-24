using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Models.Comercial
{
    public class PedidosFichasTecnicas
    {
        public int Id { get; set; }
        public int IdPedidoItem { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public string? Observacao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public int IdUsuarioUltimaAlteracao { get; set; }
        public bool Ativo { get; set; }

        public Skus? OSku { get; set; }
    }
}