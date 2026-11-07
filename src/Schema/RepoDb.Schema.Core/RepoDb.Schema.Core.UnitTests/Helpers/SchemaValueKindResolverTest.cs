#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Schema.Core.UnitTests.CustomObjects;
using RepoDb.Schema.Enumerations;
using RepoDb.Schema.Models;

namespace RepoDb.Schema.Core.UnitTests.Helpers
{
    [TestClass]
    public class SchemaValueKindResolverTest
    {
        #region Private Methods

        private static ColumnInfo CreateColumn(Type type,
            string nativeType = null,
            string provider = "MSSQL",
            string databaseType = null) =>
            new ColumnInfo
            {
                NativeType = nativeType,
                Field = new DbField("Column",
                    false,
                    false,
                    true,
                    type,
                    null,
                    null,
                    null,
                    databaseType,
                    false,
                    provider)
            };

        #endregion

        #region Public Methods

        [TestMethod]
        public void TestResolveABooleanColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(bool)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Boolean, kind);
        }

        [TestMethod]
        public void TestResolveAnIntegerColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(int)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Number, kind);
        }

        [TestMethod]
        public void TestResolveADecimalColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(decimal)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Number, kind);
        }

        [TestMethod]
        public void TestResolveADoubleColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(double)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Number, kind);
        }

        [TestMethod]
        public void TestResolveAStringColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(string)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Text, kind);
        }

        [TestMethod]
        public void TestResolveACharColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(char)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Text, kind);
        }

        [TestMethod]
        public void TestResolveADateTimeColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(DateTime),
                "datetime"),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.DateTime, kind);
        }

        [TestMethod]
        public void TestResolveADateTimeColumnWithoutANativeType()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(DateTime)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.DateTime, kind);
        }

        [TestMethod]
        public void TestResolveADateColumnByItsNativeType()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(DateTime),
                "date"),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Date, kind);
        }

        [TestMethod]
        public void TestResolveADateColumnByItsDatabaseTypeIfThereIsNoNativeType()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(DateTime),
                null,
                "PGSQL",
                "DATE"),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Date, kind);
        }

        [TestMethod]
        public void TestResolveADateColumnAsADateTimeWhenTheDialectSaysItHoldsATime()
        {
            // Setup
            var dialect = new Mock<ISchemaDialect>();
            dialect.Setup(item => item.IsDateOnly(It.IsAny<string>())).Returns(false);

            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(DateTime),
                "DATE"),
                dialect.Object);

            // Assert
            Assert.AreEqual(SchemaValueKind.DateTime, kind);
        }

        [TestMethod]
        public void TestResolveADateTimeOffsetColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(DateTimeOffset)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.DateTimeOffset, kind);
        }

        [TestMethod]
        public void TestResolveATimeSpanColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(TimeSpan)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Time, kind);
        }

        [TestMethod]
        public void TestResolveAGuidColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(Guid)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Guid, kind);
        }

        [TestMethod]
        public void TestResolveABinaryColumn()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(byte[])),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Binary, kind);
        }

        [TestMethod]
        public void TestResolveANullableColumnByItsUnderlyingType()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(int?)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.AreEqual(SchemaValueKind.Number, kind);
        }

        [TestMethod]
        public void TestResolveATypeWithoutAnEquivalentAsNull()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(CreateColumn(typeof(object)),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(kind);
        }

        [TestMethod]
        public void TestResolveAColumnWithoutAFieldAsNull()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(new ColumnInfo(),
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(kind);
        }

        [TestMethod]
        public void TestResolveANullColumnAsNull()
        {
            // Act
            var kind = SchemaValueKindResolver.Resolve(null,
                CustomSchemaDialect.Instance);

            // Assert
            Assert.IsNull(kind);
        }

        #endregion
    }
}
