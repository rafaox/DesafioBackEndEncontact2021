using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Base;
using TesteBackendEnContact.Controllers.Filters;
using TesteBackendEnContact.Controllers.Models.Contact;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactRepository
    {
        Task<ICollection<ContactViewModel>> ImportContactFile (IEnumerable<ContactViewModel> contactViewModelList);
        Task<List<ContactViewModel>> ExportContactFile ();
        Task<PagedResponseModel<ContactViewModel>> GetAllAsync(ContactFilter filter);
        Task<ContactViewModel> GetAsync(int id);
    }
}