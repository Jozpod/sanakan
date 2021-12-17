using System;

namespace Sanakan.Web.Models
{
    public class TokenData
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expire { get; set; }
    }
}