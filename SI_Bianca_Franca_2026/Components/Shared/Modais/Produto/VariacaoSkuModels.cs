namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto
{
    public class OpcaoVariacao
    {
        public int IdChave { get; set; }
        public string NomeChave { get; set; } = string.Empty;

        public List<(int IdValor, string Valor)> ValoresDisponiveis { get; set; } = new();

        public List<int> ValoresSelecionados { get; set; } = new();
    }

    public class VariacaoGerada
    {
        public string Sku { get; set; } = string.Empty;
        public string SkuOriginalNoBanco { get; set; } = string.Empty;

        public List<(int IdValor, int IdChave, string NomeChave, string Valor)> Combinacao { get; set; } = new();

        public string Variacao => string.Join(" / ", Combinacao.Select(c => c.Valor));

        public string ChaveComparacao =>
            string.Join("|", Combinacao.OrderBy(c => c.IdValor).Select(c => c.IdValor));

        public string? GtinEan { get; set; }
        public decimal? PrecoCusto { get; set; }
        public decimal Estoque { get; set; }
        public decimal EstoqueMinimo { get; set; }
        public bool Ativo { get; set; } = true;
        public bool SeraDesativada { get; set; } = false;
    }
}