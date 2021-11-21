using Sanakan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.Game.Services.Abstractions
{
    public interface ISlotMachine
    {
        long ToPay(User user);

        long Play(User user);

        string Draw(User user);
    }
}
