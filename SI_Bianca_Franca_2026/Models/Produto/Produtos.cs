using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Produto
{
    public class Produtos : Pai
    {
        public string Produto { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public decimal? PrecoVenda { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public decimal? ValorUltimaCompra { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int IdCategoria { get; set; }
        public int IdUnidadeMedida { get; set; }
        public string? Ncm { get; set; }
        public string? Cest { get; set; }
        public byte Origem { get; set; } = 0;
        public string? Observacao { get; set; }
        public Categorias? OCategoria { get; set; }
        public UnidadesMedida? OUnidadeMedida { get; set; }
    }
}