using BCryptNet = BCrypt.Net.BCrypt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.User;
using TesteBackendEnContact.Core.Domain.User;
using TesteBackendEnContact.Core.Interface.User;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Errors;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public UserRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    var sql = new StringBuilder();
                    sql.AppendLine("DELETE FROM User WHERE Id = @id;");

                    await connection.ExecuteAsync(sql.ToString(), new { id }, transaction);

                    transaction.Commit();
                }
            }
        }

        public async Task<PagedResponseModel<UserViewModel>> GetAllAsync(UserFilter filter)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                string email = string.Empty;
                string term = string.Empty;
                if (!string.IsNullOrWhiteSpace(filter.Email))
                {
                    term = "%" + filter.Email.Replace("[", "[[]").Replace("%", "[%]") + "%";
                    email = !string.IsNullOrWhiteSpace(filter.Email) ? "WHERE Email LIKE @email" : "";
                }

                var orderedBy = filter.OrderByDescending.Value ? "DESC" : "ASC";

                string query = $@"
                SELECT *
                FROM User
                {email}
                ORDER BY Id {orderedBy}
                LIMIT @pageSize
                OFFSET @offset;
                
                SELECT COUNT(*)
                FROM User
                {email}
                ;";

                connection.Open();

                using (var multi = await connection.QueryMultipleAsync(query,
                    new
                    {
                        email = term,
                        offset = (filter.Page.Value - 1) * filter.PageSize.Value,
                        pageSize = filter.PageSize.Value
                    }).ConfigureAwait(false))
                {
                    var result = multi.Read<UserDao>().ToList();
                    var users = result?.Select(item => item.Export());

                    int total = multi.ReadFirst<int>();

                    var paging = new PageData {
                        Page = filter.Page.Value,
                        PageSize = filter.PageSize.Value,
                        Total = total
                    };

                    var usersViewModel = users is null
                        ? Enumerable.Empty<UserViewModel>()
                        : (from u in users
                        select new UserViewModel
                        {
                            Id = u.Id,
                            Email = u.Email
                        }).AsEnumerable();

                    return new PagedResponseModel<UserViewModel>(paging, usersViewModel);
                }
            }
        }

        public async Task<UserViewModel> GetAsync(int id)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var query = "SELECT * FROM User WHERE Id = @id";
                var result = await connection.QuerySingleOrDefaultAsync<UserDao>(query, new { id });

                var user = result?.Export();

                if (user is null)
                    throw new AppException($@"User not found.");

                return new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email
                };
            }
        }

        public async Task<IUser> SaveAsync(RegisterUserViewModel viewModel)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var query = "SELECT * FROM User WHERE Email = @email";
                var result = await connection.QuerySingleOrDefaultAsync<UserDao>(query, new { email = viewModel.Email });

                if (result is not null)
                    throw new AppException($@"Email ${viewModel.Email} is already taken");

                var user = new User(
                    default(int),
                    viewModel.Email
                );

                user.SetPassword(viewModel.Password);

                var dao = new UserDao(user);

                if (dao.Id == 0)
                    dao.Id = await connection.InsertAsync(dao);
                else
                    await connection.UpdateAsync(dao);

                return dao.Export();
            }
        }

        public async Task<bool> ValidatePassword(IUser user, string password)
        {
            using (var connection = new SqliteConnection(databaseConfig.ConnectionString))
            {
                var query = "SELECT * FROM User WHERE Id = @id";
                var result = await connection.QuerySingleOrDefaultAsync<UserDao>(query, new { id = user.Id });

                return string.IsNullOrWhiteSpace(password) ? false : result.VerifyPassword(password);
            }
        }
    }

    [Table("User")]
    public class UserDao : IUser
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public UserDao()
        { }

        public UserDao(IUser user)
        {
            Id = user.Id;
            Email = user.Email;
            Password = user.Password;
        }

        public IUser Export() => new User(Id, Email);

        public bool VerifyPassword(string password)
        {
            return BCryptNet.Verify(password, this.Password);
        }
    }
}