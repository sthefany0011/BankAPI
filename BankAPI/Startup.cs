using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BankAPI.Data;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using BankAPI.Repositories;

namespace BankAPI
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
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankAPI", Version = "v1" });
            });


            services.AddDbContext<BankAPIContext>(options =>
                   options.UseMySql("server=localhost;userid=root;password=11041994;database=bankapi",
                    new MySqlServerVersion(new Version()), builder =>
            builder.MigrationsAssembly("BankAPI")));

            services.AddTransient<IContaRepository, ContaRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var ptBR = new CultureInfo("pt-BR");
            var localizationOption = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ptBR),
                SupportedCultures= new List<CultureInfo> { ptBR },
                SupportedUICultures = new List<CultureInfo> { ptBR },
            };

            app.UseRequestLocalization(localizationOption);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankAPI v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
