using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;

using DbCRUDReposLib;
using DbContextLib;
using SeidoDbWebApi.Logger;

namespace DbAppWebApi
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
            //Add the DbContext to the services
            var connectionString = AppConfig.ConfigurationRoot.GetConnectionString(AppConfig.ThisConnection);

            services.AddDbContext<SeidoDbContext>(options => options.UseSqlite(connectionString));

            // global cors policy
            services.AddCors();

            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "DbWebApi", Version = "v1" });
            });


            //Dependency Injection for the controller class constructors
            services.AddSingleton<ILoggerProvider, InMemoryLoggerProvider>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "DbWebApi v1");
                c.SupportedSubmitMethods(new[] {
                        SubmitMethod.Get, SubmitMethod.Put, SubmitMethod.Delete, SubmitMethod.Post});
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }
            
            app.UseHttpsRedirection();
            app.UseRouting();

            // global cors policy - the call to UseCors() must be done here
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
