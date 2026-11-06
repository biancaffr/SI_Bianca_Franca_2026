using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.NfeEntrada
{
    public class NfeEntradaDTO
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Apenas números são permitidos")]
        [StringLength(20, ErrorMessage = "Máximo 20 caracteres")]
        public string Numero { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(2, ErrorMessage = "Máximo 2 caracteres")]
        public string Modelo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(3, ErrorMessage = "Máximo 3 caracteres")]
        public string Serie { get; set; } = string.Empty;

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um fornecedor válido")]
        public int IdFornecedor { get; set; }

        public int? IdTransportadora { get; set; }
        public int? IdCondicaoPagamento { get; set; }

        public DateTime? DataEmissao { get; set; }
        public DateTime? DataChegada { get; set; }

        public string TipoFrete { get; set; } = "CIF";
        public decimal ValorFrete { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorOutrasDespesas { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorProdutos { get; set; }
        public decimal ValorTotal { get; set; }
        public string? Observacao { get; set; }
        public string Situacao { get; set; } = "PENDENTE";
        public bool Ativo { get; set; } = true;
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
        public string NomeFornecedor { get; set; } = string.Empty;
        public string NomeFornecedorCompleto { get; set; } = string.Empty;
        public string? NomeTransportadora { get; set; }
        public string? NomeCondicaoPagamento { get; set; }

        public List<NfeEntradaItensDTO> Itens { get; set; } = new();
        public List<ParcelaVisualizacaoDTO> Parcelas { get; set; } = new();
    }
}