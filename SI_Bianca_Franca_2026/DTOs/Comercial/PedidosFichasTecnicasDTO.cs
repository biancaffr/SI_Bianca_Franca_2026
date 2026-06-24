using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Comercial
{
    public class PedidosFichasTecnicasDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public string Sku { get; set; } = string.Empty;

        public string? NomeMaterial { get; set; }
        public string? UnidadeMedida { get; set; }
        public int IdProdutoMaterialOrigem { get; set; } // referência ao material genérico da ficha técnica

        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public decimal Quantidade { get; set; }

        public string? Observacao { get; set; }
    }
}