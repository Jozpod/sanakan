namespace Shinden.Models
{
    public interface ICharacterBiography : INationalText, IIndexable
    {
        ulong CharacterId { get; }
    }
}