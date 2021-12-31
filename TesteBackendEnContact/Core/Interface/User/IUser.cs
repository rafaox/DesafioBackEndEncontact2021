namespace TesteBackendEnContact.Core.Interface.User
{
    public interface IUser
    {
        int Id { get; }
        string Email { get; }
        string Password { get; }
    }
}