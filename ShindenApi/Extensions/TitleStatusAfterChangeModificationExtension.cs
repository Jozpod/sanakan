using Shinden.Models;
using Shinden.Models.Entities;

namespace Shinden.Extensions
{
    public static class TitleStatusAfterChangeExtension
    {
        public static IEmptyResponse ToModel(this API.TitleStatusAfterChange mod)
        {
            return new EmptyResponse(mod.Status == "ok" ? "Update successful." : "Update unsuccessful.");
        }
    }
}
