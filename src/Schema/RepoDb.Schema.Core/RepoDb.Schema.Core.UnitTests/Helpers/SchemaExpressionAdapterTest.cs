#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaExpressionAdapterTest
    {
        #region Private Methods

        private static IDbSetting CreateSetting(string provider,
            string opening = "\"",
            string closing = "\"")
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(item => item.Provider).Returns(provider);
            setting.SetupGet(item => item.OpeningQuote).Returns(opening);
            setting.SetupGet(item => item.ClosingQuote).Returns(closing);
            return setting.Object;
        }

        private static PlanTable CreateTable(string provider,
            params CheckConstraintInfo[] checkConstraints)
        {
            var schema = new TableSchema(new TableInfo("Employees",
                "dbo",
                "Northwind",
                provider));
            foreach (var checkConstraint in checkConstraints)
            {
                schema.CheckConstraints.Add(checkConstraint);
            }

            return new PlanTable(schema,
                schema,
                "Employees");
        }

        private static CheckConstraintInfo CreatePriceCheck() =>
            new CheckConstraintInfo("CK_Price")
            {
                Expression = "([UnitPrice]>=(0))"
            };

        private static CheckConstraintInfo CreateBirthdateCheck() =>
            new CheckConstraintInfo("CK_Birthdate")
            {
                Expression = "([BirthDate]<getdate())"
            };

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestAdaptTranslatesTheQuotedNamesOfACheckConstraint()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreEqual("(\"UnitPrice\">=(0))", table.Schema.CheckConstraints.Single().Expression);
        }

        [TestMethod]
        public void TestAdaptKeepsTheNameOfTheCheckConstraint()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreEqual("CK_Price", table.Schema.CheckConstraints.Single().Name);
        }

        [TestMethod]
        public void TestAdaptDoesNotChangeTheSchemaThatWasRead()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreEqual("([UnitPrice]>=(0))", source.CheckConstraints.Single().Expression);
        }

        [TestMethod]
        public void TestAdaptCreatesANewSchemaOfTheTable()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreNotSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptKeepsTheTableOfTheSchema()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source.Table, table.Schema.Table);
        }

        [TestMethod]
        public void TestAdaptKeepsTheColumnsOfTheSchema()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source.Columns, table.Schema.Columns);
        }

        [TestMethod]
        public void TestAdaptKeepsTheForeignKeysOfTheSchema()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source.ForeignKeys, table.Schema.ForeignKeys);
        }

        [TestMethod]
        public void TestAdaptKeepsTheIndexesOfTheSchema()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source.Indexes, table.Schema.Indexes);
        }

        [TestMethod]
        public void TestAdaptKeepsThePrimaryKeyOfTheSchema()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            table.Schema.PrimaryKey = new PrimaryKeyInfo("PK_Employees");
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source.PrimaryKey, table.Schema.PrimaryKey);
        }

        [TestMethod]
        public void TestAdaptKeepsTheUniqueConstraintsOfTheSchema()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source.UniqueConstraints, table.Schema.UniqueConstraints);
        }

        [TestMethod]
        public void TestAdaptLeavesOutACheckConstraintThatIsNotPortable()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreEqual(0, table.Schema.CheckConstraints.Count);
        }

        [TestMethod]
        public void TestAdaptReportsTheNameOfTheCheckConstraintThatIsLeftOut()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "'CK_Birthdate'");
        }

        [TestMethod]
        public void TestAdaptReportsTheFunctionThatIsNotPortable()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "getdate()");
        }

        [TestMethod]
        public void TestAdaptReportsTheProvidersInTheWarning()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            StringAssert.Contains(table.Warnings.Single(), "from MSSQL to PGSQL");
        }

        [TestMethod]
        public void TestAdaptKeepsTheCheckConstraintsThatArePortable()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck(),
                CreatePriceCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreEqual("CK_Price", table.Schema.CheckConstraints.Single().Name);
        }

        [TestMethod]
        public void TestAdaptKeepsACheckConstraintWithoutAnExpression()
        {
            // Setup
            var checkConstraint = new CheckConstraintInfo("CK_Empty");
            var table = CreateTable("MSSQL",
                checkConstraint);

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(checkConstraint, table.Schema.CheckConstraints.Single());
        }

        [TestMethod]
        public void TestAdaptKeepsACheckConstraintWithAWhiteSpaceExpression()
        {
            // Setup
            var checkConstraint = new CheckConstraintInfo("CK_Empty")
            {
                Expression = "  "
            };
            var table = CreateTable("MSSQL",
                checkConstraint);

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(checkConstraint, table.Schema.CheckConstraints.Single());
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfBothHaveTheSameDialect()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("MSSQL"));

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfBothProvidersAreOfTheSameFamily()
        {
            // Setup
            var table = CreateTable("MYSQL",
                CreateBirthdateCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("MARIADB"));

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheProviderOfTheTableIsNotKnown()
        {
            // Setup
            var table = CreateTable("FileProvider",
                CreateBirthdateCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheProviderOfTheTableIsNull()
        {
            // Setup
            var table = CreateTable(null,
                CreateBirthdateCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheProviderOfTheDestinationIsNotKnown()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting(null));

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfThereIsNoDestinationSetting()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreateBirthdateCheck());
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                null);

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNothingIfTheTableHasNoCheckConstraints()
        {
            // Setup
            var table = CreateTable("MSSQL");
            var source = table.Schema;

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreSame(source, table.Schema);
        }

        [TestMethod]
        public void TestAdaptDoesNotReportAnythingIfAllAreTranslated()
        {
            // Setup
            var table = CreateTable("MSSQL",
                CreatePriceCheck());

            // Act
            SchemaExpressionAdapter.Adapt(table,
                CreateSetting("PGSQL"));

            // Assert
            Assert.AreEqual(0, table.Warnings.Count);
        }

        #endregion
    }
}
