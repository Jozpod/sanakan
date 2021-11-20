using System.Collections.Generic;
using Sanakan.Common;
using Sanakan.DAL.Models;
using Sanakan.Extensions;
using Sanakan.Game.Models;

namespace Sanakan.Game.Services
{
    public class SlotMachine
    {
        private readonly IRandomNumberGenerator _randomNumberGenerator;
        private const int Rows = 3;
        private const int Slots = 5;

        public SlotMachineSlot[,] Row { get; private set; }

        public SlotMachine(IRandomNumberGenerator randomNumberGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
            Row = new SlotMachineSlot[Rows, Slots];
        }

        public long ToPay(User user) => user.SMConfig.Beat.Value() 
            * user.SMConfig.Multiplier.Value() 
            * user.SMConfig.Rows.Value();

        private string RowIsSelected(User user, int index)
        {
            var nok = "✖";
            var ok = "✔";

            switch (user.SMConfig.Rows)
            {
                case SlotMachineSelectedRows.r3:
                {
                    return ok;
                }

                case SlotMachineSelectedRows.r2:
                {
                    if (index == 0 || index == 1)
                    {
                        return ok;
                    }
                    else
                    {
                        return nok;
                    }
                }

                case SlotMachineSelectedRows.r1:
                default:
                {
                    if (index == 1)
                    {
                        return ok;
                    }
                    else
                    {
                        return nok;
                    }
                }
            }
        }

        public string Draw(User user)
        {
            var rows = new string[Rows];

            for (var index = 0; index < Rows; index++)
            {
                rows[index] += RowIsSelected(user, index) + " ";
                for (var j = 0; j < Slots; j++)
                {
                    rows[index] += Row[index, j].Icon(user.SMConfig.PsayMode > 0);
                }
            }
            
            return string.Join("\n", rows);
        }

        public List<List<string>> DrawUCS(User user)
        {
            var ucs = new List<List<string>>();
            for (var i = 0; i < Rows; i++)
            {
                var tl = new List<string>();
                tl.Add(RowIsSelected(user, i) + " ");
                for (int j = 0; j < Slots; j++)
                    tl.Add(Row[i, j].Icon(user.SMConfig.PsayMode > 0));

                ucs.Add(tl);
            }
            return ucs;
        }

        public long Play(User user)
        {
            var slots = new List<SlotMachineWinSlots>();

            Randomize();
            var win = GetWin(user, ref slots);
            UpdateStats(user, win, slots);

            if (user.SMConfig.PsayMode > 0)
            {
                --user.SMConfig.PsayMode;
            }

            return win;
        }

        private void UpdateStats(User user, long win, List<SlotMachineWinSlots> slots)
        {
            ++user.Stats.SlotMachineGames;
            var scLost = ToPay(user) - win;

            if (scLost > 0)
            {
                user.Stats.ScLost += scLost;
                user.Stats.IncomeInSc -= scLost;
            }
            else
            {
                user.Stats.IncomeInSc += win;
            }
        }

        private long GetWin(User user, ref List<SlotMachineWinSlots> list)
        {
            long rBeat = user.SMConfig.Beat.Value() * user.SMConfig.Multiplier.Value();
            long win = 0;

            switch(user.SMConfig.Rows)
            {
                case SlotMachineSelectedRows.r3:
                    win += CheckRow(user, 2, rBeat, ref list);
                    goto case SlotMachineSelectedRows.r2;
                case SlotMachineSelectedRows.r2:
                    win += CheckRow(user, 0, rBeat, ref list);
                    goto case SlotMachineSelectedRows.r1;
                case SlotMachineSelectedRows.r1:
                default:
                    return win + CheckRow(user, 1, rBeat, ref list);
            }
        }

        private void Randomize()
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Slots; j++)
                {
                    Row[i, j] = Next(0, SlotMachineSlot.max.Value()).ToSMS();
                }
            }
        }

        public int Next(int min, int max)
        {
            double sum = 0;
            double rMax = 100;
            double[] chance = new double[max];
            for (int i = 0; i < (max + 1); i++) sum += i;
            for (int i = 0; i < max; i++) chance[i] = (max - i) * (rMax / sum);

            int low = 0;
            int high = 0;
            int next = _randomNumberGenerator.GetRandomValue(min, (int)(rMax * 10));
            for (int i = 0; i < max; i++)
            {
                if (i > 0) low = (int)(chance[i - 1] * 10);
                high += (int)(chance[i] * 10);

                if (next >= low && next < high)
                    return i;
            }
            return 0;
        }

        private long CheckRow(User user, int index, long beat, ref List<SlotMachineWinSlots> list)
        {
            int rt = 0;
            bool broken = false;
            SlotMachineSlot ft = Row[index, 0];
            for (int i = 0; i < Slots; i++)
            {
                if(ft == Row[index, i])
                {
                    if (!broken)
                    {
                        ++rt;
                    }
                }
                else
                {
                    broken = true;
                    if (rt < 3)
                    {
                        ft = Row[index, i];
                        broken = false;
                        rt = 1;
                    }
                }
            }
            list.Add(ft.WinType(rt));
            
            if (ft == SlotMachineSlot.q)
            {
                user.SMConfig.PsayMode += ft.WinValue(rt);
                return 0;
            }

            return ft.WinValue(rt, user.SMConfig.PsayMode > 0) * beat;
        }
    }
}