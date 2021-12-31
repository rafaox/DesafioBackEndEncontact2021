namespace TesteBackendEnContact.Base
{
    public class BaseFilter
    {
        private const int DEFAULT_PAGE = 1;
        private const int DEFAULT_PAGE_SIZE = 10;
        private const int MAX_PAGE_SIZE = 100;
        private const bool DEFAULT_ORDER_BY = true;

        public int? Page
        {
            get => _page ?? DEFAULT_PAGE;
            set => _page = value.HasValue
                ? value <= 0
                    ? DEFAULT_PAGE
                    : value
                : DEFAULT_PAGE;
        }
        private int? _page;

        public int? PageSize
        {
            get => _pageSize ?? DEFAULT_PAGE_SIZE;
            set => _pageSize = value.HasValue
                ? value > MAX_PAGE_SIZE
                    ? MAX_PAGE_SIZE
                    : value <= 0
                        ? 1
                        : value
                : DEFAULT_PAGE_SIZE;
        }
        private int? _pageSize;

        public bool? OrderByDescending
        {
            get => _orderByDescending ?? DEFAULT_ORDER_BY;
            set => _orderByDescending = value.HasValue ? value : DEFAULT_ORDER_BY;
        }
        private bool? _orderByDescending;
    }
}