using System;

namespace ChocAnServices.Models
{
    class ProviderTransactionEntity
    {
        public Guid Id { get; set; }
        public int ProviderId { get; set; }
        public int MemberId { get; set; }
        public DateTime ServiceDateTime { get; set; }
        public int ServiceCode { get; set; }
        public string ServiceComment { get; set; }
        public DateTime TransactionDateTime { get; set; }
    }
}
