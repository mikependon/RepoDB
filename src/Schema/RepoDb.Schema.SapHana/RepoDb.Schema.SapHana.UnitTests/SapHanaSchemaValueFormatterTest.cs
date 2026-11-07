#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.SapHana.UnitTests
{
    [TestClass]
    public class SapHanaSchemaValueFormatterTest
    {
        #region Private Variables

        private readonly SapHanaSchemaValueFormatter _formatter = new SapHanaSchemaValueFormatter();

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
        public void TestSapHanaSchemaValueFormatterHandlesTheKinds()
        {
            // Act
            var handles = _formatter.HandlesKinds;

            // Assert
            Assert.IsTrue(handles);
        }

        [TestMethod]
        public void TestSapHanaSchemaValueFormatterComposerSupportsTheValues()
        {
            // Act
            var composer = new SapHanaSchemaComposer();

            // Assert
            Assert.IsInstanceOfType<ISchemaValueSupport>(composer);
        }

        [TestMethod]
        public void TestSapHanaSchemaValueFormatterComposerGivesTheFormatterOfTheProvider()
        {
            // Act
            var formatter = new SapHanaSchemaComposer().ValueFormatter;

            // Assert
            Assert.IsInstanceOfType<SapHanaSchemaValueFormatter>(formatter);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestamp()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp);

            // Assert
            Assert.AreEqual("CURRENT_TIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtc()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc);

            // Assert
            Assert.AreEqual("CURRENT_UTCTIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampWithOffset()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampWithOffset);

            // Assert
            Assert.AreEqual("CURRENT_TIMESTAMP", text);
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
            Assert.AreEqual("CURRENT_DATE", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTime);

            // Assert
            Assert.AreEqual("CURRENT_TIME", text);
        }

        [TestMethod]
        public void TestFormatKindNewGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid);

            // Assert
            Assert.AreEqual("SYSUUID", text);
        }

        [TestMethod]
        public void TestFormatKindNewSequentialGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewSequentialGuid);

            // Assert
            Assert.AreEqual("SYSUUID", text);
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
            Assert.AreEqual("CURRENT_USER", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDatabase()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDatabase);

            // Assert
            Assert.IsNull(text);
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
