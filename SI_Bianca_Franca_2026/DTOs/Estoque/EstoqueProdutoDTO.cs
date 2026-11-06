namespace SI_Bianca_Franca_2026.DTOs.Estoque
{
    /// <summary>Situação atual do estoque de um produto (última movimentação).</summary>
    public class EstoqueProdutoDTO
    {
        public decimal Saldo { get; set; }
        public decimal CustoMedio { get; set; }
    }
}
