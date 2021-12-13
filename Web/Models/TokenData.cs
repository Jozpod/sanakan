using System;

namespace Sanakan.Api.Models
{
    public class TokenData
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expire { get; set; }
    }
}