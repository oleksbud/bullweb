using System.Linq.Expressions;

namespace Bull.DataAccess.Repository.IRepository;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    IEnumerable<T> GetAll(List<string> includeProperties);
    T? Get(Expression<Func<T, bool>> filter);
    T? Get(Expression<Func<T, bool>> filter, List<string> includeProperties);
    void Add(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}