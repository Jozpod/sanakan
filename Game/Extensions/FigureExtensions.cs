using System;
using System.Linq;
using Sanakan.DAL.Models;
using Sanakan.ShindenApi.Utilities;

public static class FigureExtensions
    {
        public static string IsActive(this Figure fig)
        {
            return fig.IsFocus ? "**A**" : "";
        }

        public static Quality GetAvgQuality(this Figure figure)
        {
            double tavg = ((int)figure.SkeletonQuality) * 10;
            double pavg = (int)figure.HeadQuality;
            pavg += (int)figure.BodyQuality;
            pavg += (int)figure.LeftArmQuality;
            pavg += (int)figure.RightArmQuality;
            pavg += (int)figure.LeftLegQuality;
            pavg += (int)figure.RightLegQuality;
            pavg += (int)figure.ClothesQuality;
            tavg += pavg / 7;

            int rAvg = (int)Math.Floor(tavg / 10);
            var eQ = Quality.Broken;

            foreach (int v in Enum.GetValues(typeof(Quality)))
            {
                if (v > rAvg) break;
                eQ = (Quality)v;
            }

            return eQ;
        }

        public static bool CanCreateUltimateCard(this Figure figure)
        {
            bool canCreate = true;
            canCreate &= figure.SkeletonQuality != Quality.Broken;
            canCreate &= figure.HeadQuality     != Quality.Broken;
            canCreate &= figure.BodyQuality     != Quality.Broken;
            canCreate &= figure.LeftArmQuality  != Quality.Broken;
            canCreate &= figure.RightArmQuality != Quality.Broken;
            canCreate &= figure.LeftLegQuality  != Quality.Broken;
            canCreate &= figure.RightLegQuality != Quality.Broken;
            canCreate &= figure.ClothesQuality  != Quality.Broken;
            canCreate &= figure.ExperienceCount >= Rarity.SSS.ExpToUpgrade(true, figure.SkeletonQuality);
            return canCreate;
        }

        public static double ToExperience(this Item item, Quality skeleton)
        {
            double diff = ((int) skeleton - (int) item.Quality) / 10f;
            if (diff <= 0)
            {
                return 1 + item.Quality.ToValue() * -diff;
            }
            return 1 / (diff + 2);
        }

        public static string GetFiguresList(this GameDeck deck)
        {
            if (!deck.Figures.Any())
            {
                return "Nie posiadasz figurek.";
            }

            var figures = deck.Figures.Select(x => $"**[{x.Id}]** *{x.SkeletonQuality.ToName()}* [{x.Name}]({UrlHelpers.GetCharacterURL(x.Character)}) {x.IsActive()}");
            return string.Join("\n", figures);
        }

        public static string GetDesc(this Figure fig)
        {
            var name =  $"[{fig.Name}]({UrlHelpers.GetCharacterURL(fig.Character)})";

            return $"**[{fig.Id}] Figurka {fig.SkeletonQuality.ToName()}**\n{name}\n*{fig.ExperienceCount} exp*\n\n"
                + $"**Aktywna część:**\n {fig.FocusedPart.ToName()} *{fig.PartExp} exp*\n\n"
                + $"**Części:**\n*Głowa*: {fig.HeadQuality.ToName("brak")}\n*Tułów*: {fig.BodyQuality.ToName("brak")}\n"
                + $"*Prawa ręka*: {fig.RightArmQuality.ToName("brak")}\n*Lewa ręka*: {fig.LeftArmQuality.ToName("brak")}\n"
                + $"*Prawa noga*: {fig.RightLegQuality.ToName("brak")}\n*Lewa noga*: {fig.LeftLegQuality.ToName("brak")}\n"
                + $"*Ciuchy*: {fig.ClothesQuality.ToName("brak")}";
        }
    }