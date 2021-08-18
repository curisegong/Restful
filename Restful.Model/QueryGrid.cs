using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restful.Model
{
    public class QueryGrid
    {
        public int ResultCode { get; set; }
        public int Total { get; set; }
        public List<object> Data { get; set; }
    }
}
