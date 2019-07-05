using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core;
using Core.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace Api.Controllers
{
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
                    _logger.LogInformation($"Status Code: {successStatusCode} Creted\nCreated Id:{id}\nURI: {uri}");
                    return Created($"{uri}/{id}", new CreatedId(id));
                case Status204NoContent:
                    _logger.LogInformation($"Status Code: {successStatusCode} NoContent");
                    return NoContent();
                default:
                    _logger.LogInformation($"Status Code: {successStatusCode} OK");
                    if (data == null) { return Ok(); }
                    return Ok(data);
            }
        }

        private ActionResult OnError(ErrorType error, int id)
        {
            switch (error)
            {
                case ErrorType.UnknownId:
                    _logger.LogInformation($"Status Code: {Status404NotFound} NotFound");
                    return NotFound($"Not existing id: {id}");
                default:
                    _logger.LogInformation($"Status Code: {Status400BadRequest} BadRequest");
                    return BadRequest("Invalid data or unable to store changes.");

            }
        }
    }
}