// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: SearchOptionsProcessor.cs
// *
// * Description: A utility class used for processing search query parameters.
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
using System.Linq.Expressions;
using System.Reflection;

namespace ChocAn.Repository.Search
{
    internal class SearchOptionsProcessor<T>
    {
        // Search queries from API call
        private readonly string[] searchQuery;

        /// <summary>
        /// Constructor for SearchOptionsProcessor
        /// </summary>
        /// <param name="searchQuery">Array of search queries from API call</param>
        public SearchOptionsProcessor(string[] searchQuery)
        {
            this.searchQuery = searchQuery;
        }

        /// <summary>
        /// Enumerates Search terms from search query
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SearchTerm> GetAllTerms()
        {
            if (searchQuery == null) yield break;

            foreach(var expression in searchQuery)
            {
                if (string.IsNullOrEmpty(expression)) continue;

                // Each expression should look like: <field-name> <operator> <value> [| <value>] ...
                var tokens = expression.Split('\u0020',
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                // Check for bad case of no tokens found.  This can happen if 
                // expression is all space characters
                if (tokens.Length == 0)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = expression
                    };
                    continue;
                }

                // Check for bad case of not enough tokens found
                if (tokens.Length < 3)
                {
                    yield return new SearchTerm
                    {
                        ValidSyntax = false,
                        Name = tokens[0]
                    };
                    continue;
                }

                // Valid syntax
                yield  return new SearchTerm
                {
                    ValidSyntax = true,
                    Name = tokens[0],
                    Operator = tokens[1],
                    // Note the value consists of all the remaining tokens seperated by a space character
                    Value = String.Join(" ", tokens.Skip(2))
                };
            }
        }

        /// <summary>
        /// Returns terms from the Model that appear in the query
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SearchTerm> GetValidTerms()
        {
            // Get all of the terms from the query to the API
            var queryTerms = GetAllTerms()
                .Where(term => term.ValidSyntax)
                .ToArray();

            // If there are no terms to be used for sorting break
            if (!queryTerms.Any())
                yield break;

            // Get terms from the model
            var termsFromModel = GetTermsFromModel();

            // Cycle through each term from API call to determine if 
            // term appears in list of terms from the model
            foreach (var queryTerm in queryTerms)
            {
                // Search for a model term with the terms name
                var modelTerm = termsFromModel
                    .SingleOrDefault(x => x.Name.Equals(queryTerm.Name, StringComparison.OrdinalIgnoreCase));

                // If a term from the model was found then return this as a valid term
                if (modelTerm != null)
                    yield return new SearchTerm()
                    {
                        // From model
                        Name = modelTerm.Name,
                        ExpressionProvider = modelTerm.ExpressionProvider,

                        // From API call
                        Operator = queryTerm.Operator,
                        Value = queryTerm.Value,
                        ValidSyntax = queryTerm.ValidSyntax
                    };
            }
        }

        /// <summary>
        /// Enumerates sortable properties from model
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<SearchTerm> GetTermsFromModel() =>
            typeof(T).GetTypeInfo()
            .DeclaredProperties
            .Where(p => p.GetCustomAttributes<SearchableAttribute>().Any())
            .Select(p => new SearchTerm
            {
                Name = p.Name,
                ExpressionProvider = p.GetCustomAttribute<SearchableAttribute>().ExpressionProvider
            });

        /// <summary>
        /// Apply searching to query given the valid search parameters from API call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        internal IQueryable<T> Apply(IQueryable<T> query)
        {
            var validTerms = GetValidTerms().ToArray();

            // If there were no valid terms found in the API query
            if (!validTerms.Any())
                return query;

            var modifiedQuery = query;

            // build a LINQ query expression for each OrderBy term
            foreach (var term in validTerms)
            {
                var propertyInfo = ExpressionHelper.GetPropertyInfo<T>(term.Name);
                var obj = ExpressionHelper.Parameter<T>();

                // Build the LINQ expression
                // query = query.Where(x => x.Property == "Value")

                // x.Property
                var left = ExpressionHelper.GetPropertyExpression(obj, propertyInfo);

                // "Value"
                var right = term.ExpressionProvider.GetValue(term.Value);

                // x.Property == "Value"
                var comparisonExpression = term.ExpressionProvider.GetComparison(left, term.Operator, right);

                // x => x.Property == "Value"
                var lambdaExpression = ExpressionHelper.GetLambda<T, bool>(obj, comparisonExpression);

                // query = query.Where(x => x.Property == "Value")
                modifiedQuery = ExpressionHelper.CallWhere(modifiedQuery, lambdaExpression);
            }

            return modifiedQuery;
        }
    }
}