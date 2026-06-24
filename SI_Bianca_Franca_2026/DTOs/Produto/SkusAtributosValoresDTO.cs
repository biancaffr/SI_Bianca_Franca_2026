using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Produto
{
    public class SkusAtributosValoresDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione uma chave válida")]
        public int IdChave { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(150, ErrorMessage = "Máximo de 150 caracteres")]
        public string Valor { get; set; } = string.Empty;

        public bool Ativo { get; set; }
    }
}
