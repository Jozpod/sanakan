using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanakan.DAL.Models;
using Sanakan.DAL.Repositories.Abstractions;
using System.Threading.Tasks;

namespace Sanakan.DAL.Tests
{
    [TestClass]
    public class QuestionRepositoryTests : TestBase
    {
        [TestMethod]
        public async Task Should_Return_Question_By_Id()
        {
            var repository = ServiceProvider.GetRequiredService<IQuestionRepository>();
            var entity = new Question
            {
                Id = 1,
                Content = "test",
            };

            repository.Add(entity);

            await repository.SaveChangesAsync();

            var actual = await repository.GetByIdAsync(entity.Id);
            actual.Should().BeEquivalentTo(entity);
            
            repository.Remove(actual);
            await repository.SaveChangesAsync();

            actual = await repository.GetByIdAsync(entity.Id);
            actual.Should().BeNull();
        }
    }
}
