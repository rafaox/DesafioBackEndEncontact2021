using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TesteBackendEnContact.Auth;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.User;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    public class UserController : BaseController
    {
        private IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<BaseResponse<string>> Post([FromBody] RegisterUserViewModel viewModel)
        {
            await _userRepository.SaveAsync(viewModel);
            return BaseResponse<string>.Created("Registration successful");
        }

        [HttpGet]
        public async Task<BaseResponse<PagedResponseModel<UserViewModel>>> Get([FromQuery] UserFilter filter)
        {
            PagedResponseModel<UserViewModel> users = await _userRepository.GetAllAsync(filter);
            return BaseResponse<PagedResponseModel<UserViewModel>>.Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<BaseResponse<UserViewModel>> Get(int id)
        {
            UserViewModel user = await _userRepository.GetAsync(id);
            return BaseResponse<UserViewModel>.Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<BaseResponse<string>> Delete(int id)
        {
            await _userRepository.DeleteAsync(id);
            return BaseResponse<string>.Ok("User deleted successfully");
        }
    }
}