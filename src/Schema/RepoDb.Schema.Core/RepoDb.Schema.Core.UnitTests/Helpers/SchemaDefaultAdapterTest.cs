#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaDefaultAdapterTest
    {
        #region SubClasses

        private sealed class KindFormatter : SchemaValueFormatter
        {
            public override bool HandlesKinds => true;

            public override string FormatKind(ColumnDefaultValue kind,
                string destinationType,
                out string warning)
            {
                warning = null;
                return kind == ColumnDefaultValue.CurrentTimestamp ? "NOW()" : null;
            }
        }

        private sealed class RecordingFormatter : SchemaValueFormatter
        {
            public string DestinationType { get; private set; }

            protected override string FormatBoolean(bool value,
                string destinationType)
            {
                DestinationType = destinationType;
                return base.FormatBoolean(value,
                    destinationType);
            }
        }

        private sealed class WarningFormatter : SchemaValueFormatter
        {
            protected override string FormatDateTimeOffset(DateTimeOffset value,
                string destinationType,
                out string warning)
            {
                warning = "its offset was applied";
                return "'converted'";
            }
        }

        #endregion

        #region Private Methods

        private static ColumnInfo CreateColumn(string name,
            Type type,
            ColumnDefaultValue kind,
            string expression,
            string nativeType = null,
            string provider = "MSSQL") =>
            new ColumnInfo
            {
                Collation = "Latin1",
                Comment = "A comment",
                DefaultExpression = expression,
                DefaultValue = kind,
                NativeType = nativeType,
                Ordinal = 3,
                Field = new DbField(name,
                    false,
                    false,
                    true,
                    type,
                    null,
                    null,
                    null,
                    nativeType,
                    false,
                    provider)
            };

        private static ISchemaComposer CreateComposer(ISchemaValueFormatter formatter,
            bool supportsValues = true)
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(item => item.ComposeTypeName(It.IsAny<ColumnInfo>())).Returns("destination-type");
            if (supportsValues)
            {
                composer.As<ISchemaValueSupport>().SetupGet(item => item.ValueFormatter).Returns(formatter);
            }

            return composer.Object;
        }

        private static IDbSetting CreateSetting(string provider = "PGSQL")
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(item => item.Provider).Returns(provider);
            return setting.Object;
        }

        private static PlanTable CreateTable(string tableProvider,
            params ColumnInfo[] columns)
        {
            var schema = new TableSchema(new TableInfo("Products",
                "dbo",
                "Northwind",
                tableProvider));
            foreach (var column in columns)
            {
                schema.Columns.Add(column);
            }

            return new PlanTable(schema,
                schema,
                "Products");
        }

        private static ColumnInfo CreateBitColumn(string expression = "((0))",
            ColumnDefaultValue kind = ColumnDefaultValue.Literal) =>
            CreateColumn("Discontinued",
                typeof(bool),
                kind,
                expression,
                "bit");

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestAdaptWritesALiteralInTheFormatOfTheDestination()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("FALSE", table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptMarksTheConvertedDefaultAsWrittenForTheDestination()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual(ColumnDefaultValue.Unclassified, table.Schema.Columns.Single().DefaultValue);
        }

        [TestMethod]
        public void TestAdaptWritesABooleanKindInTheFormatOfTheDestination()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn("1",
                    ColumnDefaultValue.BooleanTrue));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("TRUE", table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptGivesTheTypeOfTheDestinationColumnToTheFormatter()
        {
            // Setup
            var formatter = new RecordingFormatter();
            var table = CreateTable("MSSQL",
                CreateBitColumn());

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(formatter),
                CreateSetting());

            // Assert
            Assert.AreEqual("destination-type", formatter.DestinationType);
        }

        [TestMethod]
        public void TestAdaptCreatesANewSchemaOfTheTable()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreNotSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNotChangeTheSchemaThatWasRead()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("((0))", source.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptKeepsTheOtherPropertiesOfTheColumn()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("A comment", table.Schema.Columns.Single().Comment);
        }

        [TestMethod]
        public void TestAdaptKeepsTheFieldOfTheColumn()
        {
            // Setup
            var column = CreateBitColumn();
            var table = CreateTable("MSSQL",
                column);

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(column.Field, table.Schema.Columns.Single().Field);
        }

        [TestMethod]
        public void TestAdaptKeepsTheColumnsThatAreNotChanged()
        {
            // Setup
            var other = CreateColumn("Name",
                typeof(string),
                ColumnDefaultValue.None,
                null);
            var table = CreateTable("MSSQL",
                CreateBitColumn(),
                other);

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(other, table.Schema.Columns[1]);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfBothHaveTheSameDialect()
        {
            // Setup
            var table = CreateTable("PGSQL",
                CreateColumn("Discontinued",
                    typeof(bool),
                    ColumnDefaultValue.Literal,
                    "true",
                    "boolean",
                    "PGSQL"));
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptUsesTheProviderOfTheFieldBeforeTheOneOfTheTable()
        {
            // Setup
            var table = CreateTable("PGSQL",
                CreateColumn("Discontinued",
                    typeof(bool),
                    ColumnDefaultValue.Literal,
                    "((0))",
                    "bit",
                    "MSSQL"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("FALSE", table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptUsesTheProviderOfTheTableIfTheFieldHasNone()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Discontinued",
                    typeof(bool),
                    ColumnDefaultValue.Literal,
                    "((0))",
                    "bit",
                    null));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("FALSE", table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheProviderOfTheTableIsNotKnown()
        {
            // Setup
            var table = CreateTable("FileProvider",
                CreateColumn("Discontinued",
                    typeof(bool),
                    ColumnDefaultValue.Literal,
                    "((0))",
                    "bit",
                    null));
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheDestinationIsNotKnown()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting(null));

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfThereIsNoDestinationSetting()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                null);

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheComposerGivesNoFormatter()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(null,
                    false),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheTableHasNoColumns()
        {
            // Setup
            var table = CreateTable("MSSQL");
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptLeavesAColumnWithoutADefaultValue()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Name",
                    typeof(string),
                    ColumnDefaultValue.None,
                    null));
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptLeavesADefaultThatIsNotClassified()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn("((0))",
                    ColumnDefaultValue.Unclassified));
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptLeavesANullDefault()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn("NULL",
                    ColumnDefaultValue.Null));
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptLeavesAKindThatTheFormatterDoesNotHandle()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Created",
                    typeof(DateTime),
                    ColumnDefaultValue.CurrentTimestamp,
                    "(getdate())",
                    "datetime"));
            var source = table.Schema;

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptWritesAKindThatTheFormatterHandles()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Created",
                    typeof(DateTime),
                    ColumnDefaultValue.CurrentTimestamp,
                    "(getdate())",
                    "datetime"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new KindFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("NOW()", table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptSkipsAKindThatTheDestinationHasNoEquivalentFor()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Key",
                    typeof(Guid),
                    ColumnDefaultValue.NewGuid,
                    "(newid())",
                    "uniqueidentifier"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new KindFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual(ColumnDefaultValue.None, table.Schema.Columns.Single().DefaultValue);
        }

        [TestMethod]
        public void TestAdaptReportsAKindThatTheDestinationHasNoEquivalentFor()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Key",
                    typeof(Guid),
                    ColumnDefaultValue.NewGuid,
                    "(newid())",
                    "uniqueidentifier"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new KindFormatter()),
                CreateSetting());

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "Skipped the default value of the column 'Key'");
        }

        [TestMethod]
        public void TestAdaptSkipsAnExpressionOfAnotherDialect()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Total",
                    typeof(int),
                    ColumnDefaultValue.Expression,
                    "(len('abc'))",
                    "int"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new KindFormatter()),
                CreateSetting());

            // Assert
            Assert.IsNull(table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptReportsAnExpressionOfAnotherDialect()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Total",
                    typeof(int),
                    ColumnDefaultValue.Expression,
                    "(len('abc'))",
                    "int"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new KindFormatter()),
                CreateSetting());

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "written in the language of MSSQL");
        }

        [TestMethod]
        public void TestAdaptReportsAnExpressionOfAFieldWithoutAProviderAsAnotherDatabase()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Total",
                    typeof(int),
                    ColumnDefaultValue.Expression,
                    "(len('abc'))",
                    "int",
                    null));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new KindFormatter()),
                CreateSetting());

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "written in the language of another database");
        }

        [TestMethod]
        public void TestAdaptSkipsALiteralThatIsNotUnderstood()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn("'maybe'"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual(ColumnDefaultValue.None, table.Schema.Columns.Single().DefaultValue);
        }

        [TestMethod]
        public void TestAdaptReportsALiteralThatIsNotUnderstood()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn("'maybe'"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "its value ''maybe'' is not a literal that is understood");
        }

        [TestMethod]
        public void TestAdaptSkipsALiteralOfATypeWithoutAKind()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Data",
                    typeof(object),
                    ColumnDefaultValue.Literal,
                    "'x'"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.IsNull(table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptSkipsAValueThatTheDestinationCanNotHold()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Photo",
                    typeof(byte[]),
                    ColumnDefaultValue.Literal,
                    "0xDEAD",
                    "varbinary"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual(ColumnDefaultValue.None, table.Schema.Columns.Single().DefaultValue);
        }

        [TestMethod]
        public void TestAdaptReportsAValueThatTheDestinationCanNotHold()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Photo",
                    typeof(byte[]),
                    ColumnDefaultValue.Literal,
                    "0xDEAD",
                    "varbinary"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "can not hold a Binary value");
        }

        [TestMethod]
        public void TestAdaptKeepsTheDefaultOfAValueThatWasChanged()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Created",
                    typeof(DateTimeOffset),
                    ColumnDefaultValue.Literal,
                    "'2020-01-31T10:20:30+01:00'",
                    "datetimeoffset"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new WarningFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("'converted'", table.Schema.Columns.Single().DefaultExpression);
        }

        [TestMethod]
        public void TestAdaptReportsAValueThatWasChanged()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateColumn("Created",
                    typeof(DateTimeOffset),
                    ColumnDefaultValue.Literal,
                    "'2020-01-31T10:20:30+01:00'",
                    "datetimeoffset"));

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new WarningFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual("Converted the default value of the column 'Created': its offset was applied.", table.Warnings.Single());
        }

        [TestMethod]
        public void TestAdaptDoesNotReportAValueThatWasWrittenAsItIs()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBitColumn());

            // Act
            SchemaDefaultAdapter.Adapt(table,
                CreateComposer(new SchemaValueFormatter()),
                CreateSetting());

            // Assert
            Assert.AreEqual(0, table.Warnings.Count);
        }

        #endregion
    }
}
