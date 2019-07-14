using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core;
using Core.Models;
using Core.Services;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Core.Helpers;

namespace Api.Controllers.Version1
{
    [Route(Config.ApiRoute)]
    [ApiVersion(Config.ApiVersion1)]
    [Produces(Config.ProducesJsonContent)]
    public class ApiV1Controller : BaseController
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

        /// <summary> Gets all InstantCoaches, paginated to 10 rows by default.</summary>
        /// <param name="skip">skip (page number).</param>
        /// <param name="take">
        /// Parameter take (page row number) requires an integer argument.
        /// Default value is 10.
        /// </param>
        /// <param name="showCompleted">
        /// When false shows all except 'Completed, when true, shows all.
        /// Shold be renamed to 'showAll', because by default (false) it shows all InstantCoachStatuses except 'Completed'.
        /// </param>
        [HttpGet]
        [ProducesResponseType(typeof(ListResult<InstantCoachList>), Status200OK)]
        public async Task<IActionResult> GetAsync( // CancellationToken cancellationToken,
            int skip = 0, int take = 10, bool showCompleted = false)
        {
            _logger.LogInformation(
                "GET List params:\nskip: {Skip}\ntake: {Take}\nshowCompleted: {ShowCompleted}",
                skip, take, showCompleted);
            var result = await _service.GetList(skip, take, showCompleted);
            return CreateResult(
                Result<ListResult<InstantCoachList>>.AsSuccess(result),
                successStatusCode: Status200OK);
        }

        /// <summary>Gets InstantCoach by Id.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(InstantCoachForId), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> GetAsync(int id)
        {
            _logger.LogInformation("GET by Id params:\nid: {Id}", id);
            var result = await _service.GetById(id);
            return CreateResult(result, successStatusCode: Status200OK, id);
        }

        /// <summary>Creates InstantCoach with Status from Config.InstantCoachStatusDefault.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedId), Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> PostAsync([FromBody] InstantCoachCreateClient data)
        {
            _logger.LogInformation("POST params:\ndata:\n{HttpBody}", ToLogJson(data));
            // TODO: Send errors, can only happen when any Enum is not set, and sent 0.
            if (!ModelState.IsValid) { return new BadRequestResult(); }
            var result = await _service.Create(data);
            int id = result.Value == null ? 0 : result.Value.Id;
            return CreateResult(result, successStatusCode: Status201Created, id: id);
        }

        // /// <summary>Updates InstantCoach for Id.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PutAsync(int id,
            [FromBody] InstantCoachUpdateClient data)
        {
            _logger.LogInformation("PUT params:\nid: {Id}\ndata:\n{HttpBody}", id, ToLogJson(data));
            // TODO: Send errors, can only happen when any Enum is not set, and sent 0.
            if (!ModelState.IsValid) { return new BadRequestResult(); }
            var result = await _service.Update(id, data);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        /// <summary>Marks InstantCoach as Completed for Id.</summary>
        [HttpPatch("{id:int}/completed")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PatchAsync(int id)
        {
            _logger.LogInformation("PATCH params:\nid: {Id}", id);
            var result = await _service.MarkCompleted(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        /// <summary>Deletes InstantCoach for Id.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation("DELETE params:\nid: {Id}", id);
            var result = await _service.Remove(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }
    }
}