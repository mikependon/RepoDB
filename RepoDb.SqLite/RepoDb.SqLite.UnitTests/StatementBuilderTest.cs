using Microsoft.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;
using System.Data.SQLite;

namespace RepoDb.SqLite.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            SqLiteBootstrap.Initialize();
        }

        #region SDS

        #region CreateBatchQuery

        [TestMethod]
        public void TestSdsSqLiteStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateExists(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSdsSqLiteStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
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
            builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null),
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
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSdsSqLiteStatementBuilderCreateInserAlltWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSdsSqLiteStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Id], @__RepoDb_OrderColumn_0 AS [OrderColumn] ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_1, @Address_1 ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Id], @__RepoDb_OrderColumn_1 AS [OrderColumn] ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_2, @Address_2 ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Id], @__RepoDb_OrderColumn_2 AS [OrderColumn] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null),
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateQueryIfOrderFieldsAreNotPresentAtTheFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, SSN = Order.Ascending }),
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
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
            var query = builder.CreateTruncate(new QueryBuilder(),
                "Table");
            var expected = "DELETE FROM [Table] ; VACUUM ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #endregion

        #region MDS

        #region CreateBatchQuery

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateBatchQuery()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            builder.CreateBatchQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateExists(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT @Id AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInsertWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
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
            builder.CreateInsert(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null),
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
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInserAlltWithPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; " +
                "INSERT INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestMdsSqLiteStatementBuilderCreateInsertAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Id], @__RepoDb_OrderColumn_0 AS [OrderColumn] ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_1, @Address_1 ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Id], @__RepoDb_OrderColumn_1 AS [OrderColumn] ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_2, @Address_2 ) ; SELECT CAST(last_insert_rowid() AS INT) AS [Id], @__RepoDb_OrderColumn_2 AS [OrderColumn] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateInsertAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                3,
                null,
                new DbField("Id", false, true, false, typeof(int), null, null, null, null),
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
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
            var query = builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Ascending, Name = Order.Descending }),
                null,
                null);
            var expected = "SELECT [Id], [Name], [Address] FROM [Table] ORDER BY [Id] ASC, [Name] DESC ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(MissingFieldsException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateQueryIfOrderFieldsAreNotPresentAtTheFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                OrderField.Parse(new { Id = Order.Descending, SSN = Order.Ascending }),
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnMdsSqLiteStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            builder.CreateQuery(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
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
            var query = builder.CreateTruncate(new QueryBuilder(),
                "Table");
            var expected = "DELETE FROM [Table] ; VACUUM ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        #endregion

        #endregion
    }
}
