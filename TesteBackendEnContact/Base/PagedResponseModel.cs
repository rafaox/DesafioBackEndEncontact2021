using System.Collections.Generic;
using System.Linq;

namespace TesteBackendEnContact.Base
{
    public class PagedResponseModel<T>
    {
        public PagedResponseModel(int total, BaseFilter filter, IEnumerable<T> data)
        {
            this.Paging = new PageData
            {
                Page = filter.Page.Value,
                PageSize = filter.PageSize.Value,
                Total = total
            };

            this.Data = data ?? Enumerable.Empty<T>();
        }

        public PagedResponseModel(PageData paging, IEnumerable<T> data)
        {
            this.Paging = paging;
            this.Data = data;
        }

        public PageData Paging { get; set; }
        public IEnumerable<T> Data { get; set; }
    }

    public class PageData
    {
        public int PageSize { get; set; }
        public int Page { get; set; }
        public int Total { get; set; }
    }
}