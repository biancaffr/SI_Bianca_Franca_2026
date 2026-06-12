using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Financeiro
{
    public class CondicoesPagamentosDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(150, ErrorMessage = "Máximo de 150 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um método de pagamento válido")]
        public int IdMetodoPagamento { get; set; }

        [Range(0, 100, ErrorMessage = "Valor entre 0 e 100")]
        public decimal EntradaMinimaPercentual { get; set; }

        [Range(0, 100, ErrorMessage = "Valor entre 0 e 100")]
        public decimal DescontoPercentual { get; set; }

        [Range(0, 100, ErrorMessage = "Valor entre 0 e 100")]
        public decimal AcrescimoPercentual { get; set; }

        [Range(0, 100, ErrorMessage = "Valor entre 0 e 100")]
        public decimal MultaPercentual { get; set; }

        [Range(0, 100, ErrorMessage = "Valor entre 0 e 100")]
        public decimal TaxaJurosPercentual { get; set; }

        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public List<CondicoesPagamentosParcelasDTO> Parcelas { get; set; } = new();
    }
}
