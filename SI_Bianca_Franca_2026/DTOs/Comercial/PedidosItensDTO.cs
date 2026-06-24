using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Comercial
{
    public class PedidosItensDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um produto válido")]
        public int IdProduto { get; set; }

        public string? NomeProduto { get; set; }

        public string? DescricaoPersonalizacao { get; set; }

        [Range(0.0001, double.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public decimal Quantidade { get; set; } = 1;

        [Range(0, double.MaxValue, ErrorMessage = "Valor não pode ser negativo")]
        public decimal ValorUnitario { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor não pode ser negativo")]
        public decimal ValorDesconto { get; set; }

        public decimal ValorTotal => (Quantidade * ValorUnitario) - ValorDesconto;

        public List<PedidosFichasTecnicasDTO> Materiais { get; set; } = new();
    }
}