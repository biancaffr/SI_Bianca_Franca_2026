using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Financeiro
{
    public class CondicoesPagamentos : Pai
    {
        public string CondicaoPagamento { get; set; } = string.Empty;
        public decimal DescontoPercentual { get; set; }
        public decimal MultaPercentual { get; set; }
        public decimal TaxaJurosPercentual { get; set; }
        public List<CondicoesPagamentosParcelas> Parcelas { get; set; } = new();
    }
}