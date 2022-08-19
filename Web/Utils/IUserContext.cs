namespace Sanakan.Web
{
    public interface IUserContext
    {
        /// <summary>
        /// Gets user identifier in Discord.
        /// </summary>
        ulong? DiscordId { get;  }

        bool HasWebpageClaim();
    }
}
