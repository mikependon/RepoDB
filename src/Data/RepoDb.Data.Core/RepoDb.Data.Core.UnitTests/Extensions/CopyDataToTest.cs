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
using RepoDb.Data.Enumerations;
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
