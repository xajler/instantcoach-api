using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class ApiV1Controller : ControllerBase
    {
        private readonly IInstantCoachRepository _repository;

        public ApiV1Controller(IInstantCoachRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ListResult<InstantCoachList>), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> GetAsync(
            int skip = 0, int take = 10, bool showCompleted = false)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            var result = new ListResult<InstantCoachList>
            {
                Items = new List<InstantCoachList> { new InstantCoachList() },
                TotalCount = 0
            };
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(InstantCoachDb), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> GetAsync(int id)
        {
            InstantCoachDb result = await _repository.GetById(id);
            if (result == null) { return NotFound($"Not existing id: {id}"); }
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> PostAsync([FromBody] InstantCoachCreateClient data)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            return BadRequest();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PutAsync(int id,
            [FromBody] InstantCoachUpdateClient data)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            return NotFound();
        }

        [HttpPatch("{id:int}/completed")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PatchAsync(int id)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            return NotFound();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            return NotFound();
        }
    }
}