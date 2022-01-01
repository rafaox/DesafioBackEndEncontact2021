using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using FluentMigrator.Runner;
using Microsoft.Data.Sqlite;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.Contact;
using TesteBackendEnContact.Core.Domain.Contact;
using TesteBackendEnContact.Core.Interface.Contact;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Errors;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<PagedResponseModel<ContactViewModel>> GetAllAsync(ContactFilter filter)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                int contactBookIdParam = default(int);
                string contactBookId = string.Empty;
                if (filter.ContactBookId.HasValue)
                {
                    contactBookId = "AND ct.ContactBookId = @contactBookId";
                    contactBookIdParam = filter.ContactBookId.Value;
                }

                int companyIdParam = default(int);
                string companyId = string.Empty;
                if (filter.CompanyId.HasValue)
                {
                    companyId = "AND ct.CompanyId = @companyId";
                    companyIdParam = filter.CompanyId.Value;
                }

                string name = string.Empty;
                string nameParam = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    name = "AND ct.Name LIKE @name";
                    nameParam = "%" + filter.Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
                }

                string phone = string.Empty;
                string phoneParam = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.Phone))
                {
                    phone = "AND ct.Phone LIKE @phone";
                    phoneParam = "%" + filter.Phone.Replace("[", "[[]").Replace("%", "[%]") + "%";
                }

                string email = string.Empty;
                string emailParam = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.Email))
                {
                    email = "AND ct.Email LIKE @email";
                    emailParam = "%" + filter.Email.Replace("[", "[[]").Replace("%", "[%]") + "%";
                }

                string address = string.Empty;
                string addressParam = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.Address))
                {
                    address = "AND ct.Address LIKE @address";
                    addressParam = "%" + filter.Address.Replace("[", "[[]").Replace("%", "[%]") + "%";
                }

                string companyName = string.Empty;
                string companyNameParam = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.CompanyName))
                {
                    companyName = "AND cy.Name LIKE @companyName";
                    companyNameParam = "%" + filter.CompanyName.Replace("[", "[[]").Replace("%", "[%]") + "%";
                }

                string contactBookName = string.Empty;
                string contactBookNameParam = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.ContactBookName))
                {
                    contactBookName = "AND cb.Name LIKE @contactBookName";
                    contactBookNameParam = "%" + filter.ContactBookName.Replace("[", "[[]").Replace("%", "[%]") + "%";
                }

                var orderedBy = filter.OrderByDescending.Value ? "DESC" : "ASC";

                string query = $@"
                    SELECT *
                    FROM Contact ct
                    INNER JOIN Company cy ON cy.Id = ct.CompanyId
                    INNER JOIN ContactBook cb ON cb.Id = ct.ContactBookId
                    WHERE 1 = 1
                    {contactBookId}
                    {companyId}
                    {name}
                    {phone}
                    {email}
                    {address}
                    {companyName}
                    {contactBookName}
                    ORDER BY Id {orderedBy}
                    LIMIT @pageSize
                    OFFSET @offset;

                    SELECT COUNT(*)
                    FROM Contact ct
                    INNER JOIN Company cy ON cy.Id = ct.CompanyId
                    INNER JOIN ContactBook cb ON cb.Id = ct.ContactBookId
                    WHERE 1 = 1
                    {contactBookId}
                    {companyId}
                    {name}
                    {phone}
                    {email}
                    {address}
                    {companyName}
                    {contactBookName}
                ;";

                connection.Open();

                using (var multi = await connection.QueryMultipleAsync(query,
                    new
                    {
                        contactBookId = contactBookIdParam,
                        companyId = companyIdParam,
                        name = nameParam,
                        phone = phoneParam,
                        email = emailParam,
                        address = addressParam,
                        companyName = companyNameParam,
                        contactBookName = contactBookNameParam,
                        offset = (filter.Page.Value - 1) * filter.PageSize.Value,
                        pageSize = filter.PageSize.Value
                    }).ConfigureAwait(false))
                {
                    var result = multi.Read<ContactDao>().ToList();
                    var contacts = result?.Select(item => item.Export());

                    int total = multi.ReadFirst<int>();

                    var paging = new PageData {
                        Page = filter.Page.Value,
                        PageSize = filter.PageSize.Value,
                        Total = total
                    };

                    var contactsViewModel = contacts is null || contacts.Count() == 0
                        ? Enumerable.Empty<ContactViewModel>()
                        : (from c in contacts
                           select new ContactViewModel(c.Id, c.ContactBookId, c.CompanyId, c.Name, c.Phone, c.Email, c.Address)
                          ).AsEnumerable();

                    return new PagedResponseModel<ContactViewModel>(paging, contactsViewModel);
                }
            }
        }

        public async Task<ContactViewModel> GetAsync(int id)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var query = "SELECT * FROM Contact WHERE Id = @id";
                var result = await connection.QuerySingleOrDefaultAsync<ContactDao>(query, new { id });

                var contact = result?.Export();

                if (contact is null)
                    throw new AppException($@"Contact not found.");

                return new ContactViewModel(contact.Id, contact.ContactBookId, contact.CompanyId, contact.Name, contact.Phone, contact.Email, contact.Address);
            }
        }

        public async Task<ICollection<ContactViewModel>> ImportContactFile(IEnumerable<ContactViewModel> contactViewModelList)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                List<ContactDao> contactList = new List<ContactDao>();
                List<ContactViewModel> errorList = new List<ContactViewModel>();
                foreach (var contactViewModel in contactViewModelList)
                {
                    ContactDao contact = new ContactDao(contactViewModel);

                    if (contact.IsValid())
                        contactList.Add(contact);
                    else
                        errorList.Add(contactViewModel);
                }

                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var stopwatch = new StopWatch();
                    stopwatch.Start();

                    foreach (var contact in contactList)
                        connection.Insert(contact);

                    await transaction.CommitAsync();
                    stopwatch.Stop();
                }

                return errorList.ToList();
            }
        }

        public async Task<List<ContactViewModel>> ExportContactFile()
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                string sql = "SELECT * FROM Contact";

                var contacts = await connection.QueryAsync<ContactDao>(sql);

                var contactsViewModel = contacts?.Select(item =>
                    new ContactViewModel(
                        item.Id,
                        item.ContactBookId,
                        item.CompanyId,
                        item.Name,
                        item.Phone,
                        item.Email,
                        item.Address
                    )
                ).ToList();

                return contactsViewModel;
            }
        }
    }

    [Table("Contact")]
    public class ContactDao : IContact
    {
        [Key]
        public int Id { get; set; }
        public int ContactBookId { get; set; }
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public ContactDao()
        { }

        public ContactDao(ContactViewModel contact)
        {
            Id = contact.Id;
            ContactBookId = contact.ContactBookId;
            CompanyId = contact.CompanyId;
            Name = contact.Name;
            Phone = contact.Phone;
            Email = contact.Email;
            Address = contact.Address;
        }

        public bool IsValid()
        {
            if (this.ContactBookId <= 0)
                return false;

            if (string.IsNullOrWhiteSpace(this.Name))
                return false;

            return true;
        }

        public IContact Export() => new Contact(Id, ContactBookId, CompanyId, Name, Phone, Email, Address);
    }
}