using Microsoft.VisualStudio.TestTools.UnitTesting;
using Npgsql;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;

namespace RepoDb.PostgreSql.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UsePostgreSql();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateCount

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateCount()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateCount("Table",
                null,
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateCountWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateCountIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateCount("Table",
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateCountAll

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateCountAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateCountAll("Table",
                null);
            var expected = "SELECT COUNT (*) AS \"CountValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateCountAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateCountAll("Table",
                "WhatEver");
        }

        #endregion

        #region CreateExists

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO \"Table\" ( \"Name\", \"Address\" ) VALUES ( @Name, @Address ) RETURNING CAST(\"Id\" AS INTEGER) AS \"Result\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null),
                "WhatEver");
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateInserAlltWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null),
                "WhatEver");
        }

        #endregion

        #region CreateMax

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateMax()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateMaxWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMaxIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMax("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateMaxAll

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateMaxAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MAX (\"Field\") AS \"MaxValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMaxAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMaxAll("Table",
                new Field("Field", typeof(int)),
                "WhatEver");
        }

        #endregion

        #region CreateMin

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateMin()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateMinWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMinIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMin("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateMinAll

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateMinAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT MIN (\"Field\") AS \"MinValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMinAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMinAll("Table",
                new Field("Field", typeof(int)),
                "WhatEver");
        }

        #endregion

        #region CreateMerge

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMerge("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateMergeAll("Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                "WhatEver");
        }

        #endregion

        #region CreateSkipQuery

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateSum

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateSum()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

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
        public void TestPostgreSqlStatementBuilderCreateSumWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" WHERE (\"Id\" = @Id) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSumIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateSum("Table",
                new Field("Field", typeof(int)),
                QueryGroup.Parse(new { Id = 1 }),
                "WhatEver");
        }

        #endregion

        #region CreateSumAll

        [TestMethod]
        public void TestPostgreSqlStatementBuilderCreateSumAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            var query = builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                null);
            var expected = "SELECT SUM (\"Field\") AS \"SumValue\" FROM \"Table\" ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnPostgreSqlStatementBuilderCreateSumAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<NpgsqlConnection>();

            // Act
            builder.CreateSumAll("Table",
                new Field("Field", typeof(int)),
                "WhatEver");
        }

        #endregion
    }
}
