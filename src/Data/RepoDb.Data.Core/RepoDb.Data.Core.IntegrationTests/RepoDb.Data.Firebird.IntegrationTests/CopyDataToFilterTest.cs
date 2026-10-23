#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Firebird.IntegrationTests.Entities;
using RepoDb.Data.Firebird.IntegrationTests.Setup;
using RepoDb.Enumerations;
using RepoDb.Schema;

namespace RepoDb.Data.Firebird.IntegrationTests
{
    /// <summary>
    /// Copies the rows of a table that match a filter (a <see cref="QueryGroup"/> or an expression).
    /// </summary>
    [TestClass]
    public class CopyDataToFilterTest
    {
        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
            Cleanup();
        }

        [TestCleanup]
        public void Cleanup()
        {
            SchemaReaderMapper.Clear();
            Database.Cleanup();
        }

        #region Helpers

        private static void CreatePersonTableWithoutKeys() =>
            Helper.ExecuteOnTarget(@"CREATE TABLE ""Person""
                (
                    ""Id"" INTEGER NOT NULL,
                    ""Name"" VARCHAR(128) NOT NULL,
                    ""Age"" INTEGER,
                    ""CountryId"" INTEGER,
                    ""Salary"" DECIMAL(18, 2),
                    ""Birthday"" DATE,
                    ""IsActive"" BOOLEAN NOT NULL,
                    ""CreatedDateUtc"" TIMESTAMP NOT NULL,
                    CONSTRAINT ""PK_Person"" PRIMARY KEY (""Id"")
                );");

        private static void AssertNumbers(string where,
            int actual)
        {
            var expected = Helper.SourceRows("\"Number\"", "\"Value\"", where);
            Assert.AreEqual(expected.Count, actual);
            Helper.AssertRowsEqual(expected, Helper.TargetRows("\"Number\"", "\"Value\""));
        }

        private static void AssertPersons(string where,
            int actual)
        {
            var expected = Helper.SourceRows("\"Person\"", "\"Id\"", where);
            Assert.AreEqual(expected.Count, actual);
            Helper.AssertRowsEqual(expected, Helper.TargetRows("\"Person\"", "\"Id\""));
        }

        #endregion

        #region QueryGroup

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAnEqualFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", 7))).RowCount;

                // Assert
                AssertNumbers("\"Value\" = 7", actual);
                Assert.AreEqual(1, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithANotEqualFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.NotEqual, 7))).RowCount;

                // Assert
                AssertNumbers("\"Value\" <> 7", actual);
                Assert.AreEqual(2499, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAGreaterThanFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.GreaterThan, 2490))).RowCount;

                // Assert
                AssertNumbers("\"Value\" > 2490", actual);
                Assert.AreEqual(10, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAGreaterThanOrEqualFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.GreaterThanOrEqual, 2490))).RowCount;

                // Assert
                AssertNumbers("\"Value\" >= 2490", actual);
                Assert.AreEqual(11, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithALessThanFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.LessThan, 5))).RowCount;

                // Assert
                AssertNumbers("\"Value\" < 5", actual);
                Assert.AreEqual(4, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithALessThanOrEqualFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.LessThanOrEqual, 5))).RowCount;

                // Assert
                AssertNumbers("\"Value\" <= 5", actual);
                Assert.AreEqual(5, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithABetweenFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.Between, new[] { 10, 20 }))).RowCount;

                // Assert
                AssertNumbers("\"Value\" BETWEEN 10 AND 20", actual);
                Assert.AreEqual(11, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithANotBetweenFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.NotBetween, new[] { 10, 2490 }))).RowCount;

                // Assert
                AssertNumbers("\"Value\" NOT BETWEEN 10 AND 2490", actual);
                Assert.AreEqual(19, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnInFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.In, new[] { 1, 5, 9, 99999 }))).RowCount;

                // Assert
                AssertNumbers("\"Value\" IN (1, 5, 9, 99999)", actual);
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithANotInFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.NotIn, new[] { 1, 5, 9 }))).RowCount;

                // Assert
                AssertNumbers("\"Value\" NOT IN (1, 5, 9)", actual);
                Assert.AreEqual(2497, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithALikeFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Label", Operation.Like, "Number 12%"))).RowCount;

                // Assert
                AssertNumbers("\"Label\" LIKE 'Number 12%'", actual);
                Assert.IsTrue(actual > 0);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithANotLikeFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Label", Operation.NotLike, "Number 1%"))).RowCount;

                // Assert
                AssertNumbers("\"Label\" NOT LIKE 'Number 1%'", actual);
                Assert.IsTrue(actual > 0);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnIsNullFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", null))).RowCount;

                // Assert
                AssertPersons("\"Age\" IS NULL", actual);
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Carol", Helper.TargetRows("\"Person\"", "\"Id\"").Single()["Name"]);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnIsNotNullFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Age", Operation.NotEqual, null))).RowCount;

                // Assert
                AssertPersons("\"Age\" IS NOT NULL", actual);
                Assert.AreEqual(5, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnAndFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();
                var where = new QueryGroup(new[]
                {
                    new QueryField("Age", Operation.GreaterThan, 30),
                    new QueryField("IsActive", true)
                });

                // Act
                var actual = source.CopyDataTo("Person", target, where: where).RowCount;

                // Assert
                AssertPersons("\"Age\" > 30 AND \"IsActive\" = TRUE", actual);
                CollectionAssert.AreEqual(new[] { "Bob", "Frank" }, Helper.TargetRows("\"Person\"", "\"Id\"").Select(r => (string)r["Name"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnOrFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();
                var where = new QueryGroup(new[]
                {
                    new QueryField("Name", "Alice"),
                    new QueryField("Age", Operation.GreaterThan, 50)
                }, Conjunction.Or);

                // Act
                var actual = source.CopyDataTo("Person", target, where: where).RowCount;

                // Assert
                AssertPersons("\"Name\" = 'Alice' OR \"Age\" > 50", actual);
                CollectionAssert.AreEqual(new[] { "Alice", "Frank" }, Helper.TargetRows("\"Person\"", "\"Id\"").Select(r => (string)r["Name"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithNestedFilters()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();
                var where = new QueryGroup(new[] { new QueryField("IsActive", true) },
                    new[] { new QueryGroup(new[] { new QueryField("Age", Operation.LessThan, 25), new QueryField("Age", Operation.GreaterThan, 50) }, Conjunction.Or) });

                // Act
                var actual = source.CopyDataTo("Person", target, where: where).RowCount;

                // Assert
                AssertPersons("\"IsActive\" = TRUE AND (\"Age\" < 25 OR \"Age\" > 50)", actual);
                CollectionAssert.AreEqual(new[] { "Dave", "Frank" }, Helper.TargetRows("\"Person\"", "\"Id\"").Select(r => (string)r["Name"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAFilterOfAStringValue()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, where: new QueryGroup(new QueryField("Code", "EUR"))).RowCount;

                // Assert
                Assert.AreEqual(1, actual);
                Assert.AreEqual("Euro", Helper.TargetRows("\"Currency\"", "\"Code\"").Single()["Name"]);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAFilterOfADateValue()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Birthday", Operation.GreaterThanOrEqual, new DateTime(1990, 1, 15)))).RowCount;

                // Assert
                AssertPersons("\"Birthday\" >= '1990-01-15'", actual);
                CollectionAssert.AreEqual(new[] { "Alice", "Dave" }, Helper.TargetRows("\"Person\"", "\"Id\"").Select(r => (string)r["Name"]).ToArray());
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAFilterOfADecimalValue()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo("Person", target, where: new QueryGroup(new QueryField("Salary", Operation.GreaterThan, 1000.50m))).RowCount;

                // Assert
                AssertPersons("\"Salary\" > 1000.50", actual);
                Assert.AreEqual(2, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAFilterThatMatchesAllTheRows()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.GreaterThan, 0))).RowCount;

                // Assert
                Assert.AreEqual(2500, actual);
                Assert.AreEqual(2500, Helper.TargetCount("\"Number\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAFilterThatMatchesNoRowsCopiesNothing()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", Operation.GreaterThan, 99999))).RowCount;

                // Assert
                Assert.AreEqual(0, actual);
                Assert.AreEqual(0, Helper.TargetCount("\"Number\""));
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAFilterWithTextThatLooksLikeSqlTreatsItAsAValue()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Currency");

                // Act
                var actual = source.CopyDataTo("Currency", target, where: new QueryGroup(new QueryField("Name", "x'; DROP TABLE \"Currency\"; --"))).RowCount;

                // Assert
                Assert.AreEqual(0, actual);
                Assert.AreEqual(3, Helper.SourceCount("\"Currency\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheFilterIsOfAColumnThatDoesNotExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act/Assert
                Assert.Throws<FbException>(() => source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Missing", 1))));
                Assert.AreEqual(0, Helper.TargetCount("\"Number\""));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithAFilterOfAValueThatIsNotOfTheTypeOfTheColumn()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act/Assert
                Assert.Throws<FormatException>(() => source.CopyDataTo("Number", target, where: new QueryGroup(new QueryField("Value", "not a number"))));
                Assert.AreEqual(0, Helper.TargetCount("\"Number\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithABetweenFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = (await source.CopyDataToAsync("Number", target, where: new QueryGroup(new QueryField("Value", Operation.Between, new[] { 10, 20 })))).RowCount;

                // Assert
                AssertNumbers("\"Value\" BETWEEN 10 AND 20", actual);
                Assert.AreEqual(11, actual);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAnInFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = (await source.CopyDataToAsync("Number", target, where: new QueryGroup(new QueryField("Value", Operation.In, new[] { 1, 5, 9 })))).RowCount;

                // Assert
                AssertNumbers("\"Value\" IN (1, 5, 9)", actual);
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAnIsNullFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = (await source.CopyDataToAsync("Person", target, where: new QueryGroup(new QueryField("Age", null)))).RowCount;

                // Assert
                AssertPersons("\"Age\" IS NULL", actual);
                Assert.AreEqual(1, actual);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAnOrFilter()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();
                var where = new QueryGroup(new[]
                {
                    new QueryField("Name", "Alice"),
                    new QueryField("Age", Operation.GreaterThan, 50)
                }, Conjunction.Or);

                // Act
                var actual = (await source.CopyDataToAsync("Person", target, where: where)).RowCount;

                // Assert
                AssertPersons("\"Name\" = 'Alice' OR \"Age\" > 50", actual);
                Assert.AreEqual(2, actual);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAFilterThatMatchesNoRowsCopiesNothing()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act
                var actual = (await source.CopyDataToAsync("Number", target, where: new QueryGroup(new QueryField("Value", Operation.GreaterThan, 99999)))).RowCount;

                // Assert
                Assert.AreEqual(0, actual);
                Assert.AreEqual(0, Helper.TargetCount("\"Number\""));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheFilterIsOfAColumnThatDoesNotExist()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                Helper.CreateTargetTables("Number");

                // Act/Assert
                await Assert.ThrowsAsync<FbException>(() => source.CopyDataToAsync("Number", target, where: new QueryGroup(new QueryField("Missing", 1))));
            }
        }

        #endregion

        #endregion

        #region Expression

        #region Sync

        [TestMethod]
        public void TestCopyDataToWithAnExpressionOfAGreaterThanComparison()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo<Person>(target, p => p.Age > 30).RowCount;

                // Assert
                AssertPersons("\"Age\" > 30", actual);
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionOfAnAndCondition()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo<Person>(target, p => p.Age > 30 && p.IsActive == true).RowCount;

                // Assert
                AssertPersons("\"Age\" > 30 AND \"IsActive\" = TRUE", actual);
                Assert.AreEqual(2, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionOfAnOrCondition()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo<Person>(target, p => p.Name == "Alice" || p.Name == "Eve").RowCount;

                // Assert
                AssertPersons("\"Name\" IN ('Alice', 'Eve')", actual);
                Assert.AreEqual(2, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionOfANullComparison()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo<Person>(target, p => p.Salary == null).RowCount;

                // Assert
                AssertPersons("\"Salary\" IS NULL", actual);
                Assert.AreEqual(2, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionOfAContainsOfAList()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();
                var ids = new[] { 1, 3, 5 };

                // Act
                var actual = source.CopyDataTo<Person>(target, p => ids.Contains(p.Id)).RowCount;

                // Assert
                AssertPersons("\"Id\" IN (1, 3, 5)", actual);
                Assert.AreEqual(3, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionOfAVariable()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();
                var minimumAge = 40;

                // Act
                var actual = source.CopyDataTo<Person>(target, p => p.Age >= minimumAge).RowCount;

                // Assert
                AssertPersons("\"Age\" >= 40", actual);
                Assert.AreEqual(2, actual);
            }
        }

        [TestMethod]
        public void TestCopyDataToWithAnExpressionThatMatchesNoRowsCopiesNothing()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = source.CopyDataTo<Person>(target, p => p.Age > 1000).RowCount;

                // Assert
                Assert.AreEqual(0, actual);
                Assert.AreEqual(0, Helper.TargetCount("\"Person\""));
            }
        }

        #endregion

        #region Async

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAnExpressionOfAnAndCondition()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();

                // Act
                var actual = (await source.CopyDataToAsync<Person>(target, p => p.Age > 30 && p.IsActive == true)).RowCount;

                // Assert
                AssertPersons("\"Age\" > 30 AND \"IsActive\" = TRUE", actual);
                Assert.AreEqual(2, actual);
            }
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithAnExpressionOfAContainsOfAList()
        {
            using (var source = new FbConnection(Database.ConnectionStringForSource))
            using (var target = new FbConnection(Database.ConnectionStringForTarget))
            {
                // Setup
                CreatePersonTableWithoutKeys();
                var ids = new[] { 2, 4 };

                // Act
                var actual = (await source.CopyDataToAsync<Person>(target, p => ids.Contains(p.Id))).RowCount;

                // Assert
                AssertPersons("\"Id\" IN (2, 4)", actual);
                Assert.AreEqual(2, actual);
            }
        }

        #endregion

        #endregion
    }
}
