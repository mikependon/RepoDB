#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Helpers;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class ValueConverterTest
    {
        [DataTestMethod]
        [DataRow(typeof(int))]
        [DataRow(typeof(int?))]
        [DataRow(typeof(decimal))]
        [DataRow(typeof(bool))]
        [DataRow(typeof(DateTime))]
        [DataRow(typeof(DateTimeOffset))]
        [DataRow(typeof(TimeSpan))]
        [DataRow(typeof(Guid))]
        public void TestConvertOfAnEmptyTextIntoATypeThatIsNotATextIsNull(Type type)
        {
            // Act
            var actual = ValueConverter.Convert(string.Empty, type);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestConvertOfAnEmptyTextIntoATextOrAnObjectKeepsTheEmptyText()
        {
            // Act/Assert
            Assert.AreEqual(string.Empty, ValueConverter.Convert(string.Empty, typeof(string)));
            Assert.AreEqual(string.Empty, ValueConverter.Convert(string.Empty, typeof(object)));
        }

        [TestMethod]
        public void TestConvertOfAnEmptyTextIntoABinaryGivesAnEmptyBinary()
        {
            // Act
            var actual = ValueConverter.Convert(string.Empty, typeof(byte[]));

            // Assert
            CollectionAssert.AreEqual(new byte[0], (byte[])actual);
        }

        [TestMethod]
        public void TestConvertOfAWhiteSpaceTextIntoANumberIsNotANull()
        {
            // Act/Assert
            Assert.Throws<FormatException>(() => ValueConverter.Convert(" ", typeof(int)));
        }
    }
}
