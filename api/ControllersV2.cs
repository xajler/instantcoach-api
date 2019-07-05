using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core;
using Core.Models;
using Core.Contracts;
using Api.Controllers.Version1;
using static Microsoft.AspNetCore.Http.StatusCodes;
using static Core.Helpers;

namespace Api.Controllers.Version2
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
    //                    NOTE:                         //
    // Same as v1, introduced to test multiple versions //
    //                 mainly swagger                   //
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
    [Route(Config.ApiRoute)]
    // Can omit as default version, uncomment on v3
    // [ApiVersion(Config.ApiVersion2)]
    [Produces(Config.ProducesJsonContent)]
    [ApiController]
    public class ApiV2Controller : ApiV1Controller
    {
        public ApiV2Controller(ILogger<ApiV1Controller> logger, IInstantCoachService service)
            : base(logger, service)
        {
        }
    }
}