using HealthHerb.Data;
using HealthHerb.Interface;
using HealthHerb.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace HealthHerb.Help
{
    public class Crud<T> : ICrud<T> where T : Entity
    {
        #region Properties and Fields
        private readonly AppDbContext context;
        private readonly DbSet<T> entitySet;
        #endregion

        #region Constructor
        public Crud(AppDbContext context)
        {
            this.context = context;
            entitySet = context.Set<T>();
        }
        #endregion

        #region Add
        public async Task<T> Add(T entity)
        {
            await entitySet.AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> Add(IEnumerable<T> entities)
        {
            await entitySet.AddRangeAsync(entities);
            await context.SaveChangesAsync();
            return entities;
        }
        #endregion

        #region Get
        public async Task<IEnumerable<T>> GetAll()
        {
            return await entitySet.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(string[] models)
        {
            IQueryable<T> record = entitySet;
            foreach (var model in models)
            {
                record = record.Include(model);
            }
            return await record.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression)
        {
            return await entitySet.Where(expression).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAll(Expression<Func<T, bool>> expression, string[] models)
        {
            IQueryable<T> record = entitySet;

            record = record.Where(expression);

            foreach (var model in models)
            {
                record = record.Include(model);
            }

            return await record.ToListAsync();
        }

        public async Task<T> GetById(string id)
        {
            return await entitySet.FirstOrDefaultAsync(m => m.Id.Equals(id));
        }

        public async Task<T> GetById(string id, string[] models)
        {
            IQueryable<T> record = entitySet;
            foreach (var model in models)
            {
                record = record.Include(model);
            }
            return await record.FirstOrDefaultAsync(m => m.Id.Equals(id));
        }

        public async Task<T> GetById(Expression<Func<T,bool>> expression)
        {
            return await entitySet.FirstOrDefaultAsync(expression);
        }

        public async Task<T> GetFirst()
        {
            return await entitySet.FirstOrDefaultAsync();
        }

        public async Task<T> GetFirst(Expression<Func<T, bool>> expression)
        {
            return await entitySet.Where(expression).FirstOrDefaultAsync();
        }

        public async Task<T> GetFirst(Expression<Func<T, bool>> expression, string[] models)
        {
            IQueryable<T> record = entitySet;

            record = record.Where(expression);

            foreach (var model in models)
            {
                record = record.Include(model);
            }

            return await record.FirstOrDefaultAsync();
        }
        #endregion

        #region Update
        public async Task<T> Update(T entity)
        {
            entitySet.Update(entity);
            await context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> Update(IEnumerable<T> entities)
        {
            entitySet.UpdateRange(entities);
            await context.SaveChangesAsync();
            return entities;
        }
        #endregion

        #region Delete
        public async Task Delete(string id)
        {
            var entity = await entitySet.FirstOrDefaultAsync(e => e.Id.Equals(id));
            if (entity != null)
            {
                entitySet.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task Delete(Expression<Func<T, bool>> expression)
        {
            var entities = await entitySet.Where(expression).ToListAsync();
            if (entities != null)
            {
                entitySet.RemoveRange(entities);
                await context.SaveChangesAsync();
            }
        }
        #endregion

        #region Aggregate
        public async Task<bool> Exists(Expression<Func<T, bool>> expression)
        {
            return await entitySet.AnyAsync(expression);
        }

        public async Task<int> Count()
        {
            return await entitySet.CountAsync();
        }

        public async Task<int?> Max(Expression<Func<T, int?>> expression)
        {
            return await entitySet.MaxAsync(expression);
        }
        #endregion
    }
}
