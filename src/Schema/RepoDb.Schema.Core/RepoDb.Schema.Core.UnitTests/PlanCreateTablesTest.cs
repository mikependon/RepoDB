#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Schema.Core.UnitTests
{
    [TestClass]
    public class PlanCreateTablesTest
    {
        #region Private Methods

        private static ISchemaComposer CreateComposer()
        {
            var composer = new Mock<ISchemaComposer>();
            composer.Setup(item => item.ComposeName(It.IsAny<TableInfo>()))
                .Returns<TableInfo>(table => string.IsNullOrEmpty(table.Schema) ? table.Name : string.Concat(table.Schema, ".", table.Name));
            return composer.Object;
        }

        private static TableSchema CreateOrders() =>
            new TableSchema(new TableInfo("Orders", "dbo"));

        private static TableSchema CreateOrderItems() =>
            new TableSchema(new TableInfo("OrderItems", "dbo"))
            {
                ForeignKeys = new List<ForeignKeyInfo>
                {
                    new ForeignKeyInfo("FK_OrderItems_Orders") { ReferencedTable = new TableInfo("Orders", "dbo") }
                }
            };

        #endregion

        #region Without a schema in the destination

        [TestMethod]
        public void TestPlanCreateTablesRemovesTheSchemaOfTheTable()
        {
            // Act
            var tables = Plan.CreateTables(new[] { CreateOrders() }, null, CreateComposer(), removedSchema: "dbo");

            // Assert
            Assert.IsNull(tables.Single().Schema.Table.Schema);
        }

        [TestMethod]
        public void TestPlanCreateTablesComposesTheNameWithoutTheSchema()
        {
            // Act
            var tables = Plan.CreateTables(new[] { CreateOrders() }, null, CreateComposer(), removedSchema: "dbo");

            // Assert
            Assert.AreEqual("Orders", tables.Single().Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPlanCreateTablesRemovesTheSchemaOfTheForeignKeys()
        {
            // Act
            var tables = Plan.CreateTables(new[] { CreateOrderItems() }, null, CreateComposer(), removedSchema: "dbo");

            // Assert
            Assert.IsNull(tables.Single().Schema.ForeignKeys.Single().ReferencedTable.Schema);
        }

        [TestMethod]
        public void TestPlanCreateTablesRemovesTheSchemaWithBlankTargetSchema()
        {
            // Act
            var tables = Plan.CreateTables(new[] { CreateOrders() }, " ", CreateComposer(), removedSchema: "dbo");

            // Assert
            Assert.IsNull(tables.Single().Schema.Table.Schema);
        }

        [TestMethod]
        public void TestPlanCreateTablesKeepsTheOtherSchemas()
        {
            // Act
            var tables = Plan.CreateTables(new[] { new TableSchema(new TableInfo("Invoice", "Sales")) }, null, CreateComposer(), removedSchema: "dbo");

            // Assert
            Assert.AreEqual("Sales.Invoice", tables.Single().Name, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPlanCreateTablesWithTheSameNameAfterRemovingTheSchema()
        {
            // Setup
            var schemas = new[] { CreateOrders(), new TableSchema(new TableInfo("Orders", null)) };

            // Act and Assert
            Assert.Throws<InvalidOperationException>(() => Plan.CreateTables(schemas, null, CreateComposer(), removedSchema: "dbo"));
        }

        #endregion

        #region With a schema in the destination

        [TestMethod]
        public void TestPlanCreateTablesKeepsTheSchemaByDefault()
        {
            // Act
            var tables = Plan.CreateTables(new[] { CreateOrders() }, null, CreateComposer());

            // Assert
            Assert.AreEqual("dbo", tables.Single().Schema.Table.Schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPlanCreateTablesMovesToTheTargetSchema()
        {
            // Act
            var tables = Plan.CreateTables(new[] { CreateOrders() }, "sales", CreateComposer(), removedSchema: "dbo");

            // Assert
            Assert.AreEqual("sales", tables.Single().Schema.Table.Schema, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestPlanCreateTablesKeepsTheTargetSchema()
        {
            // Act
            var tables = Plan.CreateTables(new[] { CreateOrders() }, "dbo", CreateComposer());

            // Assert
            Assert.AreEqual("dbo.Orders", tables.Single().Name, StringComparer.Ordinal);
        }

        #endregion
    }
}
