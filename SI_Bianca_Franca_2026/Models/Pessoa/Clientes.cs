using SI_Bianca_Franca_2026.Models.Financeiro;
namespace SI_Bianca_Franca_2026.Models.Pessoa
{
    public class Clientes : Pessoas
    {
        public decimal LimiteCredito { get; set; } = 0;
        public int? IdCondicaoPagamento { get; set; }

        public CondicoesPagamentos? OCondicaoPagamento { get; set; }
    }
}