using System.Collections.Generic;

namespace Sample.Fluxo.Caixa.Core.Pageable
{
    public class PagedResult<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalResults { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }
    }
}
