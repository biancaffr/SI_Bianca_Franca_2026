using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Produto
{
    public class FichasTecnicasDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um material válido")]
        public int IdProdutoMaterial { get; set; }

        public string NomeMaterial { get; set; } = string.Empty;
        public string UnidadeMedida { get; set; } = string.Empty;

        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public decimal Quantidade { get; set; }

        public string? Observacao { get; set; }
    }
}
