namespace SI_Bianca_Franca_2026.DTOs.NfeEntrada
{
    public class NfeEntradaItensDTO
    {
        public int IdProduto { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string SiglaProduto { get; set; } = string.Empty;
        public short Sequencia { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal CustoFinal { get; set; }
        public decimal ValorDesconto { get; set; } 
        public decimal ValorTotal => Math.Round((Quantidade * ValorUnitario) - ValorDesconto, 4);
    }
}