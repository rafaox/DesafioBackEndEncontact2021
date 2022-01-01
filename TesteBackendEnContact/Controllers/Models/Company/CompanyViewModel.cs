namespace TesteBackendEnContact.Controllers.Models.Company
{
    public class CompanyViewModel
    {
        public int Id { get; set; }
        public int ContactBookId { get; set; }
        public string Name { get; set; }

        public CompanyViewModel(int id, int contactBookId, string name)
        {
            Id = id;
            ContactBookId = contactBookId;
            Name = name;
        }
    }
}