// **********************************************************************************
// * Copyright (c) 2022 Robin Murray
// **********************************************************************************
// *
// * File: DefaultRabbitMQMessage.cs
// *
// * Description: Implements a generic class for sending RabbitMQ mesages.  
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

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using ChocAn.RabbitMQ.Extensions.DependencyInjection;

namespace ChocAn.RabbitMQMessages
{
    public class DefaultRabbitMQMessage<TModel>: IRabbitMQMessage<TModel>
    {
        /// <summary>
        /// Creates a T entity.
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>TModel representing created entity</returns>
        public void Send(string key, TModel entity)
        {
            var options = RabbitMQExtensions.options[key];
            if (options == null ||
                options.ExchangeUri == null ||
                options.ExchangeName == null)
                throw new ArgumentNullException(nameof(options));

            var factory = new ConnectionFactory();
            factory.Uri = new Uri(options.ExchangeUri);

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Creates exchange if it does not already exist
            channel.ExchangeDeclare(options.ExchangeName, ExchangeType.Fanout, true);

            var message = JsonSerializer.Serialize<TModel>(entity);
            var bytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(options.ExchangeName, "", null, bytes);

            channel.Close();
            connection.Close();
        }
    }
}
