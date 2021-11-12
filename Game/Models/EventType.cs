namespace Sanakan.Game.Models
{
    public enum EventType
    {
        MoreItems = 0,
        MoreExp = 1,
        IncAtk = 2,
        IncDef = 3,
        AddReset = 4,
        NewCard = 5,     // +
        
        None = 6,
        ChangeDere = 7,
        DecAtk = 8, 
        DecDef = 9,
        DecAff = 10,
        LoseCard = 11,
        Fight = 12 // -
    }
}
