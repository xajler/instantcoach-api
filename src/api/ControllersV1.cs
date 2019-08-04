using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Schema;
using Domain;
using Core.Models;
using Core.Services;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Core.Constants.Controller;

namespace Api.Controllers.Version1
{
    [Route(ApiRoute)]
    [ApiVersion(ApiVersion1)]
    [Produces(ProducesJsonContent)]
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
        /// Should be renamed to 'showAll', because by default (false) it shows all InstantCoachStatuses except 'Completed'.
        /// </param>
        [HttpGet]
        [ProducesResponseType(typeof(ListResult<InstantCoachList>), Status200OK)]
        public async Task<IActionResult> GetAsync( // CancellationToken cancellationToken,
            int skip = 0, int take = 10, bool showCompleted = false)
        {
            _logger.LogInformation(
                "GET List params [skip]: {Skip} | [take]: {Take} | [showCompleted]: {ShowCompleted}",
                skip, take, showCompleted);
            if (skip < 0) { return BadRequest("The 'skip' value must be greater or equal 0."); }
            if (take <= 0) { return BadRequest("The 'take' value must be greater than 0."); }
             var result = await _service.GetList(skip, take, showCompleted);
            return CreateResult(
                Result<ListResult<InstantCoachList>>.AsSuccess(result),
                successStatusCode: Status200OK);
        }

        /// <summary>Gets InstantCoach by Id.</summary>
        [HttpGet("{id:int:min(1)}")]
        [ProducesResponseType(typeof(InstantCoachForId), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> GetAsync(int id)
        {
            _logger.LogInformation("GET by Id params [id]: {Id}", id);
            var result = await _service.GetById(id);
            return CreateResult(result, successStatusCode: Status200OK, id);
        }

        /// <summary>Gets JSON schema for Create or Update.</summary>
        [HttpGet("schema/{schemaType}")]
        [ProducesResponseType(typeof(JSchema), Status200OK)]
        [ProducesResponseType(Status400BadRequest)]
        public ActionResult GetAsync(string schemaType)
        {
            var result = _service.GetJsonSchema(schemaType);
            if (result.Success) { return Ok(result.Value); }
            return BadRequest($"Valid schema types are: '{SchemaCreate}' or '{SchemaUpdate}'!");
        }

        /// <summary>Creates InstantCoach with Status from Config.InstantCoachStatusDefault.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(CreatedId), Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> PostAsync([FromBody] InstantCoachCreateClient data)
        {
            _logger.LogInformation("POST params [data]: {@HttpBody}", data);
            if (!ModelState.IsValid) { return ReturnBadRequestWithErrors(); }
            var result = await _service.Create(data);
            int id = result.Value == null ? 0 : result.Value.Id;
            return CreateResult(result, successStatusCode: Status201Created, id: id);
        }

        // /// <summary>Updates InstantCoach for Id.</summary>
        [HttpPut("{id:int:min(1)}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PutAsync(int id,
            [FromBody] InstantCoachUpdateClient data)
        {
            _logger.LogInformation("PUT params [id]: {Id} | [data]: {@HttpBody}", id, data);
            if (!ModelState.IsValid) { return ReturnBadRequestWithErrors(); }
            var result = await _service.Update(id, data);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        /// <summary>Marks InstantCoach as Completed for Id.</summary>
        [HttpPatch("{id:int:min(1)}/completed")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PatchAsync(int id)
        {
            _logger.LogInformation("PATCH params [id]: {Id}", id);
            var result = await _service.MarkCompleted(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }

        /// <summary>Deletes InstantCoach for Id.</summary>
        [HttpDelete("{id:int:min(1)}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            _logger.LogInformation("DELETE params [id]: {Id}", id);
            var result = await _service.Remove(id);
            return CreateResult(result, successStatusCode: Status204NoContent, id);
        }
    }
}