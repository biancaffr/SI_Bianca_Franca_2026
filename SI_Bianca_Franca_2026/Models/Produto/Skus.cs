using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Produto
{
    public class Skus
    {
        public string Sku { get; set; } = string.Empty;
        public int IdProduto { get; set; }
        public string? GtinEan { get; set; }
        public decimal? PrecoCusto { get; set; }
        public decimal Estoque { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public int IdUsuarioUltimaAlteracao { get; set; }
        public bool Ativo { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public Produtos? OProduto { get; set; }
        public List<SkusAtributosValores> Atributos { get; set; } = new();
    }
}
