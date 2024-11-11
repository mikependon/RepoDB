using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Data.SQLite;

namespace RepoDb.SQLite.System.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseSQLite();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestSdsSqLiteStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateBatchQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void ThrowExceptionOnSqLiteStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateBatchQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateInsert()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateInsertWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void ThrowExceptionOnSqLiteStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateInsertAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateInserAlltWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void ThrowExceptionOnSqLiteStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void TestSdsSqLiteStatementBuilderCreateMerge()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void TestSdsSqLiteStatementBuilderCreateMergeWithPrimaryAsQualifier()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void TestSdsSqLiteStatementBuilderCreateMergeWithIdentity()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereIsNoPrimary()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

        //    // Act
        //    builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        null,
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereAreNoFields()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

        //    // Act
        //    builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        null,
        //        null,
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereAreOtherFieldsAsQualifers()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

        //    // Act
        //    builder.CreateMerge(new QueryBuilder(),
        //        "Table",
        //        Field.From("Id", "Name", "Address"),
        //        Field.From("Id", "Name"),
        //        new DbField("Id", true, false, false, typeof(int), null, null, null, null),
        //        null);
        //}

        //[TestMethod, ExpectedException(typeof(NotSupportedException))]
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereAreHints()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void TestSdsSqLiteStatementBuilderCreateMergeAll()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void TestSdsSqLiteStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void TestSdsSqLiteStatementBuilderCreateMergeAllWithIdentity()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereAreNoFields()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereAreOtherFieldsAsQualifers()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        //public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereAreHints()
        //{
        //    // Setup
        //    var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQueryWithExpression()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQueryWithTop()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQueryOrderBy()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQueryOrderByFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQueryOrderByDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQueryOrderByFieldsDescending()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateQueryOrderByFieldsMultiDirection()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void ThrowExceptionOnSqLiteStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateSkipQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateSkipQueryWithPage()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void ThrowExceptionOnSqLiteStatementBuilderCreateSkipQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                null,
                0,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(EmptyException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateBatchSkipIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                10,
                null);
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                -1,
                10,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateSkipQuery("Table",
                Field.From("Id", "Name"),
                0,
                -1,
                OrderField.Parse(new { Id = Order.Ascending }));
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

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
        public void TestSdsSqLiteStatementBuilderCreateTruncate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateTruncate("Table");
            var expected = "DELETE FROM [Table] ; VACUUM ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion
    }
}
