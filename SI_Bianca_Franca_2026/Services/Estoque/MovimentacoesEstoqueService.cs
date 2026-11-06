using SI_Bianca_Franca_2026.DTOs.Estoque;
using SI_Bianca_Franca_2026.Models.Base;
using SI_Bianca_Franca_2026.Models.Estoque;
using SI_Bianca_Franca_2026.Repositories.Estoque;
using SI_Bianca_Franca_2026.Services.App;

namespace SI_Bianca_Franca_2026.Services.Estoque
{
    public class MovimentacoesEstoqueService
    {
        private readonly MovimentacoesEstoqueRepository _repository;
        private readonly IAppContextService _appContext;

        public MovimentacoesEstoqueService(
            MovimentacoesEstoqueRepository repository,
            IAppContextService appContext)
        {
            _repository = repository;
            _appContext = appContext;
        }

        /// <summary>Saldo e custo médio atuais do produto (sem movimentação: tudo 0).</summary>
        public async Task<EstoqueProdutoDTO> ObterEstoque(int idProduto)
            => await _repository.ObterEstoqueAsync(idProduto);

        /// <summary>
        /// Custo médio ponderado: (saldo x custo médio atual + qtde x custo unitário) / (saldo + qtde).
        /// Saldo negativo é tratado como zero.
        /// </summary>
        public static decimal CalcularCustoMedio(
            decimal saldo, decimal custoMedioAtual, decimal quantidade, decimal custoUnitario)
        {
            saldo = Math.Max(0, saldo);
            var novoSaldo = saldo + quantidade;
            if (novoSaldo <= 0) return 0;
            return Math.Round((saldo * custoMedioAtual + quantidade * custoUnitario) / novoSaldo, 4);
        }

        public async Task<int> Registrar(MovimentacoesEstoque mov)
        {
            if (!mov.Itens.Any())
                throw new Exception("A movimentação deve ter ao menos um item.");

            var agora = DateTime.Now;
            mov.DataMovimentacao = agora;
            mov.IdUsuario = _appContext.IdUsuarioAtual;
            mov.Observacao = mov.Observacao?.Trim().ToUpperInvariant();
            PreencherAuditoria(mov, agora);
            foreach (var item in mov.Itens)
                PreencherAuditoria(item, agora);

            return await _repository.InserirAsync(mov);
        }

        private void PreencherAuditoria(Pai e, DateTime agora)
        {
            e.DataCriacao = agora;
            e.DataUltimaAlteracao = agora;
            e.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
            e.Ativo = true;
        }
    }
}
