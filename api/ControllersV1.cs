using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Core;
using Core.Models;
using Core.Contracts;
using static Microsoft.AspNetCore.Http.StatusCodes;


namespace Api.Controllers.Version1
{
    [Route(Config.ApiRoute)]
    [ApiVersion(Config.ApiVersion1)]
    [Produces(Config.ProducesJsonContent)]
    [ApiController]
    public class ApiV1Controller : BaseController<ApiV1Controller>
    {
        private readonly ILogger _logger;
        private readonly IInstantCoachService _service;

        public ApiV1Controller(ILogger<ApiV1Controller> logger,
            IInstantCoachService service)
            : base(logger)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResult<InstantCoachList>), Status200OK)]
        public async Task<IActionResult> GetAsync(CancellationToken cancellationToken,
            int skip = 0, int take = 10, bool showCompleted = false)
        {
            _logger.LogInformation("GetList params:\nskip: {skip}\ntake: {take}\nshowCompleted: {showCompleted}");
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
    }
}