using System.Collections.Generic;

namespace TesteBackendEnContact.Errors
{
    public class ErrorResponse
    {
        public IEnumerable<object> Errors { get; set; }
    }
}