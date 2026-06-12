namespace SI_Bianca_Franca_2026.Models.Produto
{
    public class SkusAtributosValores
    {
        public string Sku { get; set; } = string.Empty;
        public int IdChave { get; set; }
        public string Valor { get; set; } = string.Empty;

        public SkuAtributosChaves? OChave { get; set; }
    }
}
