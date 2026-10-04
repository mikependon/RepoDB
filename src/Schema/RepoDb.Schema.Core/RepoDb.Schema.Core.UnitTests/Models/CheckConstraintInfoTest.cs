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
        public void TestCheckConstraintInfoNamePropertyDefaultValue()
        {
            // Act
            var constraint = new CheckConstraintInfo();
            var actual = constraint.Name;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCheckConstraintInfoNameProperty()
        {
            // Act
            var constraint = new CheckConstraintInfo
            {
                Name = "CK_Person_Age"
            };
            var actual = constraint.Name;
            var expected = "CK_Person_Age";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestCheckConstraintInfoExpressionPropertyDefaultValue()
        {
            // Act
            var constraint = new CheckConstraintInfo();
            var actual = constraint.Expression;

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestCheckConstraintInfoExpressionProperty()
        {
            // Act
            var constraint = new CheckConstraintInfo
            {
                Expression = "[Age]>=(0)"
            };
            var actual = constraint.Expression;
            var expected = "[Age]>=(0)";

            // Assert
            Assert.AreEqual(expected, actual, StringComparer.Ordinal);
        }

        #endregion
    }
}
