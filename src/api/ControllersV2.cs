using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Core.Services;
using Api.Controllers.Version1;
using static Core.Constants.Controller;

namespace Api.Controllers.Version2
{
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
    //                    NOTE:                         //
    // Same as v1, introduced to test multiple versions //
    //              mainly for swagger                  //
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! //
    [Route(ApiRoute)]
    // Can omit as default version, uncomment on v3
    // [ApiVersion(ApiVersion2)]
    [Produces(ProducesJsonContent)]
    public sealed class ApiV2Controller : ApiV1Controller
    {
        public ApiV2Controller(ILogger<ApiV1Controller> logger, IInstantCoachService service)
            : base(logger, service)
        {
        }
    }
}