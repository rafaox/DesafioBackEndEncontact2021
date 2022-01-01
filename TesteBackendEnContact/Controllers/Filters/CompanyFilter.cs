using TesteBackendEnContact.Base;

namespace TesteBackendEnContact.Controllers.Filters
{
    public class CompanyFilter : BaseFilter
    {
        public int? ContactBookId { get; set; }
        public string Name { get; set; }
    }
}