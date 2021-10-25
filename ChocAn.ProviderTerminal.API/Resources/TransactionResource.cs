﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ChocAn.ProviderTerminal.Api.Resources
{
    public class TransactionResource
    {
        [Range(1,999999999, ErrorMessage = "Value out of range")]
        public decimal ProviderNumber { get; set; }
        
        [Range(1, 999999999, ErrorMessage = "Value out of range")]
        public decimal MemberNumber { get; set; }
        
        public DateTime ServiceDate { get; set; }
        
        [Range(1, 999999, ErrorMessage = "Value out of range")]
        public decimal ServiceCode { get; set; }
        
        [MaxLength(100, ErrorMessage = "Value out of range")]
        public string ServiceComment { get; set; }
        public string Status { get; set; }
    }
}
