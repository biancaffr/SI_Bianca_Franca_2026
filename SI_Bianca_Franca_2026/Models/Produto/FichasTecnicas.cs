using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Produto
{
    public class FichasTecnicas : Pai
    {
        public int IdProduto { get; set; }
        public int IdProdutoMaterial { get; set; }
        public decimal Quantidade { get; set; }
        public string? Observacao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public Produtos? OProdutoMaterial { get; set; }
    }
}
