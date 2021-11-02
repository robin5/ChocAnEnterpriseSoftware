using System.Collections.Generic;
using ChocAn.MemberRepository;

namespace ChocAn.DataCenterConsole.Models
{
    public class MemberIndexViewModel
    {
        public IEnumerable<Member> Members { get; set; }
    }
}
