#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Models
{
    [TestClass]
    public class SchemaValueTest
    {
        #region Public Methods

        [TestMethod]
        public void TestFromBinarySetsTheKind()
        {
            // Act
            var value = SchemaValue.FromBinary(new byte[] { 1 });

            // Assert
            Assert.AreEqual(SchemaValueKind.Binary, value.Kind);
        }

        [TestMethod]
        public void TestFromBinarySetsTheValue()
        {
            // Setup
            var bytes = new byte[] { 1, 2 };

            // Act
            var value = SchemaValue.FromBinary(bytes);

            // Assert
            Assert.AreSame(bytes, value.Value);
        }

        [TestMethod]
        public void TestFromBooleanSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromBoolean(true);

            // Assert
            Assert.AreEqual(SchemaValueKind.Boolean, value.Kind);
        }

        [TestMethod]
        public void TestFromBooleanSetsTheValue()
        {
            // Act
            var value = SchemaValue.FromBoolean(true);

            // Assert
            Assert.AreEqual(true, value.Value);
        }

        [TestMethod]
        public void TestFromDateSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromDate(new DateTime(2020, 1, 31));

            // Assert
            Assert.AreEqual(SchemaValueKind.Date, value.Kind);
        }

        [TestMethod]
        public void TestFromDateIgnoresTheTime()
        {
            // Act
            var value = SchemaValue.FromDate(new DateTime(2020, 1, 31, 10, 20, 30));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31), value.Value);
        }

        [TestMethod]
        public void TestFromDateTimeSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30));

            // Assert
            Assert.AreEqual(SchemaValueKind.DateTime, value.Kind);
        }

        [TestMethod]
        public void TestFromDateTimeKeepsTheTime()
        {
            // Act
            var value = SchemaValue.FromDateTime(new DateTime(2020, 1, 31, 10, 20, 30));

            // Assert
            Assert.AreEqual(new DateTime(2020, 1, 31, 10, 20, 30), value.Value);
        }

        [TestMethod]
        public void TestFromDateTimeOffsetSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromDateTimeOffset(new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1)));

            // Assert
            Assert.AreEqual(SchemaValueKind.DateTimeOffset, value.Kind);
        }

        [TestMethod]
        public void TestFromDateTimeOffsetKeepsTheOffset()
        {
            // Setup
            var offset = new DateTimeOffset(2020, 1, 31, 10, 20, 30, TimeSpan.FromHours(1));

            // Act
            var value = SchemaValue.FromDateTimeOffset(offset);

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(1), ((DateTimeOffset)value.Value).Offset);
        }

        [TestMethod]
        public void TestFromGuidSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromGuid(Guid.NewGuid());

            // Assert
            Assert.AreEqual(SchemaValueKind.Guid, value.Kind);
        }

        [TestMethod]
        public void TestFromGuidSetsTheValue()
        {
            // Setup
            var guid = Guid.NewGuid();

            // Act
            var value = SchemaValue.FromGuid(guid);

            // Assert
            Assert.AreEqual(guid, value.Value);
        }

        [TestMethod]
        public void TestFromNumberSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromNumber("12.50");

            // Assert
            Assert.AreEqual(SchemaValueKind.Number, value.Kind);
        }

        [TestMethod]
        public void TestFromNumberKeepsTheTextOfTheNumber()
        {
            // Act
            var value = SchemaValue.FromNumber("12.50");

            // Assert
            Assert.AreEqual("12.50", value.Value);
        }

        [TestMethod]
        public void TestFromTextSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromText("abc");

            // Assert
            Assert.AreEqual(SchemaValueKind.Text, value.Kind);
        }

        [TestMethod]
        public void TestFromTextSetsTheValue()
        {
            // Act
            var value = SchemaValue.FromText("abc");

            // Assert
            Assert.AreEqual("abc", value.Value);
        }

        [TestMethod]
        public void TestFromTimeSetsTheKind()
        {
            // Act
            var value = SchemaValue.FromTime(TimeSpan.FromHours(10));

            // Assert
            Assert.AreEqual(SchemaValueKind.Time, value.Kind);
        }

        [TestMethod]
        public void TestFromTimeSetsTheValue()
        {
            // Act
            var value = SchemaValue.FromTime(TimeSpan.FromHours(10));

            // Assert
            Assert.AreEqual(TimeSpan.FromHours(10), value.Value);
        }

        #endregion
    }
}
