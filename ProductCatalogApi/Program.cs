using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ProductCatalogApi
{
    // We will change something here in Program later

    public class Program
    {
        public static void Main(string[] args)
        {
            // Create host is where docker container gets created
            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // This is where Startup gets created
                    webBuilder.UseStartup<Startup>();
                });
    }
}
