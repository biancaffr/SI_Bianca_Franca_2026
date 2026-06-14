using SI_Bianca_Franca_2026.Models.Base;
using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Models.Financeiro
{
    public class CondicoesPagamentos : Pai
    {
        public string Descricao { get; set; } = string.Empty;
        public int IdMetodoPagamento { get; set; }
        public decimal EntradaMinimaPercentual { get; set; }
        public decimal DescontoPercentual { get; set; }
        public decimal AcrescimoPercentual { get; set; }
        public decimal MultaPercentual { get; set; }
        public decimal TaxaJurosPercentual { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public MetodosPagamento? OMetodoPagamento { get; set; }
        public List<CondicoesPagamentosParcelas> Parcelas { get; set; } = new();
    }
}
