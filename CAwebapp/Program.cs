using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace CAwebapp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

            Log.Information("Starting web app...");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {   webBuilder.ConfigureKestrel(o =>
                        {   
                            var cert = new X509Certificate2("/home/radwa/web-app-cert/localhost.pfx", "foobar");
                            // o.Listen(IPAddress.Loopback, 3001, listenOpts => {
                            //     listenOpts.UseHttps(cert);
                            // });
                            o.ConfigureHttpsDefaults(o => 
                            o.ClientCertificateMode = 
                            ClientCertificateMode.AllowCertificate);
                            
                            o.Listen(IPAddress.Loopback, 3001, listenOpts => {
                                listenOpts.UseHttps(cert);
                            });

                        });
                    webBuilder.UseStartup<Startup>();
                    // webBuilder.UseUrls("https://localhost:3001/");
                });
    }
}
