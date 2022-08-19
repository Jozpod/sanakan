using Sanakan.ShindenApi.Models.Enums;
using System.Collections.Generic;

namespace Sanakan.ShindenApi.Converters
{
    public class PictureTypeConverter : EnumConverter<PictureType>
    {
        private static IDictionary<string, PictureType> _map = new Dictionary<string, PictureType>
        {
            { "image_picture", PictureType.Image },
        };

        public PictureTypeConverter()
            : base(_map, PictureType.NotSpecified)
        {
        }
    }
}