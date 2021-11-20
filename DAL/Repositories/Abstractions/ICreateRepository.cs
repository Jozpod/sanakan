namespace Sanakan.DAL.Repositories.Abstractions
{
    public interface ICreateRepository<T>
    {
        void Add(T entity);
    }
}
