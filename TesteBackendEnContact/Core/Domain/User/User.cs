using TesteBackendEnContact.Core.Interface.User;
using BCryptNet = BCrypt.Net.BCrypt;

namespace TesteBackendEnContact.Core.Domain.User
{
    public class User : IUser
    {
        public int Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        public User(int id, string email)
        {
            Id = id;
            Email = email;
        }

        public void SetPassword(string password)
        {
            this.Password = BCryptNet.HashPassword(password);
        }
    }
}