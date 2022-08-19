namespace Sanakan.Game.Models
{
    public enum CardsPoolType : byte
    {
        Random = 0,

        /// <summary>
        /// Characters will be chosen by title identifier.
        /// </summary>
        Title = 1,

        /// <summary>
        /// Characters will be chosen by list identifier.
        /// </summary>
        List = 2
    }
}
