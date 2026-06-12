using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Financeiro
{
    public class CondicoesPagamentosParcelasDTO
    {
        public int Id { get; set; }
        public int NumeroParcela { get; set; }

        [Range(0.0001, 100, ErrorMessage = "Percentual deve ser maior que 0")]
        public decimal Percentual { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Prazo não pode ser negativo")]
        public int PrazoDias { get; set; }
    }
}
