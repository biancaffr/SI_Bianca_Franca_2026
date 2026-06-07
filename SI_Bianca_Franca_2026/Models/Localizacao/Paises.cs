using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Localizacao
{
    public class Paises : Pai
    {
        public string Pais { get; set; } = string.Empty;
        public string Sigla { get; set; } = string.Empty;
        public string Ddi { get; set; } = string.Empty;
        public string Moeda { get; set; } = string.Empty;
        public List<Estados> Estados { get; set; } = new();
    }
}
