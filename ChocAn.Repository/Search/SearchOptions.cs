// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: SearchOptions.cs
// *
// * Description: Implements the searching options model passed into a controller.
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
using System.ComponentModel.DataAnnotations;

namespace ChocAn.Repository.Search
{
    public class SearchOptions<T> : IValidatableObject
    {
        public string[] Search { get; set; }

        /// <summary>
        /// ASP.NET Core calls this to validate parameters
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var processor = new SearchOptionsProcessor<T>(Search);

            var validTerms = processor.GetValidTerms().Select(term => term.Name);

            var invalidTerms = processor.GetAllTerms().Select(term => term.Name)
                .Except(validTerms, StringComparer.OrdinalIgnoreCase);

            // return validation error results for each invalid term encountered
            foreach(var term in invalidTerms)
            {
                yield return new ValidationResult($"Invalid search term '{term}'", new [] {nameof(Search)});
            }
        }

        /// <summary>
        /// Applies sorting options to the supplied database query
        /// </summary>
        /// <param name="query">query to apply sorting to</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IQueryable<T> Apply(IQueryable<T> query)
        {
            var processor = new SearchOptionsProcessor<T>(Search);
            return processor.Apply(query);
        }
    }
}
