using Shinden.Models;
using Shinden.Models.Entities;

namespace Shinden.Extensions
{
    public static class ModificationExtension
    {
        public static IEmptyResponse ToModel(this API.Modification mod)
        {
            long.TryParse(mod?.Updated, out var res);

            return new EmptyResponse(res == 1 ? "Update successful." : "Update unsuccessful.");
        }
    }
}
