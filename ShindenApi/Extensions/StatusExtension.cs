using Shinden.Models;
using Shinden.Models.Entities;

namespace Shinden.Extensions
{
    public static class StatusExtension
    {
        public static IEmptyResponse ToModel(this API.Status mod)
        {
            return new EmptyResponse(mod.ResponseStatus == "ok" ? "Request successful." : "Request unsuccessful.");
        }
    }
}
