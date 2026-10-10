#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class ColumnInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestColumnInfoEqualsWithSameValues()
        {
            // Act
            var a = Create();
            var b = Create();

            // Assert
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [TestMethod]
        public void TestColumnInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestColumnInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((ColumnInfo)null) == null);
        }

        [TestMethod]
        public void TestColumnInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentField()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Field = new DbField("Other", false, false, true, typeof(string), 10, null, null, "nvarchar");

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentOrdinal()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Ordinal = 2;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentDefaultExpression()
        {
            // Act
            var a = Create();
            var b = Create();
            b.DefaultExpression = "1";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentIdentitySeed()
        {
            // Act
            var a = Create();
            var b = Create();
            b.IdentitySeed = 5;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentIdentityIncrement()
        {
            // Act
            var a = Create();
            var b = Create();
            b.IdentityIncrement = 2;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentComputedExpression()
        {
            // Act
            var a = Create();
            var b = Create();
            b.ComputedExpression = null;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentCollation()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Collation = null;

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoNotEqualsWithDifferentComment()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Comment = "Other";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestColumnInfoFieldPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.Field;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestColumnInfoFieldProperty()
        {
            // Setup
            var field = new DbField("Id", true, true, false, typeof(long), 8, 19, 0, "bigint");

            // Act
            var column = new ColumnInfo
            {
                Field = field
            };
            var actual = column.Field;

            // Assert
            Assert.AreSame(field, actual);
        }

        [TestMethod]
        public void TestColumnInfoOrdinalPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.Ordinal;

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestColumnInfoOrdinalProperty()
        {
            // Act
            var column = new ColumnInfo
            {
                Ordinal = 3
            };
            var actual = column.Ordinal;
            var expected = 3;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnInfoDefaultExpressionPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.DefaultExpression;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestColumnInfoDefaultExpressionProperty()
        {
            // Act
            var column = new ColumnInfo
            {
                DefaultExpression = "((0))"
            };
            var actual = column.DefaultExpression;
            var expected = "((0))";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestColumnInfoIdentitySeedPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.IdentitySeed;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestColumnInfoIdentitySeedProperty()
        {
            // Act
            var column = new ColumnInfo
            {
                IdentitySeed = 10L
            };
            var actual = column.IdentitySeed;
            var expected = 10L;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnInfoIdentityIncrementPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.IdentityIncrement;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestColumnInfoIdentityIncrementProperty()
        {
            // Act
            var column = new ColumnInfo
            {
                IdentityIncrement = 5L
            };
            var actual = column.IdentityIncrement;
            var expected = 5L;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnInfoComputedExpressionPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.ComputedExpression;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestColumnInfoComputedExpressionProperty()
        {
            // Act
            var column = new ColumnInfo
            {
                ComputedExpression = "([FirstName]+[LastName])"
            };
            var actual = column.ComputedExpression;
            var expected = "([FirstName]+[LastName])";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestColumnInfoCollationPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.Collation;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestColumnInfoCollationProperty()
        {
            // Act
            var column = new ColumnInfo
            {
                Collation = "Latin1_General_CI_AS"
            };
            var actual = column.Collation;
            var expected = "Latin1_General_CI_AS";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestColumnInfoCommentPropertyDefaultValue()
        {
            // Act
            var column = new ColumnInfo();
            var actual = column.Comment;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestColumnInfoCommentProperty()
        {
            // Act
            var column = new ColumnInfo
            {
                Comment = "The name of the person."
            };
            var actual = column.Comment;
            var expected = "The name of the person.";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestColumnInfoToStringReturnsTheNameOfTheField()
        {
            // Act
            var actual = Create().ToString();

            // Assert
            Assert.AreEqual("Id", actual);
        }

        [TestMethod]
        public void TestColumnInfoToStringWithoutAFieldReturnsNull()
        {
            // Act
            var actual = new ColumnInfo().ToString();

            // Assert
            Assert.IsNull(actual);
        }

        #endregion

        #region Helpers

        private static ColumnInfo Create() =>
            new ColumnInfo { Field = new DbField("Id", true, true, false, typeof(int), null, null, null, "int"), Ordinal = 1, DefaultExpression = "0", IdentitySeed = 1, IdentityIncrement = 1, ComputedExpression = "[A] + 1", Collation = "Latin1_General_CI_AS", Comment = "The id" };

        #endregion
    }
}
