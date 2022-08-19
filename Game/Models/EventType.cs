namespace Sanakan.Game.Models
{
    public enum EventType : byte
    {
        MoreItems = 0,
        MoreExperience = 1,
        IncreaseAttack = 2,
        IncreaseDefence = 3,
        AddReset = 4,
        NewCard = 5,     // +

        None = 6,
        ChangeDere = 7,
        DecreaseAttack = 8,
        DecreaseDefence = 9,
        DecreaseAffection = 10,
        LoseCard = 11,
        Fight = 12 // -
    }
}
