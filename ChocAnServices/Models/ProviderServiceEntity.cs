using System;

namespace ChocAnServices.Models
{
    public class ProviderServiceEntity
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
    }
}
