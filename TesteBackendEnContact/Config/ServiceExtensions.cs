using Microsoft.Extensions.DependencyInjection;
using TesteBackendEnContact.Repository;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Config
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IContactBookRepository, ContactBookRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IContactRepository, ContactRepository>();
        }
    }
}