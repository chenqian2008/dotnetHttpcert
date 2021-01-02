using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
namespace demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12,
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback = (message, cer, chain, errors) =>
                {
                    return chain.Build(cer);
                }
            };
            var path = AppDomain.CurrentDomain.BaseDirectory + "cert\\client.pfx";
            var crt = new X509Certificate2(path, "123456789");
            handler.ClientCertificates.Add(crt);

            var client = new HttpClient(handler);
            var url = "https://localhost:5001/WeatherForecast";
            var response = client.GetAsync(url).Result;
            Console.WriteLine(response.IsSuccessStatusCode);
            var back = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(back);
        }
      
    }
}
