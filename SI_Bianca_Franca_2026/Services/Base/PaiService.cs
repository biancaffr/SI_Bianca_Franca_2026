using SI_Bianca_Franca_2026.Models.Base;
using SI_Bianca_Franca_2026.Services.App;

namespace SI_Bianca_Franca_2026.Services.Base
{
    public abstract class PaiService<T> where T : Pai, new()
    {
        protected readonly IAppContextService _appContext;

        protected PaiService(IAppContextService appContext)
        {
            _appContext = appContext;
        }

        protected void PreencherInsercao(T entity)
        {
            entity.DataCriacao = DateTime.Now;
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
        }

        protected void PreencherAtualizacao(T entity)
        {
            entity.DataUltimaAlteracao = DateTime.Now;
            entity.IdUsuarioUltimaAlteracao = _appContext.IdUsuarioAtual;
        }

    }
}
