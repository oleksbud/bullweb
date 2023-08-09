using System.Linq.Expressions;
using Bull.DataAccess.Data;
using Bull.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Bull.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class 
{
    private readonly ApplicationDbContext _context;
    internal DbSet<T> dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        this.dbSet = _context.Set<T>();
    }
    
    public IEnumerable<T> GetAll()
    { 
        IQueryable<T> query = dbSet;
        
        return  query.ToList();
    }

    public T? Get(Expression<Func<T, bool>> filter)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);

        return query.FirstOrDefault();
    }

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }
}