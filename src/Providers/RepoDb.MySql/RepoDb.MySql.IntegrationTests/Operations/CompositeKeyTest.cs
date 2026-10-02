#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using RepoDb.MySql.IntegrationTests.Setup;
using System.Linq;

namespace RepoDb.MySql.IntegrationTests.Operations
{
    /// <summary>
    /// Validates the behavior of the push operations against a table with a multi-column primary key
    /// (see https://github.com/mikependon/RepoDB/discussions/1361).
    /// </summary>
    [TestClass]
    public class CompositeKeyTest
    {
        public class MulticolumnPrimaryKey
        {
            public string col1 { get; set; }
            public sbyte col2 { get; set; }
            public string col3 { get; set; }
            public string col4 { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            Database.Initialize();

            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS `MulticolumnPrimaryKey`;");
                connection.ExecuteNonQuery(@"CREATE TABLE `MulticolumnPrimaryKey`
                    (
                        `col1` varchar(10) NOT NULL DEFAULT '',
                        `col2` tinyint NOT NULL,
                        `col3` varchar(15) NOT NULL DEFAULT '',
                        `col4` varchar(100) NOT NULL DEFAULT '',
                        PRIMARY KEY (`col1`, `col2`, `col3`)
                    ) ENGINE=InnoDB;");
            }
        }

        [TestCleanup]
        public void Cleanup()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                connection.ExecuteNonQuery(@"DROP TABLE IF EXISTS `MulticolumnPrimaryKey`;");
            }
        }

        private static MulticolumnPrimaryKey[] CreateRows() => new[]
        {
            new MulticolumnPrimaryKey { col1 = "A", col2 = 1, col3 = "X", col4 = "row-A-1-X" },
            new MulticolumnPrimaryKey { col1 = "A", col2 = 2, col3 = "Y", col4 = "row-A-2-Y" },
            new MulticolumnPrimaryKey { col1 = "A", col2 = 3, col3 = "Z", col4 = "row-A-3-Z" }
        };

        [TestMethod]
        public void TestInsertWithCompositeKey()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                foreach (var row in CreateRows())
                {
                    connection.Insert(row);
                }

                // Assert
                Assert.AreEqual(3, connection.CountAll<MulticolumnPrimaryKey>());
            }
        }

        [TestMethod]
        public void TestInsertAllWithCompositeKey()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Act
                connection.InsertAll(CreateRows());

                // Assert
                Assert.AreEqual(3, connection.CountAll<MulticolumnPrimaryKey>());
            }
        }

        [TestMethod]
        public void TestUpdateWithCompositeKeyMustOnlyAffectTheMatchingRow()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup (raw SQL so that this test does not depend on Insert)
                connection.ExecuteNonQuery(@"INSERT INTO `MulticolumnPrimaryKey` VALUES
                    ('A', 1, 'X', 'row-A-1-X'), ('A', 2, 'Y', 'row-A-2-Y'), ('A', 3, 'Z', 'row-A-3-Z');");

                // Act
                var target = new MulticolumnPrimaryKey { col1 = "A", col2 = 2, col3 = "Y", col4 = "UPDATED" };
                connection.Update(target);

                // Assert
                var rows = connection.QueryAll<MulticolumnPrimaryKey>().ToList();
                Assert.AreEqual(1, rows.Count(r => r.col4 == "UPDATED"),
                    $"Rows updated: {string.Join(", ", rows.Where(r => r.col4 == "UPDATED").Select(r => $"({r.col1},{r.col2},{r.col3})"))}");
            }
        }

        [TestMethod]
        public void TestUpdateViaExpressionWithCompositeKey()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                connection.ExecuteNonQuery(@"INSERT INTO `MulticolumnPrimaryKey` VALUES
                    ('A', 1, 'X', 'row-A-1-X'), ('A', 2, 'Y', 'row-A-2-Y'), ('A', 3, 'Z', 'row-A-3-Z');");

                // Act (workaround: explicit where on all key columns)
                var target = new MulticolumnPrimaryKey { col1 = "A", col2 = 2, col3 = "Y", col4 = "UPDATED" };
                var where = new QueryGroup(new[]
                {
                    new QueryField("col1", "A"),
                    new QueryField("col2", (sbyte)2),
                    new QueryField("col3", "Y")
                });
                var affected = connection.Update(target, where);

                // Assert
                Assert.AreEqual(1, affected);
                Assert.AreEqual(1, connection.QueryAll<MulticolumnPrimaryKey>().Count(r => r.col4 == "UPDATED"));
            }
        }

        [TestMethod]
        public void TestMergeWithCompositeKeyMustNotDuplicateOrOverwriteOtherRows()
        {
            using (var connection = new MySqlConnection(Database.ConnectionString))
            {
                // Setup
                connection.ExecuteNonQuery(@"INSERT INTO `MulticolumnPrimaryKey` VALUES
                    ('A', 1, 'X', 'row-A-1-X'), ('A', 2, 'Y', 'row-A-2-Y'), ('A', 3, 'Z', 'row-A-3-Z');");

                // Act
                connection.Merge(new MulticolumnPrimaryKey { col1 = "A", col2 = 2, col3 = "Y", col4 = "MERGED" });

                // Assert
                var rows = connection.QueryAll<MulticolumnPrimaryKey>().ToList();
                Assert.AreEqual(3, rows.Count);
                Assert.AreEqual(1, rows.Count(r => r.col4 == "MERGED"));
            }
        }
    }
}
