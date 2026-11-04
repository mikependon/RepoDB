#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RepoDb.Interfaces;
using RepoDb.Reflection;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class DataReaderMaterializationTest
    {
        [TestMethod]
        public void TestTypedNullReader()
        {
            // Setup
            DbDataReader reader = null;

            // Act
            var actual = DataReader.ToEnumerable<Row>(reader).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestTypedClosedReader()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            reader.Close();

            // Act
            var actual = DataReader.ToEnumerable<Row>(reader).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestTypedEmptyReader()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<Row>(reader).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestTypedReaderMaterializesEveryRow()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<Row>(reader).ToList();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => item.Value));
        }

        [TestMethod]
        public void TestDynamicNullReader()
        {
            // Setup
            DbDataReader reader = null;

            // Act
            var actual = DataReader.ToEnumerable(reader).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestDynamicClosedReader()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            reader.Close();

            // Act
            var actual = DataReader.ToEnumerable(reader).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestDynamicEmptyReader()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable(reader).ToList();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public void TestDynamicReaderMaterializesEveryRow()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable(reader).ToList();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => (int)((IDictionary<string, object>)item)["Value"]));
        }

        [TestMethod]
        public void TestScalarReaderMaterializesEveryRow()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();

            // Act
            var actual = DataReader.ToEnumerable<int>(reader).ToList();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual);
        }

        [TestMethod]
        public async Task TestTypedNullReaderAsync()
        {
            // Setup
            DbDataReader reader = null;

            // Act
            var actual = await DataReader.ToEnumerableAsync<Row>(reader).ToListAsync();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestTypedClosedReaderAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            reader.Close();

            // Act
            var actual = await DataReader.ToEnumerableAsync<Row>(reader).ToListAsync();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestTypedEmptyReaderAsync()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();

            // Act
            var actual = await DataReader.ToEnumerableAsync<Row>(reader).ToListAsync();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestTypedReaderMaterializesEveryRowAsync()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();

            // Act
            var actual = await DataReader.ToEnumerableAsync<Row>(reader).ToListAsync();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => item.Value));
        }

        [TestMethod]
        public async Task TestDynamicNullReaderAsync()
        {
            // Setup
            DbDataReader reader = null;

            // Act
            var actual = await DataReader.ToEnumerableAsync(reader).ToListAsync();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestDynamicClosedReaderAsync()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            reader.Close();

            // Act
            var actual = await DataReader.ToEnumerableAsync(reader).ToListAsync();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestDynamicEmptyReaderAsync()
        {
            // Setup
            using var table = CreateTable();
            using var reader = table.CreateDataReader();

            // Act
            var actual = await DataReader.ToEnumerableAsync(reader).ToListAsync();

            // Assert
            Assert.IsEmpty(actual);
        }

        [TestMethod]
        public async Task TestDynamicReaderMaterializesEveryRowAsync()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();

            // Act
            var actual = await DataReader.ToEnumerableAsync(reader).ToListAsync();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual.Select(item => (int)((IDictionary<string, object>)item)["Value"]));
        }

        [TestMethod]
        public async Task TestScalarReaderMaterializesEveryRowAsync()
        {
            // Setup
            using var table = CreateTable(42, 43);
            using var reader = table.CreateDataReader();

            // Act
            var actual = await DataReader.ToEnumerableAsync<int>(reader).ToListAsync();

            // Assert
            Assert.AreSequenceEqual(new[] { 42, 43 }, actual);
        }

        [TestMethod]
        public void TestTypedFunctionCacheReusesMatchingSchema()
        {
            // Setup
            using var table = CreateTable(42);
            using var first = table.CreateDataReader();
            using var second = table.CreateDataReader();
            var expected = FunctionCache.GetDataReaderToTypeCompiledFunction<Row>(first);

            // Act
            var actual = FunctionCache.GetDataReaderToTypeCompiledFunction<Row>(second);

            // Assert
            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void TestTypedFunctionCacheSeparatesDifferentSchema()
        {
            // Setup
            using var firstTable = CreateTable(42);
            using var secondTable = CreateTable(43);
            secondTable.Columns.Add("Other", typeof(string));
            using var first = firstTable.CreateDataReader();
            using var second = secondTable.CreateDataReader();
            var expected = FunctionCache.GetDataReaderToTypeCompiledFunction<Row>(first);

            // Act
            var actual = FunctionCache.GetDataReaderToTypeCompiledFunction<Row>(second);

            // Assert
            Assert.AreNotSame(expected, actual);
        }

        [TestMethod]
        public void TestDynamicFunctionCacheReusesMatchingSchema()
        {
            // Setup
            using var table = CreateTable(42);
            using var first = table.CreateDataReader();
            using var second = table.CreateDataReader();
            var expected = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(first);

            // Act
            var actual = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(second);

            // Assert
            Assert.AreSame(expected, actual);
        }

        [TestMethod]
        public void TestDynamicFunctionCacheSeparatesDifferentSchema()
        {
            // Setup
            using var firstTable = CreateTable(42);
            using var secondTable = CreateTable(43);
            secondTable.Columns.Add("Other", typeof(string));
            using var first = firstTable.CreateDataReader();
            using var second = secondTable.CreateDataReader();
            var expected = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(first);

            // Act
            var actual = FunctionCache.GetDataReaderToExpandoObjectCompileFunction(second);

            // Assert
            Assert.AreNotSame(expected, actual);
        }

        [TestMethod]
        public void TestFactoryMaterializesEntity()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            reader.Read();
            var factory = FunctionFactory.CompileDataReaderToType<Row>(reader, null, null);

            // Act
            var actual = factory(reader);

            // Assert
            Assert.AreEqual(42, actual.Value);
        }

        [TestMethod]
        public void TestFactoryMaterializesDynamic()
        {
            // Setup
            using var table = CreateTable(42);
            using var reader = table.CreateDataReader();
            reader.Read();
            var factory = FunctionFactory.CompileDataReaderToExpandoObject(reader, null, null);

            // Act
            var actual = (IDictionary<string, object>)factory(reader);

            // Assert
            Assert.AreEqual(42, actual["Value"]);
        }

        [TestMethod]
        public void TestFactoryRejectsMultipleScalarColumns()
        {
            // Setup
            using var table = CreateTable(42);
            table.Columns.Add("Other", typeof(int));
            using var reader = table.CreateDataReader();

            // Act
            Action act = () => FunctionFactory.CompileDataReaderToType<int>(reader, null, null);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestFactoryRejectsUnmatchedEntityColumns()
        {
            // Setup
            using var table = CreateTable(42);
            table.Columns[0].ColumnName = "Unmatched";
            using var reader = table.CreateDataReader();

            // Act
            Action act = () => FunctionFactory.CompileDataReaderToType<Row>(reader, null, null);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestFactorySetsEntityProperty()
        {
            // Setup
            var entity = new Row();
            var setter = FunctionFactory.CompileDataEntityPropertySetter(typeof(Row), new Field("Value"));

            // Act
            setter(entity, 42);

            // Assert
            Assert.AreEqual(42, entity.Value);
        }

        private static DataTable CreateTable(params int[] values)
        {
            var table = new DataTable();
            table.Columns.Add("Value", typeof(int));
            foreach (var value in values)
            {
                table.Rows.Add(value);
            }
            return table;
        }

        public class Row
        {
            public int Value { get; set; }
        }
    }
}
