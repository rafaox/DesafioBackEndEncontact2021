using TesteBackendEnContact.Core.Interface.User;

namespace TesteBackendEnContact.Auth.Interface
{
    public interface IJwtUtils
    {
        public string GenerateToken(IUser user);
        public int? ValidateToken(string token);
    }
}