// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: RabbitMQExtensions.cs
// *
// * Description: Defines extension method for supplying options to a RabbitMQ service.
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

using Microsoft.Extensions.DependencyInjection;

namespace ChocAn.RabbitMQ.Extensions.DependencyInjection
{
    public static class RabbitMQExtensions
    {
        public static Dictionary<string, RabbitMQOptions> options = new Dictionary<string, RabbitMQOptions>();
        public static IServiceCollection AddRabbitMQMessage<TService, TImplementation>(
            this IServiceCollection services,
            string key,
            Action<RabbitMQOptions> inputDelegate)
            where TService : class
            where TImplementation : class, TService
        {

            RabbitMQOptions options = new();
            inputDelegate(options);

            if (options == null ||
                string.IsNullOrWhiteSpace(options.ExchangeUri) || 
                string.IsNullOrWhiteSpace(options.ExchangeName))
                throw new ArgumentException(nameof(RabbitMQExtensions));

            RabbitMQExtensions.options.Add(key, options);

            services.AddTransient<TService, TImplementation>();

            return services;
        }
    }
}