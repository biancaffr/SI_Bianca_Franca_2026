using SI_Bianca_Franca_2026.Services.App;
using SI_Bianca_Franca_2026.Models.Pessoa;
namespace SI_Bianca_Franca_2026.Services.Pessoa
{
    public abstract class PessoasService<T> where T : Pessoas, new()
    {
        protected readonly IAppContextService _appContext;

        protected PessoasService(IAppContextService appContext)
        {
            _appContext = appContext;
        }

        public abstract Task<List<T>> ListarTudo();
        public abstract Task<T?> Pesquisar(int id);

        protected void PreencherDadosInsercao(T entity)
        {
            entity.DataCriacao = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
        }

        protected void PreencherDadosAtualizacao(T entity)
        {
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
        }
    }
}
