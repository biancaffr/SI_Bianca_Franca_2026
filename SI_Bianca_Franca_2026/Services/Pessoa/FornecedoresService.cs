using SI_Bianca_Franca_2026.DTOs.Filtros;
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

        public async Task<List<Fornecedores>> ListarTudo(FiltroBaseDto filtro)
            => await _repository.ListarAsync(filtro);

        public async Task<Fornecedores?> Pesquisar(int id)
            => await _repository.PesquisarAsync(id);

        public async Task Inserir(Fornecedores entity)
        {
            NormalizarTextos(entity);

            bool cpfCnpjExiste = await _repository.ExisteCpfCnpjCadastradoAsync(entity.CpfCnpj);
            if (cpfCnpjExiste)
                throw new Exception("Já existe um fornecedor cadastrado com este CPF/CNPJ.");

            PreencherInsercao(entity);
            await _repository.InserirAsync(entity);
        }

        public async Task Atualizar(Fornecedores entity)
        {
            NormalizarTextos(entity);

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
        {
            bool emUso = await _repository.EstaEmUsoAsync(id);
            if (emUso)
                throw new Exception("Este fornecedor não pode ser excluído pois está em uso.");
            await _repository.RemoverAsync(id);
        }

        private static void NormalizarTextos(Fornecedores e)
        {
            e.NomeRazaoSocial = e.NomeRazaoSocial.Trim().ToUpperInvariant();
            e.ApelidoNomeFantasia = e.ApelidoNomeFantasia?.Trim().ToUpperInvariant();
            e.Bairro = e.Bairro?.Trim().ToUpperInvariant();
            e.Logradouro = e.Logradouro?.Trim().ToUpperInvariant();
            e.Complemento = e.Complemento?.Trim().ToUpperInvariant();
            e.Observacao = e.Observacao?.Trim().ToUpperInvariant();
        }
    }
}