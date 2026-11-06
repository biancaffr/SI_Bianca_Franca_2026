using SI_Bianca_Franca_2026.Models.Base;
using SI_Bianca_Franca_2026.Models.Financeiro;
using SI_Bianca_Franca_2026.Models.Pessoa;

namespace SI_Bianca_Franca_2026.Models.NfeEntrada
{
    public class NfeEntradas
    {
        public string Numero { get; set; } = string.Empty;
        public string Modelo { get; set; } = "55";
        public string Serie { get; set; } = "1";
        public int IdFornecedor { get; set; }
        public int? IdTransportadora { get; set; }
        public int? IdCondicaoPagamento { get; set; }
        public DateTime DataEmissao { get; set; }
        public DateTime? DataChegada { get; set; }
        public string TipoFrete { get; set; } = "CIF";
        public decimal ValorProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacao { get; set; }
        public string Situacao { get; set; } = "PENDENTE";
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public int IdUsuarioUltimaAlteracao { get; set; }
        public bool Ativo { get; set; } = true;
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public Fornecedores? OFornecedor { get; set; }
        public Transportadoras? OTransportadora { get; set; }
        public CondicoesPagamentos? OCondicaoPagamento { get; set; }
        public List<NfeEntradaItens> Itens { get; set; } = new();
    }
}