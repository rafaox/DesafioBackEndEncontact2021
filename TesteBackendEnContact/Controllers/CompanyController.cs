using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.Company;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    public class CompanyController : BaseController
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly ICompanyRepository _companyRepository;

        public CompanyController(ILogger<CompanyController> logger, ICompanyRepository companyRepository)
        {
            _logger = logger;
            _companyRepository = companyRepository;
        }

        [HttpPost]
        public async Task<BaseResponse<string>> Post([FromBody] RegisterCompanyViewModel companyViewModel)
        {
            await _companyRepository.SaveAsync(companyViewModel);
            return BaseResponse<string>.Created("Registration successful");
        }

        [HttpDelete("{id}")]
        public async Task<BaseResponse<string>> Delete(int id)
        {
            await _companyRepository.DeleteAsync(id);
            return BaseResponse<string>.Ok("Company deleted successfully");
        }

        [HttpGet]
        public async Task<BaseResponse<PagedResponseModel<CompanyViewModel>>> Get([FromQuery] CompanyFilter filter)
        {
            PagedResponseModel<CompanyViewModel> companiesViewModel = await _companyRepository.GetAllAsync(filter);
            return BaseResponse<PagedResponseModel<CompanyViewModel>>.Ok(companiesViewModel);
        }

        [HttpGet("{id}")]
        public async Task<BaseResponse<CompanyViewModel>> Get(int id)
        {
            CompanyViewModel companyViewModel = await _companyRepository.GetAsync(id);
            return BaseResponse<CompanyViewModel>.Ok(companyViewModel);
        }
    }
}
