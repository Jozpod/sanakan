﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Models
{
    public enum ItemType
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
    }

    public static class ItemTypeExtensions
    {
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
                CompletionDate = date,
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

        public static bool HasDifferentQualities(this ItemType type)
        {
            switch (type)
            {
                case ItemType.AffectionRecoveryGreat:
                case ItemType.AffectionRecoveryBig:
                case ItemType.AffectionRecoveryNormal:
                case ItemType.AffectionRecoverySmall:
                case ItemType.IncreaseExpSmall:
                case ItemType.IncreaseExpBig:
                case ItemType.FigureSkeleton:
                case ItemType.FigureUniversalPart:
                case ItemType.FigureHeadPart:
                case ItemType.FigureBodyPart:
                case ItemType.FigureClothesPart:
                case ItemType.FigureLeftArmPart:
                case ItemType.FigureLeftLegPart:
                case ItemType.FigureRightArmPart:
                case ItemType.FigureRightLegPart:
                    return true;

                default:
                    return false;
            }
        }

        public static long CValue(this ItemType type)
        {
            switch (type)
            {
                case ItemType.AffectionRecoveryGreat:
                    return 180;
                case ItemType.AffectionRecoveryBig:
                    return 140;
                case ItemType.AffectionRecoveryNormal:
                    return 15;
                case ItemType.BetterIncreaseUpgradeCnt:
                    return 500;
                case ItemType.IncreaseUpgradeCount:
                    return 200;
                case ItemType.DereReRoll:
                    return 10;
                case ItemType.CardParamsReRoll:
                    return 15;
                case ItemType.CheckAffection:
                    return 15;
                case ItemType.SetCustomImage:
                    return 300;
                case ItemType.IncreaseExpSmall:
                    return 100;
                case ItemType.IncreaseExpBig:
                    return 400;
                case ItemType.ChangeStarType:
                    return 50;
                case ItemType.SetCustomBorder:
                    return 80;
                case ItemType.ChangeCardImage:
                    return 10;

                default:
                    return 1;
            }
        }

        public static bool IsBoosterPack(this ItemType type)
        {
            switch (type)
            {
                case ItemType.RandomBoosterPackSingleE:
                case ItemType.RandomTitleBoosterPackSingleE:
                case ItemType.RandomNormalBoosterPackB:
                case ItemType.RandomNormalBoosterPackA:
                case ItemType.RandomNormalBoosterPackS:
                case ItemType.RandomNormalBoosterPackSS:
                case ItemType.BigRandomBoosterPackE:
                    return true;

                default:
                    return false;
            }
        }

        public static bool IsPreAssembledFigure(this ItemType type)
        {
            switch (type)
            {
                case ItemType.PreAssembledAsuna:
                case ItemType.PreAssembledGintoki:
                case ItemType.PreAssembledMegumin:
                    return true;

                default:
                    return false;
            }
        }

        public static int Count(this ItemType type)
        {
            switch (type)
            {
                case ItemType.RandomNormalBoosterPackB:
                case ItemType.RandomNormalBoosterPackA:
                case ItemType.RandomNormalBoosterPackS:
                case ItemType.RandomNormalBoosterPackSS:
                    return 3;

                case ItemType.BigRandomBoosterPackE:
                    return 20;

                default:
                    return 2;
            }
        }

        public static Rarity MinRarity(this ItemType type)
        {
            switch (type)
            {
                case ItemType.RandomNormalBoosterPackSS:
                    return Rarity.SS;

                case ItemType.RandomNormalBoosterPackS:
                    return Rarity.S;

                case ItemType.RandomNormalBoosterPackA:
                    return Rarity.A;

                case ItemType.RandomNormalBoosterPackB:
                    return Rarity.B;

                default:
                    return Rarity.E;
            }
        }

        public static bool IsTradable(this ItemType type)
        {
            switch (type)
            {
                case ItemType.RandomTitleBoosterPackSingleE:
                    return false;

                default:
                    return true;
            }
        }

        public static CardSource GetSource(this ItemType type)
        {
            switch (type)
            {
                case ItemType.RandomBoosterPackSingleE:
                case ItemType.RandomNormalBoosterPackB:
                case ItemType.RandomNormalBoosterPackA:
                case ItemType.RandomNormalBoosterPackS:
                case ItemType.RandomNormalBoosterPackSS:
                case ItemType.RandomTitleBoosterPackSingleE:
                case ItemType.BigRandomBoosterPackE:
                    return CardSource.Shop;

                default:
                    return CardSource.Other;
            }
        }

        public static bool CanUseWithoutCard(this ItemType type)
        {
            switch (type)
            {
                case ItemType.FigureHeadPart:
                case ItemType.FigureBodyPart:
                case ItemType.FigureClothesPart:
                case ItemType.FigureLeftArmPart:
                case ItemType.FigureLeftLegPart:
                case ItemType.FigureRightArmPart:
                case ItemType.FigureRightLegPart:
                case ItemType.FigureUniversalPart:
                    return true;

                default:
                    return false;
            }
        }

        public static double BaseAffection(this ItemType type)
        {
            switch (type)
            {
                case ItemType.AffectionRecoveryGreat: return 1.6;
                case ItemType.AffectionRecoveryBig: return 1;
                case ItemType.AffectionRecoveryNormal: return 0.12;
                case ItemType.AffectionRecoverySmall: return 0.02;
                case ItemType.BetterIncreaseUpgradeCnt: return 1.7;
                case ItemType.IncreaseUpgradeCount: return 0.7;
                case ItemType.DereReRoll: return 0.1;
                case ItemType.CardParamsReRoll: return 0.2;
                case ItemType.CheckAffection: return 0.2;
                case ItemType.SetCustomImage: return 0.5;
                case ItemType.IncreaseExpSmall: return 0.15;
                case ItemType.IncreaseExpBig: return 0.25;
                case ItemType.ChangeStarType: return 0.3;
                case ItemType.SetCustomBorder: return 0.4;
                case ItemType.ChangeCardImage: return 0.1;

                default: return 0;
            }
        }

        public static string Desc(this ItemType type)
        {
            switch (type)
            {
                case ItemType.AffectionRecoveryGreat:
                    return "Poprawia relacje z kartą w dużym stopniu.";
                case ItemType.AffectionRecoveryBig:
                    return "Poprawia relacje z kartą w znacznym stopniu.";
                case ItemType.AffectionRecoveryNormal:
                    return "Poprawia relacje z kartą.";
                case ItemType.BetterIncreaseUpgradeCnt:
                    return "Może zwiększyć znacznie liczbę ulepszeń karty, tylko kto by chciał twoją krew?";
                case ItemType.IncreaseUpgradeCount:
                    return "Dodaje dodatkowy punkt ulepszenia do karty.";
                case ItemType.DereReRoll:
                    return "Pozwala zmienić charakter karty.";
                case ItemType.CardParamsReRoll:
                    return "Pozwala wylosować na nowo parametry karty.";
                case ItemType.RandomBoosterPackSingleE:
                    return "Dodaje nowy pakiet z dwiema losowymi kartami.\n\nWykluczone jakości to: SS, S i A.";
                case ItemType.BigRandomBoosterPackE:
                    return "Dodaje nowy pakiet z dwudziestoma losowymi kartami.\n\nWykluczone jakości to: SS, S i A.";
                case ItemType.RandomTitleBoosterPackSingleE:
                    return "Dodaje nowy pakiet z dwiema losowymi, niewymienialnymi kartami z tytułu podanego przez kupującego.\n\nWykluczone jakości to: SS i S.";
                case ItemType.AffectionRecoverySmall:
                    return "Poprawia odrobinę relacje z kartą.";
                case ItemType.RandomNormalBoosterPackB:
                    return "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości B.\n\nWykluczone jakości to: SS.";
                case ItemType.RandomNormalBoosterPackA:
                    return "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości A.\n\nWykluczone jakości to: SS.";
                case ItemType.RandomNormalBoosterPackS:
                    return "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości S.\n\nWykluczone jakości to: SS.";
                case ItemType.RandomNormalBoosterPackSS:
                    return "Dodaje nowy pakiet z trzema losowymi kartami, w tym jedną o gwarantowanej jakości SS.";
                case ItemType.CheckAffection:
                    return "Pozwala sprawdzić dokładny poziom relacji z kartą.";
                case ItemType.SetCustomImage:
                    return "Pozwala ustawić własny obrazek karcie. Zalecany wymiary 448x650.";
                case ItemType.IncreaseExpSmall:
                    return "Dodaje odrobinę punktów doświadczenia do karty.";
                case ItemType.IncreaseExpBig:
                    return "Dodaje punkty doświadczenia do karty.";
                case ItemType.ChangeStarType:
                    return "Pozwala zmienić typ gwiazdek na karcie.";
                case ItemType.SetCustomBorder:
                    return "Pozwala ustawić ramkę karcie kiedy jest wyświetlana w profilu.";
                case ItemType.ChangeCardImage:
                    return "Pozwala wybrać inny obrazek z shindena.";
                case ItemType.PreAssembledAsuna:
                case ItemType.PreAssembledGintoki:
                case ItemType.PreAssembledMegumin:
                    return "Gotowy szkielet nie wymagający użycia karty SSS.";
                case ItemType.FigureSkeleton:
                    return $"Szkielet pozwalający rozpoczęcie tworzenia figurki.";
                case ItemType.FigureUniversalPart:
                    return $"Uniwersalna część, którą można zamontować jako dowolną część ciała figurki.";
                case ItemType.FigureHeadPart:
                    return $"Część, którą można zamontować jako głowę figurki.";
                case ItemType.FigureBodyPart:
                    return $"Część, którą można zamontować jako tułów figurki.";
                case ItemType.FigureClothesPart:
                    return $"Część, którą można zamontować jako ciuchy figurki.";
                case ItemType.FigureLeftArmPart:
                    return $"Część, którą można zamontować jako lewą rękę figurki.";
                case ItemType.FigureLeftLegPart:
                    return $"Część, którą można zamontować jako lewą nogę figurki.";
                case ItemType.FigureRightArmPart:
                    return $"Część, którą można zamontować jako prawą rękę figurki.";
                case ItemType.FigureRightLegPart:
                    return $"Część, którą można zamontować jako prawą nogę figurki.";

                default:
                    return "Brak opisu.";
            }
        }

        public static string Name(this ItemType type, string quality = "")
        {
            if (!string.IsNullOrEmpty(quality))
                quality = $" {quality}";

            switch (type)
            {
                case ItemType.AffectionRecoveryGreat:
                    return $"Wielka fontanna czekolady{quality}";
                case ItemType.AffectionRecoveryBig:
                    return $"Tort czekoladowy{quality}";
                case ItemType.AffectionRecoveryNormal:
                    return $"Ciasto truskawkowe{quality}";
                case ItemType.BetterIncreaseUpgradeCnt:
                    return "Kropla twojej krwi";
                case ItemType.IncreaseUpgradeCount:
                    return "Pierścionek zaręczynowy";
                case ItemType.DereReRoll:
                    return "Bukiet kwiatów";
                case ItemType.CardParamsReRoll:
                    return "Naszyjnik z diamentem";
                case ItemType.RandomBoosterPackSingleE:
                    return "Tani pakiet losowych kart";
                case ItemType.BigRandomBoosterPackE:
                    return "Może i nie tani ale za to duży pakiet kart";
                case ItemType.RandomTitleBoosterPackSingleE:
                    return "Pakiet losowych kart z tytułu";
                case ItemType.AffectionRecoverySmall:
                    return $"Banan w czekoladzie{quality}";
                case ItemType.RandomNormalBoosterPackB:
                    return "Fioletowy pakiet losowych kart";
                case ItemType.RandomNormalBoosterPackA:
                    return "Pomarańczowy pakiet losowych kart";
                case ItemType.RandomNormalBoosterPackS:
                    return "Złoty pakiet losowych kart";
                case ItemType.RandomNormalBoosterPackSS:
                    return "Różowy pakiet losowych kart";
                case ItemType.CheckAffection:
                    return "Kryształowa kula";
                case ItemType.SetCustomImage:
                    return "Skalpel";
                case ItemType.IncreaseExpSmall:
                    return $"Mleko truskawkowe{quality}";
                case ItemType.IncreaseExpBig:
                    return $"Gorąca czekolada{quality}";
                case ItemType.ChangeStarType:
                    return "Stempel";
                case ItemType.SetCustomBorder:
                    return "Nożyczki";
                case ItemType.ChangeCardImage:
                    return "Plastelina";
                case ItemType.PreAssembledAsuna:
                    return "Szkielet Asuny (SAO)";
                case ItemType.PreAssembledGintoki:
                    return "Szkielet Gintokiego (Gintama)";
                case ItemType.PreAssembledMegumin:
                    return "Szkielet Megumin (Konosuba)";
                case ItemType.FigureSkeleton:
                    return $"Szkielet{quality}";
                case ItemType.FigureUniversalPart:
                    return $"Uniwersalna część figurki{quality}";
                case ItemType.FigureHeadPart:
                    return $"Głowa figurki{quality}";
                case ItemType.FigureBodyPart:
                    return $"Tułów figurki{quality}";
                case ItemType.FigureClothesPart:
                    return $"Ciuchy figurki{quality}";
                case ItemType.FigureLeftArmPart:
                    return $"Lewa ręka{quality}";
                case ItemType.FigureLeftLegPart:
                    return $"Lewa noga{quality}";
                case ItemType.FigureRightArmPart:
                    return $"Prawa ręka{quality}";
                case ItemType.FigureRightLegPart:
                    return $"Prawa noga{quality}";

                default:
                    return "Brak";
            }
        }

        public static FigurePart GetPartType(this ItemType type)
        {
            switch (type)
            {
                case ItemType.FigureUniversalPart:
                    return FigurePart.All;
                case ItemType.FigureHeadPart:
                    return FigurePart.Head;
                case ItemType.FigureBodyPart:
                    return FigurePart.Body;
                case ItemType.FigureClothesPart:
                    return FigurePart.Clothes;
                case ItemType.FigureLeftArmPart:
                    return FigurePart.LeftArm;
                case ItemType.FigureLeftLegPart:
                    return FigurePart.LeftLeg;
                case ItemType.FigureRightArmPart:
                    return FigurePart.RightArm;
                case ItemType.FigureRightLegPart:
                    return FigurePart.RightLeg;

                default:
                    return FigurePart.None;
            }
        }

        public static List<RarityExcluded> RarityExcluded(this ItemType type)
        {
            var result = new List<RarityExcluded>();

            switch (type)
            {
                case ItemType.RandomTitleBoosterPackSingleE:
                case ItemType.BigRandomBoosterPackE:
                    result.Add(new RarityExcluded(Rarity.SS));
                    result.Add(new RarityExcluded(Rarity.S));
                    break;

                case ItemType.RandomNormalBoosterPackB:
                case ItemType.RandomNormalBoosterPackA:
                case ItemType.RandomNormalBoosterPackS:
                    result.Add(new RarityExcluded(Rarity.SS));
                    break;

                case ItemType.RandomBoosterPackSingleE:
                    result.Add(new RarityExcluded(Rarity.SS));
                    result.Add(new RarityExcluded(Rarity.S));
                    result.Add(new RarityExcluded(Rarity.A));
                    break;

                default:
                    break;
            }

            return result;
        }

        public static Item ToItem(this ItemType type, long count = 1, Quality quality = Quality.Broken)
        {
            if (!type.HasDifferentQualities() && quality != Quality.Broken)
                quality = Quality.Broken;

            return new Item
            {
                Name = type.Name(quality.ToName()),
                Quality = quality,
                Count = count,
                Type = type,
            };
        }

        public static PreAssembledFigure ToPASType(this ItemType type)
        {
            switch (type)
            {
                case ItemType.PreAssembledAsuna:
                    return PreAssembledFigure.Asuna;
                case ItemType.PreAssembledGintoki:
                    return PreAssembledFigure.Gintoki;
                case ItemType.PreAssembledMegumin:
                    return PreAssembledFigure.Megumin;

                default:
                    return PreAssembledFigure.None;
            }
        }

        public static BoosterPack ToBoosterPack(this ItemType type)
        {
            if (!type.IsBoosterPack())
                return null;

            return new BoosterPack
            {
                Name = type.Name(),
                CardCount = type.Count(),
                MinRarity = type.MinRarity(),
                CardSourceFromPack = type.GetSource(),
                IsCardFromPackTradable = type.IsTradable(),
                RarityExcludedFromPack = type.RarityExcluded(),
            };
        }
    }
}