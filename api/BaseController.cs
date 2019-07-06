using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Core;
using Core.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private readonly ILogger _logger;

        public BaseController(ILogger logger) => _logger = logger;

        protected ActionResult CreateResult<T>(Result<T> result, int successStatusCode, int id = 0)
        {
            return CreateResult(result, successStatusCode, id, result.Value);
        }

        protected ActionResult CreateResult(Result result, int successStatusCode, int id, object data = default)
        {
            if (result.Success)
            {
                return OnSuccess(successStatusCode, id, data);
            }
            else
            {
                return OnError(result.Error, id);
            }
        }

        private ActionResult OnSuccess(int successStatusCode, int id, object data)
        {
            switch (successStatusCode)
            {
                case Status201Created:
                    var uri = Config.ApiRoute.Replace("{version:apiVersion}", Config.ApiVersion1);
                    _logger.LogInformation("Status Code: {StatusCode} Creted\nCreated Id:{Id}\nURI: {Uri}",
                    successStatusCode, id, uri);
                    return Created($"{uri}/{id}", new CreatedId(id));
                case Status204NoContent:
                    _logger.LogInformation("Status Code: {StatusCode} NoContent", successStatusCode);
                    return NoContent();
                default:
                    _logger.LogInformation("Status Code: {StatusCode} OK", successStatusCode);
                    if (data == null) { return Ok(); }
                    return Ok(data);
            }
        }

        private ActionResult OnError(ErrorType error, int id)
        {
            switch (error)
            {
                case ErrorType.UnknownId:
                    _logger.LogInformation("Status Code: {StatusCode} NotFound", Status404NotFound);
                    return NotFound($"Not existing id: {id}");
                default:
                    _logger.LogInformation("Status Code: {StatusCode} BadRequest", Status400BadRequest);
                    return BadRequest("Invalid data or unable to store changes.");

            }
        }
    }
}