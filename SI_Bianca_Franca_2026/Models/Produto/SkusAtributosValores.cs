using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Produto
{
    public class SkusAtributosValores:Pai
    {
        public int IdChave { get; set; }
        public string Valor { get; set; } = string.Empty;
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public SkuAtributosChaves? OChave { get; set; }
    }
}
