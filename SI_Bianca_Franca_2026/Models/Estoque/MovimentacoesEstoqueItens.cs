using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Estoque
{
    public class MovimentacoesEstoqueItens : Pai
    {
        public int IdMovimentacaoEstoque { get; set; }
        public int IdProduto { get; set; }
        public decimal Quantidade { get; set; }
        public decimal CustoUnitario { get; set; }

        // Estado do estoque do produto DEPOIS desta movimentação
        public decimal SaldoApos { get; set; }
        public decimal CustoMedio { get; set; }
    }
}
