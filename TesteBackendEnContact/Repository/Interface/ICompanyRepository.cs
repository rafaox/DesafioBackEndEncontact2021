using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.Company;
using TesteBackendEnContact.Core.Interface.Company;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface ICompanyRepository
    {
        Task<ICompany> SaveAsync(RegisterCompanyViewModel contactBookViewModel);
        Task DeleteAsync(int id);
        Task<PagedResponseModel<CompanyViewModel>> GetAllAsync(CompanyFilter filter);
        Task<CompanyViewModel> GetAsync(int id);
    }
}
