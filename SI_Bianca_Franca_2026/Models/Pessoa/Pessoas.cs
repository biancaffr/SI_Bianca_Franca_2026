using SI_Bianca_Franca_2026.Models.Base;
using SI_Bianca_Franca_2026.Models.Localizacao;

namespace SI_Bianca_Franca_2026.Models.Pessoa
{    public abstract class Pessoas : Pai
    {
        public string TipoPessoa { get; set; } = string.Empty;
        public string NomeRazaoSocial { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string? RgIe { get; set; }
        public string? ApelidoNomeFantasia { get; set; }
        public int? IdCidade { get; set; }
        public string? Bairro { get; set; }
        public string? Logradouro { get; set; }
        public string? Numero { get; set; }
        public string? Complemento { get; set; }
        public string? Cep { get; set; }
        public int IdPais { get; set; }
        public string? Telefone { get; set; }
        public string? Email { get; set; }
        public string? Observacao { get; set; }
        public Cidades? OCidade { get; set; }
        public Paises? OPais { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}
