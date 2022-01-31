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

        public EventType RandomizeEvent(ExpeditionCardType expedition, (double, double) duration)
        {
            var timeBased = CheckChanceBasedOnTime(expedition, duration);

            if (timeBased != EventType.None)
            {
                return timeBased;
            }

            var chance = Constants.ChanceOfEvent[expedition];

            return _randomNumberGenerator.GetRandomValue(10000) switch
            {
                int n when n < chance[EventType.MoreItems].Item2
                    && n >= chance[EventType.MoreItems].Item1 => EventType.MoreItems,
                int n when n < chance[EventType.MoreExperience].Item2
                    && n >= chance[EventType.MoreExperience].Item1 => EventType.MoreExperience,
                int n when n < chance[EventType.IncreaseAttack].Item2
                    && n >= chance[EventType.IncreaseAttack].Item1 => EventType.IncreaseAttack,
                int n when n < chance[EventType.IncreaseDefence].Item2
                    && n >= chance[EventType.IncreaseDefence].Item1 => EventType.IncreaseDefence,
                int n when n < chance[EventType.AddReset].Item2
                    && n >= chance[EventType.AddReset].Item1 => EventType.AddReset,
                int n when n < chance[EventType.NewCard].Item2
                    && n >= chance[EventType.NewCard].Item1 => EventType.NewCard,
                int n when n < chance[EventType.ChangeDere].Item2
                    && n >= chance[EventType.ChangeDere].Item1 => EventType.ChangeDere,
                int n when n < chance[EventType.DecreaseAttack].Item2
                    && n >= chance[EventType.DecreaseAttack].Item1 => EventType.DecreaseAttack,
                int n when n < chance[EventType.DecreaseDefence].Item2
                    && n >= chance[EventType.DecreaseDefence].Item1 => EventType.DecreaseDefence,
                int n when n < chance[EventType.DecreaseAffection].Item2
                    && n >= chance[EventType.DecreaseAffection].Item1 => EventType.DecreaseAffection,
                int n when n < chance[EventType.LoseCard].Item2
                    && n >= chance[EventType.LoseCard].Item1 => EventType.LoseCard,
                int n when n < chance[EventType.Fight].Item2
                    && n >= chance[EventType.Fight].Item1 => EventType.Fight,
                _ => EventType.None,
            };
        }

        public bool ExecuteEvent(
            EventType eventType,
            User user,
            Card card,
            StringBuilder stringBuilder,
            double totalExperience)
        {
            var randomValue = _randomNumberGenerator.GetRandomValue(1, 4);
            int defencePoints;
            int attackPoints;

            switch (eventType)
            {
                case EventType.NewCard:
                    var boosterPack = new BoosterPack
                    {
                        RarityExcludedFromPack = new List<RarityExcluded>(),
                        TitleId = _randomNumberGenerator.GetOneRandomFrom(Constants.CharacterTitleIds),
                        Characters = new List<BoosterPackCharacter>(),
                        CardSourceFromPack = CardSource.Expedition,
                        Name = BoosterPackTypes.Adventure,
                        IsCardFromPackTradable = true,
                        MinRarity = Rarity.E,
                        CardCount = 1,
                    };

                    user.GameDeck.BoosterPacks.Add(boosterPack);
                    stringBuilder.AppendLine("Wydarzenie: Pakiet z kartą.");
                    break;

                case EventType.IncreaseAttack:
                    card.IncreaseAttack(randomValue);
                    var attack = card.GetAttackWithBonus();
                    stringBuilder.AppendFormat("Wydarzenie: Zwiększenie ataku do {0}.\n", attack);
                    break;

                case EventType.IncreaseDefence:
                    card.IncreaseDefence(randomValue);
                    defencePoints = card.GetDefenceWithBonus();
                    stringBuilder.AppendFormat("Wydarzenie: Zwiększenie obrony do {0}.\n", defencePoints);
                    break;

                case EventType.MoreExperience:
                    var experienceToAdd = _randomNumberGenerator.GetRandomValue(1, 5);
                    card.ExperienceCount += experienceToAdd;
                    stringBuilder.AppendFormat("Wydarzenie: Dodatkowe punkty doświadczenia. (+{0} exp)", experienceToAdd);
                    break;

                case EventType.MoreItems:
                    stringBuilder.Append("Wydarzenie: Dodatkowe przedmioty.\n");
                    break;

                case EventType.AddReset:
                    ++card.RestartCount;
                    stringBuilder.Append("Wydarzenie: Zwiększenie ilości restartów karty.\n");
                    break;

                case EventType.ChangeDere:
                    stringBuilder.Append("Wydarzenie: Zmiana dere na ");
                    card.Dere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
                    stringBuilder.AppendFormat("{0}\n", card.Dere);
                    break;

                case EventType.DecreaseAffection:
                    card.Affection -= randomValue;
                    stringBuilder.Append("Wydarzenie: Zmniejszenie relacji.\n");
                    break;

                case EventType.DecreaseAttack:
                    card.DecreaseAttack(randomValue);
                    attackPoints = card.GetAttackWithBonus();
                    stringBuilder.AppendFormat("Wydarzenie: Zmniejszenie ataku do {0}.\n", attackPoints);
                    break;

                case EventType.DecreaseDefence:
                    card.DecreaseDefence(randomValue);
                    defencePoints = card.GetDefenceWithBonus();
                    stringBuilder.AppendFormat("Wydarzenie: Zmniejszenie obrony do {0}.\n", defencePoints);
                    break;

                case EventType.Fight:
                    var randomNumber = _randomNumberGenerator.GetRandomValue(1000);
                    var rarity = RarityExtensions.Random(randomNumber);
                    var name = "Miecu";
                    var title = "Bajeczka";

                    var date = _systemClock.UtcNow;
                    defencePoints = _randomNumberGenerator.GetRandomValue(rarity.GetDefenceMin(), rarity.GetDefenceMax() + 1);
                    attackPoints = _randomNumberGenerator.GetRandomValue(rarity.GetAttackMin(), rarity.GetAttackMax() + 1);
                    var dere = _randomNumberGenerator.GetOneRandomFrom(DereExtensions.ListOfDeres);
                    var characterId = 1ul;

                    var enemyCard = new Card(
                        characterId,
                        title,
                        name,
                        attackPoints,
                        defencePoints,
                        rarity,
                        dere,
                        date);

                    card.CalculateCardPower();
                    var fightResult = FightWinnerExtensions.GetFightWinner(card, enemyCard);
                    var resStr = fightResult == FightWinner.Card1 ? "zwycięstwo!" : "przegrana!";
                    stringBuilder.AppendFormat("Wydarzenie: Walka, wynik: {0}\n", resStr);
                    var eventResult = fightResult == FightWinner.Card1;
                    return eventResult;

                case EventType.LoseCard:
                    user.GameDeck.Cards.Remove(card);
                    stringBuilder.Append("Wydarzenie: Utrata karty.\n");
                    user.StoreExperience(totalExperience);
                    return false;
                default:
                    return true;
            }

            return true;
        }

        public int GetMoreItems(EventType eventType) => eventType switch
        {
            EventType.MoreItems => _randomNumberGenerator.GetRandomValue(2, 8),
            _ => 0,
        };

        private EventType CheckChanceBasedOnTime(ExpeditionCardType expedition, (double, double) duration)
        {
            switch (expedition)
            {
                case ExpeditionCardType.ExtremeItemWithExp:
                    if ((duration.Item1 > TimeSpan.FromMinutes(45).TotalMinutes
                        || duration.Item2 > Durations.FourHours.TotalMinutes)
                        && _randomNumberGenerator.TakeATry(2))
                    {
                        return EventType.LoseCard;
                    }

                    return EventType.None;

                default:
                    return EventType.None;
            }
        }
    }
}
