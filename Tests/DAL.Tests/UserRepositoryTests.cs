using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class UserRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_CRUD_Entity()
        {
            var repository = ServiceProvider.GetRequiredService<IQuestionRepository>();

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
            actual.Should().BeNull();
        }
    }
}
