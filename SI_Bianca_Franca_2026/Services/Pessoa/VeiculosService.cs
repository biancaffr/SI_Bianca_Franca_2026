using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Repositories.Pessoa;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Pessoa
{
    public class VeiculosService : PaiService<Veiculos>
    {
        private readonly VeiculosRepository _repository;

        public VeiculosService(VeiculosRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Veiculos>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<Veiculos?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(Veiculos entity)
        {
            bool placaExiste = await _repository.ExistePlacaCadastradaAsync(entity.Placa);
            if (placaExiste)
                throw new Exception("Já existe um veículo cadastrado com esta placa.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Veiculos entity)
        {
            bool placaExiste = await _repository.ExistePlacaCadastradaAsync(entity.Placa, entity.Id);
            if (placaExiste)
                throw new Exception("Já existe outro veículo cadastrado com esta placa.");

            PreencherAtualizacao(entity);
            await _repository.AtualizarAsync(entity);
        }

        public async Task Desativar(int id)
            => await _repository.AlterarStatusAsync(id, false, _appContext.IdUsuarioAtual);

        public async Task Ativar(int id)
            => await _repository.AlterarStatusAsync(id, true, _appContext.IdUsuarioAtual);

        public async Task Remover(int id)
            => await _repository.RemoverAsync(id);
    }
}
