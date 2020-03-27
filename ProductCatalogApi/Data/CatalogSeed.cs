using Microsoft.EntityFrameworkCore;
using ProductCatalogApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductCatalogApi.Data
{
    // Begin Class #11 3-7-20
    // Seed the db with sample data so there will be some when the ms starts
    // populate tables with sample data
    public static class CatalogSeed
    {
        // Powershell is the C# scripting language
        // Need to run two Powershell commands to make Entity Framework work
        // Already:
        // Wrote classes (CatalogBrand, CatalogItem, CatalogType)
        // And DbContext (CatalogContext)
        // Powershell script is what causes the conversion path to the db
        // 1. AddMigration
        // Takes your rules that you specified in your DbContext
        // Converts it into SQL
        // Will do deltas
        // First time is everything

        // 2. UpdateDatabase
        // Takes the migration (from step 1) and actually push to db (create tables, etc.)

        // Need to do these two commands again each time your Domain classes
        // or DbContext change

        // Automation of these sounds convenient

        // Conceptually AddMigration as manual step is important because
        // what if you are developing something and it is not ready yet
        // for the database

        // UpdateDatabase, though, that could be automated
        // After you have run AddMigration, it is safe to do
        // Put this at the start of the Seed method:


        // Pass in the context to the catalog, that is a reference to the db
        // so this knows where to populate the data
        public static void Seed(CatalogContext context)
        // A CatalogContext is a connection to a db
        {
            // This is the same as UpdateDatabase Powershell command
            // If there are migrations wating to be sent to the database, send them
            context.Database.Migrate();
            // Check for pending migrations and push them
            // If no pending migrations exist, do nothing

            // In production, the Migrate() call (UpdateDatabase)
            // is the only thing here in Seed() that you'll do
            // Everything else is just for class purposes.
           

            // Populate the tables with data
            if (!context.CatalogBrands.Any())
            // Any is a LINQ query...are there any records?
            // If there are no rows in the table
            {
                // Only then do we seed the table with data

                // Recall the three tables DbSet<> in CatalogContext : DbContext
                // CatalogBrands, CatalogTypes and CatalogItems

                // This is how you can talk to your table from code:
                context.CatalogBrands.AddRange( GetPreconfiguredCatalogBrands() );
                // Just one line in Entity Framework (not 10 lines of SQL query)
                // .Add() for one row, .AddRange() for multiple rows

                // Must save, otherwise changes not committed
                context.SaveChanges();

                // SaveChanges() seperate command so that you can group/bulk add
                // Then save as a final step
                // But in this case, saving before adding CatalogItems is important,
                // because CatalogItems has dependencies on CatalogBrands and
                // CatalogTypes
            }
            if (!context.CatalogTypes.Any())
            {
                context.CatalogTypes.AddRange( GetPreconfiguredCatalogTypes() );
                context.SaveChanges();
            }
            if (!context.CatalogItems.Any())
            {
                context.CatalogItems.AddRange( GetPreconfiguredCatalogItems() );
                context.SaveChanges();
            }
        }

        // Create an array of all CatalogBrand to seed the db with
        //private static CatalogBrand[] GetPreconfiguredCatalogBrands()
        private static IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
        // Instead of returning an array of CatalogBrand,
        // return an IEnumerable<CatalogBrand>
        // QUESTION:
        // Motivation to return an IEnumerable instead of an array?
        // Not really a "read only collection" as Kal suggseted
        // There is the ReadOnlyCollection collection...
        // What is the advantage of IEnumerable over array?
        // Can simply go ToArray() after create list
        // Or maybe start with array to begin with?
        {
            return new List<CatalogBrand>
            // Object init syntax (note no constructor call parens before the braces)
            // Populate them at the time of construction
            {
                // Note not setting the Id properties of these CatalogBrands
                // table will automatically populate Id
                new CatalogBrand{ Brand = "Tiffany & Co." },
                new CatalogBrand{ Brand = "DeBeers" },
                new CatalogBrand{ Brand = "Graff" }
            };
            // }.ToArray();
        }


        private static IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>
            {
                new CatalogType{ Type = "Engagement Ring" },
                new CatalogType{ Type = "Wedding Ring"},
                new CatalogType{ Type = "Fashion Ring"}
            };
        }


        // Discussion of content that needs to be deployed with your app
        // Content needs to be packaged and deployed
        // The rings pics
        // HTML, JS, CSS, etc.
        // wwwroot folder create under project (becomes special folder)
        // Drag and drop the contents of the Pics folder
        // Downloaded from Canvas
        // (Contains jewels and shoes)

        private static IEnumerable<CatalogItem> GetPreconfiguredCatalogItems()
        {
            return new List<CatalogItem>
            {
                // QUESTION:
                // Is there a way to get protection for the brands or types
                // being reordered, or one being removed?
                // CatalogTypeId = 1 is the first CatalogType
                // CatalogBrandId =1 is the first CatalogBrand

                // Notice not setting Id, the table will set that
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=3, Description = "A ring that has been around for over 100 years", Name = "World Star", Price = 199.5M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/1" },
                new CatalogItem { CatalogTypeId=1,CatalogBrandId=2, Description = "will make you world champions", Name = "White Line", Price= 88.50M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/2" },
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=3, Description = "You have already won gold medal", Name = "Prism White", Price = 129, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/3" },
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=2, Description = "Olympic runner", Name = "Foundation Hitech", Price = 12, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/4" },
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=1, Description = "Roslyn Red Sheet", Name = "Roslyn White", Price = 188.5M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/5" },
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=2, Description = "Lala Land", Name = "Blue Star", Price = 112, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/6" },
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=1, Description = "High in the sky", Name = "Roslyn Green", Price = 212, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/7"  },
                new CatalogItem { CatalogTypeId=1,CatalogBrandId=1, Description = "Light as carbon", Name = "Deep Purple", Price = 238.5M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/8" },
                new CatalogItem { CatalogTypeId=1,CatalogBrandId=2, Description = "High Jumper", Name = "Antique Ring", Price = 129, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/9" },
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=3, Description = "Dunker", Name = "Elequent", Price = 12, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/10" },
                new CatalogItem { CatalogTypeId=1,CatalogBrandId=2, Description = "All round", Name = "Inredeible", Price = 248.5M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/11" },
                new CatalogItem { CatalogTypeId=2,CatalogBrandId=1, Description = "Pricesless", Name = "London Sky", Price = 412, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/12" },
                new CatalogItem { CatalogTypeId=3,CatalogBrandId=3, Description = "You ar ethe star", Name = "Elequent", Price = 123, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/13" },
                new CatalogItem { CatalogTypeId=3,CatalogBrandId=2, Description = "A ring popular in the 16th and 17th century in Western Europe that was used as an engagement wedding ring", Name = "London Star", Price = 218.5M, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/14" },
                new CatalogItem { CatalogTypeId=3,CatalogBrandId=1, Description = "A floppy, bendable ring made out of links of metal", Name = "Paris Blues", Price = 312, PictureUrl = "http://externalcatalogbaseurltobereplaced/api/pic/15" }

                // About the PictureUrl property:
                // http://externalcatalogbaseurltobereplaced/api/pic/1
                // Will write an API that will figure out how to get this
                // picture when this is rendered in the UI
            };
        }

        // Now, before go back and run the powershell command (AddMigration)
        // Have to call our Seed() method from somewhere
        // Right after db is set up...


        // Why not call Seed() right after this line in Startup?
        //services.AddDbContext<CatalogContext>(options =>
        //        options.UseSqlServer(Configuration["ConnectionString"]));
        // Because these methods that we are running on the services object in
        // public void ConfigureServices(IServiceCollection services)
        // these are running in other threads (asynchronously)
        // Dependencies:
        // Create a CatalogContext to hook up to SQL server
        // But this might spin up a brand new docker container, a new vm
        // takes time to set up
        // Startup keeps moving on to set up all these dependencies
        // The AddDbContext<CatalogContext>() doesn't say async by it, but
        // know that when a method accepts a lambda/delegate as a param
        // that method usually kicks of a new thread

        // Need to find a place where we can know that our services are
        // set up and ready to use.
        // Go to Program.cs

    }
}
