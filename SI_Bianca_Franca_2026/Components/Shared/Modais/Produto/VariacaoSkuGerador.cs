namespace SI_Bianca_Franca_2026.Components.Shared.Modais.Produto
{
    public static class VariacaoSkuGerador
    {
        public static List<List<(int IdValor, int IdChave, string NomeChave, string Valor)>> GerarCombinacoes(
            List<OpcaoVariacao> opcoes)
        {
            var validas = opcoes.Where(o => o.IdChave > 0 && o.ValoresSelecionados.Any()).ToList();
            if (!validas.Any())
                return new();

            var resultado = new List<List<(int, int, string, string)>> { new() };

            foreach (var opcao in validas)
            {
                var novoResultado = new List<List<(int, int, string, string)>>();
                foreach (var combinacaoExistente in resultado)
                {
                    foreach (var idValor in opcao.ValoresSelecionados)
                    {
                        var valorTexto = opcao.ValoresDisponiveis
                            .FirstOrDefault(v => v.IdValor == idValor).Valor ?? string.Empty;

                        var nova = new List<(int, int, string, string)>(combinacaoExistente)
                        {
                            (idValor, opcao.IdChave, opcao.NomeChave, valorTexto)
                        };
                        novoResultado.Add(nova);
                    }
                }
                resultado = novoResultado;
            }

            return resultado;
        }

        public static string SugerirCodigoSku(string prefixoProduto, List<(int IdValor, int IdChave, string NomeChave, string Valor)> combinacao)
        {
            var partes = combinacao.Select(c => NormalizarParteCodigo(c.Valor));
            var prefixo = NormalizarParteCodigo(prefixoProduto);
            return string.Join("-", new[] { prefixo }.Concat(partes)).ToUpperInvariant();
        }

        private static string NormalizarParteCodigo(string texto)
        {
            var semAcento = new string(texto
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                            System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());

            var limpo = new string(semAcento.Where(c => char.IsLetterOrDigit(c)).ToArray());
            return limpo.Length > 4 ? limpo.Substring(0, 4) : limpo;
        }
    }
}