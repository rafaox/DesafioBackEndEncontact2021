using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactBookRepository
    {
        Task<IContactBook> SaveAsync(RegisterContactBookViewModel contactBookViewModel);
        Task DeleteAsync(int id);
        Task<PagedResponseModel<ContactBookViewModel>> GetAllAsync(ContactBookFilter filter);
        Task<ContactBookViewModel> GetAsync(int id);
    }
}
