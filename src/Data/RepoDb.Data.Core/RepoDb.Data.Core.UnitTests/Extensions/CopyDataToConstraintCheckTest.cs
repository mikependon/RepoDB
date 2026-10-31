#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Enumerations;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;
using RepoDb.Schema;

namespace RepoDb.Data.Core.UnitTests.Extensions
{
    /// <summary>
    /// Contains the tests of the <see cref="ConstraintCheckBehavior"/> argument of <c>CopyDataTo</c> and <c>CopyDataToAsync</c>: the foreign keys of the destination
    /// are not checked while the rows are copied, they are checked again afterwards (even if the copy fails), and a destination that can not do it is refused up front.
    /// </summary>
    [TestClass]
    public class CopyDataToConstraintCheckTest
    {
        #region Private Variables

        /// <summary>
        /// The statement that stops the destination from checking the foreign keys.
        /// </summary>
        private const string DisableStatement = "SET CHECKS OFF";

        /// <summary>
        /// The statement that makes the destination check the foreign keys again.
        /// </summary>
        private const string EnableStatement = "SET CHECKS ON";

        #endregion

        #region Public Methods

        [TestCleanup]
        public void Cleanup()
        {
            SchemaComposerMapper.Clear();
        }

        [TestMethod]
        public void TestCopyDataToWithDeferStopsTheChecksBeforeTheRowsAndStartsThemAfter()
        {
            // Setup
            var composer = MapComposer(true);
            var destination = new FakeDbConnection();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", destination, constraintCheckBehavior: ConstraintCheckBehavior.Defer);

            // Assert
            var statements = destination.Executions.Select(e => e.CommandText).ToList();
            Assert.AreEqual(DisableStatement, statements.First());
            Assert.AreEqual(EnableStatement, statements.Last());
            Assert.IsTrue(statements.Skip(1).Take(statements.Count - 2).Any(s => s.StartsWith("INSERT INTO", StringComparison.Ordinal)));
            composer.Verify(c => c.ComposeDisableConstraintChecks(false), Times.Once);
            composer.Verify(c => c.ComposeEnableConstraintChecks(false), Times.Once);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDeferStopsTheChecksBeforeTheRowsAndStartsThemAfter()
        {
            // Setup
            MapComposer(true);
            var destination = new FakeDbConnection();

            // Act
            await new FakeDbConnection().CopyDataToAsync("Person", "Customer", destination, constraintCheckBehavior: ConstraintCheckBehavior.Defer);

            // Assert
            var statements = destination.Executions.Select(e => e.CommandText).ToList();
            Assert.AreEqual(DisableStatement, statements.First());
            Assert.AreEqual(EnableStatement, statements.Last());
            Assert.IsTrue(statements.Skip(1).Take(statements.Count - 2).Any(s => s.StartsWith("INSERT INTO", StringComparison.Ordinal)));
        }

        [TestMethod]
        public void TestCopyDataToByDefaultDoesNotTouchTheChecks()
        {
            // Setup
            var composer = MapComposer(true);
            var destination = new FakeDbConnection();

            // Act
            new FakeDbConnection().CopyDataTo("Person", "Customer", destination);

            // Assert
            Assert.IsFalse(destination.Executions.Any(e => e.CommandText == DisableStatement || e.CommandText == EnableStatement));
            composer.Verify(c => c.ComposeDisableConstraintChecks(It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void TestCopyDataToWithDeferStartsTheChecksAgainAndThrowsTheSameErrorWhenTheCopyFails()
        {
            // Setup
            MapComposer(true);
            var destination = new FakeDbConnection();
            var failure = new InvalidOperationException("The copy failed.");
            var interceptors = new IDataInterceptor<Row>[] { new FakeDataInterceptor(row => throw failure) };

            // Act/Assert
            var thrown = Assert.Throws<InvalidOperationException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", destination, dataInterceptors: interceptors, constraintCheckBehavior: ConstraintCheckBehavior.Defer));
            Assert.AreSame(failure, thrown);
            Assert.AreEqual(DisableStatement, destination.Executions.First().CommandText);
            Assert.AreEqual(EnableStatement, destination.Executions.Last().CommandText);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDeferStartsTheChecksAgainAndThrowsTheSameErrorWhenTheCopyFails()
        {
            // Setup
            MapComposer(true);
            var destination = new FakeDbConnection();
            var failure = new InvalidOperationException("The copy failed.");
            var interceptors = new IDataInterceptor<Row>[] { new FakeDataInterceptor(row => throw failure) };

            // Act/Assert
            var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                new FakeDbConnection().CopyDataToAsync("Person", "Customer", destination, dataInterceptors: interceptors, constraintCheckBehavior: ConstraintCheckBehavior.Defer));
            Assert.AreSame(failure, thrown);
            Assert.AreEqual(EnableStatement, destination.Executions.Last().CommandText);
        }

        [TestMethod]
        public void TestCopyDataToWithDeferThrowsBeforeAnythingIsExecutedWhenTheDestinationDoesNotSupportIt()
        {
            // Setup
            MapComposer(false);
            var destination = new FakeDbConnection();

            // Act/Assert
            Assert.Throws<NotSupportedException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", destination, constraintCheckBehavior: ConstraintCheckBehavior.Defer));
            Assert.AreEqual(0, destination.Executions.Count);
        }

        [TestMethod]
        public async Task TestCopyDataToAsyncWithDeferThrowsBeforeAnythingIsExecutedWhenTheDestinationDoesNotSupportIt()
        {
            // Setup
            MapComposer(false);
            var destination = new FakeDbConnection();

            // Act/Assert
            await Assert.ThrowsAsync<NotSupportedException>(() =>
                new FakeDbConnection().CopyDataToAsync("Person", "Customer", destination, constraintCheckBehavior: ConstraintCheckBehavior.Defer));
            Assert.AreEqual(0, destination.Executions.Count);
        }

        [TestMethod]
        public void ThrowExceptionOnCopyDataToIfTheConstraintCheckBehaviorIsNotDefined()
        {
            // Act/Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new FakeDbConnection().CopyDataTo("Person", "Customer", new FakeDbConnection(), constraintCheckBehavior: (ConstraintCheckBehavior)99));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Maps the schema composer of the destination connection.
        /// </summary>
        /// <param name="supportsConstraintChecks">Whether the composer implements <see cref="IConstraintCheckSupport"/>.</param>
        /// <returns>The mock of the support (not used if the composer does not implement it).</returns>
        private static Mock<IConstraintCheckSupport> MapComposer(bool supportsConstraintChecks)
        {
            var composer = new Mock<ISchemaComposer>();
            var support = composer.As<IConstraintCheckSupport>();
            support.Setup(c => c.ComposeDisableConstraintChecks(It.IsAny<bool>())).Returns(DisableStatement);
            support.Setup(c => c.ComposeEnableConstraintChecks(It.IsAny<bool>())).Returns(EnableStatement);
            SchemaComposerMapper.Add<FakeDbConnection>(supportsConstraintChecks ? composer.Object : new Mock<ISchemaComposer>().Object, true);
            return support;
        }

        #endregion
    }
}
