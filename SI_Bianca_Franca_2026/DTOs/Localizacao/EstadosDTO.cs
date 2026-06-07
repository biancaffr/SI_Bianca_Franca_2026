using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Localizacao
{
    public class EstadosDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(55, ErrorMessage = "Máximo de 55 caracteres")]
        public string Estado { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(2, ErrorMessage = "Máximo de 2 caracteres")]
        public string Uf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um País válido")]
        public int IdPais { get; set; }

        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}
