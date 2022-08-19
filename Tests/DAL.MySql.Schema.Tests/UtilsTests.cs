using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Sanakan.DAL.MySql;
using Sanakan.DAL.MySql.Schema;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.MySql.Schema.Tests
{
    [TestClass]
    public class UtilsTests
    {
        [TestMethod]
        public async Task Should_Return_Queries()
        {
            var connectionMock = new Mock<IDbConnection>(MockBehavior.Strict);
            var commandMock = new Mock<IDbCommand>(MockBehavior.Strict);
            var dbDataReaderMock = new Mock<IDbDataReader>(MockBehavior.Strict);
            var expectedQuery = "select * from table";
            var streamReader1 = SetupTextReader(expectedQuery);
            var streamReader2 = SetupTextReader(Utils.Placeholder);

            connectionMock
                .Setup(pr => pr.CreateCommand())
                .Returns(commandMock.Object);

            commandMock
                .SetupSet(pr => pr.CommandText = It.IsAny<string>());

            commandMock
                .Setup(pr => pr.ExecuteReaderAsync())
                .ReturnsAsync(dbDataReaderMock.Object);

            dbDataReaderMock
                .SetupSequence(pr => pr.ReadAsync())
                .ReturnsAsync(true)
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            dbDataReaderMock
                .SetupSequence(pr => pr.GetTextReader(0))
                .Returns(streamReader1)
                .Returns(streamReader2);

            dbDataReaderMock
                .Setup(pr => pr.Dispose());

            var queries = await Utils.GetLastQueriesAsync(connectionMock.Object);
            queries.Should().NotBeNullOrEmpty();
        }

        [TestMethod]
        public async Task Should_Return_Table_Names()
        {
            var connectionMock = new Mock<IDbConnection>(MockBehavior.Strict);
            var commandMock = new Mock<IDbCommand>(MockBehavior.Strict);
            var dbDataReaderMock = new Mock<IDbDataReader>(MockBehavior.Strict);
            var expectedTableName = "table";

            connectionMock
                .Setup(pr => pr.CreateCommand())
                .Returns(commandMock.Object);

            commandMock
                .SetupSet(pr => pr.CommandText = It.IsAny<string>());

            commandMock
                .Setup(pr => pr.ExecuteReaderAsync())
                .ReturnsAsync(dbDataReaderMock.Object);

            dbDataReaderMock
                .SetupSequence(pr => pr.ReadAsync())
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            dbDataReaderMock
                .Setup(pr => pr.GetString(0))
                .Returns(expectedTableName);

            dbDataReaderMock
                .Setup(pr => pr.Dispose());

            var tableName = await Utils.GetTableNamesAsync(connectionMock.Object);
            tableName.Should().NotBeNullOrEmpty();
            tableName.First().Should().Be(expectedTableName);
        }

        private TextReader SetupTextReader(string text)
        {
            return new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(text)));
        }
    }
}
