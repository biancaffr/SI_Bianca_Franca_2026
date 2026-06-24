using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Produto
{
    public class SkusAtributosValoresRelacionamentoDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um valor válido")]
        public int IdValor { get; set; }

    }
}
