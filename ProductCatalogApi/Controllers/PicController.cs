using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// From Dockerfile


// Attributes in square brackets
// Extra metadata on class, like schema

namespace ProductCatalogApi.Controllers
{
    // First attribute:
    // To get to this controller, the route must be api/pic
    // [controller] is replaced with name of the controller class
    // Class is called PicController, so the name of the controller is pic
    [Route("api/[controller]")]
    // To get to this class, you need to come through this route
    // Route "Specifies an attribute route on a controller."

    //Route("api/whatever")] this is valid but bad
    
    // Route is a part of the URL
    // App will be hosted on a port
    // http://localhost:whateverport/restofthepathhereistheroute
    //                              /api/pic
    //                              /thispartoftheroutespecifieswhichcontrollertogoto

    // Routing ability of dotnetcore handles this
    // Routers in the dotnet core listen on the port
    // They listen for all of the allowed routes


    // Why is the iconography of a firewall a brick wall and not a cement wall
    // Not everything needs to be sealed away from the internet
    // Each brick is like a port

    // Port 80 is a common port for web

    // Second attribute:
    // I am an Api
    [ApiController]
    // Each controller has a different responsibility 

    // Beyond those two attributes, PicController is just a normal class
    // that we can write methods on and each method you write becomes an api
    public class PicController : ControllerBase
    {
        // Lifecycle of any web application (MVC or otherwise):

            // Recall there is client and server (rather than all on one box as in UWP)
            // Note that the user of your website could be in Hawaii on a Mac
            // with no dotnet on it. All they need is a browser and an internet connection.
            // Client is lightweight browser with internet connection.
            // Go to URL, to server (godaddy)
            // when you buy a domain, you map an ipaddress to the domain
            // godaddy does the DNS resolution from domain name to ip address

            // server at ip address looks at the route
            // see that request is for "home"
            // controller renders the home page: gets all data need back
            // then (view) sends to client, that's the response
            // then the client sees the stuff on the page
            // only js, html and css on the client browser

        // Routes can be a GET route or a POST route

        // When user comes to homepage, that's a GET request
        // Generally, user not submitting any data in the form
        // No body of a message sent from a client


        // In a GET,
        // Everything sent to the server is through the URL
        // All params come in the form of part of the URL
        // (not in the body of the message)

        // POST request
        // A form to be filled out
        // Contact form, etc.
        // Fill out form and hit submit button
        // Same lifecycle happens
        // Now user is submitting data as a part of the body of the message
        // Server knows that there is some body that it needs to parse through
        // a json object
        // needs to process and send a response back

        // Difference between GET and POST:
        // GET sends no message body
        // POST does send a message body


        // POSTMAN
        // User of webpage wouldn't know what type of request (GET, POST, etc. being sent)
        // But as developer, need to know what request making on client's behalf

        // Two parts to this:
        // 1) Server (WebMVC controller or microservices controller?)
        // needs to know what type of request is coming in
        // Is it a GET, POST or other?
        // *Sever needs to mark APIs appropriately, with correct request type*

        // 2) When write views: js, or whatever is being executed on the client (web browser) side
        // When it sends request to server, it needs to send the right type of request
        // on the user's behalf
        // *in js, request needs to be sent with the correct request type*


        // On server-side code, we mark every api with its request type
        [HttpGet("{id}")]
        // HttpGet
        // "Identifies an action that supports the HttpGet method."

        // Recall since GET request, user will not send any request body.
        // All data will come through the URL

        // Route:
        // /api/pic/getimage (usual convention when multiple apis on one controller class)
        // /api/pic gets to the PicController class
        // there can be multiple apis within PicController
        // each method has its own name, which becomes the rest of the route

        // But wait, this method requires a parameter
        // Need the id of the image to render
        // Recall wat the PictureUrl properties look like in the seed data:
        // http://externalcatalogbaseurltobereplaced/api/pic/1

        // But wait, it's not /api/pic/getimage/1...why not?
        // Leaving off the /getimage means that whenever you get to
        // the PicController by going to /api/pic, you'll automatically
        // get routed to GetImage.
        // This makes GetImage the default route for PicController
        // If you try this with mutliple methods on the PicController,
        // you'll get an ambigious route error and it won't work.
        // Can only do this with one method per controller.

        // The id is part of the route, we need to extract it and give it to
        // method as a parameter

        // Looking at attribute again: [HttpGet("{id}")]
        // When you get a GET request, whatever is in the curly braces will be
        // extracted into a variable of name id. **Name and caps must match exactly.**
        // Parsing URL and feeding respective params.

        public IActionResult GetImage(int id)
        {
            // IActionResult
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult?view=aspnetcore-3.1
            // "Defines a contract that represents the result of an action method."

            // Job of GetImage:
            // Find the pic represented by id in the Pics folder on the server,
            // return it to the caller.

            // But wait.
            // We don't know where the wwwroot folder is.
            // Need Dependency Injection
            // Per Brillan's question in class: Could use an env var
            // ...but you don't need to.
            // Could have microservices controller inject the path in which this
            // service is deployed.
            // "Tell me where that deployed location is (in the linux vm)."
            // Then I'll look there for the wwwroot folder.
            // Tell me the IWebHostEnvironment it is hosted on
            // To recieve injection, modify the contstructor of the class
            // (added below GetImage() for notetaking purposes)

            // Get the path to wwwroot from the IWebHostEnvironment that
            // our PicController constructor got injected with

            // Per Sathis' question at the very end of classtime:
            // IWebHostEnvironment is one of the dependencies that
            // is automatically provided for you (just like IConfiguration
            // was provided automatically for you)
            // We needed to do the work to inject the DbContext in
            // Startup.ConfigureServices(), but IWebHostEnvironment and
            // IConfiguration are automatically injected for you by Startup

            var wwwrootPath = _env.WebRootPath;

            // Brillan's path construction solution:
            // var imagePath = $"{wwwrootPath}/Pics/Ring{id}.jpg";

            // QUESTION:
            // The pitch for Path.Combine is that "operations are performed in
            // a cross-platform manner." but, we already have whacks in the strings
            // At this rate, what is the advantage of using Path.Combine over simple
            // string interpolation (see my solution above)?
            // Is there a way to use Path.Combine() in a more compatible way
            // in this situation?
            var path = Path.Combine($"{wwwrootPath}/Pics/", $"Ring{id}.jpg");

            // Can we just return this path back to the user?
            // No, we are supposed to send them the image file itself, not the path
            // to the file. The method is called GetImage() not GetImagePath()

            // Need to send the image file back as a bunch of bytes.

            // Must fully resolve File class name since there is a name collision with base class ControllerBase

            var buffer = System.IO.File.ReadAllBytes(path);

            // Content-Type cheatsheet:
            // https://stackoverflow.com/questions/23714383/what-are-all-the-possible-values-for-http-content-type-header/48704300#48704300
            return File(buffer, "image/jpeg");
            // ControllerBase.File(Byte[], String) 
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.controllerbase.file?view=aspnetcore-3.1#Microsoft_AspNetCore_Mvc_ControllerBase_File_System_Byte___System_String_
            // "Returns a file with the specified fileContents as content (Status200OK), and the specified contentType as the Content-Type."
            // Returns type is FileContentResult:
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.filecontentresult?view=aspnetcore-3.1
            // "Represents an ActionResult that, when executed, will write a binary file to the response."
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.actionresult?view=aspnetcore-3.1

            // Inheritance
            // Object -> ActionResult -> FileResult -> FileContentResult
            // Also ActionResult : IActionResult
            // Therefore, FileContentResult implements IActionResult
            // (is convertable to that type)

            // Same as:
            // return this.File();
            // Since this class PicController : ControllerBase
            // and File() is an instance (non-static) method on ControllerBase

            // Presumably, someone else will call
            // IActionResult.ExecuteResultAsync(ActionContext)
            // on the FileContentResult object returned by GetImage()
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.iactionresult.executeresultasync?view=aspnetcore-3.1#Microsoft_AspNetCore_Mvc_IActionResult_ExecuteResultAsync_Microsoft_AspNetCore_Mvc_ActionContext_
            // "Executes the result operation of the actoin method asynchronously. This method is called by MVC to process the result of an action method."

            // QUESTION:
            // Do we call this from somewhere?
            // Maybe in our WebMVC app?
            // Does dotnet core or someone else call it for us?
            // Maybe this has already been answered in a recent class.



            // NOW,
            // Test this API
            // But in IIS again, because docker still not ready to use
            // Go to the browser window that opened and modify the url
            // remove the weatherforecast and do something like this:
            // http://localhost:52750/api/pic/1
            // It will bring up the blue ring

            // NOW TEST THROUGH POSTMAN
            // Postman is the industry standard way to test APIs
            // Test all of your APIs through Postman
            // (Use native Win app on local box)

            // While using IIS
            // Right click on project in Solution Explorer
            // Properties > Debug > Web Server Settings > AppUrl
            // Can even change it as long as the app isn't running
            // But this will not work once we start using docker

            // END: Class #12 3-8-20
        }

        private readonly IWebHostEnvironment _env;
        // private global var for this class
        // so that GetImage() can access

        // readonly, const, static

        // static, one shared instance for all instances of class

        // const must be assigned immediately
        // (also expression must be evaluatable at compile time)

        // readonly prevents repointing of the reference
        // only time can point a readonly reference member on a class
        // is in the class' constructor

        // const is static behind the scenes, but also prevents you from
        // changing the data

        // QUESTION:
        // CLARIFICATION:
        // ?? readonly is static behind the scenes too?
        // I don't think so.
        // Every instance of PicController can have its own readonly _env, right?
        // _env is an instance member that is assigned during the construction
        // of a PicController instance
        // Class #12 03-08-2020
        // 59:23
        // Also about how readonly seems nonsensical for a value type
        // (though the compiler seems to allow it...maybe because ref exists?)

        public PicController(IWebHostEnvironment env) : base()
        // inject me where is being hosted, I will go read off of there

        // (IHostingEnvironment is deprecated)
        {
            // IWebHostEnvironment
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment?view=aspnetcore-3.1
            // "Provides information about the web hosting environment an application
            // is running in."

            // store off the environment so that can use it in GetImage()
            _env = env;
        }

    }
}