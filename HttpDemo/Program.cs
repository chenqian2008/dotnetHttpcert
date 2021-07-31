using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HttpDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();

                    webBuilder.ConfigureKestrel(kerstrel =>
                    {
                        kerstrel.ConfigureHttpsDefaults(https =>
                        {
                            var serverPath = AppDomain.CurrentDomain.BaseDirectory + "cert\\server.pfx";
                            var serverCertificate = new X509Certificate2(serverPath, "123456");
                            https.ServerCertificate = serverCertificate;
                            https.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
                            https.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls | SslProtocols.None | SslProtocols.Tls11;
                            https.ClientCertificateValidation = (cer, chain, error) =>
                                       {
                                           return chain.Build(cer);
                                       };

                        });
                    });
                });
    }
}
