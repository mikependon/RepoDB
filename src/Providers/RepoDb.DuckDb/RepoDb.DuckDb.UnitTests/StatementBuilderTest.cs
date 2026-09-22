#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuckDB.NET.Data;
using RepoDb.Enumerations;
using RepoDb.Exceptions;

namespace RepoDb.DuckDb.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDuckDb();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<ArgumentNullException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" WHERE (\"Id\" = $Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCountAll("Table",
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS \"ExistsValue\" FROM \"Table\" WHERE (\"Id\" = $Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) RETURNING NULL AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) RETURNING \"Id\" AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( $Name, $Address ) RETURNING \"Id\" AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsert("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                    "WhatEver"));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) , " +
                "( $Id_1, $Name_1, $Address_1 ) , ( $Id_2, $Name_2, $Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateInsertAllWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) , ( $Id_1, $Name_1, $Address_1 ) , " +
                "( $Id_2, $Name_2, $Address_2 ) RETURNING \"Id\" AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( $Name, $Address ) , ( $Name_1, $Address_1 ) , " +
                "( $Name_2, $Address_2 ) RETURNING \"Id\" AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    3,
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                    "WhatEver"));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" WHERE (\"Id\" = $Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMax("Table",
                    new Field("Field", typeof(int)),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMaxAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" WHERE (\"Id\" = $Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMin("Table",
                    new Field("Field", typeof(int)),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMinAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) ON CONFLICT (\"Id\") " +
                "DO UPDATE SET \"Name\" = $Name, \"Address\" = $Address RETURNING \"Id\" AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) ON CONFLICT (\"Id\") " +
                "DO UPDATE SET \"Name\" = $Name, \"Address\" = $Address RETURNING \"Id\" AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            // DuckDB has no MySQL-style LAST_INSERT_ID() special-casing: the identity column is returned
            // and reconciled via the same ON CONFLICT ... RETURNING clause as any other qualifier-excluded field.
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) ON CONFLICT (\"Id\") " +
                "DO UPDATE SET \"Name\" = $Name, \"Address\" = $Address RETURNING \"Id\" AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id", "Name"),
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name, \"Address\" = $Address RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_0 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id_1, $Name_1, $Address_1 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name_1, \"Address\" = $Address_1 RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_1 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id_2, $Name_2, $Address_2 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name_2, \"Address\" = $Address_2 RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_2 AS \"OrderColumn\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name, \"Address\" = $Address RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_0 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id_1, $Name_1, $Address_1 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name_1, \"Address\" = $Address_1 RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_1 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id_2, $Name_2, $Address_2 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name_2, \"Address\" = $Address_2 RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_2 AS \"OrderColumn\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            // Same simplification as CreateMerge's identity test: no LAST_INSERT_ID()-style special-casing needed.
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id, $Name, $Address ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name, \"Address\" = $Address RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_0 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id_1, $Name_1, $Address_1 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name_1, \"Address\" = $Address_1 RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_1 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( $Id_2, $Name_2, $Address_2 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = $Name_2, \"Address\" = $Address_2 RETURNING \"Id\" AS \"Result\", $__RepoDb_OrderColumn_2 AS \"OrderColumn\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    3,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    3,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id", "Name"),
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" WHERE (\"Id\" = $Id AND \"Name\" = $Name) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" ASC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC, \"Name\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateQuery("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateSkipQuery

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<ArgumentNullException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateSkipQueryIfTheSkipValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateSkipQueryIfTheTakeValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending }),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" WHERE (\"Id\" = $Id) ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSum("Table",
                    new Field("Field", typeof(int)),
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestDuckDbStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnDuckDbStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<DuckDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSumAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion
    }
}
