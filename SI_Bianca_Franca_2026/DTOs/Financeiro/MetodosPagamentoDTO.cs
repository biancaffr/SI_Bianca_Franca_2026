using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Financeiro
{
    public class MetodosPagamentoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(10, ErrorMessage = "Máximo de 5 caracteres")]
        public string Sigla { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(100, ErrorMessage = "Máximo de 50 caracteres")]
        public string Metodo { get; set; } = string.Empty;

        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}
