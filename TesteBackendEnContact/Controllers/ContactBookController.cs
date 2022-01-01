using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.ContactBook;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    public class ContactBookController : BaseController
    {
        private readonly ILogger<ContactBookController> _logger;
        private readonly IContactBookRepository _contactBookRepository;

        public ContactBookController(ILogger<ContactBookController> logger, IContactBookRepository contactBookRepository)
        {
            _logger = logger;
            _contactBookRepository = contactBookRepository;
        }

        [HttpPost]
        public async Task<BaseResponse<string>> Post([FromBody] RegisterContactBookViewModel contactBookViewModel)
        {
            await _contactBookRepository.SaveAsync(contactBookViewModel);
            return BaseResponse<string>.Created("Registration successful");
        }

        [HttpDelete("{id}")]
        public async Task<BaseResponse<string>> Delete(int id)
        {
            await _contactBookRepository.DeleteAsync(id);
            return BaseResponse<string>.Ok("Contact book deleted successfully");
        }

        [HttpGet]
        public async Task<BaseResponse<PagedResponseModel<ContactBookViewModel>>> Get([FromQuery] ContactBookFilter filter)
        {
            PagedResponseModel<ContactBookViewModel> contactBooks = await _contactBookRepository.GetAllAsync(filter);
            return BaseResponse<PagedResponseModel<ContactBookViewModel>>.Ok(contactBooks);
        }

        [HttpGet("{id}")]
        public async Task<BaseResponse<ContactBookViewModel>> Get(int id)
        {
            ContactBookViewModel contactBook = await _contactBookRepository.GetAsync(id);
            return BaseResponse<ContactBookViewModel>.Ok(contactBook);
        }
    }
}
