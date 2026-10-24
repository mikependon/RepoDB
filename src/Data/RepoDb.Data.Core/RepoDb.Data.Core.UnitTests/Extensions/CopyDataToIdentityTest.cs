#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Models;
using RepoDb.DbSettings;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class CopyDataToIdentityTest
    {
        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            DbSettingMapper.Add<FakeBulkOptionsDbConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<FakeBulkOptionsDbConnection>(new FakeDbHelper(), true);
            DbSettingMapper.Add<FakeBulkIdentityBehaviorDbConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<FakeBulkIdentityBehaviorDbConnection>(new FakeDbHelper(), true);
            FakeBulkOperations.Reset();
        }

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheIdentityBehaviorIsNotDefined()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                source.CopyDataTo("Person", "Customer", new FakeDbConnection(), identityBehavior: (LoadIdentityBehavior)99));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheIdentityBehaviorIsNotDefined()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                source.CopyDataToAsync("Person", "Customer", new FakeDbConnection(), identityBehavior: (LoadIdentityBehavior)99));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheIdentityValuesAreGeneratedWithTheRelatedTables()
        {
            // Setup
            var source = new FakeDbConnection();

            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                source.CopyDataTo("Person", "Person", new FakeDbConnection(), relationshipBehavior: CopyRelationshipBehavior.Parents, identityBehavior: LoadIdentityBehavior.Ignore));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToWithMappingsIfTheIdentityBehaviorIsNotDefined()
        {
            // Setup
            var source = new FakeDbConnection();
            var mappings = new[] { new CopyColumnMapping("Id", "Id") };

            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                source.CopyDataTo("Person", "Customer", new FakeDbConnection(), mappings, identityBehavior: (LoadIdentityBehavior)99));
        }

        #endregion

        #region Bulk Insert

        [TestMethod]
        public void TestCopyDataToKeepsTheIdentityValuesByDefaultInTheBulkInsert()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            source.CopyDataTo("Person", "Customer", new FakeBulkOptionsDbConnection());

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.KeepIdentity, FakeBulkOperations.Calls.Single().Options);
            CollectionAssert.AreEqual(new[] { "Id", "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public void TestCopyDataToDoesNotPassTheKeepIdentityOptionToTheBulkInsertIfTheIdentityValuesAreGenerated()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            source.CopyDataTo("Person", "Customer", new FakeBulkOptionsDbConnection(), identityBehavior: LoadIdentityBehavior.Ignore);

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.Default, FakeBulkOperations.Calls.Single().Options);
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncDoesNotSendTheIdentityColumnToTheBulkInsertIfTheIdentityValuesAreGenerated()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            await source.CopyDataToAsync("Person", "Customer", new FakeBulkIdentityBehaviorDbConnection(), identityBehavior: LoadIdentityBehavior.Ignore);

            // Assert
            Assert.IsNull(FakeBulkOperations.Calls.Single().IdentityBehavior);
            CollectionAssert.AreEqual(new[] { "Name", "Birthday" }, FakeBulkOperations.LastColumns);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncPassesTheKeepIdentityValueToTheBulkInsertByDefault()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(3) };

            // Act
            await source.CopyDataToAsync("Person", "Customer", new FakeBulkIdentityBehaviorDbConnection());

            // Assert
            Assert.AreEqual(FakeBulkImportIdentityBehavior.KeepIdentity, FakeBulkOperations.Calls.Single().IdentityBehavior);
        }

        #endregion

        #region Batch Insert

        [TestMethod]
        public void TestCopyDataToInsertsTheIdentityColumnByDefaultInTheBatchInsert()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(2) };
            var destination = new FakeDbConnection();

            // Act
            source.CopyDataTo("Person", "Customer", destination);

            // Assert
            var execution = destination.Executions.First(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal));
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(execution.CommandText, @"\bId\b"));
            Assert.AreEqual(3, execution.Parameters.Count);
        }

        [TestMethod]
        public void TestCopyDataToLeavesTheIdentityColumnToTheDatabaseInTheBatchInsertIfTheIdentityValuesAreGenerated()
        {
            // Setup
            var source = new FakeDbConnection { Data = FakeDbConnection.CreatePersons(2) };
            var destination = new FakeDbConnection();

            // Act
            source.CopyDataTo("Person", "Customer", destination, identityBehavior: LoadIdentityBehavior.Ignore);

            // Assert
            var executions = destination.Executions.Where(e => e.CommandText.StartsWith("INSERT INTO", StringComparison.Ordinal)).ToList();
            Assert.AreEqual(2, executions.Count);
            Assert.IsTrue(executions.All(e => !System.Text.RegularExpressions.Regex.IsMatch(e.CommandText, @"\bId\b")));
            Assert.IsTrue(executions.All(e => e.Parameters.Count == 2));
        }

        #endregion
    }
}
