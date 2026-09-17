#region Copyright Attributions

// Copyright (c) 2018 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RepoDb.UnitTests
{
    public partial class QueryGroupTest
    {
        // Operations

        [TestMethod]
        public void TestQueryGroupParseExpressionForEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt == 1).GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForNotEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt != 1).GetString(m_dbSetting);
            var expected = "([PropertyInt] <> @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForGreaterThan()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt > 1).GetString(m_dbSetting);
            var expected = "([PropertyInt] > @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForGreaterThanOrEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt >= 1).GetString(m_dbSetting);
            var expected = "([PropertyInt] >= @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForLessThan()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt < 1).GetString(m_dbSetting);
            var expected = "([PropertyInt] < @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForLessThanOrEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => e.PropertyInt <= 1).GetString(m_dbSetting);
            var expected = "([PropertyInt] <= @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        // Equals Boolean

        [TestMethod]
        public void TestQueryGroupParseExpressionForEqualEqualsFalse()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1) == false).GetString(m_dbSetting);
            var expected = "NOT ([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestQueryGroupParseExpressionForEqualEqualsTrue()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == 1) == true).GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        // Parameter

        [TestMethod]
        public void TestQueryGroupParseExpressionForParameterEqual()
        {
            // Act
            var actual = QueryGroup.Parse<QueryGroupTestExpressionClass>(e => (e.PropertyInt == e.PropertyInt)).GetString(m_dbSetting);
            var expected = "([PropertyInt] = @PropertyInt)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        // ExpectedException

        [TestMethod]
        public void ThrowExceptionOnParseExpressionWithoutProperty()
        {
            Assert.Throws<NotSupportedException>(() => QueryGroup.Parse<QueryGroupTestExpressionClass>(e => true).GetString(m_dbSetting));
        }
    }
}
