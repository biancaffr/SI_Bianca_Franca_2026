using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Pessoa
{
    public class VeiculosDTO
    {
        public int Id { get; set; }

        public int? IdTransportadora { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um Estado válido")]
        public int IdEstado { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [MaxLength(10)]
        public string Placa { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Rntrc { get; set; }

        [MaxLength(20)]
        public string? Renavam { get; set; }

        [MaxLength(50)]
        public string? TipoVeiculo { get; set; }

        [MaxLength(100)]
        public string? MarcaModelo { get; set; }

        public string? Observacao { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}
