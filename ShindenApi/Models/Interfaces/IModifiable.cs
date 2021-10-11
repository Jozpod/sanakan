using System;
using System.Threading.Tasks;

namespace Shinden.Models
{
    public interface IModifiable<T>
    {
         Task ModifyAsync(Action<T> func);
    }
}