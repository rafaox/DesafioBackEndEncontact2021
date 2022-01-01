using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.ContactBook;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Errors;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class ContactBookRepository : IContactBookRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactBookRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<IContactBook> SaveAsync(RegisterContactBookViewModel contactBookViewModel)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var contactBook = new ContactBook(default(int), contactBookViewModel.Name);
                var dao = new ContactBookDao(contactBook);

                dao.Id = await connection.InsertAsync(dao);

                return dao.Export();
            }
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var query = "DELETE FROM ContactBook WHERE Id = @id;";
                    await connection.ExecuteAsync(query, new { id }, transaction);

                    transaction.Commit();
                }
            }
        }

        public async Task<PagedResponseModel<ContactBookViewModel>> GetAllAsync(ContactBookFilter filter)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                string name = string.Empty;
                string term = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    term = "%" + filter.Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    name = !string.IsNullOrWhiteSpace(filter.Name) ? "WHERE Name LIKE @name" : "";
                }

                var orderedBy = filter.OrderByDescending.Value ? "DESC" : "ASC";

                string query = $@"
                SELECT *
                FROM ContactBook
                {name}
                ORDER BY Id {orderedBy}
                LIMIT @pageSize
                OFFSET @offset;
                
                SELECT COUNT(*)
                FROM ContactBook
                {name}
                ;";

                connection.Open();

                using (var multi = await connection.QueryMultipleAsync(query,
                    new
                    {
                        name = term,
                        offset = (filter.Page.Value - 1) * filter.PageSize.Value,
                        pageSize = filter.PageSize.Value
                    }).ConfigureAwait(false))
                {
                    var result = multi.Read<ContactBookDao>().ToList();
                    var contactBooks = result?.Select(item => item.Export());

                    int total = multi.ReadFirst<int>();

                    var paging = new PageData {
                        Page = filter.Page.Value,
                        PageSize = filter.PageSize.Value,
                        Total = total
                    };

                    var contactBooksViewModel = contactBooks is null || contactBooks.Count() == 0
                        ? Enumerable.Empty<ContactBookViewModel>()
                        : (from c in contactBooks
                        select new ContactBookViewModel
                        {
                            Id = c.Id,
                            Name = c.Name
                        }).AsEnumerable();

                    return new PagedResponseModel<ContactBookViewModel>(paging, contactBooksViewModel);
                }
            }
        }

        public async Task<ContactBookViewModel> GetAsync(int id)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var query = "SELECT * FROM ContactBook WHERE Id = @id";
                var result = await connection.QuerySingleOrDefaultAsync<ContactBookDao>(query, new { id });

                var contactBook = result?.Export();

                if (contactBook is null)
                    throw new AppException($@"Contact book not found.");

                return new ContactBookViewModel
                {
                    Id = contactBook.Id,
                    Name = contactBook.Name
                };
            }
        }
    }

    [Table("ContactBook")]
    public class ContactBookDao : IContactBook
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ContactBookDao()
        { }

        public ContactBookDao(IContactBook contactBook)
        {
            Id = contactBook.Id;
            Name = contactBook.Name;
        }

        public IContactBook Export() => new ContactBook(Id, Name);
    }
}
