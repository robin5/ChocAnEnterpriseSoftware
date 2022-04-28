using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocAn.RabbitMQ.Extensions.DependencyInjection
{
    public class RabbitMQOptions
    {
        public string? ExchangeUri { get; set; }
        public string? ExchangeName { get; set; }
    }
}
