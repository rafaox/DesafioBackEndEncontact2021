using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Compression;
using TesteBackendEnContact.Auth;
using TesteBackendEnContact.Config;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Errors;

namespace TesteBackendEnContact
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();            
            services.AddFluentMigratorCore()
                    .ConfigureRunner(rb => rb
                        .AddSQLite()
                        .WithGlobalConnectionString(Configuration.GetConnectionString("DefaultConnection"))
                        .ScanIn(typeof(Startup).Assembly).For.Migrations())
                    .AddLogging(lb => lb.AddFluentMigratorConsole());
            services.AddSingleton(new DatabaseConfig { ConnectionString = Configuration.GetConnectionString("DefaultConnection") });
            
            services.AddHttpContextAccessor();
            services.AddControllers().ConfigureApiBehaviours();
            services.RegisterApiVersioning();
            services.RegisterSwaggerGen("TesteBackendEnContact", "Documentação de auxílio");
            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Fastest);
            services.AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>());
            services.AddRouting(options => options.LowercaseUrls = true);
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.RegisterApiVersioning();
            services.JwtAuthConfig(Configuration);
            services.RegisterJwt();
            services.ConfigureServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.RegisterSwaggerUI("TesteBackendEnContact v1", serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>());
            }

            app.UseHttpsRedirection();
            app.UseResponseCompression();
            app.UseRouting();
            app.UseMiddleware<ErrorHandlerMiddleware>();
            app.UseMiddleware<JwtMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
        }
    }
}
