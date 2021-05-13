using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Estudos_API.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Estudos_API
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
            services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddControllers();

            // swagger
            services.AddSwaggerGen(config => {
                config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo{ Title = "Estudo de API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // gera arquivo json
            app.UseSwagger();

            // modifica a rota do swagger
            // app.UseSwagger(config => {
            //     config.RouteTemplate = "thais/{documentName}/swagger.json";
            // });

            // define o local onde a documentação irá ficar -> vews html do swagger
            app.UseSwaggerUI(config => {
                // config.SwaggerEndpoint("/thais/v1/swagger.json", "estudo api v1"); // -> swagger com a rota modificada
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "estudo api v1");
            });
        }
    }
}
