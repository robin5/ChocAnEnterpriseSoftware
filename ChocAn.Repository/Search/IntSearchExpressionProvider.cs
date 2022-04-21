// **********************************************************************************
// * Copyright (c) 2022 Robin Murray
// **********************************************************************************
// *
// * File: IntSearchExpressionProvider.cs
// *
// * Description: A utility class used for creating an expression from a search parameter
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
using System.Linq.Expressions;

namespace ChocAn.Repository.Search
{
    internal class IntSearchExpressionProvider : DefaultSearchExpressionProvider
    {
        /// <summary>
        /// Returns a Constant expression specifying the input value
        /// </summary>
        /// <param name="input">Value to be converted into a contant expression</param>
        /// <returns>Constant expression specifying the input value</returns>
        /// <exception cref="ArgumentException">Throws an exception when the input cannot be converted into an int</exception>
        public override ConstantExpression GetValue(string input)
        {
            if (int.TryParse(input, out int value))
            {
                return Expression.Constant(value);
            }
            throw new ArgumentException("Invalid search value");
        }

        /// <summary>
        /// Returns a comparison expression based on the op parameter
        /// </summary>
        /// <param name="left">Left side of expression</param>
        /// <param name="op">Operator</param>
        /// <param name="right">Right side of expression</param>
        /// <returns></returns>
        public override Expression GetComparison(MemberExpression left, string op, ConstantExpression right)
        {
            return op switch
            {
                "gt" => Expression.GreaterThan(left, right),
                "gte" => Expression.GreaterThanOrEqual(left, right),
                "lt" => Expression.LessThan(left, right),
                "lte" => Expression.LessThanOrEqual(left, right),
                // If nothing matches fall back to base implementation
                _ => base.GetComparison(left, op, right),
            };
        }
    }
}
