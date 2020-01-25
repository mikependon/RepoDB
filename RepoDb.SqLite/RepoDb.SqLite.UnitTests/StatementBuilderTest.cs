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

        #region CreateBatchQuery

        [TestMethod]
        public void TestSqLiteStatementBuilderCreateBatchQuery()
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
        public void TestSqLiteStatementBuilderCreateBatchQueryWithPage()
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
        public void TestSqLiteStatementBuilderCreateExists()
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
        public void TestSqLiteStatementBuilderCreateInsert()
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
        public void TestSqLiteStatementBuilderCreateInsertWithPrimary()
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
        public void TestSqLiteStatementBuilderCreateInsertWithIdentity()
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
        public void TestSqLiteStatementBuilderCreateInsertAll()
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
        public void TestSqLiteStatementBuilderCreateInserAlltWithPrimary()
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
        public void TestSqLiteStatementBuilderCreateInsertAllWithIdentity()
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
            var expected = "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name, @Address ) ; SELECT CAST(last_insert_rowid() AS INT) ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_1, @Address_1 ) ; SELECT CAST(last_insert_rowid() AS INT) ; " +
                "INSERT INTO [Table] ( [Name], [Address] ) VALUES ( @Name_2, @Address_2 ) ; SELECT CAST(last_insert_rowid() AS INT) ;";

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

        [TestMethod]
        public void TestSqLiteStatementBuilderCreateMerge()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSqLiteStatementBuilderCreateMergeWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSqLiteStatementBuilderCreateMergeWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id) AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereAreOtherFieldsAsQualifers()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMerge(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null,
                "WhatEver");
        }

        #endregion

        #region CreateMergeAll

        [TestMethod]
        public void TestSqLiteStatementBuilderCreateMergeAll()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSqLiteStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(@Id AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(@Id_1 AS BIGINT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(@Id_2 AS BIGINT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod]
        public void TestSqLiteStatementBuilderCreateMergeAllWithIdentity()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            var query = builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                new DbField("Id", false, true, false, typeof(int), null, null, null, null));
            var expected = "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id, @Name, @Address ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id) AS INT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_1, @Name_1, @Address_1 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_1) AS INT) AS [Result] ; " +
                "INSERT OR REPLACE INTO [Table] ( [Id], [Name], [Address] ) VALUES ( @Id_2, @Name_2, @Address_2 ) ; SELECT CAST(COALESCE(last_insert_rowid(), @Id_2) AS INT) AS [Result] ;";

            // Assert
            Assert.AreEqual(expected, query);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereIsNoPrimary()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(PrimaryFieldNotFoundException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                null,
                3,
                null,
                null);
        }

        [TestMethod, ExpectedException(typeof(InvalidQualifiersException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereAreOtherFieldsAsQualifers()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMergeAll(new QueryBuilder(),
                "Table",
                Field.From("Id", "Name", "Address"),
                Field.From("Id", "Name"),
                3,
                new DbField("Id", true, false, false, typeof(int), null, null, null, null),
                null);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void ThrowExceptionOnSqLiteStatementBuilderCreateMergeAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SQLiteConnection>();

            // Act
            builder.CreateMergeAll(new QueryBuilder(),
                "Table",
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
        public void TestSqLiteStatementBuilderCreateQuery()
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
        public void TestSqLiteStatementBuilderCreateQueryWithExpression()
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
        public void TestSqLiteStatementBuilderCreateQueryWithTop()
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
        public void TestSqLiteStatementBuilderCreateQueryOrderBy()
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
        public void TestSqLiteStatementBuilderCreateQueryOrderByFields()
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
        public void TestSqLiteStatementBuilderCreateQueryOrderByDescending()
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
        public void TestSqLiteStatementBuilderCreateQueryOrderByFieldsDescending()
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
        public void TestSqLiteStatementBuilderCreateQueryOrderByFieldsMultiDirection()
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
        public void TestSqLiteStatementBuilderCreateTruncate()
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
    }
}
