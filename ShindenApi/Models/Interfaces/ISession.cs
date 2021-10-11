using System;

namespace Shinden.Models
{
    public interface ISession
    {
         string Id { get; }
         string Name { get; }
         string Hash { get; }
         DateTime Expires { get; }

         bool IsValid();
         UserAuth GetAuth();
         void Renew(ISession session);
    }
}