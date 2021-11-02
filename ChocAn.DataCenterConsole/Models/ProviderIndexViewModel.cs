using System.Collections.Generic;
using ChocAn.ProviderRepository;

namespace ChocAn.DataCenterConsole.Models
{
    public class ProviderIndexViewModel
    {
        public IEnumerable<Provider> Providers { get; set; }
    }
}
