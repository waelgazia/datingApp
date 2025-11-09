using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

using DatingApp.API.helper;
using DatingApp.API.Globals;

namespace DatingApp.API.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(LogUserActivity))]
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Add X-Pagination header with the pagination metadata
        /// </summary>
        public void AddPaginationHeader<T>(PagedList<T> paginatedResult) where T : class
        {
            JsonSerializerOptions settings = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            Response.Headers.Append(
                HeaderNames.Pagination, JsonSerializer.Serialize(paginatedResult.PaginationMetadata, settings));
        }
    }
}
