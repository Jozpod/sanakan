using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.Game.Models;
using Sanakan.Game.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sanakan.Game.Services
{
    internal class EventsService : IEventsService
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private readonly ISystemClock _systemClock;

        public EventsService(
            IRandomNumberGenerator randomNumberGenerator,
            ISystemClock systemClock)
        {
            _randomNumberGenerator = randomNumberGenerator;
            _systemClock = systemClock;
        }

        private EventType CheckChanceBasedOnTime(ExpeditionCardType expedition, (double, double) duration)
        {
            switch (expedition)
            {
                case ExpeditionCardType.ExtremeItemWithExp:
                    if (duration.Item1 > TimeSpan.FromMinutes(45).TotalMinutes 
                        || duration.Item2 > TimeSpan.FromHours(4).TotalMinutes)
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

        public EventType RandomizeEvent(ExpeditionCardType expedition, (double, double) duration)
        {
            var timeBased = CheckChanceBasedOnTime(expedition, duration);

            if (timeBased != EventType.None)
            {
                return timeBased;
            }

            var chance = Constants.ChanceOfEvent[expedition];

            switch (_randomNumberGenerator.GetRandomValue(10000))
            {
                case int n when (n < chance[EventType.MoreItems].Item2
                                && n >= chance[EventType.MoreItems].Item1):
                    return EventType.MoreItems;

                case int n when (n < chance[EventType.MoreExp].Item2
                                && n >= chance[EventType.MoreExp].Item1):
                    return EventType.MoreExp;

                case int n when (n < chance[EventType.IncAtk].Item2
                                && n >= chance[EventType.IncAtk].Item1):
                    return EventType.IncAtk;

                case int n when (n < chance[EventType.IncDef].Item2
                                && n >= chance[EventType.IncDef].Item1):
                    return EventType.IncDef;

                case int n when (n < chance[EventType.AddReset].Item2
                                && n >= chance[EventType.AddReset].Item1):
                    return EventType.AddReset;

                case int n when (n < chance[EventType.NewCard].Item2
                                && n >= chance[EventType.NewCard].Item1):
                    return EventType.NewCard;

                case int n when (n < chance[EventType.ChangeDere].Item2
                                && n >= chance[EventType.ChangeDere].Item1):
                    return EventType.ChangeDere;

                case int n when (n < chance[EventType.DecAtk].Item2
                                && n >= chance[EventType.DecAtk].Item1):
                    return EventType.DecAtk;

                case int n when (n < chance[EventType.DecDef].Item2
                                && n >= chance[EventType.DecDef].Item1):
                    return EventType.DecDef;

                case int n when (n < chance[EventType.DecAff].Item2
                                && n >= chance[EventType.DecAff].Item1):
                    return EventType.DecAff;

                case int n when (n < chance[EventType.LoseCard].Item2
                                && n >= chance[EventType.LoseCard].Item1):
                    return EventType.LoseCard;

                case int n when (n < chance[EventType.Fight].Item2
                                && n >= chance[EventType.Fight].Item1):
                    return EventType.Fight;

                default: return EventType.None;
            }
        }

        public (bool, string) ExecuteEvent(EventType eventType, User user, Card card, string message)
        {
            var stringBuilder = new StringBuilder(message, 100);
            var randomValue = _randomNumberGenerator.GetRandomValue(1, 4);

            switch (eventType)
            {
                case EventType.NewCard:
                {
                    var boosterPack = new BoosterPack
                    {
                        RarityExcludedFromPack = new List<RarityExcluded>(),
                        TitleId = _randomNumberGenerator.GetOneRandomFrom(Constants.CharacterTitleIds),
                        Characters = new List<BoosterPackCharacter>(),
                        CardSourceFromPack = CardSource.Expedition,
                        Name = "Losowa karta z wyprawy",
                        IsCardFromPackTradable = true,
                        MinRarity = Rarity.E,
                        CardCount = 1
                    };

                    user.GameDeck.BoosterPacks.Add(boosterPack);
                    stringBuilder.AppendLine("Wydarzenie: Pakiet z kartą.");
                }
                break;

                case EventType.IncAtk:
                {
                    var max = card.Rarity.GetAttackMax();
                    card.Attack += randomValue;

                    if (card.Attack > max)
                    {
                        card.Attack = max;
                    }

                    stringBuilder.AppendFormat("Wydarzenie: Zwiększenie ataku do {0}.\n", card.Attack);
                }
                break;

                case EventType.IncDef:
                {
                    var max = card.Rarity.GetDefenceMax();
                    card.Defence += randomValue;

                    if (card.Defence > max)
                    {
                        card.Defence = max;
                    }

                    stringBuilder.AppendFormat("Wydarzenie: Zwiększenie obrony do {0}.\n", card.Defence);
                }
                break;

                case EventType.MoreExp:
                {
                    var addExp = _randomNumberGenerator.GetRandomValue(1, 5);
                    card.ExperienceCount += addExp;

                    stringBuilder.AppendFormat("Wydarzenie: Dodatkowe punkty doświadczenia. (+{0} exp)", addExp);
                }
                break;

                case EventType.MoreItems:
                {
                    stringBuilder.Append("Wydarzenie: Dodatkowe przedmioty.\n");
                }
                break;

                case EventType.AddReset:
                {
                    ++card.RestartCount;
                    stringBuilder.Append("Wydarzenie: Zwiększenie ilości restartów karty.\n");
                }
                break;

                case EventType.ChangeDere:
                {
                    stringBuilder.Append("Wydarzenie: Zmiana dere na ");
                }
                break;

                case EventType.DecAff:
                {
                    card.Affection -= randomValue;
                    stringBuilder.Append("Wydarzenie: Zmniejszenie relacji.\n");
                }
                break;

                case EventType.DecAtk:
                {
                    var min = card.Rarity.GetAttackMin();
                    card.Attack -= randomValue;

                    if (card.Attack < min)
                    {
                        card.Attack = min;
                    }

                    stringBuilder.AppendFormat("Wydarzenie: Zmniejszenie ataku do {0}.\n", card.Attack);
                }
                break;

                case EventType.DecDef:
                {
                    var min = card.Rarity.GetDefenceMin();
                    card.Defence -= randomValue;

                    if (card.Defence < min)
                    {
                        card.Defence = min;
                    }

                    stringBuilder.AppendFormat("Wydarzenie: Zmniejszenie obrony do {0}.\n", card.Defence);
                }
                break;

                case EventType.Fight:
                {
                    var randomNumber = _randomNumberGenerator.GetRandomValue(1000);
                    var rarity = RarityExtensions.Random(randomNumber);
                    var name = "Miecu";
                    var title = "Bajeczka";
                        
                    var date = _systemClock.UtcNow;
                    var defence = _randomNumberGenerator.GetRandomValue(rarity.GetDefenceMin(), rarity.GetDefenceMax() + 1); ;
                    var attack = _randomNumberGenerator.GetRandomValue(rarity.GetAttackMin(), rarity.GetAttackMax() + 1);
                    var dere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
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

                    var result = FightWinnerExtensions.GetFightWinner(card, enemyCard);

                    var resStr = result == FightWinner.Card1 ? "zwycięstwo!" : "przegrana!";
                    stringBuilder.AppendFormat("Wydarzenie: Walka, wynik: {0}\n", resStr);
                    message = stringBuilder.ToString();
                    var eventResult = result == FightWinner.Card1;
                    return (eventResult, message);
                }

                case EventType.LoseCard:
                {
                        user.GameDeck.Cards.Remove(card);
                        stringBuilder.Append("Wydarzenie: Utrata karty.\n");
                        message = stringBuilder.ToString();
                    }

                return (false, message);

                default:
                return (true, message);
            }

            return (true, message);
        }

        public int GetMoreItems(EventType eventType)
        {
            switch (eventType)
            {
                case EventType.MoreItems:
                    return _randomNumberGenerator.GetRandomValue(2, 8);

                default:
                    return 0;
            }
        }
    }
}
