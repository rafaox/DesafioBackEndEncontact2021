using TesteBackendEnContact.Base;

namespace TesteBackendEnContact.Controllers.Filters
{
    public class ContactFilter : BaseFilter
    {
        public int? Id { get; set; }
        public int? ContactBookId { get; set; }
        public int? CompanyId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string ContactBookName { get; set; }
    }
}