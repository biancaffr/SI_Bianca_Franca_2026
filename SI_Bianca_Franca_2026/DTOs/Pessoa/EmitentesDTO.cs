using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Pessoa
{
    public class EmitentesDTO : PessoasDTO
    {
        [MaxLength(20)]
        public string? InscricaoMunicipal { get; set; }

        [MaxLength(50)]
        public string? RegimeTributario { get; set; }
    }
}
