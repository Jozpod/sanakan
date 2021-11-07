using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class StaffTypeConverter : EnumConverter<StaffType>
    {
        private static IDictionary<string, StaffType> _map = new Dictionary<string, StaffType>
        {
            { "colaboration", StaffType.Colaboration },
            { "company", StaffType.Company },
            { "person", StaffType.Person },
        };

        public StaffTypeConverter() : base(_map, StaffType.NotSpecified) {}
    }
}