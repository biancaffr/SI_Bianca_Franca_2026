using System.ComponentModel.DataAnnotations;

namespace SI_Bianca_Franca_2026.Models.Base
{
    public abstract class Pai
    {
        public int Id { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataUltimaAlteracao { get; set; }
        public int IdUsuarioUltimaAlteracao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }
}
