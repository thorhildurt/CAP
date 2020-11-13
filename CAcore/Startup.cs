using System;
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
using CAcore.Data;
using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Extensions;
using Pomelo.EntityFrameworkCore.MySql;
using MySqlConnector;
using AutoMapper;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;

namespace CAcore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static string CertThumbprint { get; set;}

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set DbConnectionString in user secrets
            var dbConnectionString = Configuration["DbConnectionString"];
            

            services.AddDbContext<CAcoreContext>(options => options.UseMySql(dbConnectionString));
            services.AddControllers();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<ICAcoreRepo, MySqlCAcoreRepo>();
            services.AddSingleton<IConfiguration>(Configuration);
            // Uncomment to test using hardcoded mock data in MockCAcoreRepo
            // services.AddScoped<ICAcoreRepo, MockCAcoreRepo>();

            CertThumbprint = Configuration["CertThumbprint"];
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

            //  Windows-specific, just for testing CRL, probably needs to be served on the web server
            app.UseStaticFiles(new StaticFileOptions() {
            FileProvider =  new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
            RequestPath = new PathString("")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
