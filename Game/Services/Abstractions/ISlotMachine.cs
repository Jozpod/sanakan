using Sanakan.DAL.Models;

namespace Sanakan.Game.Services.Abstractions
{
    public interface ISlotMachine
    {
        long ToPay(SlotMachineConfig slotMachineConfig);

        long Play(User user);

        string Draw(User user);
    }
}
