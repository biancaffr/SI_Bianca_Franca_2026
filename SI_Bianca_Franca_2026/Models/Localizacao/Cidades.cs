using SI_Bianca_Franca_2026.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SI_Bianca_Franca_2026.Models.Localizacao
{
    public class Cidades : Pai
    {
        public string Cidade { get; set; } = string.Empty;
        public int IdEstado { get; set; }
        public string CodigoIbge { get; set; } = string.Empty;
        public string Ddd { get; set; } = string.Empty;
        public Estados? OEstado { get; set; }
    }
}
