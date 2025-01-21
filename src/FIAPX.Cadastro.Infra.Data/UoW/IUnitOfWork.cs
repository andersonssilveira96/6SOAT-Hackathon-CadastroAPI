namespace FIAPX.Cadastro.Infra.Data.UoW
{
    public interface IUnitOfWork
    {
        Task CommitAsync();        
    }
}
