using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.IntegrationTests.Models;
using RepoDb.IntegrationTests.Setup;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb.IntegrationTests
{
    [TestClass]
    public class QueryMultipleTest
    {
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            TypeMapper.AddMap(typeof(DateTime), DbType.DateTime2, true);
            SetupHelper.InitDatabase();
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            SetupHelper.CleanDatabase();
        }

        [TestMethod]
        public void TestQueryMultiple()
        {
            // Setup
            var dog = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Dog"
            };
            var cat = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Cat"
            };
            var lion = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Lion"
            };

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                connection.Insert(dog);
                connection.Insert(cat);
                connection.Insert(lion);

                // Act (Query)
                var result = connection.ExecuteQueryMultiple(@"
                    SELECT * FROM [dbo].[Animal] WHERE Id = @Id;
                    SELECT * FROM [dbo].[Animal];
                    SELECT COUNT(*) FROM [dbo].[Animal];", new { dog.Id });

                // Act (Extract)
                var dogs = result.Extract<Animal>();
                result.NextResult();
                var animals = result.Extract<Animal>();
                result.NextResult();
                var count = (int)result.Scalar();

                // Assert (Insert)
                Assert.AreEqual(dogs.First().Id, dog.Id);
                Assert.AreEqual(animals.Count(), 3);
                Assert.AreEqual(count, 3);
            }
        }

        [TestMethod]
        public void TestQueryMultipleUsingNextMethods()
        {
            // Setup
            var dog = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Dog"
            };
            var cat = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Cat"
            };
            var lion = new Animal
            {
                Id = Guid.NewGuid(),
                Name = "Lion"
            };

            using (var connection = new SqlConnection(Constants.TestDatabase))
            {
                // Act (Insert)
                connection.DeleteAll<Animal>();
                connection.Insert(dog);
                connection.Insert(cat);
                connection.Insert(lion);

                // Act (Query)
                var result = connection.ExecuteQueryMultiple(@"
                    SELECT * FROM [dbo].[Animal] WHERE Id = @Id;
                    SELECT * FROM [dbo].[Animal];
                    SELECT COUNT(*) FROM [dbo].[Animal];", new { dog.Id });

                // Act (Extract)
                var dogs = result.Extract<Animal>();
                var animals = result.ExtractNext<Animal>();
                var count = (int)result.ScalarNext();

                // Assert (Insert)
                Assert.AreEqual(dogs.First().Id, dog.Id);
                Assert.AreEqual(animals.Count(), 3);
                Assert.AreEqual(count, 3);
            }
        }
    }
}