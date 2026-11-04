#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Enumerations;
using RepoDb.Requests;
using RepoDb.UnitTests.CustomObjects;

namespace RepoDb.UnitTests.Requests
{
    [TestClass]
    [DoNotParallelize]
    public class RequestIdentityTest : RepositoryOperationTestBase
    {
        [TestMethod]
        public void TestAverageAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestAverageAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestAverageAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestAverageAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestAverageAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new AverageAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestAverageRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestAverageRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestAverageRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestAverageRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestAverageRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new AverageRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestBatchQueryRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) });
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestBatchQueryRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) });
            var second = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) });

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestBatchQueryRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestBatchQueryRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestBatchQueryRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new BatchQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                page: 0,
                rowsPerBatch: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestCountAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestCountAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestCountAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestCountAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestCountAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new CountAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestCountRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestCountRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestCountRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestCountRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestCountRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new CountRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestDeleteAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestDeleteAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestDeleteAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestDeleteAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestDeleteAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new DeleteAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestDeleteRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestDeleteRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestDeleteRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestDeleteRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestDeleteRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new DeleteRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestExistsRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestExistsRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestExistsRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestExistsRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestExistsRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new ExistsRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestInsertAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestInsertAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestInsertAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestInsertAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestInsertAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new InsertAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestInsertRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestInsertRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestInsertRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestInsertRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestInsertRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new InsertRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMaxAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMaxAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMaxAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMaxAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMaxAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MaxAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMaxRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMaxRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMaxRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMaxRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMaxRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MaxRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMergeAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMergeAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMergeAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMergeAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMergeAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MergeAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMergeRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMergeRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMergeRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMergeRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMergeRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MergeRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMinAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMinAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMinAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMinAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMinAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MinAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestMinRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMinRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMinRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestMinRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestMinRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new MinRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestQueryAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestQueryAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestQueryAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestQueryAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestQueryAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new QueryAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestQueryMultipleRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestQueryMultipleRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestQueryMultipleRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestQueryMultipleRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestQueryMultipleRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new QueryMultipleRequest(index: 2,
                type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestQueryRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestQueryRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestQueryRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestQueryRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestQueryRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new QueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                where: new QueryGroup(new QueryField("Id", 1)),
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                top: 2,
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSkipQueryRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) });
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestSkipQueryRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) });
            var second = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) });

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSkipQueryRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestSkipQueryRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSkipQueryRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new SkipQueryRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                skip: 0,
                take: 2,
                orderBy: new[] { new OrderField("Id", Order.Ascending) },
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSumAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestSumAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSumAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestSumAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSumAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new SumAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestSumRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestSumRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSumRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestSumRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestSumRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new SumRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                field: new Field("Id"),
                where: new QueryGroup(new QueryField("Id", 1)),
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestTruncateRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new TruncateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestTruncateRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new TruncateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new TruncateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestTruncateRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new TruncateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestTruncateRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new TruncateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                statementBuilder: new CustomStatementBuilder());
            var second = new TruncateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestUpdateAllRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestUpdateAllRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestUpdateAllRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestUpdateAllRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestUpdateAllRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new UpdateAllRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                fields: new[] { new Field("Id"), new Field("Name") },
                qualifiers: new[] { new Field("Id"), new Field("Name") },
                batchSize: 2,
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TestUpdateRequestWithDefaultsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestUpdateRequestWithDefaultsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);
            var second = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null);

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestUpdateRequestWithOptionsHasStableIdentity()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var request = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var expected = request.GetHashCode();

            // Act
            var actual = request.GetHashCode();

            // Assert
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(typeof(CoverageEntity), request.Type);
        }

        [TestMethod]
        public void TestUpdateRequestWithOptionsEquivalentRequestsAreEqual()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void TestUpdateRequestDistinguishesHints()
        {
            // Setup
            using var connection = new CoverageDbConnection();
            var first = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (NOLOCK)",
                statementBuilder: new CustomStatementBuilder());
            var second = new UpdateRequest(type: typeof(CoverageEntity),
                connection: connection,
                transaction: null,
                where: new QueryGroup(new QueryField("Id", 1)),
                fields: new[] { new Field("Id"), new Field("Name") },
                hints: "WITH (UPDLOCK)",
                statementBuilder: new CustomStatementBuilder());

            // Act
            var actual = first.Equals(second);

            // Assert
            Assert.IsFalse(actual);
        }

    }
}
