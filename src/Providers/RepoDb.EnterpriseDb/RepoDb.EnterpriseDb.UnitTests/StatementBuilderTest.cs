#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Connector.EnterpriseDb;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;

namespace RepoDb.EnterpriseDb.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void TestEnterpriseDbStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCount("Table",
                    QueryGroup.Parse(new { Id = 1 }),
                    "WhatEver"));
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateCountAll("Table",
                    "WhatEver"));
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS \"ExistsValue\" FROM \"Table\" WHERE (\"Id\" = @Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) RETURNING NULL AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( @Name, @Address ) RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsert("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null),
                    "WhatEver"));
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) " +
                "VALUES " +
                "( @Id, @Name, @Address ) , " +
                "( @Id_1, @Name_1, @Address_1 ) , " +
                "( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateInserAlltWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) " +
                "VALUES " +
                "( @Id, @Name, @Address ) , " +
                "( @Id_1, @Name_1, @Address_1 ) , " +
                "( @Id_2, @Name_2, @Address_2 ) " +
                "RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) " +
                "VALUES " +
                "( @Name, @Address ) , " +
                "( @Name_1, @Address_1 ) , " +
                "( @Name_2, @Address_2 ) " +
                "RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateInsertAll("Table",
                    Field.From("Id", "Name", "Address"),
                    3,
                    null,
                    new DbField("Id", false, true, false, typeof(int), null, null, null, null),
                    "WhatEver"));
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void TestEnterpriseDbStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMaxAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void TestEnterpriseDbStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMinAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) " +
                "ON CONFLICT (\"Id\") DO " +
                "UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) " +
                "ON CONFLICT (\"Id\") DO " +
                "UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) OVERRIDING SYSTEM VALUE VALUES ( @Id, @Name, @Address ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<PrimaryFieldNotFoundException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    null,
                    null,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMerge("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id", "Name"),
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_0 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_1, \"Address\" = @Address_1 RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_1 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_2, \"Address\" = @Address_2 RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_2 AS \"OrderColumn\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id, @Name, @Address ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_0 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id_1, @Name_1, @Address_1 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_1, \"Address\" = @Address_1 RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_1 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) VALUES ( @Id_2, @Name_2, @Address_2 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_2, \"Address\" = @Address_2 RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_2 AS \"OrderColumn\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) OVERRIDING SYSTEM VALUE VALUES ( @Id, @Name, @Address ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name, \"Address\" = @Address RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_0 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) OVERRIDING SYSTEM VALUE VALUES ( @Id_1, @Name_1, @Address_1 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_1, \"Address\" = @Address_1 RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_1 AS \"OrderColumn\" ; " +
                "INSERT INTO \"Table\" ( \"Id\", \"Name\", \"Address\" ) OVERRIDING SYSTEM VALUE VALUES ( @Id_2, @Name_2, @Address_2 ) ON CONFLICT (\"Id\") DO UPDATE SET \"Name\" = @Name_2, \"Address\" = @Address_2 RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\", @__RepoDb_OrderColumn_2 AS \"OrderColumn\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateMergeAll("Table",
                    Field.From("Id", "Name", "Address"),
                    Field.From("Id", "Name"),
                    3,
                    new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                    null,
                    "WhatEver"));
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" WHERE (\"Id\" = @Id AND \"Name\" = @Name) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" DESC, \"Name\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT \"Id\", \"Name\", \"Address\" FROM \"Table\" ORDER BY \"Id\" ASC, \"Name\" DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void TestEnterpriseDbStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT \"Id\", \"Name\" FROM \"Table\" ORDER BY \"Id\" ASC LIMIT 10 OFFSET 30 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void TestEnterpriseDbStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                null,
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestEnterpriseDbStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

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
        public void TestEnterpriseDbStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void ThrowExceptionOnEnterpriseDbStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<EDBConnection>();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                builder.CreateSumAll("Table",
                    new Field("Field", typeof(int)),
                    "WhatEver"));
        }

        #endregion
    }
}
