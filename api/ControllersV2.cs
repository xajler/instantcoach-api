using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core;
using Core.Services;
using Api.Controllers.Version1;

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
    public class ApiV2Controller : ApiV1Controller
    {
        public ApiV2Controller(ILogger<ApiV1Controller> logger, IInstantCoachService service)
            : base(logger, service)
        {
        }
    }
}