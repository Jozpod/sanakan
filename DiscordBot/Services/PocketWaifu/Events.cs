using System;
using System.Collections.Generic;
using DiscordBot.Services.PocketWaifu;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.Extensions;
using Sanakan.ShindenApi;
using Shinden;

namespace Sanakan.Services.PocketWaifu
{
    public class Events
    {
        private readonly IShindenClient _shindenClient;
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ISystemClock _systemClock;

        public Events(
            IShindenClient shindenClient,
            IRandomNumberGenerator randomNumberGenerator,
            ISystemClock systemClock)
        {
            _shindenClient = shindenClient;
            _randomNumberGenerator = randomNumberGenerator;
            _systemClock = systemClock;
        }

        private static List<ulong> _titles = new ()
        {
            7431,
            50646,
            10831,
            54081,
            53776,
            12434,
            44867,
            51100,
            4961,
            55260,
            53382,
            53685,
            35405,
            54195,
            2763,
            43864,
            52427,
            52111,
            53257,
            45085
        };

        private static Dictionary<CardExpedition, Dictionary<EventType, Tuple<int, int>>> _chanceOfEvent = new Dictionary<CardExpedition, Dictionary<EventType, Tuple<int, int>>>
        {
            {CardExpedition.NormalItemWithExp, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(0,    1500)},
                    {EventType.MoreExp,     new Tuple<int, int>(1500, 3900)},
                    {EventType.IncAtk,      new Tuple<int, int>(3900, 7400)},
                    {EventType.IncDef,      new Tuple<int, int>(7400, 10000)},
                    {EventType.AddReset,    new Tuple<int, int>(-1,   -2)},
                    {EventType.NewCard,     new Tuple<int, int>(-3,   -4)},
                    {EventType.ChangeDere,  new Tuple<int, int>(-5,   -6)},
                    {EventType.DecAtk,      new Tuple<int, int>(-7,   -8)},
                    {EventType.DecDef,      new Tuple<int, int>(-9,   -10)},
                    {EventType.DecAff,      new Tuple<int, int>(-11,  -12)},
                    {EventType.LoseCard,    new Tuple<int, int>(-13,  -14)},
                    {EventType.Fight,       new Tuple<int, int>(-15,  -16)},
                }
            },
            {CardExpedition.ExtremeItemWithExp, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(0,    900)},
                    {EventType.MoreExp,     new Tuple<int, int>(900,  1900)},
                    {EventType.IncAtk,      new Tuple<int, int>(1900, 3000)},
                    {EventType.IncDef,      new Tuple<int, int>(3000, 4000)},
                    {EventType.AddReset,    new Tuple<int, int>(4000, 4200)},
                    {EventType.NewCard,     new Tuple<int, int>(4200, 4300)},
                    {EventType.ChangeDere,  new Tuple<int, int>(4300, 5000)},
                    {EventType.DecAtk,      new Tuple<int, int>(5000, 6200)},
                    {EventType.DecDef,      new Tuple<int, int>(6200, 7400)},
                    {EventType.DecAff,      new Tuple<int, int>(7400, 9000)},
                    {EventType.LoseCard,    new Tuple<int, int>(9000, 10000)},
                    {EventType.Fight,       new Tuple<int, int>(-1,  -2)},
                }
            },
            {CardExpedition.DarkItemWithExp, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(0,    1000)},
                    {EventType.MoreExp,     new Tuple<int, int>(1000, 2500)},
                    {EventType.IncAtk,      new Tuple<int, int>(2500, 5000)},
                    {EventType.IncDef,      new Tuple<int, int>(5000, 7000)},
                    {EventType.AddReset,    new Tuple<int, int>(-1,   -2)},
                    {EventType.Fight,       new Tuple<int, int>(7000, 7300)},
                    {EventType.ChangeDere,  new Tuple<int, int>(7300, 7900)},
                    {EventType.DecAtk,      new Tuple<int, int>(7900, 8500)},
                    {EventType.DecDef,      new Tuple<int, int>(8500, 9000)},
                    {EventType.DecAff,      new Tuple<int, int>(9000, 10000)},
                    {EventType.LoseCard,    new Tuple<int, int>(-3,   -4)},
                    {EventType.NewCard,     new Tuple<int, int>(-5,   -6)},
                }
            },
            {CardExpedition.LightItemWithExp, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(0,    1000)},
                    {EventType.MoreExp,     new Tuple<int, int>(1000, 2500)},
                    {EventType.IncAtk,      new Tuple<int, int>(2500, 5000)},
                    {EventType.IncDef,      new Tuple<int, int>(5000, 7000)},
                    {EventType.AddReset,    new Tuple<int, int>(-1,   -2)},
                    {EventType.Fight,       new Tuple<int, int>(7000, 7300)},
                    {EventType.ChangeDere,  new Tuple<int, int>(7300, 7900)},
                    {EventType.DecAtk,      new Tuple<int, int>(7900, 8500)},
                    {EventType.DecDef,      new Tuple<int, int>(8500, 9000)},
                    {EventType.DecAff,      new Tuple<int, int>(9000, 10000)},
                    {EventType.LoseCard,    new Tuple<int, int>(-3,   -4)},
                    {EventType.NewCard,     new Tuple<int, int>(-5,   -6)},
                }
            },
            {CardExpedition.DarkItems, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(-1,   -2)},
                    {EventType.MoreExp,     new Tuple<int, int>(-3,   -4)},
                    {EventType.IncAtk,      new Tuple<int, int>(0,    2200)},
                    {EventType.IncDef,      new Tuple<int, int>(2200, 4100)},
                    {EventType.AddReset,    new Tuple<int, int>(-5,   -6)},
                    {EventType.Fight,       new Tuple<int, int>(4100, 4400)},
                    {EventType.ChangeDere,  new Tuple<int, int>(4400, 5400)},
                    {EventType.DecAtk,      new Tuple<int, int>(5400, 6600)},
                    {EventType.DecDef,      new Tuple<int, int>(6600, 8000)},
                    {EventType.DecAff,      new Tuple<int, int>(8000, 10000)},
                    {EventType.LoseCard,    new Tuple<int, int>(-7,   -8)},
                    {EventType.NewCard,     new Tuple<int, int>(-9,   -10)},
                }
            },
            {CardExpedition.LightItems, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(-1,   -2)},
                    {EventType.MoreExp,     new Tuple<int, int>(-3,   -4)},
                    {EventType.IncAtk,      new Tuple<int, int>(0,    2200)},
                    {EventType.IncDef,      new Tuple<int, int>(2200, 4100)},
                    {EventType.AddReset,    new Tuple<int, int>(-5,   -6)},
                    {EventType.Fight,       new Tuple<int, int>(4100, 4400)},
                    {EventType.ChangeDere,  new Tuple<int, int>(4400, 5400)},
                    {EventType.DecAtk,      new Tuple<int, int>(5400, 6600)},
                    {EventType.DecDef,      new Tuple<int, int>(6600, 8000)},
                    {EventType.DecAff,      new Tuple<int, int>(8000, 10000)},
                    {EventType.LoseCard,    new Tuple<int, int>(-7,   -8)},
                    {EventType.NewCard,     new Tuple<int, int>(-9,   -10)},
                }
            },
            {CardExpedition.DarkExp, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(-1,   -2)},
                    {EventType.MoreExp,     new Tuple<int, int>(-3,   -4)},
                    {EventType.IncAtk,      new Tuple<int, int>(0,    2200)},
                    {EventType.IncDef,      new Tuple<int, int>(2200, 4100)},
                    {EventType.AddReset,    new Tuple<int, int>(-5,   -6)},
                    {EventType.Fight,       new Tuple<int, int>(4100, 4400)},
                    {EventType.ChangeDere,  new Tuple<int, int>(4400, 5400)},
                    {EventType.DecAtk,      new Tuple<int, int>(5400, 6600)},
                    {EventType.DecDef,      new Tuple<int, int>(6600, 8000)},
                    {EventType.DecAff,      new Tuple<int, int>(8000, 10000)},
                    {EventType.LoseCard,    new Tuple<int, int>(-7,   -8)},
                    {EventType.NewCard,     new Tuple<int, int>(-9,   -10)},
                }
            },
            {CardExpedition.LightExp, new Dictionary<EventType, Tuple<int, int>>
                {
                    {EventType.MoreItems,   new Tuple<int, int>(-1,   -2)},
                    {EventType.MoreExp,     new Tuple<int, int>(-3,   -4)},
                    {EventType.IncAtk,      new Tuple<int, int>(0,    2200)},
                    {EventType.IncDef,      new Tuple<int, int>(2200, 4100)},
                    {EventType.AddReset,    new Tuple<int, int>(-5,   -6)},
                    {EventType.Fight,       new Tuple<int, int>(4100, 4400)},
                    {EventType.ChangeDere,  new Tuple<int, int>(4400, 5400)},
                    {EventType.DecAtk,      new Tuple<int, int>(5400, 6600)},
                    {EventType.DecDef,      new Tuple<int, int>(6600, 8000)},
                    {EventType.DecAff,      new Tuple<int, int>(8000, 10000)},
                    {EventType.LoseCard,    new Tuple<int, int>(-7,   -8)},
                    {EventType.NewCard,     new Tuple<int, int>(-9,   -10)},
                }
            }
        };

        private EventType CheckChanceBasedOnTime(CardExpedition expedition, Tuple<double, double> duration)
        {
            switch (expedition)
            {
                case CardExpedition.ExtremeItemWithExp:
                    if (duration.Item1 > 45 || duration.Item2 > 240)
                    {
                        if (_randomNumberGenerator.TakeATry(2))
                        {
                            return EventType.LoseCard;
                        }
                    }
                    return EventType.None;

                default:
                    return EventType.None;
            }
        }

        public EventType RandomizeEvent(CardExpedition expedition, Tuple<double, double> duration)
        {
            var timeBased = CheckChanceBasedOnTime(expedition, duration);
            if (timeBased != EventType.None)
            {
                return timeBased;
            }

            var c = _chanceOfEvent[expedition];

            switch (_randomNumberGenerator.GetRandomValue(10000))
            {
                case int n when (n < c[EventType.MoreItems].Item2
                                && n >= c[EventType.MoreItems].Item1):
                    return EventType.MoreItems;

                case int n when (n < c[EventType.MoreExp].Item2
                                && n >= c[EventType.MoreExp].Item1):
                    return EventType.MoreExp;

                case int n when (n < c[EventType.IncAtk].Item2
                                && n >= c[EventType.IncAtk].Item1):
                    return EventType.IncAtk;

                case int n when (n < c[EventType.IncDef].Item2
                                && n >= c[EventType.IncDef].Item1):
                    return EventType.IncDef;

                case int n when (n < c[EventType.AddReset].Item2
                                && n >= c[EventType.AddReset].Item1):
                    return EventType.AddReset;

                case int n when (n < c[EventType.NewCard].Item2
                                && n >= c[EventType.NewCard].Item1):
                    return EventType.NewCard;

                case int n when (n < c[EventType.ChangeDere].Item2
                                && n >= c[EventType.ChangeDere].Item1):
                    return EventType.ChangeDere;

                case int n when (n < c[EventType.DecAtk].Item2
                                && n >= c[EventType.DecAtk].Item1):
                    return EventType.DecAtk;

                case int n when (n < c[EventType.DecDef].Item2
                                && n >= c[EventType.DecDef].Item1):
                    return EventType.DecDef;

                case int n when (n < c[EventType.DecAff].Item2
                                && n >= c[EventType.DecAff].Item1):
                    return EventType.DecAff;

                case int n when (n < c[EventType.LoseCard].Item2
                                && n >= c[EventType.LoseCard].Item1):
                    return EventType.LoseCard;

                case int n when (n < c[EventType.Fight].Item2
                                && n >= c[EventType.Fight].Item1):
                    return EventType.Fight;

                default: return EventType.None;
            }
        }

        public bool ExecuteEvent(EventType e, User user, Card card, ref string msg)
        {
            var randomValue = _randomNumberGenerator.GetRandomValue(1, 4);

            switch (e)
            {
                case EventType.NewCard:
                {
                    var boosterPack = new BoosterPack
                    {
                        RarityExcludedFromPack = new List<RarityExcluded>(),
                        Title = _randomNumberGenerator.GetOneRandomFrom(_titles),
                        Characters = new List<BoosterPackCharacter>(),
                        CardSourceFromPack = CardSource.Expedition,
                        Name = "Losowa karta z wyprawy",
                        IsCardFromPackTradable = true,
                        MinRarity = Rarity.E,
                        CardCount = 1
                    };

                    user.GameDeck.BoosterPacks.Add(boosterPack);
                    msg += "Wydarzenie: Pakiet z kartą.\n";
                }
                break;

                case EventType.IncAtk:
                {
                    var max = card.Rarity.GetAttackMax();
                    card.Attack += randomValue;

                    if (card.Attack > max)
                        card.Attack = max;

                    msg += $"Wydarzenie: Zwiększenie ataku do {card.Attack}.\n";
                }
                break;

                case EventType.IncDef:
                {
                    var max = card.Rarity.GetDefenceMax();
                    card.Defence += randomValue;

                    if (card.Defence > max)
                        card.Defence = max;

                    msg += $"Wydarzenie: Zwiększenie obrony do {card.Defence}.\n";
                }
                break;

                case EventType.MoreExp:
                {
                    var addExp = _randomNumberGenerator.GetRandomValue(1, 5);
                    card.ExpCnt += addExp;

                    msg += $"Wydarzenie: Dodatkowe punkty doświadczenia. (+{addExp} exp)\n";
                }
                break;

                case EventType.MoreItems:
                {
                    msg += "Wydarzenie: Dodatkowe przedmioty.\n";
                }
                break;

                case EventType.AddReset:
                {
                    ++card.RestartCnt;
                    msg += "Wydarzenie: Zwiększenie ilości restartów karty.\n";
                }
                break;

                case EventType.ChangeDere:
                {
                    msg += "Wydarzenie: Zmiana dere na ";
                }
                break;

                case EventType.DecAff:
                {
                    card.Affection -= randomValue;
                    msg += "Wydarzenie: Zmniejszenie relacji.\n";
                }
                break;

                case EventType.DecAtk:
                {
                    var min = card.Rarity.GetAttackMin();
                    card.Attack -= randomValue;

                    if (card.Attack < min)
                        card.Attack = min;

                    msg += $"Wydarzenie: Zmniejszenie ataku do {card.Attack}.\n";
                }
                break;

                case EventType.DecDef:
                {
                    var min = card.Rarity.GetDefenceMin();
                    card.Defence -= randomValue;

                    if (card.Defence < min)
                        card.Defence = min;

                    msg += $"Wydarzenie: Zmniejszenie obrony do {card.Defence}.\n";
                }
                break;

                case EventType.Fight:
                {
                    var rarity = WaifuService.RandomizeRarity(_randomNumberGenerator);
                    var name = "Miecu";
                    var title = "Bajeczka";
                        
                    var date = _systemClock.UtcNow;
                    var defence = WaifuService.RandomizeDefence(_randomNumberGenerator, rarity);
                    var attack = WaifuService.RandomizeAttack(_randomNumberGenerator, rarity);
                    var dere = WaifuService.RandomizeDere(_randomNumberGenerator);
                    var characterId = 1ul;

                    var enemyCard = new Card(
                        characterId,
                        title,
                        name,
                        attack,
                        defence,
                        rarity,
                        dere,
                        date);
                        
                    card.CalculateCardPower();

                    var result = WaifuService.GetFightWinner(card, enemyCard);

                    var resStr = result == FightWinner.Card1 ? "zwycięstwo!" : "przegrana!";
                    msg += $"Wydarzenie: Walka, wynik: {resStr}\n";

                    return result == FightWinner.Card1;
                }

                case EventType.LoseCard:
                {
                    user.GameDeck.Cards.Remove(card);
                    msg += "Wydarzenie: Utrata karty.\n";
                }
                return false;

                default:
                    return true;
            }

            return true;
        }

        public int GetMoreItems(EventType e)
        {
            switch (e)
            {
                case EventType.MoreItems:
                    return _randomNumberGenerator.GetRandomValue(2, 8);

                default:
                    return 0;
            }
        }
    }
}
