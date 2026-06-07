using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Pessoas
{
    public abstract class PessoasDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        public string TipoPessoa { get; set; } = "FISICA";

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(150)]
        public string NomeRazaoSocial { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? CpfCnpj { get; set; }

        [MaxLength(20)]
        public string? RgIe { get; set; }

        [MaxLength(100)]
        public string? ApelidoNomeFantasia { get; set; }

        public int? IdCidade { get; set; }

        [MaxLength(80)]
        public string? Bairro { get; set; }

        [MaxLength(150)]
        public string? Logradouro { get; set; }

        [MaxLength(10)]
        public string? Numero { get; set; }

        [MaxLength(100)]
        public string? Complemento { get; set; }

        [MaxLength(9)]
        public string? Cep { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um País válido")]
        public int IdPais { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }

        [MaxLength(254)]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string? Email { get; set; }

        public string? Observacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}
