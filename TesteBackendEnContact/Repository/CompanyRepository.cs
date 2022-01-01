using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.Sqlite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.Company;
using TesteBackendEnContact.Core.Domain.Company;
using TesteBackendEnContact.Core.Interface.Company;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Errors;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public CompanyRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<ICompany> SaveAsync(RegisterCompanyViewModel companyViewModel)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var company = new Company(default(int), companyViewModel.ContactBookId, companyViewModel.Name);
                var dao = new CompanyDao(company);

                if (dao.Id == 0)
                    dao.Id = await connection.InsertAsync(dao);
                else
                    await connection.UpdateAsync(dao);

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
                    var sql = new StringBuilder();
                    sql.AppendLine("DELETE FROM Company WHERE Id = @id;");
                    sql.AppendLine("UPDATE Contact SET CompanyId = null WHERE CompanyId = @id;");

                    await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);

                    transaction.Commit();
                }
            }
        }

        public async Task<PagedResponseModel<CompanyViewModel>> GetAllAsync(CompanyFilter filter)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                int contactBookIdParam = default(int);
                string contactBookId = string.Empty;
                if (filter.ContactBookId.HasValue)
                {
                    contactBookId = "AND ContactBookId = @contactBookId";
                    contactBookIdParam = filter.ContactBookId.Value;
                }

                string name = string.Empty;
                string nameParam = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    nameParam = "%" + filter.Name.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    name = "AND Name LIKE @name";
                }

                var orderedBy = filter.OrderByDescending.Value ? "DESC" : "ASC";

                string query = $@"
                SELECT *
                FROM Company
                WHERE 1 = 1
                {contactBookId}
                {name}
                ORDER BY Id {orderedBy}
                LIMIT @pageSize
                OFFSET @offset;

                SELECT COUNT(*)
                FROM Company
                WHERE 1 = 1
                {contactBookId}
                {name}
                ;";

                connection.Open();

                using (var multi = await connection.QueryMultipleAsync(query,
                    new
                    {
                        contactBookId = contactBookIdParam,
                        name = nameParam,
                        offset = (filter.Page.Value - 1) * filter.PageSize.Value,
                        pageSize = filter.PageSize.Value
                    }).ConfigureAwait(false))
                {
                    var result = multi.Read<CompanyDao>().ToList();
                    var companies = result?.Select(item => item.Export());

                    int total = multi.ReadFirst<int>();

                    var paging = new PageData {
                        Page = filter.Page.Value,
                        PageSize = filter.PageSize.Value,
                        Total = total
                    };

                    var contactBooksViewModel = companies is null || companies.Count() == 0
                        ? Enumerable.Empty<CompanyViewModel>()
                        : (from c in companies
                           select new CompanyViewModel(c.Id, c.ContactBookId, c.Name)
                          ).AsEnumerable();

                    return new PagedResponseModel<CompanyViewModel>(paging, contactBooksViewModel);
                }
            }
        }

        public async Task<CompanyViewModel> GetAsync(int id)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var query = "SELECT * FROM Company WHERE Id = @id";
                var result = await connection.QuerySingleOrDefaultAsync<CompanyDao>(query, new { id });

                var company = result?.Export();

                if (company is null)
                    throw new AppException($@"Company not found.");

                return new CompanyViewModel(company.Id, company.ContactBookId, company.Name);
            }
        }
    }

    [Table("Company")]
    public class CompanyDao : ICompany
    {
        [Key]
        public int Id { get; set; }
        public int ContactBookId { get; set; }
        public string Name { get; set; }

        public CompanyDao()
        { }

        public CompanyDao(ICompany company)
        {
            Id = company.Id;
            ContactBookId = company.ContactBookId;
            Name = company.Name;
        }

        public ICompany Export() => new Company(Id, ContactBookId, Name);
    }
}
