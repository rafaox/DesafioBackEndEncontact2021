using Microsoft.AspNetCore.Mvc;
using TesteBackendEnContact.Auth;

namespace TesteBackendEnContact.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiversion}/[controller]")]
    public class BaseController : Controller
    { }
}