using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductCatalogApi.Data;

namespace ProductCatalogApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Create host is where docker container gets created
            var host = CreateHostBuilder(args).Build();
            //Build() returns an IHost Interface, "A program abstraction."
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.ihost?view=dotnet-plat-ext-3.1

            // Creates a host
            // Within the host (see CreateHostBuilder() below)
            // Creates a Startup
            // Which sets up your services one-by-one
            // Then run the microservice

            // Create host is the application in which all docker containers are created
            // Right after Build() returns, you can guarantee that services are up
            // Call Seed() between then and Run()

            // host is your host machine that is actually kicking off all these VMs

            // WAIT,
            // First must check to ensure that the CatalogContext service is running
            using (var scope = host.Services.CreateScope())
            // The IHost.Services property contains "The programs configured services."

            // Services is of type IServiceProvider, whose docs page appears to be
            // missing, but here is the ServiceProviderService extension class
            // ("Extension methods for getting services from an IServiceProvider.")
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceproviderserviceextensions?view=dotnet-plat-ext-3.1

            // ServiceProviderServiceExtensions.CreateScope(IServiceProvider)
            // Or,
            // IServiceProvider.CreateScope()
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceproviderserviceextensions.createscope?view=dotnet-plat-ext-3.1#Microsoft_Extensions_DependencyInjection_ServiceProviderServiceExtensions_CreateScope_System_IServiceProvider_
            // "Creates/Returns an IServiceScope that can be used to resolve scoped services."
            // IServiceScope Interface
            // "The Dispose() method ends the scope lifetime. Once Dispose is called, any scoped services that have been resolved from ServiceProvider will be disposed."
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescope?view=dotnet-plat-ext-3.1


            // Ask Startup about each of the services that are being set up
            // Are they set up?
            // Scope of each service means its status in setup?
            {
            // Recall from the using statement above that since CreateScope()
            // returned an IServiceScope, var scope is of type IServiceScope

            // IServiceScope (the scope) has property ServiceProvider of type IServiceProvider:
            // "The IServiceProvider used to resolve dependencies from the scope."
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.iservicescope.serviceprovider?view=dotnet-plat-ext-3.1#Microsoft_Extensions_DependencyInjection_IServiceScope_ServiceProvider

                var serviceProviders = scope.ServiceProvider;

                // In this situation, ServiceProvider will be CatalogContext
                var context = serviceProviders.GetRequiredService<CatalogContext>();
                // Of all the service providers, I'm looking for just one, the CatalogContext
                // GetRequiredService<CatalogContext>() won't return until the
                // db service is available. This is a wait scenario.

                // ServiceProviderServiceExtensions.GetRequiredService()
                // GetRequiredService<T>(IServiceProvider)
                // Or,
                // IServiceProvider.GetRequiredService<CatalogContext>();
                //https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.serviceproviderserviceextensions.getrequiredservice?view=dotnet-plat-ext-3.1#Microsoft_Extensions_DependencyInjection_ServiceProviderServiceExtensions_GetRequiredService__1_System_IServiceProvider_
                // "Returns a service object of type CatalogContext."

                // Recall the signature of CatalogSeed.Seed():
                // public static void Seed(CatalogContext context)

                // Finally seed the db with the data
                CatalogSeed.Seed(context);
            }
            // QUESTION:
            // Totally understand why it's important to check and see that the
            // CatalogContext service is running. Why is it so involed to do so?
            // What is there to clean up from getting the scope?

            // Now that data is all set up, finally run the host
            host.Run();
            // (This used to happen right after the Build() step at the top of Main())

            // Brillan's question in-class:
            // Only breaking this up: CreateHostBuilder(args).Build().Run();
            // because seeding the db with sample data? Answer: yes.
            // Unless you want to do something between your creation and your deployment
            // In real production, you may not need to break this up.


            // Now go back up to using statement:
            // using (var scope = host.Services.CreateScope()) {}
            // overloading of the term/keyword
            // guarantees that the object will be disposed at the end
            // of the block. ***ONLY*** if the object type implements IDisposable
            // Fellow student's good guess
            // Don't have to wait until G0 is running low, G1 is running low
            // Calls Dispose() on the object proactively
            // Object can be cleaned up without needing to run the finalizer


            // QUESTION
            // Why do I keep thinking of exception handling with the using
            // statement?
            // Kal says no relationship between using statement and try/catch/finally
            // Am I confuse?


            // ---
            // Now done here in Program.cs
            // Want to see this in action
            // See db get created

            // Delete any existing CatalogDb from previous runs
            // Then, note that there is no CatalogDb in LocalDb

            // First! Run Powershell Command
            // Need to AddMigrations
            // Need to inform EntityFramework of all these changes
            // No folder called Migrations in project (yet)

            // Open Powershell prompt
            // View > Other Windows > Package Manager Console (Ctrl+`)
            // (Powershell useful for managing NuGet packages)
            // Make sure you're pointing at the right project:
            // Default project: src\services\ProductCatalogApi
            
            // PM> Add-Migration Initial
            // (Will get warning about CatalogItem.Price column not having type specified
            // See how Migrations folder appeared in project
            // Contains instructions for EF to create tables
            // Converted your code into SQL-like (yet still C#) syntax
            // DO NOT EDIT THE AUTOGEN FILE
            // We've captured the delta.
            // This initial migration goes from nothing to all tables set up.

            // **Could** run UpdateDatabase manually
            // But already wrote the code to do that from CatalogSeed.Seed()

            // So...
            // Almost ready to build and run code.
            // Program
            // Main
            // Startup
            // CatalogSeed
            // CatalogContext.Database.Migrate() (does UpdateDatabase, which applies migrations)

            // But first!
            // Run chooses Docker because we added docker support, but not configured yet
            // Run as IIS Express instead for now (Internet Information Services)
            // IIS is a provider that opens up the port for your web applications
            // to run (on Windows)

            // (On Linux, called tomcat?)

            // We are building a microservice, even though not a web application,
            // it is still a Web API (haven't written any yet, but we will)

            // OK, RUN
            // Confirm that seed data was set


            // REMEMBER:
            // When using the SQL Server Object Explorer to view db tables
            // and data, remember to refresh the view/window to see the created db.
            // Right click on a table, View Data to see the data in the table.


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
