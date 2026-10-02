#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Vertica.IntegrationTests.Setup;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Vertica.Data.VerticaClient;

namespace RepoDb.Vertica.IntegrationTests.Operations
{
    /// <summary>
    /// Verifies the <see cref="KeyColumnReturnBehavior"/> option against a table whose identity ("Id")
    /// and primary ("Code") columns are different. A dedicated table per behavior is used because the
    /// execution contexts are cached per table name.
    /// </summary>
    [TestClass]
    public class KeyColumnReturnBehaviorTest
    {
        private const long CodeValue = 9001;

        public class KeyBehaviorTable
        {
            [Identity]
            public long? Id { get; set; }

            [Primary]
            public long? Code { get; set; }
            public string Name { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();
        }

        #region Helpers

        private static string TableNameOf(KeyColumnReturnBehavior behavior, string operation) =>
            string.Concat("KeyBehavior", behavior, operation);

        private static void CreateTable(string tableName)
        {
            using var connection = new VerticaConnection(Database.ConnectionString);
            connection.ExecuteNonQuery($@"DROP TABLE IF EXISTS ""{tableName}"" CASCADE;");
            connection.ExecuteNonQuery($@"CREATE TABLE ""{tableName}""
                (
                    ""Id"" IDENTITY(1, 1),
                    ""Code"" BIGINT NOT NULL,
                    ""Name"" VARCHAR(256),
                    PRIMARY KEY (""Code"")
                );");
        }

        private static void WithBehavior(KeyColumnReturnBehavior behavior, Action action)
        {
            var original = GlobalConfiguration.Options.KeyColumnReturnBehavior;
            GlobalConfiguration.Options.KeyColumnReturnBehavior = behavior;
            try
            {
                action();
            }
            finally
            {
                GlobalConfiguration.Options.KeyColumnReturnBehavior = original;
            }
        }

        private static long ToLong(object value) =>
            Convert.ToInt64(value, CultureInfo.InvariantCulture);

        private static bool ReturnsPrimary(KeyColumnReturnBehavior behavior) =>
            behavior == KeyColumnReturnBehavior.Primary || behavior == KeyColumnReturnBehavior.PrimaryOrElseIdentity;

        /// <summary>
        /// Asserts that only the column selected by the behavior was written back to the entity.
        /// </summary>
        private static void AssertEntityKeys(KeyColumnReturnBehavior behavior, KeyBehaviorTable entity)
        {
            Assert.AreEqual(CodeValue, entity.Code);
            if (ReturnsPrimary(behavior))
            {
                Assert.IsNull(entity.Id, "The identity must not be set when the primary is the return key.");
            }
            else
            {
                Assert.IsTrue(entity.Id > 0, "The identity must be set when it is the return key.");
            }
        }

        private static void AssertRow(string tableName)
        {
            using var connection = new VerticaConnection(Database.ConnectionString);
            Assert.AreEqual(1, connection.CountAll(tableName));
            var row = connection.QueryAll<KeyBehaviorTable>(tableName).Single();
            Assert.AreEqual(CodeValue, row.Code);
            Assert.IsTrue(row.Id > 0);
        }

        public static IEnumerable<object[]> Behaviors => new[]
        {
            new object[] { KeyColumnReturnBehavior.Primary },
            new object[] { KeyColumnReturnBehavior.Identity },
            new object[] { KeyColumnReturnBehavior.PrimaryOrElseIdentity },
            new object[] { KeyColumnReturnBehavior.IdentityOrElsePrimary }
        };

        #endregion

        [TestMethod]
        [DynamicData(nameof(Behaviors))]
        public void TestVerticaConnectionInsertWithKeyColumnReturnBehavior(KeyColumnReturnBehavior behavior)
        {
            var tableName = TableNameOf(behavior, "Insert");
            CreateTable(tableName);
            var entity = new KeyBehaviorTable { Code = CodeValue, Name = "Insert" };

            WithBehavior(behavior, () =>
            {
                using var connection = new VerticaConnection(Database.ConnectionString);
                var result = ToLong(connection.Insert(tableName, entity));

                if (ReturnsPrimary(behavior))
                {
                    Assert.AreEqual(CodeValue, result);
                }
                else
                {
                    Assert.IsTrue(result > 0 && result != CodeValue);
                    Assert.AreEqual(result, entity.Id);
                }
            });

            AssertEntityKeys(behavior, entity);
            AssertRow(tableName);
        }

        [TestMethod]
        [DynamicData(nameof(Behaviors))]
        public void TestVerticaConnectionInsertAllWithKeyColumnReturnBehavior(KeyColumnReturnBehavior behavior)
        {
            var tableName = TableNameOf(behavior, "InsertAll");
            CreateTable(tableName);
            var entity = new KeyBehaviorTable { Code = CodeValue, Name = "InsertAll" };

            WithBehavior(behavior, () =>
            {
                using var connection = new VerticaConnection(Database.ConnectionString);
                Assert.AreEqual(1, connection.InsertAll(tableName, new[] { entity }));
            });

            AssertEntityKeys(behavior, entity);
            AssertRow(tableName);
        }

        [TestMethod]
        [DynamicData(nameof(Behaviors))]
        public void TestVerticaConnectionMergeWithKeyColumnReturnBehavior(KeyColumnReturnBehavior behavior)
        {
            var tableName = TableNameOf(behavior, "Merge");
            CreateTable(tableName);
            var entity = new KeyBehaviorTable { Code = CodeValue, Name = "Merge" };

            WithBehavior(behavior, () =>
            {
                using var connection = new VerticaConnection(Database.ConnectionString);
                var result = ToLong(connection.Merge(tableName, entity, qualifiers: Field.From("Code")));

                if (ReturnsPrimary(behavior))
                {
                    Assert.AreEqual(CodeValue, result);
                }
                else
                {
                    Assert.IsTrue(result > 0);
                }
            });

            AssertRow(tableName);
        }

        [TestMethod]
        [DynamicData(nameof(Behaviors))]
        public void TestVerticaConnectionMergeAllWithKeyColumnReturnBehavior(KeyColumnReturnBehavior behavior)
        {
            var tableName = TableNameOf(behavior, "MergeAll");
            CreateTable(tableName);
            var entity = new KeyBehaviorTable { Code = CodeValue, Name = "MergeAll" };

            WithBehavior(behavior, () =>
            {
                using var connection = new VerticaConnection(Database.ConnectionString);
                Assert.AreEqual(1, connection.MergeAll(tableName, new[] { entity }, qualifiers: Field.From("Code")));
            });

            AssertEntityKeys(behavior, entity);
            AssertRow(tableName);
        }
    }
}
