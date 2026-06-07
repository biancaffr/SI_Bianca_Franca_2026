using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models
{
    public class Usuarios
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string? Telefone { get; set; }
        public DateTime? UltimoAcesso { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public bool Ativo { get; set; }
    }
}
