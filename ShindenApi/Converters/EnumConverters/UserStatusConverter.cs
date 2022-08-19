using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class UserStatusConverter : EnumConverter<UserStatus>
    {
        private static IDictionary<string, UserStatus> _map = new Dictionary<string, UserStatus>
        {
            { "active", UserStatus.Active },
        };

        public UserStatusConverter()
            : base(_map, UserStatus.NotSpecified)
        {
        }
    }
}