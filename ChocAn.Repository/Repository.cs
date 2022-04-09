using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq.Expressions;
using System;
using ChocAn.Repository.Paging;

namespace ChocAn.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext context;
        protected readonly DbSet<T> dbSet;

        /// <summary>
        ///  Constructor for TDbContext
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public Repository(DbContext context)
        {
            this.context = context;
            dbSet = context.Set<T>();
        }

        /// <summary>
        /// Adds a T entity to the data source
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        virtual public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            context.SaveChanges();
            return entity;
        }

        /// <summary>
        /// Retrieves a T entity from the data source
        /// </summary>
        /// <param name="id">ID of T entity to retrieve</param>
        /// <returns></returns>
        virtual public async Task<T> GetAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        /// <summary>
        /// Updates a T entity from the data source
        /// </summary>
        /// <param name="changes">Changes to be applied to T entity</param>
        /// <returns></returns>
        virtual public async Task<T> UpdateAsync(T changes)
        {
            var entity = dbSet.Attach(changes);
            entity.State = EntityState.Modified;
            await context.SaveChangesAsync();
            return changes;
        }

        /// <summary>
        /// Deletes a T entity from the data source
        /// </summary>
        /// <param name="id">ID of entity to deleted</param>
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
        /// Retrieves all T entities from the data source
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all T Entities in the database</returns>
        virtual public async IAsyncEnumerable<T> GetAllAsync(PagingOptions pagingOptions)
        {
            var count = dbSet.Count<T>();

            var query = dbSet
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value);

            //var enumerator = dbSet.AsAsyncEnumerable().GetAsyncEnumerator();
            var enumerator = query.AsAsyncEnumerable<T>().GetAsyncEnumerator();
            T entity;

            await enumerator.MoveNextAsync();
            while (null != (entity = enumerator.Current))
            {
                yield return entity;
                await enumerator.MoveNextAsync();
            }
            await enumerator.DisposeAsync();
        }

        /// <summary>
        /// Retrieves all T entities with name from the data source
        /// </summary>
        /// <param name="name">Name of T entities to retrieve</param>
        /// <returns>An enumerator that provides asynchronous iteration over all T Entities in the database</returns>
        virtual public async IAsyncEnumerable<T> GetAllByNameAsync(string name)
        {
            await Task.Delay(0);
            yield return null;
        }
    }
}
