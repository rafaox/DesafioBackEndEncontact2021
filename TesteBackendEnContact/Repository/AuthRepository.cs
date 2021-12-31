using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using TesteBackendEnContact.Auth.Interface;
using TesteBackendEnContact.Controllers.Models.Auth;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Errors;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DatabaseConfig _databaseConfig;
        private readonly IUserRepository _userRepository;
        private readonly IJwtUtils _jwtUtils;

        public AuthRepository(DatabaseConfig databaseConfig, IUserRepository userRepository, IJwtUtils jwtUtils)
        {
            _databaseConfig = databaseConfig;
            _userRepository = userRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<AuthenticateResponseViewModel> Authenticate(AuthenticateRequestViewModel viewModel)
        {
            using var connection = new SqliteConnection(_databaseConfig.ConnectionString);

            var query = "SELECT * FROM User WHERE Email = @email";
            var user = await connection.QuerySingleOrDefaultAsync<UserDao>(query, new { email = viewModel.Email });

            if (user is null)
                throw new AppException("User not found");
            
            if (!await _userRepository.ValidatePassword(user, viewModel.Password))
                throw new AppException("Incorrect password");

            var response = new AuthenticateResponseViewModel {
                Id = user.Id,
                Email = user.Email,
                JwtToken = _jwtUtils.GenerateToken(user)
            };

            return response;
        }
    }
}