using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Financeiro
{
    public class CondicoesPagamentosParcelas : Pai
    {
        public int IdCondicaoPagamento { get; set; }
        public int NumeroParcela { get; set; }
        public decimal Percentual { get; set; }
        public int PrazoDias { get; set; }
    }
}