using SI_Bianca_Franca_2026.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace SI_Bianca_Franca_2026.Models.Localizacao
{
    public class Estados : Pai
    {
        public string Estado { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public int IdPais { get; set; }
        public Paises? OPais { get; set; }
        public List<Cidades> Cidades { get; set; } = new();
    }
}
