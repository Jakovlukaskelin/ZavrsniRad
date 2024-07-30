using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace As.Zavrsni.Aplication.Common
{
    public class ProcessResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }

    public class ProcessResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
    public class PagedResponse<T> : ProcessResponse
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }

        public PagedResponse()
        {
            Items = new List<T>();
        }
    }
}
