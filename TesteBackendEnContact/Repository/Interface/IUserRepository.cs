using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.User;
using TesteBackendEnContact.Core.Interface.User;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IUserRepository
    {
        Task<IUser> SaveAsync(RegisterUserViewModel user);
        Task DeleteAsync(int id);
        Task<PagedResponseModel<UserViewModel>> GetAllAsync(UserFilter filter);
        Task<UserViewModel> GetAsync(int id);
        Task<bool> ValidatePassword(IUser user, string password);
    }
}