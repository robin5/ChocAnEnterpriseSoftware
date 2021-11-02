using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChocAn.DataCenterConsole.Models
{
    public class MemberDetailsViewModel
    {
        public decimal Number { get; set; }
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public decimal ZipCode { get; set; }
        public string Status { get; set; }
    }
}
