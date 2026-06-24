using SI_Bianca_Franca_2026.Models.Comercial;
using SI_Bianca_Franca_2026.Repositories.Comercial;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Comercial
{
    public class PedidosService : PaiService<Pedidos>
    {
        private readonly PedidosRepository _repository;

        public PedidosService(PedidosRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Pedidos>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<List<Pedidos>> ListarPorStatus(string status)
            => await _repository.ListarPorStatusAsync(status);

        public async Task<Pedidos?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task<int> Inserir(Pedidos entity, List<PedidosItens> itens)
        {
            ValidarItens(itens);

            PreencherInsercao(entity);
            entity.ValorTotal = itens.Sum(i => i.ValorTotal);
            int idPedido = await _repository.InserirAsync(entity);

            await SalvarItensAsync(idPedido, itens);

            return idPedido;
        }

        public async Task Atualizar(Pedidos entity, List<PedidosItens> itens)
        {
            ValidarItens(itens);

            PreencherAtualizacao(entity);
            entity.ValorTotal = itens.Sum(i => i.ValorTotal);
            await _repository.AtualizarAsync(entity);

            await SalvarItensAsync(entity.Id, itens);
        }

        public async Task AtualizarStatus(int id, string novoStatus)
            => await _repository.AtualizarStatusAsync(id, novoStatus, _appContext.IdUsuarioAtual);

        public async Task Desativar(int id)
            => await _repository.AlterarStatusAtivoAsync(id, false, _appContext.IdUsuarioAtual);

        public async Task Ativar(int id)
            => await _repository.AlterarStatusAtivoAsync(id, true, _appContext.IdUsuarioAtual);

        private void ValidarItens(List<PedidosItens> itens)
        {
            if (!itens.Any())
                throw new Exception("O pedido deve ter ao menos um item.");
        }

        private async Task SalvarItensAsync(int idPedido, List<PedidosItens> itens)
        {
            var itensExistentes = await _repository.ListarItensAsync(idPedido);
            var idsAtuaisNaTela = itens.Where(i => i.Id != 0).Select(i => i.Id).ToHashSet();

            foreach (var itemRemovido in itensExistentes.Where(i => !idsAtuaisNaTela.Contains(i.Id)))
                await _repository.RemoverItemAsync(itemRemovido.Id);

            foreach (var item in itens)
            {
                item.IdPedido = idPedido;
                item.ValorTotal = (item.Quantidade * item.ValorUnitario) - item.ValorDesconto;
                item.DataCriacao = item.Id == 0 ? DateTime.Now : item.DataCriacao;
                item.DataUltimaAlteracao = DateTime.Now;
                item.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
                item.Ativo = true;

                int idItem;
                if (item.Id == 0)
                {
                    idItem = await _repository.InserirItemAsync(item);
                }
                else
                {
                    idItem = item.Id;
                    await _repository.AtualizarItemAsync(item);
                    await _repository.DeletarMateriaisDoItemAsync(idItem);
                }

                foreach (var material in item.Materiais)
                {
                    material.IdPedidoItem = idItem;
                    material.DataCriacao = DateTime.Now;
                    material.DataUltimaAlteracao = DateTime.Now;
                    material.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
                    material.Ativo = true;
                    await _repository.InserirMaterialAsync(material);
                }
            }
        }
    }
}