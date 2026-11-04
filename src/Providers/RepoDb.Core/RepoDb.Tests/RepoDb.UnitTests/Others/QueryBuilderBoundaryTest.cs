#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class QueryBuilderBoundaryTest
    {
        [TestMethod]
        public void TestWriteTextIgnoresNull()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.WriteText(null).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestWriteTextIgnoresEmpty()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.WriteText(string.Empty).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestWriteTextIgnoresWhitespace()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.WriteText("   ").GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEmptyBuilderReturnsNull()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.GetString();

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestClearAllowsReuse()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.Clear().Delete().ToString();

            // Assert
            Assert.AreEqual("DELETE", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestWriteTextSeparatesTokens()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Select().WriteText("*").From().WriteText("Table").GetString();

            // Assert
            Assert.AreEqual("SELECT * FROM Table", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTopFromNull()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.TopFrom(null).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitNull()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Limit(null).GetString();

            // Assert
            Assert.AreEqual("LIMIT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOffsetNull()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Offset(null).GetString();

            // Assert
            Assert.AreEqual("OFFSET", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTopFromZero()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.TopFrom(0).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitZero()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Limit(0).GetString();

            // Assert
            Assert.AreEqual("LIMIT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOffsetZero()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Offset(0).GetString();

            // Assert
            Assert.AreEqual("OFFSET", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTopFromNegative()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.TopFrom(-1).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitNegative()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Limit(-1).GetString();

            // Assert
            Assert.AreEqual("LIMIT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOffsetNegative()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Offset(-1).GetString();

            // Assert
            Assert.AreEqual("OFFSET", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestTopFromPositive()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.TopFrom(5).GetString();

            // Assert
            Assert.AreEqual("SELECT TOP (5)", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitPositive()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Limit(5).GetString();

            // Assert
            Assert.AreEqual("LIMIT 5", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOffsetPositive()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.Offset(5).GetString();

            // Assert
            Assert.AreEqual("OFFSET 5", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitTakeSkipNull()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.LimitTake(5, null).GetString();

            // Assert
            Assert.AreEqual("LIMIT 5", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitTakeSkip0()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.LimitTake(5, 0).GetString();

            // Assert
            Assert.AreEqual("LIMIT 5", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitTakeSkip3()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.LimitTake(5, 3).GetString();

            // Assert
            Assert.AreEqual("LIMIT 3, 5", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitOffsetSkipNull()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.LimitOffset(5, null).GetString();

            // Assert
            Assert.AreEqual("LIMIT 5", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitOffsetSkip0()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.LimitOffset(5, 0).GetString();

            // Assert
            Assert.AreEqual("LIMIT 5", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestLimitOffsetSkip3()
        {
            // Setup
            var builder = new QueryBuilder();

            // Act
            var actual = builder.LimitOffset(5, 3).GetString();

            // Assert
            Assert.AreEqual("LIMIT 5 OFFSET 3", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMaxFromIgnoresNullField()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.MaxFrom(null).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestMinFromIgnoresNullField()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.MinFrom(null).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestAvgFromIgnoresNullField()
        {
            // Setup
            var builder = new QueryBuilder().Select();

            // Act
            var actual = builder.AvgFrom(null).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestParametersFromNullFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = null;

            // Act
            var actual = builder.ParametersFrom(fields, 0, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestParametersAsFieldsFromNullFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = null;

            // Act
            var actual = builder.ParametersAsFieldsFrom(fields, 0, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestWhereFromNullFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = null;

            // Act
            var actual = builder.WhereFrom(fields, 0, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOnConflictNullFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = null;

            // Act
            var actual = builder.OnConflict(fields, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestParametersFromEmptyFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = Array.Empty<Field>();

            // Act
            var actual = builder.ParametersFrom(fields, 0, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestParametersAsFieldsFromEmptyFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = Array.Empty<Field>();

            // Act
            var actual = builder.ParametersAsFieldsFrom(fields, 0, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestWhereFromEmptyFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = Array.Empty<Field>();

            // Act
            var actual = builder.WhereFrom(fields, 0, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestOnConflictEmptyFields()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            IEnumerable<Field> fields = Array.Empty<Field>();

            // Act
            var actual = builder.OnConflict(fields, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestWhereFromNullGroup()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            QueryGroup group = null;

            // Act
            var actual = builder.WhereFrom(group, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestWhereFromEmptyGroup()
        {
            // Setup
            var builder = new QueryBuilder().Select();
            QueryGroup group = new QueryGroup(Array.Empty<QueryField>());

            // Act
            var actual = builder.WhereFrom(group, new CustomDbSetting()).GetString();

            // Assert
            Assert.AreEqual("SELECT", actual, StringComparer.Ordinal);
        }

    }
}
