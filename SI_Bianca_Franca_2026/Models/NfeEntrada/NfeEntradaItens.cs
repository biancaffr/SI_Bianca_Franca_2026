using SI_Bianca_Franca_2026.Models.Produto;

namespace SI_Bianca_Franca_2026.Models.NfeEntrada
{
    public class NfeEntradaItens
    {
        public string NotaNumero { get; set; } = string.Empty;
        public string NotaModelo { get; set; } = string.Empty;
        public string NotaSerie { get; set; } = string.Empty;
        public int IdFornecedor { get; set; }
        public int IdProduto { get; set; }
        public short Sequencia { get; set; }
        public decimal Quantidade { get; set; }
        public decimal ValorUnitario { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal PercentualDesconto { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal RateioFrete { get; set; }
        public decimal RateioSeguro { get; set; }
        public decimal RateioOutras { get; set; }
        public decimal? CustoFinal { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }

        public Produtos? OProduto { get; set; }
    }
}