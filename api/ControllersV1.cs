using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Core;
using Core.Models;
using Core.Contracts;
using static System.Console;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace Api.Controllers.Version1
{
    [Route(Config.ApiRoute)]
    [ApiVersion(Config.ApiVersion1)]
    [Produces(Config.ProducesJsonContent)]
    [ApiController]
    public class ApiV1Controller : ControllerBase
    {
        private readonly IInstantCoachService _service;

        public ApiV1Controller(IInstantCoachService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResult<InstantCoachList>), Status200OK)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken,
            int skip = 0, int take = 10, bool showCompleted = false)
        {
            var result = await _service.GetList(skip, take, showCompleted);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(InstantCoachDb), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> GetAsync(int id)
        {
            var result = await _service.GetById(id);
            if (!result.Success) { return NotFound($"Not existing id: {id}"); }
            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> PostAsync([FromBody] InstantCoachCreateClient data)
        {
            var result = await _service.Create(data);
            return CreateResult(result,
                successStatusCode: Status201Created, id: result.Value);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PutAsync(int id,
            [FromBody] InstantCoachUpdateClient data)
        {
            var result = await _service.Update(id, data);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        [HttpPatch("{id:int}/completed")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PatchAsync(int id)
        {
            var result = await _service.MarkCompleted(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var result = await _service.Remove(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        private ActionResult CreateResult(Result result, int successStatusCode, int id)
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
                    return Created($"{uri}/{id}", id);
                case Status204NoContent:
                    WriteLine("its no contet");
                    return NoContent();
                default:
                    WriteLine("its just OK");
                    return Ok();
            }
        }

        private ActionResult OnError(ErrorType error, int id)
        {
            switch (error)
            {
                case ErrorType.UnknownId:
                    return NotFound($"Not existing id: {id}");
                default:
                    return BadRequest("Invalid data or unable to store changes.");

            }
        }
    }
}