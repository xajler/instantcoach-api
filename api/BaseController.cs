using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core;
using Core.Models;
using static System.Console;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace Api.Controllers
{
    public class BaseController<T> : ControllerBase where T: ControllerBase
    {
        private readonly ILogger<T> _logger;

        public BaseController(ILogger<T> logger) => _logger = logger;

        protected ActionResult CreateResult(Result result, int successStatusCode, int id)
        {
            WriteLine($"Result is: {result}");
            WriteLine($"StatusCode is: {successStatusCode}");
            if (result.Success)
            {
                return OnSuccess(successStatusCode, id);
            }
            else
            {
                return OnError(result.Error, id);
            }
        }

        private ActionResult OnSuccess(int successStatusCode, int id)
        {
            switch (successStatusCode)
            {
                case Status201Created:
                    var uri = Config.ApiRoute.Replace("{version:apiVersion}", Config.ApiVersion1);
                    _logger.LogInformation($"Status Code: {successStatusCode} Creted\nCreated Id:{id}\nURI: {uri}");
                    return Created($"{uri}/{id}", id);
                case Status204NoContent:
                    _logger.LogInformation($"Status Code: {successStatusCode} NoContent");
                    return NoContent();
                default:
                    _logger.LogInformation($"Status Code: {successStatusCode} OK");
                    return Ok();
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