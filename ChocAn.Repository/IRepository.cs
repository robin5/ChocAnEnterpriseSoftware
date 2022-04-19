using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;

namespace ChocAn.Repository
{
    /// <summary>
    /// Defines a generic repository pattern for T entities
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Adds a T entity to the data source
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// Retrieves a T entity from the data source
        /// </summary>
        /// <param name="id">ID of T entity to retrieve</param>
        /// <returns></returns>
        Task<T> GetAsync(object id);

        /// <summary>
        /// Updates a T entity from the data source
        /// </summary>
        /// <param name="changes">Changes to be applied to T entity</param>
        /// <returns></returns>
        Task<int> UpdateAsync(T entity);

        /// <summary>
        /// Deletes a T entity from the data source
        /// </summary>
        /// <param name="id">ID of entity to deleted</param>
        /// <returns></returns>
        Task<T> DeleteAsync(object id);

        /// <summary>
        /// Retrieves all T entities from the data source
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all T Entities in the database</returns>
        IAsyncEnumerable<T> GetAllAsync(PagingOptions pagingOptions, SortOptions<T> sortOptions, SearchOptions<T> searchOptions);

        /// <summary>
        /// Retrieves all T entities with name from the data source
        /// </summary>
        /// <param name="name">Name of T entities to retrieve</param>
        /// <returns>An enumerator that provides asynchronous iteration over all T Entities in the database</returns>
        IAsyncEnumerable<T> GetAllByNameAsync(string name);
    }
}
