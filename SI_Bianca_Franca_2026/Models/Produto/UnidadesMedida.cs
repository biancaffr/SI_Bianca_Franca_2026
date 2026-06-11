using SI_Bianca_Franca_2026.Models.Base;

namespace SI_Bianca_Franca_2026.Models.Produto
{
    public class UnidadesMedida : Pai
    {
        public string Sigla { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;
    }

}
