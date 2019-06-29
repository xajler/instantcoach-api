using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Core;
using Core.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Api.Controllers.Version1
{
    [Route(Config.ApiRoute)]
    [ApiVersion(Config.ApiVersion1)]
    [Produces(Config.ProducesJsonContent)]
    [ApiController]
    public class ApiV1Controller : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<InstantCoach>), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<IActionResult> GetAsync(
            int skip = 0, int take = 10, bool showCompleted = false)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            return Ok(new List<InstantCoach> { new InstantCoach() });
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(InstantCoach), Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> GetAsync(int id)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            return NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(InstantCoach), Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        public async Task<ActionResult> PostAsync([FromBody] InstantCoach model)
        {
            await Task.CompletedTask; // Dummy await, to supress compiler warnings
            return BadRequest();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        public async Task<ActionResult> PutAsync(int id, [FromBody] InstantCoach model)
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