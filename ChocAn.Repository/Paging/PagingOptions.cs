using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocAn.Repository.Paging
{
    public class PagingOptions
    {
        //public int PageSize { get; set; }
        //public int PageCount { get; set; } = 0;
        //public int PageOffset { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Offset must be a positive number")]
        public int? Offset { get; set; }

        [Range(1, 100, ErrorMessage = "Limit must be greater than 0 and less then or equal to 100")]
        public int? Limit { get; set; }
    }
}
