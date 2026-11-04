#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Collections.Generic;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.UnitTests.Others
{
    [TestClass]
    public class DataEntityDataReaderBehaviorTest
    {
        [TestMethod]
        public void TestEntityReaderGetBoolean()
        {
            // Setup
            var entity = new ValueEntity { Value = true };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetBoolean(0);

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetByte()
        {
            // Setup
            var entity = new ValueEntity { Value = (byte)42 };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetByte(0);

            // Assert
            Assert.AreEqual((byte)42, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetChar()
        {
            // Setup
            var entity = new ValueEntity { Value = 'a' };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetChar(0);

            // Assert
            Assert.AreEqual('a', actual);
        }

        [TestMethod]
        public void TestEntityReaderGetDateTime()
        {
            // Setup
            var entity = new ValueEntity { Value = new DateTime(2026, 1, 2) };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetDateTime(0);

            // Assert
            Assert.AreEqual(new DateTime(2026, 1, 2), actual);
        }

        [TestMethod]
        public void TestEntityReaderGetDecimal()
        {
            // Setup
            var entity = new ValueEntity { Value = 42M };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetDecimal(0);

            // Assert
            Assert.AreEqual(42M, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetDouble()
        {
            // Setup
            var entity = new ValueEntity { Value = 42D };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetDouble(0);

            // Assert
            Assert.AreEqual(42D, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetFloat()
        {
            // Setup
            var entity = new ValueEntity { Value = 42F };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetFloat(0);

            // Assert
            Assert.AreEqual(42F, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetGuid()
        {
            // Setup
            var entity = new ValueEntity { Value = new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff") };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetGuid(0);

            // Assert
            Assert.AreEqual(new Guid("6f9619ff-8b86-d011-b42d-00c04fc964ff"), actual);
        }

        [TestMethod]
        public void TestEntityReaderGetInt16()
        {
            // Setup
            var entity = new ValueEntity { Value = (short)42 };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetInt16(0);

            // Assert
            Assert.AreEqual((short)42, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetInt32()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetInt32(0);

            // Assert
            Assert.AreEqual(42, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetInt64()
        {
            // Setup
            var entity = new ValueEntity { Value = 42L };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetInt64(0);

            // Assert
            Assert.AreEqual(42L, actual);
        }

        [TestMethod]
        public void TestEntityReaderGetString()
        {
            // Setup
            var entity = new ValueEntity { Value = "value" };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();

            // Act
            var actual = reader.GetString(0);

            // Assert
            Assert.AreEqual("value", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEntityReaderRejectsGetBytes()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());

            // Act
            Action act = () => reader.GetBytes(0, 0, new byte[1], 0, 1);

            // Assert
            Assert.ThrowsExactly<NotSupportedException>(act);
        }

        [TestMethod]
        public void TestEntityReaderRejectsGetChars()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());

            // Act
            Action act = () => reader.GetChars(0, 0, new char[1], 0, 1);

            // Assert
            Assert.ThrowsExactly<NotSupportedException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetsDataTypeName()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());

            // Act
            var actual = reader.GetDataTypeName(0);

            // Assert
            Assert.AreEqual("Object", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEntityReaderRejectsGetSchemaTable()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());

            // Act
            Action act = () => reader.GetSchemaTable();

            // Assert
            Assert.ThrowsExactly<NotSupportedException>(act);
        }

        [TestMethod]
        public void TestEntityReaderRejectsNextResult()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());

            // Act
            Action act = () => reader.NextResult();

            // Assert
            Assert.ThrowsExactly<NotSupportedException>(act);
        }

        [TestMethod]
        public void TestEntityReaderRejectsNullEntities()
        {
            // Setup
            IEnumerable<ValueEntity> entities = null;

            // Act
            Action act = () => new DataEntityDataReader<ValueEntity>(entities);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(act);
        }

        [TestMethod]
        public void TestEntityReaderReadRejectsClosed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Close();

            // Act
            Action act = () => reader.Read();

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderResetRejectsClosed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Close();

            // Act
            Action act = () => reader.Reset();

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetValueRejectsClosed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Close();

            // Act
            Action act = () => reader.GetValue(0);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetNameRejectsClosed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Close();

            // Act
            Action act = () => reader.GetName(0);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetFieldTypeRejectsClosed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Close();

            // Act
            Action act = () => reader.GetFieldType(0);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetOrdinalRejectsClosed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Close();

            // Act
            Action act = () => reader.GetOrdinal("Value");

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderReadRejectsDisposed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Dispose();

            // Act
            Action act = () => reader.Read();

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderResetRejectsDisposed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Dispose();

            // Act
            Action act = () => reader.Reset();

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetValueRejectsDisposed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Dispose();

            // Act
            Action act = () => reader.GetValue(0);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetNameRejectsDisposed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Dispose();

            // Act
            Action act = () => reader.GetName(0);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetFieldTypeRejectsDisposed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Dispose();

            // Act
            Action act = () => reader.GetFieldType(0);

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetOrdinalRejectsDisposed()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(Array.Empty<ValueEntity>());
            reader.Dispose();

            // Act
            Action act = () => reader.GetOrdinal("Value");

            // Assert
            Assert.ThrowsExactly<InvalidOperationException>(act);
        }

        [TestMethod]
        public void TestEntityReaderOrderingColumnName()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetName(1);

            // Assert
            Assert.AreEqual("__RepoDb_OrderColumn", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestEntityReaderOrderingColumnType()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetFieldType(1);

            // Assert
            Assert.AreEqual(typeof(int), actual);
        }

        [TestMethod]
        public void TestEntityReaderOrderingColumnOrdinal()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetOrdinal("__repodb_ordercolumn");

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void TestEntityReaderOrderingColumnValue()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>("Table", new[] { entity }, null, null, true);
            reader.Read();

            // Act
            var actual = reader.GetValue(1);

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestEntityReaderFindsColumnIgnoringCase()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetOrdinal("value");

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestEntityReaderUnknownColumnOrdinal()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetOrdinal("Unknown");

            // Assert
            Assert.AreEqual(-1, actual);
        }

        [TestMethod]
        public void TestEntityReaderReadsValue()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>("Table", new[] { entity }, null, null, true);
            reader.Read();

            // Act
            var actual = reader["Value"];

            // Assert
            Assert.AreEqual(42, actual);
        }

        [TestMethod]
        public void TestDictionaryReaderOrderingColumnName()
        {
            // Setup
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 };
            using var reader = new DataEntityDataReader<Dictionary<string, object>>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetName(1);

            // Assert
            Assert.AreEqual("__RepoDb_OrderColumn", actual, StringComparer.Ordinal);
        }

        [TestMethod]
        public void TestDictionaryReaderOrderingColumnType()
        {
            // Setup
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 };
            using var reader = new DataEntityDataReader<Dictionary<string, object>>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetFieldType(1);

            // Assert
            Assert.AreEqual(typeof(int), actual);
        }

        [TestMethod]
        public void TestDictionaryReaderOrderingColumnOrdinal()
        {
            // Setup
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 };
            using var reader = new DataEntityDataReader<Dictionary<string, object>>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetOrdinal("__repodb_ordercolumn");

            // Assert
            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void TestDictionaryReaderOrderingColumnValue()
        {
            // Setup
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 };
            using var reader = new DataEntityDataReader<Dictionary<string, object>>("Table", new[] { entity }, null, null, true);
            reader.Read();

            // Act
            var actual = reader.GetValue(1);

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestDictionaryReaderFindsColumnIgnoringCase()
        {
            // Setup
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 };
            using var reader = new DataEntityDataReader<Dictionary<string, object>>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetOrdinal("value");

            // Assert
            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void TestDictionaryReaderUnknownColumnOrdinal()
        {
            // Setup
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 };
            using var reader = new DataEntityDataReader<Dictionary<string, object>>("Table", new[] { entity }, null, null, true);

            // Act
            var actual = reader.GetOrdinal("Unknown");

            // Assert
            Assert.AreEqual(-1, actual);
        }

        [TestMethod]
        public void TestDictionaryReaderReadsValue()
        {
            // Setup
            var entity = new Dictionary<string, object>(StringComparer.Ordinal) { ["Value"] = 42 };
            using var reader = new DataEntityDataReader<Dictionary<string, object>>("Table", new[] { entity }, null, null, true);
            reader.Read();

            // Act
            var actual = reader["Value"];

            // Assert
            Assert.AreEqual(42, actual);
        }

        [TestMethod]
        public void TestEntityReaderCopiesValues()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });
            reader.Read();
            var values = new object[1];

            // Act
            var actual = reader.GetValues(values);

            // Assert
            Assert.AreEqual(1, actual);
            Assert.AreEqual(42, values[0]);
        }

        [TestMethod]
        public void TestEntityReaderGetValuesRejectsNull()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { new ValueEntity() });
            reader.Read();

            // Act
            Action act = () => reader.GetValues(null);

            // Assert
            Assert.ThrowsExactly<ArgumentNullException>(act);
        }

        [TestMethod]
        public void TestEntityReaderGetValuesRejectsWrongSize()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { new ValueEntity() });
            reader.Read();

            // Act
            Action act = () => reader.GetValues(new object[2]);

            // Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(act);
        }

        [TestMethod]
        public void TestEntityReaderIsDBNullDatabaseNull()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { new ValueEntity { Value = DBNull.Value } });
            reader.Read();

            // Act
            var actual = reader.IsDBNull(0);

            // Assert
            Assert.AreEqual(true, actual);
        }

        [TestMethod]
        public void TestEntityReaderIsDBNullValue()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { new ValueEntity { Value = 42 } });
            reader.Read();

            // Act
            var actual = reader.IsDBNull(0);

            // Assert
            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestEntityReaderResetRewinds()
        {
            // Setup
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { new ValueEntity { Value = 42 } });
            reader.Read();

            // Act
            reader.Reset();
            var actual = reader.Read();

            // Assert
            Assert.IsTrue(actual);
            Assert.AreEqual(0, reader.Position);
        }

        [TestMethod]
        public void TestEntityReaderEnumeratesOriginalEntities()
        {
            // Setup
            var entity = new ValueEntity { Value = 42 };
            using var reader = new DataEntityDataReader<ValueEntity>(new[] { entity });

            // Act
            var enumerator = reader.GetEnumerator();
            enumerator.MoveNext();

            // Assert
            Assert.AreSame(entity, enumerator.Current);
        }

        public class ValueEntity
        {
            public object Value { get; set; }
        }
    }
}
