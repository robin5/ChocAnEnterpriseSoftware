using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using System;

namespace ChocAn.GenericRepository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DbContext context;
        protected readonly DbSet<T> dbSet;

        /// <summary>
        ///  Constructor for TDbContext
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public GenericRepository(DbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        /// <summary>
        /// Adds entity to the database
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        virtual public async Task<T> AddAsync(T obj)
        {
            await dbSet.AddAsync(obj);
            context.SaveChanges();
            return obj;
        }

        /// <summary>
        /// Retrieves entity from the database
        /// </summary>
        /// <param name="id">ID of T entity to retrieve</param>
        /// <returns></returns>
        virtual public async Task<T> GetAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        /// <summary>
        /// Updates entity in the database
        /// </summary>
        /// <param name="changes">Changes to be applied to T entity</param>
        /// <returns></returns>
        virtual public async Task<T> UpdateAsync(T changes)
        {
            var member = dbSet.Attach(changes);
            member.State = EntityState.Modified;
            await context.SaveChangesAsync();
            return changes;
        }

        /// <summary>
        /// Deletes entity from the database
        /// </summary>
        /// <param name="id">ID of member to deleted</param>
        /// <returns></returns>
        virtual public async Task<T> DeleteAsync(object id)
        {
            var entity = await dbSet.FindAsync(id);
            if (null != entity)
            {
                dbSet.Remove(entity);
                await context.SaveChangesAsync();
            }
            return entity;
        }

        /// <summary>
        /// Retrieves all T entities from the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all T entities in the database</returns>
        virtual public async IAsyncEnumerable<T> GetAllAsync()
        {
            var enumerator = dbSet.AsAsyncEnumerable().GetAsyncEnumerator();
            T entity;

            await enumerator.MoveNextAsync();
            while (null != (entity = enumerator.Current))
            {
                yield return entity;
                await enumerator.MoveNextAsync();
            }
        }

        /// <summary>
        /// Retrieves all T entities with the given name from the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all T entities in the database</returns>
        virtual public async IAsyncEnumerable<T> FindAllByNameAsync(string name)
        {
            var enumerator = dbSet.AsAsyncEnumerable().GetAsyncEnumerator();
            T entity;

            await enumerator.MoveNextAsync();
            while (null != (entity = enumerator.Current))
            {
                yield return entity;
                await enumerator.MoveNextAsync();
            }
        }
    }
}
