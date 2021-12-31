using Microsoft.Extensions.DependencyInjection;
using TesteBackendEnContact.Auth;
using TesteBackendEnContact.Auth.Interface;

namespace TesteBackendEnContact.Config
{
    public static class JwtExtensions
    {
        public static void RegisterJwt(this IServiceCollection services)
        {
            services.AddScoped<IJwtUtils, JwtUtils>();
        }
    }
}