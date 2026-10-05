#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Turso.Data.Sqlite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Exceptions;
using System;

namespace RepoDb.Turso.UnitTests
{
    [TestClass]
    public class StatementBuilderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseTurso();
        }

        #region CreateBatchQuery

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateBatchQuery()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateBatchQueryWithPage()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<ArgumentNullException>(() =>
                builder.CreateBatchQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateBatchQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateBatchQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateBatchQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateBatchQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateBatchQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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

        #region CreateExists

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateExists()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateExists("Table",
                QueryGroup.Parse(new { Id = 1 }));
            var expected = "SELECT 1 AS [ExistsValue] FROM [Table] WHERE ([Id] = @Id) LIMIT 1 ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        #endregion

        #region CreateInsert

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateInsert()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateInsertWithPrimary()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateInsertWithIdentity()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateInsertIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
        public void TestMdsTursoStatementBuilderCreateInsertAll()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateInserAlltWithPrimary()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateInsertAllWithIdentity()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateInsertAllIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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

        #region CreateMerge

        //[TestMethod]
        //public void TestMdsTursoStatementBuilderCreateMerge()
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
        //public void TestMdsTursoStatementBuilderCreateMergeWithPrimaryAsQualifier()
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
        //public void TestMdsTursoStatementBuilderCreateMergeWithIdentity()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeIfThereIsNoPrimary()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeIfThereAreNoFields()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeIfThereAreOtherFieldsAsQualifers()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeIfThereAreHints()
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
        //public void TestMdsTursoStatementBuilderCreateMergeAll()
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
        //public void TestMdsTursoStatementBuilderCreateMergeAllWithPrimaryAsQualifier()
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
        //public void TestMdsTursoStatementBuilderCreateMergeAllWithIdentity()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeAllIfThereIsNoPrimary()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeAllIfThereAreNoFields()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeAllIfThereAreOtherFieldsAsQualifers()
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
        //public void ThrowExceptionOnMdsTursoStatementBuilderCreateMergeAllIfThereAreHints()
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
        public void TestMdsTursoStatementBuilderCreateQuery()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateQueryWithExpression()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateQueryWithTop()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateQueryOrderBy()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateQueryOrderByFields()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateQueryOrderByDescending()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateQueryOrderByFieldsDescending()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateQueryOrderByFieldsMultiDirection()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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
        public void TestMdsTursoStatementBuilderCreateSkipQuery()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateSkipQueryWithSkip()
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
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderSkipBatchQueryIfThereAreNoFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<ArgumentNullException>(() =>
                builder.CreateSkipQuery("Table",
                    null,
                    0,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateSkipQueryIfThereAreNoOrderFields()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<EmptyException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    10,
                    null));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateSkipQueryIfThePageValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    -1,
                    10,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateSkipQueryIfTheRowsPerBatchValueIsNullOrOutOfRange()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                builder.CreateSkipQuery("Table",
                    Field.From("Id", "Name"),
                    0,
                    -1,
                    OrderField.Parse(new { Id = Order.Ascending })));
        }

        [TestMethod]
        public void ThrowExceptionOnMdsTursoStatementBuilderCreateSkipQueryIfThereAreHints()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

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

        #region CreateTruncate

        [TestMethod]
        public void TestMdsTursoStatementBuilderCreateTruncate()
        {
            // Setup
            var builder = StatementBuilderMapper.Get<SqliteConnection>();

            // Act
            var query = builder.CreateTruncate("Table");
            var expected = "DELETE FROM [Table] ;";

            // Assert
            Assert.AreEqual(expected, query, StringComparer.Ordinal);
        }

        #endregion
    }
}
