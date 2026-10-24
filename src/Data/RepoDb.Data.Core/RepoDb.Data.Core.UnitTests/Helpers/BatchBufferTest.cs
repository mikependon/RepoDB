#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Data.Core.UnitTests.CustomObjects;
using RepoDb.Data.Helpers;
using RepoDb.Data.Interfaces;
using RepoDb.Data.Models;

namespace RepoDb.Data.Core.UnitTests.Helpers
{
    [TestClass]
    public class BatchBufferTest
    {
        #region Helpers

        private static IDataReader CreateSource(int rows)
        {
            var table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            table.Columns.Add("Name", typeof(string));
            for (var i = 1; i <= rows; i++)
            {
                table.Rows.Add(i, i % 2 == 0 ? (object)DBNull.Value : "Name " + i);
            }
            return table.CreateDataReader();
        }

        private static List<int> Ids(IDataReader reader)
        {
            var ids = new List<int>();
            while (reader.Read())
            {
                ids.Add(reader.GetInt32(0));
            }
            return ids;
        }

        #endregion

        #region Constructors

        [TestMethod]
        public void ThrowExceptionOnBatchBufferIfTheSourceIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentNullException>(() => new BatchBuffer(null, 10));
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void ThrowExceptionOnBatchBufferIfTheBatchSizeIsLessThanOne(int batchSize)
        {
            // Act/Assert
            using (var source = CreateSource(1))
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => new BatchBuffer(source, batchSize));
            }
        }

        #endregion

        #region Fill

        [TestMethod]
        public void TestFillBuffersTheRowsUpToTheBatchSizeThenTheRest()
        {
            // Setup
            using (var source = CreateSource(5))
            using (var buffer = new BatchBuffer(source, 2))
            {
                var batches = new List<List<int>>();

                // Act
                while (buffer.Fill())
                {
                    using (var reader = buffer.CreateReader())
                    {
                        batches.Add(Ids(reader));
                    }
                }

                // Assert
                Assert.AreEqual(3, batches.Count);
                CollectionAssert.AreEqual(new[] { 1, 2 }, batches[0]);
                CollectionAssert.AreEqual(new[] { 3, 4 }, batches[1]);
                CollectionAssert.AreEqual(new[] { 5 }, batches[2]);
            }
        }

        [TestMethod]
        public void TestFillCountsTheRowsThatAreBuffered()
        {
            // Setup
            using (var source = CreateSource(3))
            using (var buffer = new BatchBuffer(source, 2))
            {
                // Act/Assert
                Assert.AreEqual(0, buffer.Count);
                Assert.IsTrue(buffer.Fill());
                Assert.AreEqual(2, buffer.Count);
                Assert.IsTrue(buffer.Fill());
                Assert.AreEqual(1, buffer.Count);
                Assert.IsFalse(buffer.Fill());
                Assert.AreEqual(0, buffer.Count);
            }
        }

        [TestMethod]
        public void TestFillClearsTheRowsOfThePreviousBatch()
        {
            // Setup
            using (var source = CreateSource(4))
            using (var buffer = new BatchBuffer(source, 2))
            {
                // Act
                buffer.Fill();
                buffer.Fill();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    CollectionAssert.AreEqual(new[] { 3, 4 }, Ids(reader));
                }
            }
        }

        [TestMethod]
        public void TestFillOfASourceWithoutRowsReturnsFalse()
        {
            // Setup
            using (var source = CreateSource(0))
            using (var buffer = new BatchBuffer(source, 2))
            {
                // Act/Assert
                Assert.IsFalse(buffer.Fill());
            }
        }

        [TestMethod]
        public void TestFillKeepsTheNullValuesAsDbNull()
        {
            // Setup
            using (var source = CreateSource(2))
            using (var buffer = new BatchBuffer(source, 10))
            {
                // Act
                buffer.Fill();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    reader.Read();
                    Assert.AreEqual("Name 1", reader.GetValue(1));
                    reader.Read();
                    Assert.IsTrue(reader.IsDBNull(1));
                }
            }
        }

        [TestMethod]
        public async Task TestFillAsyncBuffersTheRowsUpToTheBatchSizeThenTheRest()
        {
            // Setup
            using (var source = CreateSource(5))
            using (var buffer = new BatchBuffer(source, 2))
            {
                var counts = new List<int>();

                // Act
                while (await buffer.FillAsync())
                {
                    counts.Add(buffer.Count);
                }

                // Assert
                CollectionAssert.AreEqual(new[] { 2, 2, 1 }, counts);
            }
        }

        [TestMethod]
        public async Task ThrowExceptionOnFillAsyncIfTheTokenIsCancelled()
        {
            // Setup
            using (var source = CreateSource(3))
            using (var buffer = new BatchBuffer(source, 2))
            using (var tokenSource = new CancellationTokenSource())
            {
                tokenSource.Cancel();

                // Act/Assert
                await Assert.ThrowsAsync<OperationCanceledException>(() => buffer.FillAsync(tokenSource.Token));
            }
        }

        #endregion

        #region CreateReader

        [TestMethod]
        public void TestCreateReaderHasTheColumnsOfTheSourceEvenIfNoRowIsBuffered()
        {
            // Setup
            using (var source = CreateSource(1))
            using (var buffer = new BatchBuffer(source, 2))
            using (var reader = buffer.CreateReader())
            {
                // Assert
                Assert.AreEqual(2, reader.FieldCount);
                Assert.AreEqual("Id", reader.GetName(0));
                Assert.AreEqual(typeof(int), reader.GetFieldType(0));
                Assert.AreEqual("Name", reader.GetName(1));
                Assert.IsFalse(reader.Read());
            }
        }

        #endregion

        #region Excluded Columns

        [TestMethod]
        public void TestFillLeavesTheExcludedColumnsOutOfTheBatches()
        {
            // Setup
            using (var source = CreateSource(3))
            using (var buffer = new BatchBuffer(source, 2, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "id" }))
            {
                // Act
                buffer.Fill();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    Assert.AreEqual(1, reader.FieldCount);
                    Assert.AreEqual("Name", reader.GetName(0));
                    reader.Read();
                    Assert.AreEqual("Name 1", reader.GetValue(0));
                    reader.Read();
                    Assert.IsTrue(reader.IsDBNull(0));
                }
            }
        }

        [TestMethod]
        public void TestFillKeepsAllTheColumnsIfNoColumnIsExcluded()
        {
            // Setup
            using (var source = CreateSource(1))
            using (var buffer = new BatchBuffer(source, 2, null))
            using (var reader = buffer.CreateReader())
            {
                // Assert
                Assert.AreEqual(2, reader.FieldCount);
            }
        }

        #endregion

        #region Clear

        [TestMethod]
        public void TestClearRemovesTheBufferedRows()
        {
            // Setup
            using (var source = CreateSource(3))
            using (var buffer = new BatchBuffer(source, 3))
            {
                buffer.Fill();

                // Act
                buffer.Clear();

                // Assert
                Assert.AreEqual(0, buffer.Count);
            }
        }

        #endregion

        #region Interceptors

        private static List<string> ColumnNames(IDataReader reader)
        {
            var names = new List<string>();
            for (var i = 0; i < reader.FieldCount; i++)
            {
                names.Add(reader.GetName(i));
            }
            return names;
        }

        [TestMethod]
        public void TestFillBuffersTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>>
            {
                new FakeDataInterceptor(row => { row["Id"] = (int)row["Id"] * 10; return row; }),
                new FakeDataInterceptor(row => { row["Id"] = (int)row["Id"] + 1; return row; })
            };
            using (var buffer = new BatchBuffer(CreateSource(3), 10, null, interceptors))
            {
                // Act
                buffer.Fill();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    CollectionAssert.AreEqual(new[] { 11, 21, 31 }, Ids(reader));
                }
            }
        }

        [TestMethod]
        public void TestFillBuffersTheColumnsThatTheInterceptorsAddOrRemove()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>>
            {
                new FakeDataInterceptor(row => row.Add("Extra", typeof(string), "e" + row["Id"])),
                new FakeDataInterceptor(row => { row.Remove("Name"); return row; })
            };
            using (var buffer = new BatchBuffer(CreateSource(2), 10, null, interceptors))
            {
                // Act
                buffer.Fill();

                // Assert
                Assert.IsTrue(buffer.HasColumn("extra"));
                Assert.IsFalse(buffer.HasColumn("Name"));
                using (var reader = buffer.CreateReader())
                {
                    CollectionAssert.AreEqual(new[] { "Id", "Extra" }, ColumnNames(reader));
                    reader.Read();
                    Assert.AreEqual("e1", reader.GetString(1));
                }
            }
        }

        [TestMethod]
        public void TestFillKeepsTheNullValuesThatTheInterceptorsGiveAsDbNull()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>> { new FakeDataInterceptor(row => { row["Name"] = null; return row; }) };
            using (var buffer = new BatchBuffer(CreateSource(1), 10, null, interceptors))
            {
                // Act
                buffer.Fill();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    reader.Read();
                    Assert.IsTrue(reader.IsDBNull(1));
                }
            }
        }

        [TestMethod]
        public void TestFillLeavesTheExcludedColumnsOutOfTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>> { new FakeDataInterceptor(row => row.Add("Extra", typeof(int), 5)) };
            using (var buffer = new BatchBuffer(CreateSource(2), 10, new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "id", "extra" }, interceptors))
            {
                // Act
                buffer.Fill();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    CollectionAssert.AreEqual(new[] { "Name" }, ColumnNames(reader));
                }
            }
        }

        [TestMethod]
        public void TestFillCallsTheInterceptorsOncePerRow()
        {
            // Setup
            var interceptor = new FakeDataInterceptor(row => row);
            using (var buffer = new BatchBuffer(CreateSource(5), 2, null, new List<IDataInterceptor<Row>> { interceptor }))
            {
                // Act
                while (buffer.Fill())
                {
                }

                // Assert
                Assert.AreEqual(5, interceptor.Calls);
            }
        }

        [TestMethod]
        public async Task TestFillAsyncBuffersTheRowsThatTheInterceptorsGive()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>> { new FakeDataInterceptor(row => { row["Id"] = (int)row["Id"] + 100; return row; }) };
            using (var buffer = new BatchBuffer(CreateSource(2), 10, null, interceptors))
            {
                // Act
                await buffer.FillAsync();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    CollectionAssert.AreEqual(new[] { 101, 102 }, Ids(reader));
                }
            }
        }

        [TestMethod]
        public void TestCreateReaderOfABufferWithInterceptorsHasTheColumnsOfTheSourceIfNoRowIsBuffered()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>> { new FakeDataInterceptor(row => row.Add("Extra", typeof(int), 1)) };
            using (var buffer = new BatchBuffer(CreateSource(0), 10, null, interceptors))
            {
                // Act
                buffer.Fill();

                // Assert
                using (var reader = buffer.CreateReader())
                {
                    CollectionAssert.AreEqual(new[] { "Id", "Name" }, ColumnNames(reader));
                }
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFillIfAnInterceptorGivesAnotherShapeToAnotherRow()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>> { new FakeDataInterceptor(row => (int)row["Id"] == 2 ? row.Add("Extra", typeof(int), 1) : row) };
            using (var buffer = new BatchBuffer(CreateSource(3), 10, null, interceptors))
            {
                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => buffer.Fill());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFillIfAnInterceptorReturnsNull()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>> { new FakeDataInterceptor(row => null) };
            using (var buffer = new BatchBuffer(CreateSource(1), 10, null, interceptors))
            {
                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => buffer.Fill());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnFillIfAValueDoesNotFitTheTypeOfItsColumn()
        {
            // Setup
            var interceptors = new List<IDataInterceptor<Row>> { new FakeDataInterceptor(row => { row["Id"] = "not a number"; return row; }) };
            using (var buffer = new BatchBuffer(CreateSource(1), 10, null, interceptors))
            {
                // Act/Assert
                Assert.Throws<InvalidOperationException>(() => buffer.Fill());
            }
        }

        [TestMethod]
        public void ThrowExceptionOnBatchBufferIfAnInterceptorIsNull()
        {
            // Act/Assert
            Assert.Throws<ArgumentException>(() => new BatchBuffer(CreateSource(1), 10, null, new List<IDataInterceptor<Row>> { null }));
        }

        #endregion
    }
}
