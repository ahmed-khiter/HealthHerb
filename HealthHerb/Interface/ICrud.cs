using HealthHerb.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HealthHerb.Interface
{
    public interface ICrud<T> where T : Entity
    {
        Task<T> Add(T entity);
        Task<IEnumerable<T>> Add(IEnumerable<T> entities);
        Task<T> Update(T entity);
        Task<IEnumerable<T>> Update(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAll(string[] models);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression);
        Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression, string[] models);
        Task<T> GetById(string id);
        Task<T> GetById(string id, string[] models);
        Task<T> GetById(Expression<Func<T, bool>> expression);
        Task<T> GetFirst();
        Task<T> GetFirst(Expression<Func<T, bool>> expression);
        Task<T> GetFirst(Expression<Func<T, bool>> expression, string[] models);
        Task Delete(string id);
        Task Delete(Expression<Func<T, bool>> expression);
        Task<bool> Exists(Expression<Func<T, bool>> expression);
        Task<int> Count();
        Task<int?> Max(Expression<Func<T, int?>> expression);
    }
}
