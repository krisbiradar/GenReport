using GenReport.Domain.DBContext;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Threading.Tasks;

namespace GenReport.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private ApplicationDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GenReportTestDb_" + System.Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ApplicationDbContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public async Task EnsureCreatedAsync_CreatesDatabase()
        {
            // Act
            var created = await _dbContext.Database.EnsureCreatedAsync();

            // Assert
            Assert.IsTrue(created, "Database should be created.");
        }

        [Test]
        public async Task EnsureDeletedAsync_DeletesDatabase()
        {
            // Arrange
            await _dbContext.Database.EnsureCreatedAsync();

            // Act
            var deleted = await _dbContext.Database.EnsureDeletedAsync();

            // Assert
            Assert.IsTrue(deleted, "Database should be deleted.");
        }
    }
}
