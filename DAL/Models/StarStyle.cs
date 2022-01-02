using System;

namespace Sanakan.DAL.Models
{
    public enum StarStyle : byte
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
        public static StarStyle Parse(string str)
        {
            return str.ToLower() switch
            {
                "waz" or "waż" or "wąz" or "wąż" or "snek" or "snake" => StarStyle.Snek,
                "pig" or "świnia" or "swinia" or "świnka" or "swinka" => StarStyle.Pig,
                "biała" or "biala" or "white" => StarStyle.White,
                "full" or "pełna" or "pelna" => StarStyle.Full,
                "empty" or "pusta" => StarStyle.Empty,
                "black" or "czarna" => StarStyle.Black,
                _ => throw new Exception("Could't parse input!"),
            };
        }
    }
}
