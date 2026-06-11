using SI_Bianca_Franca_2026.Models.Pessoa;
using SI_Bianca_Franca_2026.Repositories.Pessoa;
using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Services.Base;

namespace SI_Bianca_Franca_2026.Services.Pessoa
{
    public class FornecedoresService : PaiService<Fornecedores>
    {
        private readonly FornecedoresRepository _repository;

        public FornecedoresService(FornecedoresRepository repository, IAppContextService appContext)
            : base(appContext)
        {
            _repository = repository;
        }

        public async Task<List<Fornecedores>> ListarTudo()
            => await _repository.ListarTudoAsync();

        public async Task<Fornecedores?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(Fornecedores entity)
        {
            bool cpfCnpjExiste = await _repository.ExisteCpfCnpjCadastradoAsync(entity.CpfCnpj);
            if (cpfCnpjExiste)
                throw new Exception("Já existe um fornecedor cadastrado com este CPF/CNPJ.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Fornecedores entity)
        {
            bool cpfCnpjExiste = await _repository.ExisteCpfCnpjCadastradoAsync(entity.CpfCnpj, entity.Id);
            if (cpfCnpjExiste)
                throw new Exception("Já existe outro fornecedor cadastrado com este CPF/CNPJ.");

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