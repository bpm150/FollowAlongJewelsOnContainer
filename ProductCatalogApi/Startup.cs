using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProductCatalogApi.Data;


// https://youtu.be/1k11n8FO_UU?list=PLdbymrfiqF-wmh3VsbxysBsu2O9w6Z3Ks&t=3232

// Injection: who is going to tell me where
//

namespace ProductCatalogApi
{
    public class Startup
    {
        // Startup is a class.
        // Also being injected.
        // Whenever you see soemting being passed as a constructor param
        
        // Who is going to create an instance of the CatalogContext class?
        // (Not me.)
        // When this service is set up, want the context to be created and ready
        // to go. Want context and db already set up, then apps can start using them.
        // Same goes for Startup.
        // when you set up this container,
        // dotnet core will kick off, instantiate Startup and pass it construction params
        // dotnet core also responsbile for providing the config file for me
  
        // To appsettings.json...


        // appsettings.json will get injected into Startup
        // in the form of this configuration object

        // This is much easier than manually parsing these
        // files as Kal has done over the last 20 years
        // Now the infrastructure is here.
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // ** ConfigureServices is the method that will get called right off the bat
        // Use to configure your microservices

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Haven't written a controller yet.
            // Will come back to this.
            services.AddControllers();
            // Controllers are your routes
            // For services to work, will trigger all your controllers


            // Services need a database context of type CatalogContext
            // Inject a database context into my service

            // Means that Startup will create an instance of CatalogContext
            // This is CatalogContext instance will connect to your db

            // In memory, Startup creates an instance of CatalogContext
            // that is now going to be connected to the physical db

            // Recall that CatalogContext constructor accepts a DbContextOptions param
            // Now it's time to provide what the options are
            services.AddDbContext<CatalogContext>(options =>
                options.UseSqlServer(Configuration["ConnectionString"]));
            // "Registers the given context as a service in the IServiceCollection. You use this method when using dependency injection in your application, such as with ASP.NET."
            // AddDbContext<TContext>(IServiceCollection, Action<DbContextOptionsBuilder>, ServiceLifetime, ServiceLifetime)
            // Or,
            // IServiceCollection.AddDbContext<CatalogContext>(Action<DbContextOptionsBuilder>)
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.entityframeworkservicecollectionextensions.adddbcontext?view=efcore-3.1#Microsoft_Extensions_DependencyInjection_EntityFrameworkServiceCollectionExtensions_AddDbContext__1_Microsoft_Extensions_DependencyInjection_IServiceCollection_System_Action_Microsoft_EntityFrameworkCore_DbContextOptionsBuilder__Microsoft_Extensions_DependencyInjection_ServiceLifetime_Microsoft_Extensions_DependencyInjection_ServiceLifetime_

            // options is a DbContextOptionsBuilder
            // For the first time, explicitly saying that going to use sql server

            // UseSqlServer(DbContextOptionsBuilder, < invoking object (extension method)
            //              String,                  < actual param
            //              Action<SqlServerDbContextOptionsBuilder> < is null by default
            // Or,
            //DbContextOptionsBuilder.
            //          UseSqlServer(String)
            // "Configures the context to connect to a Microsoft SQL Server database."
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.sqlserverdbcontextoptionsextensions.usesqlserver?view=efcore-3.1#Microsoft_EntityFrameworkCore_SqlServerDbContextOptionsExtensions_UseSqlServer_Microsoft_EntityFrameworkCore_DbContextOptionsBuilder_System_String_System_Action_Microsoft_EntityFrameworkCore_Infrastructure_SqlServerDbContextOptionsBuilder__

            // Configuration is an IConfiguration
            // (has a key/value getter and setter)
            // It corresponds to the appsettings.json file
            // Use the key "ConnectionString" to look up the value of the
            // ConnectionString

            // At this point,
            // a) My services need a database           services.AddDbContext
            // b) what kind of database?                <CatalogContext>
            // c) Startup creates an instance of CatalogContext
            // d) when it does so, it needs to pass     a DbContextOptionsBuilder  
            // e) use sql server                        (options => options.UseSqlServer
            // f) here is the connection string from the config that says where it is
            //                                      (Configuration["ConnectionString"]));


            // THEN:
            // DbContextOptions injected into the constructor for CatalogContext
            // which will in turn pass the options on to the base class (DbContext)
            // which is when Entity Framework will create the database there for you.

            // That is how you connect the object in memory to the actual SQL server

            // Now whenever you refer to CatalogContext in code, it knows which actual
            // physical db and table it needs to go to

            // Next week: Still have powershell commands to run

            // Now have:
            // Models defined
            // Entity Framework defined
            // Now a matter of executing it and seeing it in action

            // See tables created by rules I've defined then can write
            // controllers in code (APIs) to really use that table,
            // seed in some data, and see the catalog work

            // Once models defined, all about writing APIs

            // Discussion about what happens before Startup...Main() in Program.cs...







        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
