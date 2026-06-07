using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.DTOs.Pessoa
{
    public class ClientesDTO : PessoasDTO
    {
        [Range(0, double.MaxValue, ErrorMessage = "Limite de crédito não pode ser negativo")]
        public decimal LimiteCredito { get; set; } = 0;
    }
}
