using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models.Auth;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IAuthRepository
    {
        Task<AuthenticateResponseViewModel> Authenticate(AuthenticateRequestViewModel viewModel);
    }
}