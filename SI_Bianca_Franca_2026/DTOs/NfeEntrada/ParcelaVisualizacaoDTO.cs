namespace SI_Bianca_Franca_2026.DTOs.NfeEntrada
{
    public class ParcelaVisualizacaoDTO
    {
        public int Numero { get; set; }
        public int Total { get; set; }
        public DateTime Vencimento { get; set; }
        public decimal Valor { get; set; }
        public string MetodoPagamento { get; set; } = string.Empty;
    }
}