using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Produto
{
    public class ProdutosDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(70, ErrorMessage = "Máximo de 70 caracteres")]
        public string Produto { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Máximo de 20 caracteres")]
        public string? Referencia { get; set; }

        [Range(0, 999999.9999, ErrorMessage = "Máx: 999.999,9999")]
        public decimal EstoqueMinimo { get; set; }

        [Range(0, 99999.99, ErrorMessage = "Máx: 99.999,99")]
        public decimal? PrecoVenda { get; set; }

        [Range(0, 99999.99, ErrorMessage = "Máx: 99.999,99")]
        public decimal? ValorUltimaCompra { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma categoria válida")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma unidade de medida válida")]
        public int IdUnidadeMedida { get; set; }

        [MaxLength(8, ErrorMessage = "Máximo de 8 caracteres")]
        public string? Ncm { get; set; }

        [MaxLength(7, ErrorMessage = "Máximo de 7 caracteres")]
        public string? Cest { get; set; }

        public byte Origem { get; set; } = 0;
        public string? Observacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}