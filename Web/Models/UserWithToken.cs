using Sanakan.DAL.Models;
using System;

namespace Sanakan.Api.Models
{
    /// <summary>
    /// User with token.
    /// </summary>
    public class UserWithToken
    {
        /// <summary>
        /// The json web token
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// The expire date
        /// </summary>
        public DateTime? Expire { get; set; }

        /// <summary>
        /// The user
        /// </summary>
        public User User { get; set; }
    }
}
