namespace Sanakan.Services.SlotMachine
{
    public class SlotEqualRandom : ISlotRandom
    {
        public int Next(int min, int max) 
            => Services.Fun.GetRandomValue(min, max);
    }
}