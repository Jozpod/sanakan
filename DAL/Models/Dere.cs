﻿using System.Collections.Generic;

namespace Sanakan.DAL.Models
{
    /// <summary>
    ///  Describes character personality archetypes.
    /// </summary>
    public enum Dere : byte
    {
        /// <summary>
        /// The type of character who in the beginning acts rudely towards their love interest. But once the timing is perfect they reveal their gentle side.
        /// </summary>
        Tsundere = 0,

        /// <summary>
        /// The type of character who believes they are god-like beings. They are overly proud and arrogant and only care about their own opinions.
        /// </summary>
        Kamidere = 1,

        /// <summary>
        /// The type of character who is a kind person with a positive attitude. They tend to spread positivity and happiness towards others around them.
        /// </summary>
        Deredere = 2,

        /// <summary>
        /// The type of character who in the beginning acts nice and sweet but soon becomes obsessive over their love interest. They turn into a stalker and use violence against anyone who tries to get close to their love interest.
        /// </summary>
        Yandere = 3,

        /// <summary>
        /// The type of character who is quiet and reserved. They usually come out of their shell and show their cute side when alone with their love interest.
        /// </summary>
        Dandere = 4,

        /// <summary>
        /// The type of character who is dull and sarcastic. They often don’t show any sign of emotion. But when alone with their love interest, they display their cute and caring side.
        /// </summary>
        Kuudere = 5,

        /// <summary>
        /// The type of character who is often a dangerous antagonist of a series but switches sides after falling in love or after becoming fond of another character, usually the protagonist or someone in the hero's team.
        /// </summary>
        Mayadere = 6,

        /// <summary>
        /// The type of character who is really shy and uses violence as a way to handle it.
        /// </summary>
        Bodere = 7,

        /// <summary>
        /// The type of character which is mixture of Kuudere and Dandere.
        /// </summary>
        Yami = 8,

        /// <summary>
        /// The type of character which behaves like Raito from Kyoukai Resident anime???.
        /// </summary>
        Raito = 9,

        /// <summary>
        /// The type of character which behaves like Yato from Noragami anime???.
        /// </summary>
        Yato = 10
    }

    public static class DereExtensions
    {
        public static IEnumerable<Dere> ListOfDeres = new[]
        {
                Dere.Tsundere,
                Dere.Kamidere,
                Dere.Deredere,
                Dere.Yandere,
                Dere.Dandere,
                Dere.Kuudere,
                Dere.Mayadere,
                Dere.Bodere
        };

        public static double ValueModifier(this Rarity rarity) => rarity switch
        {
            Rarity.SS => 1.15,
            Rarity.S => 1.05,
            Rarity.A => 0.95,
            Rarity.B => 0.90,
            Rarity.C => 0.85,
            Rarity.D => 0.80,
            Rarity.E => 0.70,
            _ => 1.3,
        };

        public static double ValueModifierReverse(this Dere dere) => 2d - dere.ValueModifier();

        public static double ValueModifier(this Dere dere) => dere switch
        {
            Dere.Tsundere => 0.6,
            _ => 1,
        };
    }
}
