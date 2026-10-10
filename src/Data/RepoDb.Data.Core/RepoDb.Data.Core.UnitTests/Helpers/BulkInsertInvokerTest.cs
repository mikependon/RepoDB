#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Helpers;
using RepoDb.Interfaces;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class BulkInsertInvokerTest
    {
        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            FakeBulkOperations.Reset();
            DbSettingMapper.Add<FakeBulkSettingDbConnection>(new FakeBulkDbSetting(), true);
            DbSettingMapper.Add<FakeBulkSettingThrowingDbConnection>(new FakeBulkDbSetting(), true);
            DbSettingMapper.Add<FakeBulkLimitedSettingDbConnection>(new FakeBulkLimitedDbSetting(), true);
        }

        #endregion

        #region Classes

        private sealed class UncachedDbConnection : FakeBulkConnectionBase
        {
        }

        #endregion

        #region Helpers

        private static IDataReader GetReader(int rows = 3) =>
            FakeDbConnection.CreatePersons(rows).CreateDataReader();

        #endregion

        #region IsAvailable

        [TestMethod]
        public void TestIsAvailableIsTrueIfTheBulkOperationsOfTheConnectionHaveABulkInsert()
        {
            // Act/Assert
            Assert.IsTrue(BulkInsertInvoker.IsAvailable(new FakeBulkDbConnection(), GetReader(), false));
        }

        [TestMethod]
        public void TestIsAvailableIsTrueIfTheBulkOperationsOfTheConnectionHaveABulkInsertAsync()
        {
            // Act/Assert
            Assert.IsTrue(BulkInsertInvoker.IsAvailable(new FakeBulkDbConnection(), GetReader(), true));
        }

        [TestMethod]
        public void TestIsAvailableIsFalseIfThereAreNoBulkOperationsForTheConnection()
        {
            // Act/Assert
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeDbConnection(), GetReader(), false));
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeDbConnection(), GetReader(), true));
        }

        [TestMethod]
        public void TestIsAvailableIsFalseIfTheBulkOperationsOfTheConnectionHaveOnlyMethodsThatAreNotBulkInsertExtensions()
        {
            // Act/Assert
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeBulkDecoyDbConnection(), GetReader(), false));
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeBulkDecoyDbConnection(), GetReader(), true));
        }

        [TestMethod]
        public void TestIsAvailableIsFalseIfOnlyTheSynchronousBulkInsertExists()
        {
            // Act/Assert
            Assert.IsTrue(BulkInsertInvoker.IsAvailable(new FakeBulkTypedDbConnection(), GetReader(), false));
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeBulkTypedDbConnection(), GetReader(), true));
        }

        [TestMethod]
        public void TestIsAvailableFindsTheBulkInsertOfTheBaseTypeOfTheConnection()
        {
            // Act/Assert
            Assert.IsTrue(BulkInsertInvoker.IsAvailable(new FakeBulkDerivedDbConnection(), GetReader(), false));
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeBulkDerivedDbConnection(), GetReader(), true));
        }

        [TestMethod]
        public void TestIsAvailableIgnoresABulkOperationsFileThatIsNotAnAssembly()
        {
            // Setup
            var file = Path.Combine(AppContext.BaseDirectory, "RepoDb.Data.Core.UnitTests.Corrupt.BulkOperations.dll");
            File.WriteAllText(file, "This is not an assembly.");

            try
            {
                // Act
                var actual = BulkInsertInvoker.IsAvailable(new UncachedDbConnection(), GetReader(), false);

                // Assert
                Assert.IsFalse(actual);
            }
            finally
            {
                File.Delete(file);
            }
        }

        [TestMethod]
        public void TestIsAvailableReturnsTheSameAnswerWhenCalledAgain()
        {
            // Act/Assert
            Assert.IsTrue(BulkInsertInvoker.IsAvailable(new FakeBulkDbConnection(), GetReader(), false));
            Assert.IsTrue(BulkInsertInvoker.IsAvailable(new FakeBulkDbConnection(), GetReader(), false));
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeDbConnection(), GetReader(), false));
            Assert.IsFalse(BulkInsertInvoker.IsAvailable(new FakeDbConnection(), GetReader(), false));
        }

        #endregion

        #region BulkInsert

        [TestMethod]
        public void TestBulkInsertReturnsTheNumberOfInsertedRows()
        {
            // Act
            var actual = BulkInsertInvoker.BulkInsert(new FakeBulkDbConnection(), "Person", GetReader(4), 100, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            Assert.AreEqual(4, actual);
        }

        [TestMethod]
        public void TestBulkInsertGivesTheArgumentsByTheirNames()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            var trace = new Mock<ITrace>().Object;

            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, 45, "MyTraceKey", trace, transaction);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert", call.Method);
            Assert.AreEqual("Person", call.TableName);
            Assert.AreEqual(250, call.BatchSize);
            Assert.AreEqual(45, call.Timeout);
            Assert.AreEqual("MyTraceKey", call.TraceKey);
            Assert.AreSame(trace, call.Trace);
            Assert.AreSame(transaction, call.Transaction);
            Assert.AreEqual(3, call.RowCount);
        }

        [TestMethod]
        public void TestBulkInsertGivesTheDefaultValuesOfTheArgumentsThatItDoesNotKnow()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual(3, call.Retries);
            Assert.AreEqual(0L, call.Skipped);
            Assert.AreEqual("FakeTraceKey", call.TraceKey);
            Assert.IsNull(call.Timeout);
            Assert.IsNull(call.Trace);
            Assert.IsNull(call.Transaction);
        }

        [TestMethod]
        public void TestBulkInsertDoesNotGiveATransactionOrATraceOfAnotherTypeThanTheArgument()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            var trace = new Mock<ITrace>().Object;

            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkTypedDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, trace, transaction);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert (typed)", call.Method);
            Assert.IsNull(call.Transaction);
            Assert.IsNull(call.Trace);
        }

        [TestMethod]
        public void TestBulkInsertGivesATransactionThatIsOfTheTypeOfTheArgument()
        {
            // Setup
            var transaction = new FakeDbTransaction();

            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkTypedDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, transaction);

            // Assert
            Assert.AreSame(transaction, FakeBulkOperations.Calls.Single().Transaction);
        }

        [TestMethod]
        public void TestBulkInsertKeepsTheIdentityValuesIfTheOptionsHaveTheKeepIdentityOption()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkOptionsDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.KeepIdentity, FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public async Task TestBulkInsertAsyncKeepsTheIdentityValuesIfTheOptionsHaveTheKeepIdentityOption()
        {
            // Act
            await BulkInsertInvoker.BulkInsertAsync(new FakeBulkOptionsDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.KeepIdentity, FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public void TestBulkInsertKeepsTheOtherDefaultOptionsWhenItKeepsTheIdentityValues()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkDefaultOptionsDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.TableLock | FakeBulkCopyOptions.KeepIdentity, FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public void TestBulkInsertKeepsTheIdentityValuesIfTheOptionsAreNullable()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkNullableOptionsDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            Assert.AreEqual(FakeBulkCopyOptions.KeepIdentity, FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public void TestBulkInsertDoesNotChangeTheOptionsThatHaveNoKeepIdentityOption()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkOtherOptionsDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            Assert.AreEqual(FakeBulkOtherOptions.Fast, FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public void TestBulkInsertDoesNotChangeTheOptionsThatAreNotAnEnumeration()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkTextOptionsDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            Assert.AreEqual("text", FakeBulkOperations.Calls.Single().Options);
        }

        [TestMethod]
        public void TestBulkInsertOfAMethodWithOnlyTheRequiredArguments()
        {
            // Act
            var actual = BulkInsertInvoker.BulkInsert(new FakeBulkMinimalDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, 10, "MyTraceKey", null, null);

            // Assert
            Assert.AreEqual(3, actual);
            Assert.AreEqual("BulkInsert (minimal)", FakeBulkOperations.Calls.Single().Method);
        }

        [TestMethod]
        public void TestBulkInsertUsesTheMethodOfTheTypeOfTheConnectionBeforeTheOneOfItsBaseType()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkPreferredDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);
            BulkInsertInvoker.BulkInsert(new FakeBulkDerivedDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            CollectionAssert.AreEqual(new[] { "BulkInsert (preferred)", "BulkInsert (base)" }, FakeBulkOperations.Calls.Select(c => c.Method).ToArray());
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertIfThereIsNoBulkInsertForTheConnection()
        {
            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsert(new FakeDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null));

            // Assert
            StringAssert.Contains(exception.Message, typeof(FakeDbConnection).FullName);
            StringAssert.Contains(exception.Message, "BulkOperations");
        }

        [TestMethod]
        public void TestBulkInsertThrowsTheExceptionOfTheMethodAndNotTheInvocationException()
        {
            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsert(new FakeBulkThrowingDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null));

            // Assert
            Assert.AreEqual("The fake bulk insert failed.", exception.Message);
        }

        #endregion

        #region Column Mappings Behavior

        [TestMethod]
        [DataRow(CopySchemaColumnMappingsBehavior.Automatic, "Automatic")]
        [DataRow(CopySchemaColumnMappingsBehavior.Strict, "Strict")]
        [DataRow(CopySchemaColumnMappingsBehavior.StrictBypass, "StrictBypass")]
        public void TestBulkInsertAppliesTheColumnMappingsBehaviorToTheSettingOfTheConnection(CopySchemaColumnMappingsBehavior behavior,
            string expected)
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkSettingDbConnection(), "Person", GetReader(), 250, behavior, null, null, null, null);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsert (setting)", call.Method);
            Assert.AreEqual(expected, call.ColumnMappingsBehavior);
        }

        [TestMethod]
        [DataRow(CopySchemaColumnMappingsBehavior.Automatic, "Automatic")]
        [DataRow(CopySchemaColumnMappingsBehavior.Strict, "Strict")]
        [DataRow(CopySchemaColumnMappingsBehavior.StrictBypass, "StrictBypass")]
        public async Task TestBulkInsertAsyncAppliesTheColumnMappingsBehaviorToTheSettingOfTheConnection(CopySchemaColumnMappingsBehavior behavior,
            string expected)
        {
            // Act
            await BulkInsertInvoker.BulkInsertAsync(new FakeBulkSettingDbConnection(), "Person", GetReader(), 250, behavior, null, null, null, null, CancellationToken.None);

            // Assert
            var call = FakeBulkOperations.Calls.Single();
            Assert.AreEqual("BulkInsertAsync (setting)", call.Method);
            Assert.AreEqual(expected, call.ColumnMappingsBehavior);
        }

        [TestMethod]
        public void TestBulkInsertSetsTheBehaviorBackAfterTheBulkInsert()
        {
            // Setup
            var setting = (FakeBulkDbSetting)DbSettingMapper.Get(new FakeBulkSettingDbConnection());
            setting.BulkColumnMappingsBehavior = FakeBulkColumnMappingsBehavior.Strict;

            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkSettingDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.StrictBypass, null, null, null, null);

            // Assert
            Assert.AreEqual("StrictBypass", FakeBulkOperations.Calls.Single().ColumnMappingsBehavior);
            Assert.AreEqual(FakeBulkColumnMappingsBehavior.Strict, setting.BulkColumnMappingsBehavior);
        }

        [TestMethod]
        public async Task TestBulkInsertAsyncSetsTheBehaviorBackAfterTheBulkInsert()
        {
            // Setup
            var setting = (FakeBulkDbSetting)DbSettingMapper.Get(new FakeBulkSettingDbConnection());
            setting.BulkColumnMappingsBehavior = FakeBulkColumnMappingsBehavior.Strict;

            // Act
            await BulkInsertInvoker.BulkInsertAsync(new FakeBulkSettingDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual("Automatic", FakeBulkOperations.Calls.Single().ColumnMappingsBehavior);
            Assert.AreEqual(FakeBulkColumnMappingsBehavior.Strict, setting.BulkColumnMappingsBehavior);
        }

        [TestMethod]
        public void TestBulkInsertSetsTheBehaviorBackIfTheBulkInsertThrows()
        {
            // Setup
            var setting = (FakeBulkDbSetting)DbSettingMapper.Get(new FakeBulkSettingThrowingDbConnection());
            setting.BulkColumnMappingsBehavior = FakeBulkColumnMappingsBehavior.Automatic;

            // Act/Assert
            Assert.Throws<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsert(new FakeBulkSettingThrowingDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Strict, null, null, null, null));
            Assert.AreEqual(FakeBulkColumnMappingsBehavior.Automatic, setting.BulkColumnMappingsBehavior);
        }

        [TestMethod]
        public void TestBulkInsertTakesTurnsWhenTheBehaviorsAreDifferent()
        {
            // Act
            var tasks = Enumerable.Range(0, 20).Select(i => Task.Run(() =>
                BulkInsertInvoker.BulkInsert(new FakeBulkSettingDbConnection(), "Person" + i, GetReader(), 250, i % 2 == 0 ? CopySchemaColumnMappingsBehavior.Strict : CopySchemaColumnMappingsBehavior.StrictBypass, null, null, null, null))).ToArray();
            Task.WaitAll(tasks);

            // Assert
            var calls = FakeBulkOperations.Calls;
            Assert.AreEqual(20, calls.Length);
            Assert.IsTrue(calls.All(c => int.Parse(c.TableName.Substring("Person".Length)) % 2 == 0 ? c.ColumnMappingsBehavior == "Strict" : c.ColumnMappingsBehavior == "StrictBypass"));
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertIfTheBehaviorIsNotTheDefaultAndTheSettingOfTheConnectionHasNoBehavior()
        {
            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsert(new FakeBulkDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Strict, null, null, null, null));

            // Assert
            StringAssert.Contains(exception.Message, "BulkColumnMappingsBehavior");
            StringAssert.Contains(exception.Message, "Strict");
            Assert.AreEqual(0, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public async Task ThrowExceptionOnBulkInsertAsyncIfTheBehaviorIsNotTheDefaultAndTheSettingOfTheConnectionHasNoBehavior()
        {
            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsertAsync(new FakeBulkDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.StrictBypass, null, null, null, null, CancellationToken.None));
        }

        [TestMethod]
        public void TestBulkInsertIgnoresTheDefaultBehaviorIfTheSettingOfTheConnectionHasNoBehavior()
        {
            // Act
            var actual = BulkInsertInvoker.BulkInsert(new FakeBulkDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null);

            // Assert
            Assert.AreEqual(3, actual);
        }

        [TestMethod]
        public void ThrowExceptionOnBulkInsertIfTheBehaviorOfTheSettingDoesNotDefineTheValue()
        {
            // Act
            var exception = Assert.Throws<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsert(new FakeBulkLimitedSettingDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.StrictBypass, null, null, null, null));

            // Assert
            StringAssert.Contains(exception.Message, "StrictBypass");
            StringAssert.Contains(exception.Message, typeof(FakeBulkLimitedColumnMappingsBehavior).FullName);
        }

        [TestMethod]
        public void TestBulkInsertAppliesTheBehaviorThatTheSettingDefines()
        {
            // Act
            BulkInsertInvoker.BulkInsert(new FakeBulkLimitedSettingDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Strict, null, null, null, null);

            // Assert
            Assert.AreEqual(1, FakeBulkOperations.Calls.Length);
        }

        [TestMethod]
        public void TestCopySchemaColumnMappingsBehaviorHasTheSameNamesAndValuesAsTheBehaviorOfTheBulkOperations()
        {
            // Act
            var actual = Enum.GetValues(typeof(CopySchemaColumnMappingsBehavior)).Cast<CopySchemaColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();
            var expected = Enum.GetValues(typeof(FakeBulkColumnMappingsBehavior)).Cast<FakeBulkColumnMappingsBehavior>().Select(v => (v.ToString(), (short)v)).ToArray();

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion

        #region BulkInsertAsync

        [TestMethod]
        public async Task TestBulkInsertAsyncReturnsTheNumberOfInsertedRows()
        {
            // Act
            var actual = await BulkInsertInvoker.BulkInsertAsync(new FakeBulkDbConnection(), "Person", GetReader(4), 100, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null, CancellationToken.None);

            // Assert
            Assert.AreEqual(4, actual);
        }

        [TestMethod]
        public async Task TestBulkInsertAsyncGivesTheArgumentsByTheirNames()
        {
            // Setup
            var transaction = new Mock<IDbTransaction>().Object;
            var trace = new Mock<ITrace>().Object;
            using (var tokenSource = new CancellationTokenSource())
            {
                // Act
                await BulkInsertInvoker.BulkInsertAsync(new FakeBulkDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, 45, "MyTraceKey", trace, transaction, tokenSource.Token);

                // Assert
                var call = FakeBulkOperations.Calls.Single();
                Assert.AreEqual("BulkInsertAsync", call.Method);
                Assert.AreEqual("Person", call.TableName);
                Assert.AreEqual(250, call.BatchSize);
                Assert.AreEqual(45, call.Timeout);
                Assert.AreEqual("MyTraceKey", call.TraceKey);
                Assert.AreSame(trace, call.Trace);
                Assert.AreSame(transaction, call.Transaction);
                Assert.AreEqual(tokenSource.Token, call.CancellationToken);
                Assert.AreEqual(3, call.RowCount);
            }
        }

        [TestMethod]
        public async Task TestBulkInsertAsyncOfAMethodWithOnlyTheRequiredArgumentsAndTheCancellationToken()
        {
            // Setup
            using (var tokenSource = new CancellationTokenSource())
            {
                // Act
                var actual = await BulkInsertInvoker.BulkInsertAsync(new FakeBulkMinimalDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, 10, null, null, null, tokenSource.Token);

                // Assert
                Assert.AreEqual(3, actual);
                var call = FakeBulkOperations.Calls.Single();
                Assert.AreEqual("BulkInsertAsync (minimal)", call.Method);
                Assert.AreEqual(tokenSource.Token, call.CancellationToken);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnBulkInsertAsyncIfThereIsNoBulkInsertAsyncForTheConnection()
        {
            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsertAsync(new FakeDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null, CancellationToken.None));

            // Assert
            StringAssert.Contains(exception.Message, typeof(FakeDbConnection).FullName);
        }

        [TestMethod]
        public async Task ThrowExceptionOnBulkInsertAsyncIfOnlyTheSynchronousBulkInsertExists()
        {
            // Act/Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsertAsync(new FakeBulkTypedDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null, CancellationToken.None));
        }

        [TestMethod]
        public async Task TestBulkInsertAsyncThrowsTheExceptionOfTheMethodAndNotTheInvocationException()
        {
            // Act
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                BulkInsertInvoker.BulkInsertAsync(new FakeBulkThrowingDbConnection(), "Person", GetReader(), 250, CopySchemaColumnMappingsBehavior.Automatic, null, null, null, null, CancellationToken.None));

            // Assert
            Assert.AreEqual("The fake bulk insert failed.", exception.Message);
        }

        #endregion
    }
}
