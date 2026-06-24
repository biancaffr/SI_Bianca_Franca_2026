namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Comercial
{
    public class ItemPedidoEdicao
    {
        public int Id { get; set; }
        public int IdProduto { get; set; }
        public string? NomeProduto { get; set; }
        public string? UnidadeMedida { get; set; }
        public string? DescricaoPersonalizacao { get; set; }
        public decimal Quantidade { get; set; } = 1;
        public decimal ValorUnitario { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal => (Quantidade * ValorUnitario) - ValorDesconto;

        public List<MaterialPedidoEdicao> Materiais { get; set; } = new();
    }

    public class MaterialPedidoEdicao
    {
        public int Id { get; set; } 
        public int IdProdutoMaterial { get; set; }
        public string? NomeProdutoMaterial { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal Quantidade { get; set; }
        public string? Observacao { get; set; }
        public bool VeioDaFicha { get; set; }
    }
}