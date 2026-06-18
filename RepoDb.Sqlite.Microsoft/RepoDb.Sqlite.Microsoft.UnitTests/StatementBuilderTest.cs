using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;

namespace RepoDb.Sqlite.Microsoft.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSqlite();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                3,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 30, 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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

        #region CreateExists

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS [ExistsValue] FROM [Table] WHERE ([Id] = @Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT NULL AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateInsert("Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                "WhatEver");
        }

        #endregion

        #region CreateInsertAll

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) , ( @Id_1, @Name_1, @Address_1 ) , ( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInserAlltWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null, false),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ," +
                " ( @Id_1, @Name_1, @Address_1 ) , ( @Id_2, @Name_2, @Address_2 ) RETURNING CAST([Id] AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false));
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) , ( @Name_1, @Address_1 ) , ( @Name_2, @Address_2 ) RETURNING CAST([Id] AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateInsertAll("Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null, false),
                "WhatEver");
        }

        #endregion

        #region CreateMerge

        //[TestMethod]
        //public void TestMdsSqLiteStatementBuilderCreateMerge()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    var query = builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null);
        //    var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ;";

        //    // Assert
        //    Assert.AreEqual(expected, query);
        //}

        //[TestMethod]
        //public void TestMdsSqLiteStatementBuilderCreateMergeWithPrimaryAsQualifier()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    var query = builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        Field.From("Id"),
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null);
        //    var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ;";

        //    // Assert
        //    Assert.AreEqual(expected, query);
        //}

        //[TestMethod]
        //public void TestMdsSqLiteStatementBuilderCreateMergeWithIdentity()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    var query = builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        new DbField("Id", false, true, false, typeof(int), null, null, null, null));
        //    var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id) AS BIGINT) AS [Result] ;";

        //    // Assert
        //    Assert.AreEqual(expected, query);
        //}

        //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeIfThereIsNoPrimary()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        null,
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeIfThereAreNoFields()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        null,
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeIfThereAreOtherFieldsAsQualifers()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        Field.From("Id", "Name"),
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(NotSupportedException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeIfThereAreHints()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        Field.From("Id", "Name"),
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null,
        //        "WhatEver");
        //}

        #endregion

        #region CreateMergeAll

        //[TestMethod]
        //public void TestMdsSqLiteStatementBuilderCreateMergeAll()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    var query = builder.CreateMergeAll(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        3,
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null);
        //    var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
        //        "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
        //        "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result] ;";

        //    // Assert
        //    Assert.AreEqual(expected, query);
        //}

        //[TestMethod]
        //public void TestMdsSqLiteStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    var query = builder.CreateMergeAll(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        Field.From("Id"),
        //        3,
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null);
        //    var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
        //        "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
        //        "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result] ;";

        //    // Assert
        //    Assert.AreEqual(expected, query);
        //}

        //[TestMethod]
        //public void TestMdsSqLiteStatementBuilderCreateMergeAllWithIdentity()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    var query = builder.CreateMergeAll(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        3,
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        new DbField("Id", false, true, false, typeof(int), null, null, null, null));
        //    var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id) AS INT) AS [Result] ; " +
        //        "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_1) AS INT) AS [Result] ; " +
        //        "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_2) AS INT) AS [Result] ;";

        //    // Assert
        //    Assert.AreEqual(expected, query);
        //}

        //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMergeAll(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        3,
        //        null,
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeAllIfThereAreNoFields()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMergeAll(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        3,
        //        null,
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeAllIfThereAreOtherFieldsAsQualifers()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMergeAll(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        Field.From("Id", "Name"),
        //        3,
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(NotSupportedException))]
        //public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateMergeAllIfThereAreHints()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SqliteConnection>();

        //    // Act
        //    builder.CreateMergeAll(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        Field.From("Id", "Name"),
        //        3,
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null,
        //        "WhatEver");
        //}

        #endregion

        #region CreateQuery

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                QueryGroup.Parse(new { Id = 1, Name = "Michael" }),
                null,
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] WHERE ([Id] = @Id AND [Name] = @Name) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                10,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Ascending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] ASC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] DESC, [Name] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateQuery("Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
        public void TestMdsSqLiteStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateSkipQueryWithSkip()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                30,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
            var expected = "SELECT [Id], [Name] FROM [Table] ORDER BY [Id] ASC LIMIT 30, 10 ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NullReferenceException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderSkipBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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

        #region CreateTruncate

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateTruncate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateTruncate("Table");
            var expected = "DELETE FROM [Table] ; VACUUM ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion
    }
}
