using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChocAn.ProviderTerminal.Api.Resources
{
    public class TransactionResource
    {
        [Range(1,999999999, ErrorMessage = "Value out of range")]
        public int ProviderId { get; set; }
        
        [Range(1, 999999999, ErrorMessage = "Value out of range")]
        public int MemberId { get; set; }

        [Range(1, 999999, ErrorMessage = "Value out of range")]
        public int ServiceId { get; set; }

        public DateTime ServiceDate { get; set; }
        
        [MaxLength(100, ErrorMessage = "Value out of range")]
        public string ServiceComment { get; set; }
    }
}
