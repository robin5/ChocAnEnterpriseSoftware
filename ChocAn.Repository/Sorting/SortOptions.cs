// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: SortOptions.cs
// *
// * Description: Implements the sorting options model passed into a controller.
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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocAn.Repository.Sorting
{
    public class SortOptions<T> : IValidatableObject
    {
        public string[] OrderBy { get; set; }

        /// <summary>
        /// ASP.NET Core calls this to validate parameters
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Create processor to process orderby query parameters
            var processor = new SortOptionsProcessor<T>(OrderBy);

            // Retrieve valid term from processor
            var validTerms = processor.GetValidTerms().Select(x => x.Name);

            // Retrieve invalid term for processor
            var invalidTerms = processor.GetAllTerms().Select(x => x.Name)
                .Except(validTerms, StringComparer.OrdinalIgnoreCase);

            // Yield validation results for invalid parameters

            foreach(var term in invalidTerms)
            {
                yield return new ValidationResult(
                    $"Invalid sort term encountered.",
                    new[] { nameof(OrderBy) });
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
            // Create processor to process orderby query parameters
            var processor = new SortOptionsProcessor<T>(OrderBy);
            return processor.Apply(query);
        }
    }
}
