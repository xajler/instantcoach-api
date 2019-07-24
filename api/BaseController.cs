using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Core.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Core.Constants.Controller;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        private readonly ILogger _logger;

        protected BaseController(ILogger logger) => _logger = logger;

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

        protected void LogModelValidationErrors()
        {
            string errors = string.Join("; ", ModelState.Values
                                      .SelectMany(x => x.Errors)
                                      .Select(x => x.ErrorMessage));
            _logger.LogError("Validaton errors: {ValidationErrors}", errors);
        }

        private ActionResult OnSuccess(int successStatusCode, int id, object data)
        {
            switch (successStatusCode)
            {
                case Status201Created:
                    var uri = ApiRoute.Replace("{version:apiVersion}", ApiVersion1);
                    _logger.LogInformation("[Status Code]: {StatusCode} Created | [Created Id]: {Id} | [URI]: {Uri}",
                    successStatusCode, id, uri);
                    return Created($"{uri}/{id}", new CreatedId(id));
                case Status204NoContent:
                    _logger.LogInformation("[Status Code]: {StatusCode} NoContent", successStatusCode);
                    return NoContent();
                default:
                    _logger.LogInformation("[Status Code]: {StatusCode} OK", successStatusCode);
                    if (data == null) { return Ok(); }
                    return Ok(data);
            }
        }

        private ActionResult OnError(ErrorType error, int id)
        {
            if(error == ErrorType.UnknownId)
            {
                _logger.LogInformation("Status Code: {StatusCode} NotFound", Status404NotFound);
                return NotFound(new Msg { Message = $"Not existing id: {id}" });
            }
            else
            {
                _logger.LogInformation("Status Code: {StatusCode} BadRequest", Status400BadRequest);
                // TODO: show errors in response.
                return BadRequest(new Msg { Message ="Invalid data or unable to store changes." });
            }
        }
    }
}