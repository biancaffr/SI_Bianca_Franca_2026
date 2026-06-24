using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Comercial
{
    public class PedidosDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um cliente válido")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um emitente válido")]
        public int IdEmitente { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        public DateTime? DataPrevisaoEntrega { get; set; }
        public DateTime? DataEntrega { get; set; }

        public string Status { get; set; } = "AGUARDANDO_APROVACAO_ARTE";
        public int? IdVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacao { get; set; }

        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public List<PedidosItensDTO> Itens { get; set; } = new();
    }
}