#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Schema.MariaDbConnector.UnitTests
{
    [TestClass]
    public class MariaDbConnectorSchemaValueFormatterKindTest
    {
        #region Private Variables

        private readonly MariaDbConnectorSchemaValueFormatter _formatter = new MariaDbConnectorSchemaValueFormatter();

        #endregion

        #region Private Methods

        private string FormatKind(ColumnDefaultValue kind,
            string destinationType) =>
            _formatter.FormatKind(kind,
                destinationType,
                out _);

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestMariaDbConnectorSchemaValueFormatterHandlesTheKinds()
        {
            // Act
            var handles = _formatter.HandlesKinds;

            // Assert
            Assert.IsTrue(handles);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampOfADateTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp,
                "datetime(6)");

            // Assert
            Assert.AreEqual("CURRENT_TIMESTAMP(6)", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampOfADateTimeWithoutFraction()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp,
                "datetime");

            // Assert
            Assert.AreEqual("CURRENT_TIMESTAMP", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampOfADate()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp,
                "date");

            // Assert
            Assert.AreEqual("(CURRENT_DATE)", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampOfATime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp,
                "time(3)");

            // Assert
            Assert.AreEqual("(CURRENT_TIME(3))", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampOfAnotherType()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestamp,
                "varchar(30)");

            // Assert
            Assert.AreEqual("(NOW())", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtcOfADateTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc,
                "datetime(3)");

            // Assert
            Assert.AreEqual("(UTC_TIMESTAMP(3))", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtcOfATime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc,
                "time");

            // Assert
            Assert.AreEqual("(UTC_TIME)", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtcOfADate()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc,
                "date");

            // Assert
            Assert.AreEqual("(UTC_DATE())", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampUtcOfAnotherType()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampUtc,
                "varchar(30)");

            // Assert
            Assert.AreEqual("(UTC_TIMESTAMP())", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTimestampWithOffsetInUtc()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTimestampWithOffset,
                "datetime(6)");

            // Assert
            Assert.AreEqual("(UTC_TIMESTAMP(6))", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDate()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDate,
                null);

            // Assert
            Assert.AreEqual("(CURRENT_DATE)", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentTime()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentTime,
                "time(2)");

            // Assert
            Assert.AreEqual("(CURRENT_TIME(2))", text);
        }

        [TestMethod]
        public void TestFormatKindNewGuidOfAText()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid,
                "char(36)");

            // Assert
            Assert.AreEqual("(UUID())", text);
        }

        [TestMethod]
        public void TestFormatKindNewGuidOfABinary()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid,
                "binary(16)");

            // Assert
            Assert.AreEqual("(UNHEX(REPLACE(UUID(), '-', '')))", text);
        }

        [TestMethod]
        public void TestFormatKindNewGuidOfAnotherType()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewGuid,
                "int");

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindNewSequentialGuidOfAText()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.NewSequentialGuid,
                "varchar(36)");

            // Assert
            Assert.AreEqual("(UUID())", text);
        }

        [TestMethod]
        public void TestFormatKindRandomNumber()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.RandomNumber,
                "double");

            // Assert
            Assert.AreEqual("(RAND())", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentUserOfAText()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentUser,
                "varchar(64)");

            // Assert
            Assert.AreEqual("(CURRENT_USER())", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentUserOfAnotherType()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentUser,
                "int");

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDatabaseOfAText()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDatabase,
                "varchar(64)");

            // Assert
            Assert.AreEqual("(DATABASE())", text);
        }

        [TestMethod]
        public void TestFormatKindCurrentDatabaseOfAnotherType()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.CurrentDatabase,
                "int");

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindSequenceNextValue()
        {
            // Act
            var text = FormatKind(ColumnDefaultValue.SequenceNextValue,
                null);

            // Assert
            Assert.IsNull(text);
        }

        [TestMethod]
        public void TestFormatKindOfTheOffsetGivesAWarning()
        {
            // Act
            _formatter.FormatKind(ColumnDefaultValue.CurrentTimestampWithOffset,
                "datetime",
                out var warning);

            // Assert
            Assert.IsNotNull(warning);
        }

        #endregion
    }
}
