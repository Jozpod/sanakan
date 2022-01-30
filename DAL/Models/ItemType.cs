using System;
using System.Collections.Generic;

namespace Sanakan.DAL.Models
{
    /// <summary>
    /// Describes items which can be bought from shop and then consumed by given card.
    /// </summary>
    public enum ItemType : byte
    {
        AffectionRecoverySmall = 0,
        AffectionRecoveryNormal = 1,
        AffectionRecoveryBig = 2,
        IncreaseUpgradeCount = 3,
        CardParamsReRoll = 4,
        DereReRoll = 5,
        RandomBoosterPackSingleE = 6,
        RandomNormalBoosterPackB = 7,
        RandomTitleBoosterPackSingleE = 8,
        RandomNormalBoosterPackA = 9,
        RandomNormalBoosterPackS = 10,
        RandomNormalBoosterPackSS = 11,
        AffectionRecoveryGreat = 12,

        /// <summary>
        /// Blood samples.
        /// </summary>
        BetterIncreaseUpgradeCnt = 13,
        CheckAffection = 14,
        SetCustomImage = 15,
        IncreaseExpSmall = 16,
        IncreaseExpBig = 17,
        ChangeStarType = 18,
        SetCustomBorder = 19,
        ChangeCardImage = 20,

        PreAssembledMegumin = 21,
        PreAssembledGintoki = 22,
        PreAssembledAsuna = 23,

        FigureSkeleton = 24,
        FigureUniversalPart = 25,
        FigureHeadPart = 26,
        FigureBodyPart = 27,
        FigureLeftArmPart = 28,
        FigureRightArmPart = 29,
        FigureLeftLegPart = 30,
        FigureRightLegPart = 31,
        FigureClothesPart = 32,

        BigRandomBoosterPackE = 33,
        ResetCardValue = 34,
    }

    public static class ItemTypeExtensions
    {
        public static ItemType RandomizeItemFromMarket(int number) => number switch
        {
            var _ when number < 2 => ItemType.IncreaseExpSmall,
            var _ when number < 15 => ItemType.IncreaseUpgradeCount,
            var _ when number < 80 => ItemType.AffectionRecoveryBig,
            var _ when number < 145 => ItemType.CardParamsReRoll,
            var _ when number < 230 => ItemType.DereReRoll,
            var _ when number < 480 => ItemType.AffectionRecoveryNormal,
            _ => ItemType.AffectionRecoverySmall,
        };

        public static ItemType RandomizeItemFromBlackMarket(int number) => number switch
        {
            var _ when number < 2 => ItemType.IncreaseExpSmall,
            var _ when number < 12 => ItemType.BetterIncreaseUpgradeCnt,
            var _ when number < 25 => ItemType.IncreaseUpgradeCount,
            var _ when number < 70 => ItemType.AffectionRecoveryGreat,
            var _ when number < 120 => ItemType.AffectionRecoveryBig,
            var _ when number < 180 => ItemType.CardParamsReRoll,
            var _ when number < 250 => ItemType.DereReRoll,
            var _ when number < 780 => ItemType.AffectionRecoveryNormal,
            _ => ItemType.AffectionRecoverySmall,
        };

        public static Figure? ToPAFigure(this ItemType type, DateTime date)
        {
            if (!type.IsPreAssembledFigure())
            {
                return null;
            }

            var pas = type.ToPASType();

            return new Figure
            {
                PAS = pas,
                ExperienceCount = 0,
                PartExp = 0,
                Health = 300,
                RestartCount = 0,
                IsFocus = false,
                IsComplete = false,
                Dere = Dere.Yandere,
                Title = pas.GetTitleName(),
                BodyQuality = Quality.Alpha,
                HeadQuality = Quality.Alpha,
                Name = pas.GetCharacterName(),
                CompletedOn = date,
                FocusedPart = FigurePart.Head,
                ClothesQuality = Quality.Alpha,
                LeftArmQuality = Quality.Alpha,
                LeftLegQuality = Quality.Alpha,
                RightArmQuality = Quality.Alpha,
                RightLegQuality = Quality.Alpha,
                SkeletonQuality = Quality.Alpha,
                Character = pas.GetCharacterId(),
                Attack = Rarity.SSS.GetAttackMin(),
                Defence = Rarity.SSS.GetDefenceMin(),
            };
        }

        public static bool HasDifferentQualities(this ItemType type) => type switch
        {
            ItemType.AffectionRecoveryGreat
                or ItemType.AffectionRecoveryBig
                or ItemType.AffectionRecoveryNormal
                or ItemType.AffectionRecoverySmall
                or ItemType.IncreaseExpSmall
                or ItemType.IncreaseExpBig
                or ItemType.FigureSkeleton
                or ItemType.FigureUniversalPart
                or ItemType.FigureHeadPart
                or ItemType.FigureBodyPart
                or ItemType.FigureClothesPart
                or ItemType.FigureLeftArmPart
                or ItemType.FigureLeftLegPart
                or ItemType.FigureRightArmPart
                or ItemType.FigureRightLegPart => true,
            _ => false,
        };

        public static long CValue(this ItemType type) => type switch
        {
            ItemType.AffectionRecoveryGreat => 180,
            ItemType.AffectionRecoveryBig => 140,
            ItemType.AffectionRecoveryNormal => 15,
            ItemType.BetterIncreaseUpgradeCnt => 500,
            ItemType.IncreaseUpgradeCount => 200,
            ItemType.DereReRoll => 10,
            ItemType.CardParamsReRoll => 15,
            ItemType.CheckAffection => 15,
            ItemType.SetCustomImage => 300,
            ItemType.IncreaseExpSmall => 100,
            ItemType.IncreaseExpBig => 400,
            ItemType.ChangeStarType => 50,
            ItemType.SetCustomBorder => 80,
            ItemType.ChangeCardImage => 10,
            ItemType.ResetCardValue => 5,
            _ => 1,
        };

        public static bool IsBoosterPack(this ItemType type) => type switch
        {
            ItemType.RandomBoosterPackSingleE
                or ItemType.RandomTitleBoosterPackSingleE
                or ItemType.RandomNormalBoosterPackB
                or ItemType.RandomNormalBoosterPackA
                or ItemType.RandomNormalBoosterPackS
                or ItemType.RandomNormalBoosterPackSS
                or ItemType.BigRandomBoosterPackE => true,
            _ => false,
        };

        public static bool IsPreAssembledFigure(this ItemType type) => type switch
        {
            ItemType.PreAssembledAsuna
                or ItemType.PreAssembledGintoki
                or ItemType.PreAssembledMegumin => true,
            _ => false,
        };

        public static uint Count(this ItemType type) => type switch
        {
            ItemType.RandomNormalBoosterPackB
                or ItemType.RandomNormalBoosterPackA
                or ItemType.RandomNormalBoosterPackS
                or ItemType.RandomNormalBoosterPackSS => 3,
            ItemType.BigRandomBoosterPackE => 20,
            _ => 2,
        };

        public static Rarity MinRarity(this ItemType type) => type switch
        {
            ItemType.RandomNormalBoosterPackSS => Rarity.SS,
            ItemType.RandomNormalBoosterPackS => Rarity.S,
            ItemType.RandomNormalBoosterPackA => Rarity.A,
            ItemType.RandomNormalBoosterPackB => Rarity.B,
            _ => Rarity.E,
        };

        public static bool IsTradable(this ItemType type) => type switch
        {
            ItemType.RandomTitleBoosterPackSingleE => false,
            _ => true,
        };

        public static CardSource GetSource(this ItemType type) => type switch
        {
            ItemType.RandomBoosterPackSingleE
                or ItemType.RandomNormalBoosterPackB
                or ItemType.RandomNormalBoosterPackA
                or ItemType.RandomNormalBoosterPackS
                or ItemType.RandomNormalBoosterPackSS
                or ItemType.RandomTitleBoosterPackSingleE
                or ItemType.BigRandomBoosterPackE => CardSource.Shop,
            _ => CardSource.Other,
        };

        public static bool CanUseWithoutCard(this ItemType type) => type switch
        {
            ItemType.FigureHeadPart
                or ItemType.FigureBodyPart
                or ItemType.FigureClothesPart
                or ItemType.FigureLeftArmPart
                or ItemType.FigureLeftLegPart
                or ItemType.FigureRightArmPart
                or ItemType.FigureRightLegPart
                or ItemType.FigureUniversalPart => true,
            _ => false,
        };

        public static double BaseAffection(this ItemType type) => type switch
        {
            ItemType.AffectionRecoveryGreat => 1.6,
            ItemType.AffectionRecoveryBig => 1,
            ItemType.AffectionRecoveryNormal => 0.12,
            ItemType.AffectionRecoverySmall => 0.02,
            ItemType.BetterIncreaseUpgradeCnt => 1.7,
            ItemType.IncreaseUpgradeCount => 0.7,
            ItemType.DereReRoll => 0.1,
            ItemType.CardParamsReRoll => 0.2,
            ItemType.CheckAffection => 0.2,
            ItemType.SetCustomImage => 0.5,
            ItemType.IncreaseExpSmall => 0.15,
            ItemType.IncreaseExpBig => 0.25,
            ItemType.ChangeStarType => 0.3,
            ItemType.SetCustomBorder => 0.4,
            ItemType.ChangeCardImage => 0.1,
            ItemType.ResetCardValue => 0.1,
            _ => 0,
        };

        public static string Desc(this ItemType type) => type switch
        {
            ItemType.AffectionRecoveryGreat => "Poprawia relacje z kartą w dużym stopniu.",
            ItemType.AffectionRecoveryBig => "Poprawia relacje z kartą w znacznym stopniu.",
            ItemType.AffectionRecoveryNormal => "Poprawia relacje z kartą.",
            ItemType.BetterIncreaseUpgradeCnt => "Może zwiększyć znacznie liczbę ulepszeń karty, tylko kto by chciał twoją krew?",
            ItemType.IncreaseUpgradeCount => "Dodaje dodatkowy punkt ulepszenia do karty.",
            ItemType.DereReRoll => "Pozwala zmienić charakter karty.",
            ItemType.CardParamsReRoll => "Pozwala wylosować na nowo parametry karty.",
            ItemType.RandomBoosterPackSingleE => "Dodaje nowy pakiet z dwiema losowymi kartami.\n\nWykluczone jakości to: SS, S i A.",
            ItemType.BigRandomBoosterPackE => "Dodaje nowy pakiet z dwudziestoma losowymi kartami.\n\nWykluczone jakości to: SS, S i A.",
            ItemType.RandomTitleBoosterPackSingleE => "Dodaje nowy pakiet z dwiema losowymi, niewymienialnymi kartami z tytułu podanego przez kupującego.\n\nWykluczone jakości to: SS i S.",
            ItemType.AffectionRecoverySmall => "Poprawia odrobinę relacje z kartą.",
            ItemType.RandomNormalBoosterPackB => "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości B.\n\nWykluczone jakości to: SS.",
            ItemType.RandomNormalBoosterPackA => "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości A.\n\nWykluczone jakości to: SS.",
            ItemType.RandomNormalBoosterPackS => "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości S.\n\nWykluczone jakości to: SS.",
            ItemType.RandomNormalBoosterPackSS => "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości SS.",
            ItemType.CheckAffection => "Pozwala sprawdzić dokładny poziom relacji z kartą.",
            ItemType.SetCustomImage => "Pozwala ustawić własny obrazek karcie. Zalecany wymiary 448x650.",
            ItemType.IncreaseExpSmall => "Dodaje odrobinę punktów doświadczenia do karty.",
            ItemType.IncreaseExpBig => "Dodaje punkty doświadczenia do karty.",
            ItemType.ChangeStarType => "Pozwala zmienić typ gwiazdek na karcie.",
            ItemType.SetCustomBorder => "Pozwala ustawić ramkę karcie kiedy jest wyświetlana w profilu.",
            ItemType.ChangeCardImage => "Pozwala wybrać inny obrazek z shindena.",
            ItemType.PreAssembledAsuna or ItemType.PreAssembledGintoki or ItemType.PreAssembledMegumin => "Gotowy szkielet nie wymagający użycia karty SSS.",
            ItemType.FigureSkeleton => $"Szkielet pozwalający rozpoczęcie tworzenia figurki.",
            ItemType.FigureUniversalPart => $"Uniwersalna część, którą można zamontować jako dowolną część ciała figurki.",
            ItemType.FigureHeadPart => $"Część, którą można zamontować jako głowę figurki.",
            ItemType.FigureBodyPart => $"Część, którą można zamontować jako tułów figurki.",
            ItemType.FigureClothesPart => $"Część, którą można zamontować jako ciuchy figurki.",
            ItemType.FigureLeftArmPart => $"Część, którą można zamontować jako lewą rękę figurki.",
            ItemType.FigureLeftLegPart => $"Część, którą można zamontować jako lewą nogę figurki.",
            ItemType.FigureRightArmPart => $"Część, którą można zamontować jako prawą rękę figurki.",
            ItemType.FigureRightLegPart => $"Część, którą można zamontować jako prawą nogę figurki.",
            ItemType.ResetCardValue => $"Resetuje warość karty do początkowego poziomu.",
            _ => "Brak opisu.",
        };

        public static string ToString(this ItemType type, string? quality = null)
        {
            if (!string.IsNullOrEmpty(quality))
            {
                quality = $" {quality}";
            }

            return type switch
            {
                ItemType.AffectionRecoveryGreat => $"Wielka fontanna czekolady{quality}",
                ItemType.AffectionRecoveryBig => $"Tort czekoladowy{quality}",
                ItemType.AffectionRecoveryNormal => $"Ciasto truskawkowe{quality}",
                ItemType.BetterIncreaseUpgradeCnt => "Kropla twojej krwi",
                ItemType.IncreaseUpgradeCount => "Pierścionek zaręczynowy",
                ItemType.DereReRoll => "Bukiet kwiatów",
                ItemType.CardParamsReRoll => "Naszyjnik z diamentem",
                ItemType.RandomBoosterPackSingleE => "Tani pakiet losowych kart",
                ItemType.BigRandomBoosterPackE => "Może i nie tani ale za to duży pakiet kart",
                ItemType.RandomTitleBoosterPackSingleE => "Pakiet losowych kart z tytułu",
                ItemType.AffectionRecoverySmall => $"Banan w czekoladzie{quality}",
                ItemType.RandomNormalBoosterPackB => "Fioletowy pakiet losowych kart",
                ItemType.RandomNormalBoosterPackA => "Pomarańczowy pakiet losowych kart",
                ItemType.RandomNormalBoosterPackS => "Złoty pakiet losowych kart",
                ItemType.RandomNormalBoosterPackSS => "Różowy pakiet losowych kart",
                ItemType.CheckAffection => "Kryształowa kula",
                ItemType.SetCustomImage => "Skalpel",
                ItemType.IncreaseExpSmall => $"Mleko truskawkowe{quality}",
                ItemType.IncreaseExpBig => $"Gorąca czekolada{quality}",
                ItemType.ChangeStarType => "Stempel",
                ItemType.SetCustomBorder => "Nożyczki",
                ItemType.ChangeCardImage => "Plastelina",
                ItemType.PreAssembledAsuna => "Szkielet Asuny (SAO)",
                ItemType.PreAssembledGintoki => "Szkielet Gintokiego (Gintama)",
                ItemType.PreAssembledMegumin => "Szkielet Megumin (Konosuba)",
                ItemType.FigureSkeleton => $"Szkielet{quality}",
                ItemType.FigureUniversalPart => $"Uniwersalna część figurki{quality}",
                ItemType.FigureHeadPart => $"Głowa figurki{quality}",
                ItemType.FigureBodyPart => $"Tułów figurki{quality}",
                ItemType.FigureClothesPart => $"Ciuchy figurki{quality}",
                ItemType.FigureLeftArmPart => $"Lewa ręka{quality}",
                ItemType.FigureLeftLegPart => $"Lewa noga{quality}",
                ItemType.FigureRightArmPart => $"Prawa ręka{quality}",
                ItemType.FigureRightLegPart => $"Prawa noga{quality}",
                ItemType.ResetCardValue => $"Marker",
                _ => "Brak",
            };
        }

        public static FigurePart GetPartType(this ItemType type) => type switch
        {
            ItemType.FigureUniversalPart => FigurePart.All,
            ItemType.FigureHeadPart => FigurePart.Head,
            ItemType.FigureBodyPart => FigurePart.Body,
            ItemType.FigureClothesPart => FigurePart.Clothes,
            ItemType.FigureLeftArmPart => FigurePart.LeftArm,
            ItemType.FigureLeftLegPart => FigurePart.LeftLeg,
            ItemType.FigureRightArmPart => FigurePart.RightArm,
            ItemType.FigureRightLegPart => FigurePart.RightLeg,
            _ => FigurePart.None,
        };

        public static ICollection<RarityExcluded> RarityExcluded(this ItemType type) => type switch
        {
            ItemType.RandomTitleBoosterPackSingleE
                or ItemType.BigRandomBoosterPackE => new[]
                {
                    new RarityExcluded(Rarity.SS),
                    new RarityExcluded(Rarity.S),
                },
            ItemType.RandomNormalBoosterPackB
                or ItemType.RandomNormalBoosterPackA
                or ItemType.RandomNormalBoosterPackS => new[]
                {
                    new RarityExcluded(Rarity.SS)
                },
            ItemType.RandomBoosterPackSingleE => new[]
                {
                    new RarityExcluded(Rarity.SS),
                    new RarityExcluded(Rarity.S),
                    new RarityExcluded(Rarity.A),
                },
            _ => Array.Empty<RarityExcluded>(),
        };

        public static Item ToItem(this ItemType type, long count = 1, Quality quality = Quality.Broken)
        {
            if (!type.HasDifferentQualities() && quality != Quality.Broken)
            {
                quality = Quality.Broken;
            }

            return new Item
            {
                Name = ToString(type, quality.ToName()),
                Quality = quality,
                Count = count,
                Type = type,
            };
        }

        public static PreAssembledFigure ToPASType(this ItemType type) => type switch
        {
            ItemType.PreAssembledAsuna => PreAssembledFigure.Asuna,
            ItemType.PreAssembledGintoki => PreAssembledFigure.Gintoki,
            ItemType.PreAssembledMegumin => PreAssembledFigure.Megumin,
            _ => PreAssembledFigure.None,
        };

        public static BoosterPack? ToBoosterPack(this ItemType type)
        {
            if (!type.IsBoosterPack())
            {
                return null;
            }

            return new BoosterPack
            {
                Name = ToString(type),
                CardCount = type.Count(),
                MinRarity = type.MinRarity(),
                CardSourceFromPack = type.GetSource(),
                IsCardFromPackTradable = type.IsTradable(),
                RarityExcludedFromPack = type.RarityExcluded(),
            };
        }
    }
}
