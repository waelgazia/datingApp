using Microsoft.AspNetCore.Mvc;

using DatingApp.API.Base;

namespace DatingApp.API.Controllers
{
    public class BuggyController : BaseApiController
    {
        [HttpGet("not-authorized")]
        public IActionResult GetNotAuthorized()
        {
            return Unauthorized();
        }

        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            return BadRequest();
        }

        [HttpGet("server-error")]
        public IActionResult GetServerError()
        {
            throw new Exception("This is a server error!");
        }
    }
}
