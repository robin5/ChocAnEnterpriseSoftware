using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChocAn.GenericRepository
{
    /// <summary>
    /// Defines a generic repository pattern for entities
    /// </summary>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Adds a Member entity to the database
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        Task<T> AddAsync(T obj);

        /// <summary>
        /// Retrieves a Member entity from the database
        /// </summary>
        /// <param name="id">ID of Member entity to retrieve</param>
        /// <returns></returns>
        Task<T> GetAsync(object id);

        /// <summary>
        /// Updates a Member entity in the database
        /// </summary>
        /// <param name="memberChanges">Changes to be applied to Member entity</param>
        /// <returns></returns>
        Task<T> UpdateAsync(T obj);

        /// <summary>
        /// Deletes Member entity from the database
        /// </summary>
        /// <param name="id">ID of member to deleted</param>
        /// <returns></returns>
        Task<T> DeleteAsync(object id);

        /// <summary>
        /// Retrieves all Member entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all Member Entities in the database</returns>
        IAsyncEnumerable<T> GetAllAsync();

        /// <summary>
        /// Retrieves all Member entities in the database similar to the 
        /// </summary>
        /// <param name="name">Name of Member entities to retrieve</param>
        /// <returns>An enumerator that provides asynchronous iteration over all Member Entities in the database</returns>
        IAsyncEnumerable<T> FindAllByNameAsync(string name);
    }
}
