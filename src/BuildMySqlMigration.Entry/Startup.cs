using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shashlik.Identity;
using Shashlik.Kernel;

namespace BuildMySqlMigration.Entry
{
    // dotnet ef migrations add init -c ShashlikIdentityDbContext  -o DefaultMigrations -p ./Shashlik.Identity.MySql/Shashlik.Identity.MySql.csproj -s ./BuildMySqlMigration.Entry/BuildMySqlMigration.Entry.csproj
    // dotnet ef migrations add init -c ConfigurationDbContext  -o DefaultConfigurationMigrations -p ./Shashlik.Ids4.MySqlStore/Shashlik.Ids4.MySqlStore.csproj -s ./BuildMySqlMigration.Entry/BuildMySqlMigration.Entry.csproj
    // dotnet ef migrations add init -c PersistedGrantDbContext  -o DefaultPersistedMigrations -p ./Shashlik.Ids4.MySqlStore/Shashlik.Ids4.MySqlStore.csproj -s ./BuildMySqlMigration.Entry/BuildMySqlMigration.Entry.csproj

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddShashlik(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env
        )
        {
            ShashlikIdentityDbContext dbContext1 = app.ApplicationServices.GetRequiredService<ShashlikIdentityDbContext>();
            ConfigurationDbContext dbContext2 = app.ApplicationServices.GetRequiredService<ConfigurationDbContext>();
            PersistedGrantDbContext dbContext3 = app.ApplicationServices.GetRequiredService<PersistedGrantDbContext>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}