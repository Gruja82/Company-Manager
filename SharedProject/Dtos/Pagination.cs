using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.Dtos
{
    public class Pagination<T>
    {
        public List<T> DataList { get; set; } = new();
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 4;
        public int TotalPages { get; set; }
    }
}
