#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Db2.UnitTests
{
    [TestClass]
    public class Db2SchemaValueFormatterTest
    {
        #region Private Variables

        private readonly Db2SchemaValueFormatter _formatter = new Db2SchemaValueFormatter();

        #endregion

        #region Private Methods

        private string FormatKind(ColumnDefaultValue kind,
            string destinationType = null) =>
            _formatter.FormatKind(kind,
                destinationType,
                out _);

        private string FormatKind(ColumnDefaultValue kind,
            out string warning) =>
            _formatter.FormatKind(kind,
                null,
                out warning);

        private string Format(SchemaValue value) =>
            _formatter.Format(value,
                null,
                out _);

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestDb2SchemaValueFormatterHandlesTheKinds()
        {
            // Act
            var handles = _formatter.HandlesKinds;

            // Assert
            Assert.IsTrue(handles);
        }

        [TestMethod]
        public void TestDb2SchemaValueFormatterComposerSupportsTheValues()
        {
            // Act
            var composer = new Db2SchemaComposer();

            // Assert
            Assert.IsInstanceOfType<ISchemaValueSupport>(composer);
        }

        [TestMethod]
        public void TestDb2SchemaValueFormatterComposerGivesTheFormatterOfTheProvider()
        {
            // Act
            var formatter = new Db2SchemaComposer().ValueFormatter;

            // Assert
            Assert.IsInstanceOfType<Db2SchemaValueFormatter>(formatter);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestamp()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp);

            // Assert
            Assert.AreEqual("CURRENT TIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtc()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc);

            // Assert
            Assert.AreEqual("CURRENT TIMESTAMP - CURRENT TIMEZONE", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampWithOffset()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampWithOffset);

            // Assert
            Assert.AreEqual("CURRENT TIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampWithOffsetGivesAWarning()
        {
            // Act
            FormatKind(ColumnDefaultValue.CurrentTimestampWithOffset,
                out var warning);

            // Assert
            Assert.IsNotNull(warning);
        }

        [TestMethod]
        public void TestFormatKindCurrentDate()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDate);

            // Assert
            Assert.AreEqual("CURRENT DATE", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTime);

            // Assert
            Assert.AreEqual("CURRENT TIME", text);
        }

        [TestMethod]
        public void TestFormatKindNewGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindNewSequentialGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewSequentialGuid);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindRandomNumber()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.RandomNumber);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindCurrentUser()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentUser);

            // Assert
            Assert.AreEqual("CURRENT USER", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDatabase()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDatabase);

            // Assert
            Assert.AreEqual("CURRENT SERVER", text);
        }

        [TestMethod]
        public void TestFormatKindOfASequenceHasNoEquivalent()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.SequenceNextValue);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindOfALiteralHasNoEquivalent()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.Literal);

            // Assert
            Assert.IsNull(text);
        }

        #endregion
    }
}
