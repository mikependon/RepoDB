#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Schema.Enumerations;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    [TestClass]
    public class CopyDataToTest
    {
        #region Helpers

        private static IDbConnection GetConnection() =>
            new Mock<IDbConnection>().Object;

        private static readonly CopyDataRelationshipBehavior[] RelationshipBehaviors =
        {
            CopyDataRelationshipBehavior.Parents,
            CopyDataRelationshipBehavior.Children,
            CopyDataRelationshipBehavior.ParentsAndChildren
        };

        private static readonly CopySchemaExistsBehavior[] ExistsBehaviors =
        {
            CopySchemaExistsBehavior.Align,
            CopySchemaExistsBehavior.Throw,
            CopySchemaExistsBehavior.Drop
        };

        #endregion

        #region Arguments

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => ((IDbConnection)null).CopyDataTo("Person", "Person", GetConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheSourceTableIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo(null, "Person", GetConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTargetTableIsWhiteSpace()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo("Person", " ", GetConnection()));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheDestinationConnectionIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => GetConnection().CopyDataTo("Person", "Person", (IDbConnection)null));
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheBatchSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => GetConnection().CopyDataTo("Person", "Person", GetConnection(), batchSize: 0));
        }

        #endregion

        #region Schema

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            foreach (var behavior in RelationshipBehaviors)
            {
                // Act/Assert
                Assert.Throws<ArgumentException>(() =>
                    GetConnection().CopyDataTo("Person", "Customer", GetConnection(), relationshipBehavior: behavior));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheTableExistenceBehaviorIsNotSkip()
        {
            foreach (var behavior in ExistsBehaviors)
            {
                // Act/Assert
                Assert.Throws<ArgumentException>(() =>
                    GetConnection().CopyDataTo("Person", "Customer", GetConnection(), tableExistenceBehavior: behavior));
            }
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() =>
                GetConnection().CopyDataTo("Person", "Sales.Customer", new CustomDbConnection()));
            Assert.Throws<ArgumentException>(() =>
                GetConnection().CopyDataTo("Person", "[Sales].[Customer]", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheTargetTableHasASchema()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                GetConnection().CopyDataToAsync("Person", "Sales.Customer", new CustomDbConnection()));
        }

        [TestMethod]
        public void TestCopyDataToWithATargetTableOfAnotherSchemaCopiesTheSchema()
        {
            // Act/Assert (the table has the same name, so it is valid and the schema is copied, but there is no schema reader for the connection)
            Assert.Throws<MissingMappingException>(() =>
                GetConnection().CopyDataTo("Person", "Sales.Person", new CustomDbConnection()));
            Assert.Throws<MissingMappingException>(() =>
                GetConnection().CopyDataTo("Person", "[Sales].[Person]", new CustomDbConnection()));
            Assert.Throws<MissingMappingException>(() =>
                new CustomDbConnection().CopyDataTo("[dbo].[Person]", "Sales.Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithATargetTableOfAnotherSchemaCopiesTheSchema()
        {
            // Act/Assert
            await Assert.ThrowsAsync<MissingMappingException>(() =>
                GetConnection().CopyDataToAsync("Person", "Sales.Person", new CustomDbConnection()));
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheRelationshipBehaviorIsNotTableOnly()
        {
            foreach (var behavior in RelationshipBehaviors)
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    GetConnection().CopyDataToAsync("Person", "Customer", GetConnection(), relationshipBehavior: behavior));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheTablesHaveDifferentNamesAndTheTableExistenceBehaviorIsNotSkip()
        {
            foreach (var behavior in ExistsBehaviors)
            {
                // Act/Assert
                await Assert.ThrowsAsync<ArgumentException>(() =>
                    GetConnection().CopyDataToAsync("Person", "Customer", GetConnection(), tableExistenceBehavior: behavior));
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnCopyDataToAsyncIfTheBatchSizeIsNotGreaterThanZero()
        {
            // Act/Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => GetConnection().CopyDataToAsync("Person", "Person", GetConnection(), batchSize: 0));
        }

        #endregion
    }
}
