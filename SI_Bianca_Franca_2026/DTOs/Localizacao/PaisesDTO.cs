using SI_Bianca_Franca_2026.Models.Localizacao;
using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Localizacao
{
    public class PaisesDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(60, ErrorMessage = "Máximo de 60 caracteres")]
        public string Pais { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(3, ErrorMessage = "Máximo de 3 caracteres")]
        public string Sigla { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(5, ErrorMessage = "Máximo de 5 caracteres")]
        public string Ddi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(3, ErrorMessage = "Máximo de 3 caracteres")]
        public string Moeda { get; set; } = string.Empty;

        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}
