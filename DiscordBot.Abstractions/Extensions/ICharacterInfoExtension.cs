using System.Collections.Generic;
using Discord;
using Sanakan.DiscordBot.Abstractions.Models;
using Sanakan.ShindenApi.Models;
using Sanakan.ShindenApi.Models.Enums;

namespace Sanakan.Extensions
{
    public static class ICharacterInfoExtension
    {
        public static Embed ToEmbed(this CharacterInfo info)
        {
            return new EmbedBuilder()
            {
                Title = $"{info} ({info.CharacterId})".ElipseTrimToLength(EmbedBuilder.MaxTitleLength),
                Description = info?.Biography?.Biography?.ElipseTrimToLength(1000),
                Color = EMType.Info.Color(),
                ImageUrl = info.PictureUrl,
                Fields = info.GetFields(),
                Url = info.CharacterUrl,
            }.Build();
        }

        public static List<EmbedFieldBuilder> GetFields(this CharacterInfo info)
        {
            var fields = new List<EmbedFieldBuilder>
            {
                new EmbedFieldBuilder
                {
                    Name = "Płeć",
                    Value = info.Gender.ToModel(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Wiek",
                    Value = info.Age!.GetQMarksIfEmpty(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Wzrost",
                    Value = info.Height.GetQMarksIfEmpty(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Waga",
                    Value = info.Weight.GetQMarksIfEmpty(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Grupa krwii",
                    Value = info.Bloodtype.GetQMarksIfEmpty(),
                    IsInline = true
                },
                new EmbedFieldBuilder
                {
                    Name = "Historyczna",
                    Value = info.IsReal ? "Tak" : "Nie",
                    IsInline = true
                }
            };

            if (info.Gender == Gender.Female)
            {
                fields.Add(new EmbedFieldBuilder()
                {
                    Name = "Biust",
                    Value = info.Bust.GetQMarksIfEmpty(),
                    IsInline = true
                });

                fields.Add(new EmbedFieldBuilder()
                {
                    Name = "Talia",
                    Value = info.Waist.GetQMarksIfEmpty(),
                    IsInline = true
                });

                fields.Add(new EmbedFieldBuilder()
                {
                    Name = "Biodra",
                    Value = info.Hips.GetQMarksIfEmpty(),
                    IsInline = true
                });
            }

            return fields;
        }

        public static string ToModel(this Gender gender)
        {
            switch (gender)
            {
                case Gender.Other: return "Helikopter bojowy";
                case Gender.Female: return "Kobieta";
                case Gender.Male: return "Mężczyzna";
                default: return "Homoniewiadomo";
            }
        }
    }
}