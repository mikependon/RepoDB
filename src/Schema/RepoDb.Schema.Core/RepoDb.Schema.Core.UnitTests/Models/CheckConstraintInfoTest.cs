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
    public class CheckConstraintInfoTest
    {
        #region Methods

        [TestMethod]
        public void TestCheckConstraintInfoEqualsWithSameValues()
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
        public void TestCheckConstraintInfoEqualsWithSameInstance()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a == a);
        }

        [TestMethod]
        public void TestCheckConstraintInfoEqualsWithNull()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals((object)null));
            Assert.IsFalse(a.Equals(null));
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(((CheckConstraintInfo)null) == null);
        }

        [TestMethod]
        public void TestCheckConstraintInfoEqualsWithOtherType()
        {
            // Act
            var a = Create();

            // Assert
            Assert.IsFalse(a.Equals(new object()));
        }

        [TestMethod]
        public void TestCheckConstraintInfoNotEqualsWithDifferentName()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Name = "CK_Other";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestCheckConstraintInfoNotEqualsWithDifferentExpression()
        {
            // Act
            var a = Create();
            var b = Create();
            b.Expression = "[Age] > 18";

            // Assert
            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a == b);
            Assert.IsTrue(a != b);
        }

        [TestMethod]
        public void TestCheckConstraintInfoNamePropertyDefaultValue()
        {
            // Act
            var constraint = new CheckConstraintInfo(null);
            var actual = constraint.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCheckConstraintInfoNameProperty()
        {
            // Act
            var constraint = new CheckConstraintInfo("CK_Person_Age");
            var actual = constraint.Name;
            var expected = "CK_Person_Age";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCheckConstraintInfoExpressionPropertyDefaultValue()
        {
            // Act
            var constraint = new CheckConstraintInfo(null);
            var actual = constraint.Expression;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCheckConstraintInfoExpressionProperty()
        {
            // Act
            var constraint = new CheckConstraintInfo(null)
            {
                Expression = "[Age]>=(0)"
            };
            var actual = constraint.Expression;
            var expected = "[Age]>=(0)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        #endregion

        #region Helpers

        private static CheckConstraintInfo Create() =>
            new CheckConstraintInfo("CK_Person_Age") { Expression = "[Age] > 0" };

        #endregion
    }
}
