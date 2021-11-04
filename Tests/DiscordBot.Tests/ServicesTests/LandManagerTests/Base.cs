using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Services;

namespace DiscordBot.ServicesTests.LandManagerTests
{
    [TestClass]
    public abstract class Base
    {
        protected readonly LandManager _landManager;

        public Base()
        {
            _landManager = new();
        }
    }
}
