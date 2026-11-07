#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Vertica.UnitTests
{
    [TestClass]
    public class VerticaSchemaValueFormatterTest
    {
        #region Private Variables

        private readonly VerticaSchemaValueFormatter _formatter = new VerticaSchemaValueFormatter();

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
        public void TestVerticaSchemaValueFormatterHandlesTheKinds()
        {
            // Act
            var handles = _formatter.HandlesKinds;

            // Assert
            Assert.IsTrue(handles);
        }

        [TestMethod]
        public void TestVerticaSchemaValueFormatterComposerSupportsTheValues()
        {
            // Act
            var composer = new VerticaSchemaComposer();

            // Assert
            Assert.IsInstanceOfType<ISchemaValueSupport>(composer);
        }

        [TestMethod]
        public void TestVerticaSchemaValueFormatterComposerGivesTheFormatterOfTheProvider()
        {
            // Act
            var formatter = new VerticaSchemaComposer().ValueFormatter;

            // Assert
            Assert.IsInstanceOfType<VerticaSchemaValueFormatter>(formatter);
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
            Assert.AreEqual("(CURRENT_TIMESTAMP AT TIME ZONE 'UTC')", text);
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
            Assert.AreEqual("UUID_GENERATE()", text);
        }

        [TestMethod]
        public void TestFormatKindNewSequentialGuid()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewSequentialGuid);

            // Assert
            Assert.AreEqual("UUID_GENERATE()", text);
        }

        [TestMethod]
        public void TestFormatKindRandomNumber()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.RandomNumber);

            // Assert
            Assert.AreEqual("RANDOM()", text);
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
            Assert.AreEqual("CURRENT_DATABASE()", text);
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
