namespace Sanakan.Services.SlotMachine
{
    public interface ISlotRandom
    {
        int Next(int min, int max);
    }
}