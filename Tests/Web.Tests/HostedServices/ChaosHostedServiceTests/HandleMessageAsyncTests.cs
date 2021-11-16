using Discord.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.Common;
using Sanakan.Configuration;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using Sanakan.ShindenApi;
using Sanakan.Web.Controllers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sanakan.Web.Tests.HostedServices.ChaosHostedServiceTests
{
    [TestClass]
    public class HandleMessageAsyncTests : Base
    {

        [TestMethod]
        public async Task Should_Swap_User_Nicknames()
        {
            _discordSocketClientAccessorMock.Raise(pr => pr.LoggedIn += null);
        }
    }
}
