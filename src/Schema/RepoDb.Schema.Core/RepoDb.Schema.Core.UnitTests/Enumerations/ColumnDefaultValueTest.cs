#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.Core.UnitTests.Enumerations
{
    [TestClass]
    public class ColumnDefaultValueTest
    {
        #region Methods

        [TestMethod]
        public void TestColumnDefaultValueDefaultValue()
        {
            // Act
            var actual = default(ColumnDefaultValue);
            var expected = ColumnDefaultValue.Unclassified;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestColumnDefaultValueUnclassifiedIsZero()
        {
            // Act/Assert
            Assert.AreEqual(0, (int)ColumnDefaultValue.Unclassified);
        }

        [TestMethod]
        public void TestColumnDefaultValueValues()
        {
            // Act
            var actual = Enum.GetNames(typeof(ColumnDefaultValue));
            var expected = new[]
            {
                "Unclassified", "None", "Literal", "Null", "BooleanTrue", "BooleanFalse", "CurrentTimestamp", "CurrentTimestampUtc", "CurrentTimestampWithOffset",
                "CurrentDate", "CurrentTime", "NewGuid", "NewSequentialGuid", "RandomNumber", "CurrentUser", "CurrentDatabase", "SequenceNextValue", "Expression", "Unsupported"
            };

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        #endregion
    }
}
