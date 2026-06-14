using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Produto
{
    public class ProdutosDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(150, ErrorMessage = "Máximo de 150 caracteres")]
        public string Produto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        public string Tipo { get; set; } = "PRODUTO_FINAL";

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
