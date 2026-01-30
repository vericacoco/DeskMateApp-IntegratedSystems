using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Repository.Data;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;



public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> entities;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        entities = _context.Set<T>();
    }

    public int SaveChanges() => _context.SaveChanges();

    public T Insert(T entity)
    {
        _context.Add(entity);
        _context.SaveChanges();
        return entity;
    }

    public ICollection<T> InsertMany(ICollection<T> entity)
    {
        _context.AddRange(entity);
        _context.SaveChanges();
        return entity;
    }

    public T Update(T entity)
    {
        _context.Update(entity);
        _context.SaveChanges();
        return entity;
    }

    public T Delete(T entity)
    {
        _context.Remove(entity);
        _context.SaveChanges();
        return entity;
    }

    public E? Get<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> query = entities;

        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);

        return orderBy != null
            ? orderBy(query).Select(selector).FirstOrDefault()
            : query.Select(selector).FirstOrDefault();
    }

    public IEnumerable<E> GetAll<E>(Expression<Func<T, E>> selector,
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
    {
        IQueryable<T> query = entities;

        if (include != null) query = include(query);
        if (predicate != null) query = query.Where(predicate);

        return orderBy != null
            ? orderBy(query).Select(selector).ToList()
            : query.Select(selector).ToList();
    }


    
}
