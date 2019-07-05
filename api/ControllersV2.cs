using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core;
using Core.Models;
using Core.Contracts;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Core.Helpers;

namespace Api.Controllers.Version2
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
    //                    NOTE:                         //
    // Same as v1, introduced to test multiple versions //
    // Smainly testing and setinp up swagger            //
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
    [Route(Config.ApiRoute)]
    // Can omit as default version, uncomment on v3
    [ApiVersion(Config.ApiVersion2)]
    [Produces(Config.ProducesJsonContent)]
    [ApiController]
    public class ApiV2Controller : BaseController
    {
        private readonly ILogger _logger;
        private readonly IInstantCoachService _service;

        public ApiV2Controller(ILogger<ApiV2Controller> logger,
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
            _logger.LogInformation($"GET List params:\nskip: {skip}\ntake: {take}\nshowCompleted: {showCompleted}");
            var result = await _service.GetList(skip, take, showCompleted);
            return CreateResult(SuccessResult(result), successStatusCode: Status200OK);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(InstantCoach), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> GetAsync(int id)
        {
            _logger.LogInformation($"GET by Id params:\nid: {id}");
            var result = await _service.GetById(id);
            return CreateResult(result, successStatusCode: Status200OK, id);
        }

        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> PostAsync([FromBody] InstantCoachCreateClient data)
        {
            _logger.LogInformation($"POST params:\ndata:\n{ToLogJson(data)}");
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
            _logger.LogInformation($"PUT params:\nid: {id}\ndata:\n{ToLogJson(data)}");
            var result = await _service.Update(id, data);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        [HttpPatch("{id:int}/completed")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PatchAsync(int id)
        {
            _logger.LogInformation($"PATCH params:\nid: {id}");
            var result = await _service.MarkCompleted(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation($"DELETE params:\nid: {id}");
            var result = await _service.Remove(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }
    }
}