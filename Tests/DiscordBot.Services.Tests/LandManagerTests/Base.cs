using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DiscordBot.Services;
using Sanakan.DiscordBot.Services.Abstractions;
using Sanakan.Services;

namespace DiscordBot.ServicesTests.LandManagerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly ILandManager _landManager;

        public Base()
        {
            _landManager = new LandManager();
        }
    }
}
