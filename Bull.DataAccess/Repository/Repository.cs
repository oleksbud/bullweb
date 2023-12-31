﻿using System.Linq.Expressions;
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
    
    public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }
       
        return  query.ToList();
    }
    
    public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter, List<string> includeProperties)
    {
        IQueryable<T> query = dbSet;
        query = query.Where(filter);
        IncludeProperties(includeProperties, ref query);

        return  query.ToList();
    }

    public T? Get(Expression<Func<T, bool>> filter, bool tracked = false)
    {
        IQueryable<T> query = dbSet;
        if (!tracked)
        {
            query = query.AsNoTracking();
        }
        query = query.Where(filter);

        return query.FirstOrDefault();
    }

    public T? Get(Expression<Func<T, bool>> filter, List<string> includeProperties)
    {
        IQueryable<T> query = dbSet.AsNoTracking();
        IncludeProperties(includeProperties, ref query);
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

    private static void IncludeProperties(List<string> includeProperties, ref IQueryable<T> query)
    {
        if (includeProperties.Count > 0)
        {
            foreach (var prop in includeProperties)
            {
                query = query.Include(prop);
            }
        }
    }
}