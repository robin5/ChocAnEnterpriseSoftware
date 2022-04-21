// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: Repository.cs
// *
// * Description: Implements a generic repository pattern for interacting with databases
// *
// **********************************************************************************
// * Author: Robin Murray
// **********************************************************************************
// *
// * Granting License: The MIT License (MIT)
// * 
// *   Permission is hereby granted, free of charge, to any person obtaining a copy
// *   of this software and associated documentation files (the "Software"), to deal
// *   in the Software without restriction, including without limitation the rights
// *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// *   copies of the Software, and to permit persons to whom the Software is
// *   furnished to do so, subject to the following conditions:
// *   The above copyright notice and this permission notice shall be included in
// *   all copies or substantial portions of the Software.
// *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// *   THE SOFTWARE.
// * 
// **********************************************************************************

using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;

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
            var result = await dbSet.AddAsync(entity);
            context.SaveChanges();
            return result.Entity;
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
        virtual public async Task<int> UpdateAsync(T changes)
        {
            var entity = dbSet.Attach(changes);
            entity.State = EntityState.Modified;
            
            return await context.SaveChangesAsync(); ;
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
        virtual public async IAsyncEnumerable<T> GetAllAsync(
            PagingOptions pagingOptions, 
            SortOptions<T> sortOptions,
            SearchOptions<T> searchOptions)
        {
            var query = dbSet.AsQueryable<T>();

            query = searchOptions.Apply(query);
            query = sortOptions.Apply(query);
            query = query
                .Skip(pagingOptions.Offset.Value)
                .Take(pagingOptions.Limit.Value);

            //var size = await dbSet.CountAsync<T>();

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
