using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Pessoa
{
    public class TransportadorasDTO : PessoasDTO
    {
        [MaxLength(20)]
        public string? Rntrc { get; set; }
    }
}
