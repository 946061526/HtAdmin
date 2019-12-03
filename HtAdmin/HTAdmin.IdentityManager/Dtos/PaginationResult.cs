using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTAdmin.IdentityManager.Dtos
{
    public class PaginationResult<T> where T : class, new()
    {
        public int TotalCount { get; set; }
        public List<T> Data { get; set; }
    }
}
