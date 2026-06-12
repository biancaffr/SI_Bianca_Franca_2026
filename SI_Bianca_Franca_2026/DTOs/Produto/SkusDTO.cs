using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Produto
{
    public class SkusDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(50, ErrorMessage = "Máximo de 50 caracteres")]
        public string Sku { get; set; } = string.Empty;

        public string? SkuOriginal { get; set; } 

        [MaxLength(14, ErrorMessage = "Máximo de 14 caracteres")]
        public string? GtinEan { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Preço não pode ser negativo")]
        public decimal? PrecoCusto { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Estoque não pode ser negativo")]
        public decimal Estoque { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Estoque mínimo não pode ser negativo")]
        public decimal EstoqueMinimo { get; set; }

        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public List<SkusAtributosValoresDTO> Atributos { get; set; } = new();
    }
}
