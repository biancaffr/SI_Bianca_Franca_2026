using SI_Bianca_Franca_2026.Models.Base;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Models.Comercial
{
    public class Pedidos : Pai
    {
        public int IdCliente { get; set; }
        public int IdEmitente { get; set; }
        public DateTime DataPedido { get; set; }
        public DateTime? DataPrevisaoEntrega { get; set; }
        public DateTime? DataEntrega { get; set; }
        public string Status { get; set; } = "AGUARDANDO_APROVACAO_ARTE";
        public int? IdVenda { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public Clientes? OCliente { get; set; }
        public Emitentes? OEmitente { get; set; }
        public List<PedidosItens> Itens { get; set; } = new();
    }
}