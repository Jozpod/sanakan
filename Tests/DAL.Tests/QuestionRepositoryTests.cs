using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.Configuration;
using Sanakan.DAL.Builder;
using Sanakan.DAL.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class QuestionRepositoryTests
    {
        private SanakanDbContext _dbContext;
        private ServiceProvider _serviceProvider;

        public QuestionRepositoryTests()
        {
            var serviceCollection = new ServiceCollection();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            var configurationRoot = builder.Build();
            
            serviceCollection.AddOptions();
            serviceCollection.AddDbContext<SanakanDbContext>();
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.Configure<SanakanConfiguration>(configurationRoot);
            serviceCollection.AddRepositories();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        [TestInitialize()]
        public async Task TestInitialize() {
            _dbContext = _serviceProvider.GetRequiredService<SanakanDbContext>();
            await _dbContext.Database.EnsureCreatedAsync();
        }

        [TestCleanup()]
        public async Task TestCleanup() {
            if (_dbContext != null)
            {
                await _dbContext.Database.EnsureDeletedAsync();
            }
        }

        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = _serviceProvider.GetRequiredService<IQuestionRepository>();

            var question = new Models.Question
            {
                Id = 1,
                Content = "test",
            };

            repository.Add(question);

            await repository.SaveChangesAsync();

            var actual = await repository.GetByIdAsync(question.Id);
            actual.Should().BeEquivalentTo(question);
            
            repository.Remove(actual);
            await repository.SaveChangesAsync();

            actual = await repository.GetByIdAsync(question.Id);
            actual.Should().BeEquivalentTo(question);
        }
    }
}
