using System.Collections.Generic;

namespace ChocAn.DataCenterConsole.Models
{
    public class FindViewModel<TModel>
    {
        public string Find { get; set; }
        public IEnumerable<TModel> Items { get; set; }
    }
}
