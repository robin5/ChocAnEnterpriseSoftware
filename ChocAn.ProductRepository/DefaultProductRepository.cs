// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultProductRepository.cs
// *
// * Description: Provides access to Product items stored in a database context
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

using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChocAn.Data;
using ChocAn.Repository;

namespace ChocAn.ProductRepository
{
    /// <summary>
    /// Implements repository pattern for Product entities
    /// </summary>
    public class DefaultProductRepository : Repository<Product>
    {
        /// <summary>
        ///  Constructor for DefaultProductRepository
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public DefaultProductRepository(ProductDbContext context)
            : base(context)
        {
        }

        override public async IAsyncEnumerable<Product> GetAllByNameAsync(string name)
        {
            var query = dbSet.Where<Product>(a => a.Name.Contains(name));
            var enumerator = query.AsAsyncEnumerable<Product>().GetAsyncEnumerator();
            Product entity;

            await enumerator.MoveNextAsync();
            while (null != (entity = enumerator.Current))
            {
                yield return entity;
                await enumerator.MoveNextAsync();
            }
            await enumerator.DisposeAsync();
        }
    }
}
