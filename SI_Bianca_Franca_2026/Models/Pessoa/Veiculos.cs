using SI_Bianca_Franca_2026.Models.Base;
using SI_Bianca_Franca_2026.Models.Localizacao;

namespace SI_Bianca_Franca_2026.Models.Pessoa
{
    public class Veiculos : Pai
    {
        public int? IdTransportadora { get; set; }
        public int IdEstado { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string? Rntrc { get; set; }
        public string? Renavam { get; set; }
        public string? TipoVeiculo { get; set; }
        public string? MarcaModelo { get; set; }
        public string? Observacao { get; set; }
        public string NomeUsuarioAlteracao { get; set; } = string.Empty;

        public Transportadoras? OTransportadora { get; set; }
        public Estados? OEstado { get; set; }
    }
}
