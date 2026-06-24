// Models/Produto/SkusAtributosValoresRelacionamento.cs
namespace SI_Bianca_Franca_2026.Models.Produto
{
    public class SkusAtributosValoresRelacionamento
    {
        public string Sku { get; set; } = string.Empty;
        public int IdValor { get; set; }

        public SkusAtributosValores? OValor { get; set; }
    }
}