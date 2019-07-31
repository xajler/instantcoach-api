using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace Api
{
    public sealed class ApiVersioningErrorResponseProvider : DefaultErrorResponseProvider
    {
        public override IActionResult CreateResponse(ErrorResponseContext context)
        {
            var msg = "An API version is required, but was not specified. Please set HTTP Header 'X-Api-Version' with API version number.";

            if (context.ErrorCode == "ApiVersionUnspecified")
            {
                context = new ErrorResponseContext(
                    context.Request,
                    context.StatusCode,
                    context.ErrorCode,
                    msg,
                    context.MessageDetail);
            }

            return base.CreateResponse(context);
        }
    }
}