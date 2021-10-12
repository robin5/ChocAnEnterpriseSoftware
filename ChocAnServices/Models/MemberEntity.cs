using System;

namespace ChocAnServices.Models
{
    public class MemberEntity
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Status { get; set; }
    }
}
