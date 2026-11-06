using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Estoque
{
    public class MovimentacoesEstoque : Pai
    {
        public DateTime DataMovimentacao { get; set; }
        public string TipoMovimentacao { get; set; } = string.Empty;
        public int? IdUsuario { get; set; }
        public int? IdVenda { get; set; }
        public int? IdOrdemServico { get; set; }

        // NF-e de entrada de origem (chave composta)
        public string? NotaNumero { get; set; }
        public string? NotaModelo { get; set; }
        public string? NotaSerie { get; set; }
        public int? IdFornecedor { get; set; }

        public string? Observacao { get; set; }

        public List<MovimentacoesEstoqueItens> Itens { get; set; } = new();
    }
}
