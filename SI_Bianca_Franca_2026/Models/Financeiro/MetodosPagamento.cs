using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Financeiro
{
    public class MetodosPagamento : Pai
    {
        public string Sigla { get; set; } = string.Empty;
        public string Metodo { get; set; } = string.Empty;
    }
}
