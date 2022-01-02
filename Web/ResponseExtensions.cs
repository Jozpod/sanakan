using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sanakan.Web
{
    [ExcludeFromCodeCoverage]
    public static class ResponseExtensions
    {
        public static ObjectResult BaseResponse(string str, int statusCode = StatusCodes.Status200OK) =>
            new ObjectResult(new ShindenPayload
            {
                Message = str,
                Success = true,
            })
            {
                StatusCode = statusCode,
            };

        public static IActionResult ShindenOk(string str) => BaseResponse(str);

        public static IActionResult ShindenRichOk(string str, params ulong[] ids)
        {
            return new ObjectResult(new RichPayload
            {
                Message = str,
                Success = true,
                Ids = ids,
            });
        }

        public static IActionResult ShindenRichOk(string str, ulong id)
        {
            return new ObjectResult(new RichPayload
            {
                Message = str,
                Success = true,
                Id = id,
            });
        }

        public static IActionResult ShindenBadRequest(string str) => BaseResponse(str, StatusCodes.Status400BadRequest);

        public static IActionResult ShindenNotFound(string str) => BaseResponse(str, StatusCodes.Status404NotFound);

        public static IActionResult ShindenMethodNotAllowed(string str) => BaseResponse(str, StatusCodes.Status405MethodNotAllowed);

        public static IActionResult ShindenForbidden(string str) => BaseResponse(str, StatusCodes.Status403Forbidden);

        public static IActionResult ShindenNotAcceptable(string str) => BaseResponse(str, StatusCodes.Status406NotAcceptable);

        public static IActionResult ShindenInternalServerError(string str) => BaseResponse(str, StatusCodes.Status500InternalServerError);

        public static IActionResult ShindenServiceUnavailable(string str) => BaseResponse(str, StatusCodes.Status503ServiceUnavailable);

        public static IActionResult ShindenUnauthorized(string str) => BaseResponse(str, StatusCodes.Status401Unauthorized);

        public class ShindenPayload
        {
            public string Message { get; set; } = string.Empty;

            public bool Success { get; set; }
        }

        private class RichPayload : ShindenPayload
        {
            public IEnumerable<ulong> Ids { get; set; } = null;

            public ulong Id { get; set; }
        }
    }
}
