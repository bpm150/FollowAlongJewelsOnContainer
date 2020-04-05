using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProductCatalogApi.Data;
using ProductCatalogApi.Domain;
using ProductCatalogApi.ViewModel;

namespace ProductCatalogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        // Dependency Injection to access db context
        // Recall that we did not need db access for PicController
        // Only needed file system access (IWebHostEnvironment injection) for that
        private readonly CatalogContext _context;

        private readonly IConfiguration _config;

        public CatalogController(CatalogContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // Need to query the CatalogItems table 

        // Almost all apis should be async (all apis that are called from the front end)
        // Async: Don't want to block your webui.
        // Don't want to block your front end from accessing your api back end.

            // Items is async because it has an await inside it

        [HttpGet] // <- no uri params defined here.
                  //PicController used these: [HttpGet("{id}")]
        //[Route("[action]/{pageIndex}/{pageSize}")] // <- using route instead of uri
        [Route("[action]")]
        // [action] == name of api method, "items" in this case
        // TODO: Finish the query example below
        // which makes the route, /api/catalog/items/?pageIndex=1&pageSize=10
        public async Task<IActionResult> Items( 
            [FromQuery]int pageIndex = 0, // Attributes can be on method params, too
            [FromQuery]int pageSize = 6)
        {
            // Pagination is the job of both the back end (to serve
            // the correct amount of data) and the front end (to display
            // the data correctly)

            // Allowing the caller to pass in simple ints as params to this method
            // is more robust than requiring them to create something like a
            // page object and passing it by json, etc.
            // More opportunities to make a mistake and for things to break




            // QUERY DB CONTEXT FOR THE DATA

            // (a)wait for the secondary thread to finish, then store the result
            var items = await _context.CatalogItems // this await is the need for Items() (this method) to be async
                .Skip(pageIndex * pageSize)
                .Take(pageSize) // Construct the query
                .ToListAsync(); // Execute the query
            // _context is a CatalogContext
            // CatalogItems is a DbSet<CatalogItem>
            // CatalogItem is the model class for each individual item
            // items is a List<CatalogItem>

            // Replace externalcatalogbaseurltobereplaced with actual domain
            items = ChangePictureUrl(items);

            // Also neat trick with writing a method that takes a known object
            // (and optionally its result is assigned to a known object)
            // Then visual studio will know how to write the function prototype
            // for you

            // Example class for Ainur
            //var count = await EntityFrameworkQueryableExtensions.LongCountAsync<CatalogItem>(_context.CatalogItems);


            var viewModel = new PaginatedItemsViewModel<CatalogItem>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,



            // We have to query the database again 
            ItemCount = await _context.CatalogItems.LongCountAsync(),

                //EntityFrameworkQueryableExtensions.LongCountAsync<TSource>(this IQueryable<TSource>, CancellationToken),
                // So:
                // IQueryable<CatalogItem>.LongCountAsync<CatalogItem>()
                //"Asynchronously returns an Int64 that represents the total number of elements in a sequence."
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.entityframeworkqueryableextensions.longcountasync?view=efcore-3.1#Microsoft_EntityFrameworkCore_EntityFrameworkQueryableExtensions_LongCountAsync__1_System_Linq_IQueryable___0__System_Linq_Expressions_Expression_System_Func___0_System_Boolean___System_Threading_CancellationToken_

                // QUESTION:
                // LongCountAsync<>() doesn't return a long, what's with the name?

                // DbSet implements IQueryable<TEntity>
                // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbset-1?view=efcore-3.1

                // Whoopsie! I'm thinking too far ahead.
                // Page count is a concern of the view (WebMVC)
                // This is supposed to be ItemCount
                // XX PageCount = (long)Math.Ceiling(
                // XX           _context.CatalogItems.Count() / (decimal)pageSize),
                // Ceiling() is decimal or double, cast one int to match the decimal version
                // Convert to long for assignment to long property
                // Also forgot to await since querying the database
                // QUESTION:
                // Also what happens when you call Count() on the db?
                // Count is not async, so what happens?
                // Blocks the thread

                Data = items
                // Number of items in the catalog divided by page size
                // rounded up to the next integer
            };// NOW return viewModel instead of items  END: Class #13 3-14-20

            // Now we have the page of data that the caller requested
            return Ok(viewModel);
            // OLD: return Ok(items); (from before PaginatedItemsViewModel)
            // OLD: Same as this.Ok(items); (from before PaginatedItemsViewModel)
            // ControllerBase.Ok(Object)
            // "Creates an OkObjectResult object that produces an Status200OK response."
            //https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.ok?view=aspnetcore-3.1#Microsoft_AspNetCore_Mvc_ControllerBase_Ok


            // Inheritance
            // Object -> ActionResult -> ObjectResult -> OkObjectResult
            // Also ActionResult : IActionResult
            // Therefore, OkObjectResult implements IActionResult
            // (is convertable to that type)


            // NOW:
            // Test in Postman
            // Remember to make sure that the correct request type is selected:
            // GET, POST, etc.


            // Back when was just:
            // return Ok(items);
            // ...the pic urls still had the placeholder in them:
            // "http://externalcatalogbaseurltobereplaced/api/pic/1"
            // Could not fix by changing it to in the db
            // http://localhost:52750/api/pic/1
            // would have to keep going back to the db and updating the records
            // with new port number.
            // Do the conversion here in Items(), before returning items
        }

        private List<CatalogItem> ChangePictureUrl(List<CatalogItem> items)
        {
            // Do a linq query to replace the "externalcatalogbaseurltobereplaced" in
            // "http://externalcatalogbaseurltobereplaced/api/pic/1"
            // with whatever "localhost:52750" is at the moment

            // Note that this is *NOT* the same as this path lookup/creation
            // in PicController.GetImage():
            // var path = Path.Combine($"{wwwrootPath}/Pics/", $"Ring{id}.jpg");
            // Result:
            // "C:\\Users\\Brillan\\Code\\FollowAlongJewelsOnContainer\\ProductCatalogApi\\wwwroot/Pics/Ring1.jpg"

            // Store the base url in appsettings.json
            // TO appsettings.json
            // "ExternalCatalogBaseUrl": "http://localhost:52750/"

            // Can access this through the IConfiguration dependency that is available
            // to all class constructors. (IWebHostEnvironment is another dependency
            // that is available to all class constructors, PicController uses it)

            // Add IConfiguration DI to CatalogController constructor
            items.ForEach(i => i.PictureUrl = i.PictureUrl.
                        Replace("http://externalcatalogbaseurltobereplaced",
                        _config["ExternalCatalogBaseUrl"]));
            // String.Replace(String, String)
            // https://docs.microsoft.com/en-us/dotnet/api/system.string.replace?view=netcore-3.1#System_String_Replace_System_String_System_String_
            //"Returns a new string in which all occurrences of a specified string in the current instance are replaced with another specified string."
            // Note: Replace() does NOT mutate the invoking string.
            // In fact, it cannot. Strings in C# are always immutable.
            // The two params are the string to search for
            // "oldValue" and the string to replace it with, "newValue"

            // When an object (_config) refers to the IConfigration object
            // that is read in from the appsettings.json
            // Treat the object as a dictionary and use array subscript notation
            // to look up the value by key:
            // _config["Key"]


            // Once the app is deployed, the port cannot change
            // Have to specify the port somewhere.
            // Yes, someone has to update the ExternalCatalogBaseUrl in appsettings.json
            // if the port number changes

            return items;


            // QUESTION:
            // CLARIFICATION:
            // To get DI, a class must:
            // (a) inherit from an appropriate EntityFrameworkCore or ASP.NET Core class
            // What are all the relevant choices for this?
            // (b) accept an eligible dependency class object into its constructor
            // IConfiguration and IWebHostEnvironment are two that are automatically
            // provided. Are any others automatically provided?
            // Recall that we needed to set up the CatalogContext DI
            // in Setup.ConfigureServices()


            // NOW,
            // To ViewModel\PaginatedItemsViewModel
            //     also give more info about data that you are sending
            // things that the view needs to know in order to be able to render properly
            // page count, page index and page size (even though client passed
            // those as parameters) also the data itself
        }
    }
}