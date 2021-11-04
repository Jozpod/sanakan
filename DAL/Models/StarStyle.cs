using System;

namespace Sanakan.DAL.Models
{
    public enum StarStyle
    {
        Full = 0,
        White = 1,
        Black = 2,
        Empty = 3,
        Pig = 4,
        Snek = 5
    }

    public static class StarStyleExtensions
    {
        public static StarStyle Parse(this StarStyle star, string str)
        {
            switch (str.ToLower())
            {
                case "waz":
                case "waż":
                case "wąz":
                case "wąż":
                case "snek":
                case "snake":
                    return StarStyle.Snek;

                case "pig":
                case "świnia":
                case "swinia":
                case "świnka":
                case "swinka":
                    return StarStyle.Pig;

                case "biała":
                case "biala":
                case "white":
                    return StarStyle.White;

                case "full":
                case "pełna":
                case "pelna":
                    return StarStyle.Full;

                case "empty":
                case "pusta":
                    return StarStyle.Empty;

                case "black":
                case "czarna":
                    return StarStyle.Black;

                default:
                    throw new Exception("Could't parse input!");
            }
        }
    }
}
