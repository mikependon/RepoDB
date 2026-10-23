#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.AuroraDbMySqlConnector.UnitTests
{
    [TestClass]
    public class AuroraDbMySqlConnectorDefaultValueComposerTest
    {
        #region Helpers

        private static ColumnInfo GetColumn(ColumnDefaultValue kind,
            string expression,
            string provider = "MSSQL") =>
            new ColumnInfo
            {
                DefaultValue = kind,
                DefaultExpression = expression,
                Field = new DbField("Column", false, false, true, typeof(string), null, null, null, "varchar", expression != null, provider)
            };

        private static string Compose(ColumnDefaultValue kind,
            string expression,
            string type = "datetime",
            string provider = "MSSQL") =>
            AuroraDbMySqlConnectorDefaultValueComposer.Compose(GetColumn(kind, expression, provider), type, out _);

        private static string Reason(ColumnDefaultValue kind,
            string expression,
            string type = "varchar(36)",
            string provider = "MSSQL")
        {
            AuroraDbMySqlConnectorDefaultValueComposer.Compose(GetColumn(kind, expression, provider), type, out var reason);
            return reason;
        }

        #endregion

        #region No Default

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeReturnsNullIfTheColumnHasNoDefaultValue()
        {
            // Act/Assert
            Assert.IsNull(Compose(ColumnDefaultValue.None, null));
            Assert.IsNull(Compose(ColumnDefaultValue.None, "getdate()"));
            Assert.IsNull(Compose(ColumnDefaultValue.Unclassified, null));
            Assert.IsNull(Compose(ColumnDefaultValue.Unclassified, " "));
            Assert.IsNull(AuroraDbMySqlConnectorDefaultValueComposer.Compose(null, "int", out var reason));
            Assert.IsNull(reason);
        }

        #endregion

        #region As It Is

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeUsesTheExpressionAsItIsIfTheDefaultIsNotClassified()
        {
            // Act/Assert
            Assert.AreEqual("getdate()", Compose(ColumnDefaultValue.Unclassified, "getdate()"));
            Assert.AreEqual("((0))", Compose(ColumnDefaultValue.Unclassified, "((0))"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeUsesTheExpressionAsItIsIfTheProviderIsCompatible()
        {
            // Act/Assert
            Assert.AreEqual("current_timestamp()", Compose(ColumnDefaultValue.CurrentTimestamp, "current_timestamp()", provider: "AURORAMYSQL"));
            Assert.AreEqual("'it\\'s'", Compose(ColumnDefaultValue.Literal, "'it\\'s'", provider: "MYSQLC"));
            Assert.AreEqual("lower('X')", Compose(ColumnDefaultValue.Unsupported, "lower('X')", provider: "auroramysql"));
        }

        #endregion

        #region Literals

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheLiterals()
        {
            // Act/Assert
            Assert.AreEqual("0", Compose(ColumnDefaultValue.Literal, "((0))", "int"));
            Assert.AreEqual("'N/A'", Compose(ColumnDefaultValue.Literal, "(N'N/A')", "varchar(10)"));
            Assert.AreEqual("'abc'", Compose(ColumnDefaultValue.Literal, "'abc'::character varying", "varchar(10)", "PGSQL"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesNullAndTheBooleans()
        {
            // Act/Assert
            Assert.AreEqual("NULL", Compose(ColumnDefaultValue.Null, "NULL", "int"));
            Assert.AreEqual("1", Compose(ColumnDefaultValue.BooleanTrue, "true", "tinyint(1)", "PGSQL"));
            Assert.AreEqual("0", Compose(ColumnDefaultValue.BooleanFalse, "false", "tinyint(1)", "PGSQL"));
        }

        #endregion

        #region Date And Time

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheCurrentTimestampOfADateAndTimeColumn()
        {
            // Act/Assert
            Assert.AreEqual("CURRENT_TIMESTAMP", Compose(ColumnDefaultValue.CurrentTimestamp, "(getdate())", "datetime"));
            Assert.AreEqual("CURRENT_TIMESTAMP", Compose(ColumnDefaultValue.CurrentTimestamp, "(getdate())", "timestamp"));
            Assert.AreEqual("CURRENT_TIMESTAMP", Compose(ColumnDefaultValue.CurrentTimestamp, "(getdate())", "datetime(0)"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeGivesTheCurrentTimestampTheDigitsOfTheFractionalSecondsOfTheColumn()
        {
            // Act/Assert
            Assert.AreEqual("CURRENT_TIMESTAMP(6)", Compose(ColumnDefaultValue.CurrentTimestamp, "(sysdatetime())", "datetime(6)"));
            Assert.AreEqual("CURRENT_TIMESTAMP(3)", Compose(ColumnDefaultValue.CurrentTimestamp, "now()", "timestamp(3)", "PGSQL"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheCurrentTimestampInUtcAsAnExpression()
        {
            // Act/Assert
            Assert.AreEqual("(UTC_TIMESTAMP)", Compose(ColumnDefaultValue.CurrentTimestampUtc, "(getutcdate())", "datetime"));
            Assert.AreEqual("(UTC_TIMESTAMP(6))", Compose(ColumnDefaultValue.CurrentTimestampUtc, "(sysutcdatetime())", "datetime(6)"));
            Assert.AreEqual("(UTC_TIMESTAMP)", Compose(ColumnDefaultValue.CurrentTimestampWithOffset, "(sysdatetimeoffset())", "datetime"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheCurrentDateAndTimeByTheTypeOfTheColumn()
        {
            // Act/Assert
            Assert.AreEqual("(CURRENT_DATE)", Compose(ColumnDefaultValue.CurrentDate, "(CONVERT([date],getdate()))", "date"));
            Assert.AreEqual("(CURRENT_TIME)", Compose(ColumnDefaultValue.CurrentTime, "current_time", "time"));
            Assert.AreEqual("(CURRENT_TIME(3))", Compose(ColumnDefaultValue.CurrentTime, "current_time", "time(3)", "PGSQL"));
            Assert.AreEqual("(CURRENT_DATE)", Compose(ColumnDefaultValue.CurrentTimestamp, "getdate()", "date"));
            Assert.AreEqual("(CURRENT_TIME)", Compose(ColumnDefaultValue.CurrentTimestamp, "getdate()", "time"));
            Assert.AreEqual("(UTC_DATE())", Compose(ColumnDefaultValue.CurrentTimestampUtc, "getutcdate()", "date"));
            Assert.AreEqual("(UTC_TIME)", Compose(ColumnDefaultValue.CurrentTimestampUtc, "getutcdate()", "time"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheCurrentTimestampOfAColumnThatIsNotADateAndTimeAsAnExpression()
        {
            // Act/Assert
            Assert.AreEqual("(NOW())", Compose(ColumnDefaultValue.CurrentTimestamp, "getdate()", "varchar(30)"));
            Assert.AreEqual("(UTC_TIMESTAMP())", Compose(ColumnDefaultValue.CurrentTimestampUtc, "getutcdate()", "varchar(30)"));
        }

        #endregion

        #region Guids

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheNewGuidOfATextColumn()
        {
            // Act/Assert
            Assert.AreEqual("(UUID())", Compose(ColumnDefaultValue.NewGuid, "(newid())", "char(36)"));
            Assert.AreEqual("(UUID())", Compose(ColumnDefaultValue.NewGuid, "(newid())", "varchar(36)"));
            Assert.AreEqual("(UUID())", Compose(ColumnDefaultValue.NewSequentialGuid, "(newsequentialid())", "char(36)"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheNewGuidOfABinaryColumnAsItsBytes()
        {
            // Act/Assert
            Assert.AreEqual("(UNHEX(REPLACE(UUID(), '-', '')))", Compose(ColumnDefaultValue.NewGuid, "(newid())", "binary(16)"));
            Assert.AreEqual("(UNHEX(REPLACE(UUID(), '-', '')))", Compose(ColumnDefaultValue.NewGuid, "(newid())", "varbinary(16)"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeDoesNotComposeTheNewGuidOfAColumnThatCanNotHoldIt()
        {
            // Act
            var actual = Compose(ColumnDefaultValue.NewGuid, "(newid())", "int");
            var reason = Reason(ColumnDefaultValue.NewGuid, "(newid())", "int");

            // Assert
            Assert.IsNull(actual);
            StringAssert.Contains(reason, "GUID", System.StringComparison.Ordinal);
        }

        #endregion

        #region Others

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeComposesTheRandomNumberTheUserAndTheDatabase()
        {
            // Act/Assert
            Assert.AreEqual("(RAND())", Compose(ColumnDefaultValue.RandomNumber, "rand()", "double"));
            Assert.AreEqual("(CURRENT_USER())", Compose(ColumnDefaultValue.CurrentUser, "(suser_sname())", "varchar(128)"));
            Assert.AreEqual("(DATABASE())", Compose(ColumnDefaultValue.CurrentDatabase, "(db_name())", "varchar(128)"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeDoesNotComposeTheUserAndTheDatabaseOfAColumnThatIsNotAText()
        {
            // Act/Assert
            Assert.IsNull(Compose(ColumnDefaultValue.CurrentUser, "(suser_sname())", "int"));
            Assert.IsNull(Compose(ColumnDefaultValue.CurrentDatabase, "(db_name())", "datetime"));
            Assert.IsNotNull(Reason(ColumnDefaultValue.CurrentUser, "(suser_sname())", "int"));
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeDoesNotComposeTheNextValueOfASequence()
        {
            // Act
            var actual = Compose(ColumnDefaultValue.SequenceNextValue, "(NEXT VALUE FOR [dbo].[OrderSeq])", "bigint");
            var reason = Reason(ColumnDefaultValue.SequenceNextValue, "(NEXT VALUE FOR [dbo].[OrderSeq])", "bigint");

            // Assert
            Assert.IsNull(actual);
            StringAssert.Contains(reason, "sequence", System.StringComparison.Ordinal);
            StringAssert.Contains(reason, "MySQL", System.StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeDoesNotComposeTheDefaultThatIsNotKnownOfAnotherProvider()
        {
            // Act
            var unsupported = Compose(ColumnDefaultValue.Unsupported, "LAST_INSERT_ID()", "bigint");
            var expression = Compose(ColumnDefaultValue.Expression, "1 + 2", "int");
            var reason = Reason(ColumnDefaultValue.Unsupported, "LAST_INSERT_ID()", "bigint");

            // Assert
            Assert.IsNull(unsupported);
            Assert.IsNull(expression);
            StringAssert.Contains(reason, "LAST_INSERT_ID()", System.StringComparison.Ordinal);
            StringAssert.Contains(reason, "MSSQL", System.StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeNamesAnotherDatabaseIfTheProviderIsUnknown()
        {
            // Act
            var reason = Reason(ColumnDefaultValue.Unsupported, "LAST_INSERT_ID()", "bigint", provider: null);

            // Assert
            StringAssert.Contains(reason, "another database", System.StringComparison.Ordinal);
        }

        [TestMethod]
        public void TestAuroraDbMySqlConnectorDefaultValueComposerComposeHasNoReasonIfTheDefaultIsComposed()
        {
            // Act/Assert
            Assert.IsNull(Reason(ColumnDefaultValue.CurrentTimestamp, "getdate()", "datetime"));
            Assert.IsNull(Reason(ColumnDefaultValue.Literal, "((0))", "int"));
            Assert.IsNull(Reason(ColumnDefaultValue.None, null, "int"));
        }

        #endregion
    }
}
