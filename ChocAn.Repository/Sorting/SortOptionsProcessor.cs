// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: SortOptionsProcessor.cs
// *
// * Description: Processes sorting options passed to the API
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChocAn.Repository.Sorting
{
    internal class SortOptionsProcessor<T>
    {
        private readonly string[] orderBy;

        /// <summary>
        /// Constructor for SortOptionsProcessor
        /// </summary>
        /// <param name="orderBy">Array of OrderBy entries</param>
        public SortOptionsProcessor(string[] orderBy)
        {
            this.orderBy = orderBy;
        }

        /// <summary>
        /// Enumerates Order By terms from API call
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SortTerm> GetAllTerms()
        {
            if (orderBy == null) yield break;

            foreach (var term in orderBy)
            {
                if (string.IsNullOrEmpty(term)) continue;

                var tokens = term.Split('\u0020',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 1)
                {
                    yield return new SortTerm { Name = tokens[0] };
                }
                else if (tokens.Length > 1)
                {
                    yield return new SortTerm
                    {
                        Name = tokens[0],
                        IsDescending = tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                    };
                }
            }
        }

        /// <summary>
        /// Returns terms from the Model that appear in the query
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SortTerm> GetValidTerms()
        {
            // Get all of the terms from the query to the API
            var queryTerms = GetAllTerms().ToArray();

            // If there are no terms to be used for sorting break
            if (!queryTerms.Any())
                yield break;

            // get terms from the model
            var sortableProperties = GetTermsFromModel();

            // Cycle through each term from API call
            foreach (var term in queryTerms)
            {
                // Find sortable property whose name equals this term name
                var sortableProperty = sortableProperties
                    .SingleOrDefault(x => x.Name.Equals(term.Name, StringComparison.OrdinalIgnoreCase));

                // If property was found return this as a valid term
                if (sortableProperty != null)
                    yield return new SortTerm()
                    {
                        Name = sortableProperty.Name,
                        IsDescending = term.IsDescending,
                        Default = sortableProperty.Default,
                    };
            }
        }

        /// <summary>
        /// Apply sorting to query given the valid orderBy parameters from API call.
        /// Note: If there were no valid terms in query, sorting will still occur
        /// according to the default term which appears on the model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public IQueryable<T> Apply(IQueryable<T> query)
        {
            var validTerms = GetValidTerms().ToArray();

            // If there was no valid terms then create a default one from the model 
            if (!validTerms.Any())
            {
                validTerms = GetTermsFromModel()
                    .Where(t => t.Default)
                    .ToArray();
            }

            // If there were no valid terms found in the API query, and none 
            // set on the model then return original query
            if (!validTerms.Any())
                return query;

            var modifiedQuery = query;
            
            // A flag for signally using OrderBy on the first iteration,
            // and then using ThenBy on all subsequent iterations
            var useThenby = false;

            // build a LINQ query expression for each OrderBy term
            foreach (var term in validTerms)
            {
                var propertyInfo = ExpressionHelper.GetPropertyInfo<T>(term.Name);
                var obj = ExpressionHelper.Parameter<T>();

                // Build the LINQ expression
                // query = query.Orderby(x => x.Property)

                // x => x.Property
                var key = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);
                var keySelector = ExpressionHelper.GetLambda(typeof(T), propertyInfo.PropertyType, obj, key);

                // query.OrderBy/ThenBy[Descending](x => x.Property)
                modifiedQuery = ExpressionHelper
                    .CallOrderByOrThenBy(modifiedQuery, useThenby, term.IsDescending, propertyInfo.PropertyType, keySelector);

                useThenby = true;
            }

            return modifiedQuery;
        }

        /// <summary>
        /// Enumerates sortable properties from model
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<SortTerm> GetTermsFromModel() =>
            typeof(T).GetTypeInfo()
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SortableAttribute>().Any())
            .Select(p => new SortTerm
            {
                Name = p.Name,
                Default = p.GetCustomAttribute<SortableAttribute>().Default
            });
    }
}
