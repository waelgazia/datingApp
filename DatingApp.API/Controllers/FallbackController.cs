using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers;

/*
  The difference between this controller and API controller is that
  this is a Controller is a base class for an MVC controller with a view
  support (meaning it can return an HTML template)
*/
public class FallbackController : Controller
{
    /// <summary>
    /// Any routes that the API server does not know about will be passed to
    /// the Angular app, which is contained in index.html
    /// </summary>
    public ActionResult Index()
    {
        string indexPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "index.html");
        return PhysicalFile(indexPath, "text/HTML");
    }
}
