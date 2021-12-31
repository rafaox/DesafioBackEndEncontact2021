using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TesteBackendEnContact.Auth;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Models.Auth;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthRepository _authRepository;

        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<BaseResponse<AuthenticateResponseViewModel>> Authenticate([FromBody] AuthenticateRequestViewModel viewModel)
        {
            AuthenticateResponseViewModel response = await _authRepository.Authenticate(viewModel);
            return BaseResponse<AuthenticateResponseViewModel>.Ok(response);
        }
    }
}