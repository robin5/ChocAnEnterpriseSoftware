using System.Collections.Generic;
using ChocAn.ProviderServiceRepository;

namespace ChocAn.DataCenterConsole.Models
{
    public class ProviderServiceIndexViewModel
    {
        public IEnumerable<ProviderService> ProviderServices { get; set; }
    }
}
