#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections;
using System.Data.Common;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.DbHelpers;
using RepoDb.Connector.EnterpriseDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.EnterpriseDb.UnitTests
{
    [TestClass]
    public class DbHelperProviderTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseEnterpriseDb();
        }

        private static MethodInfo GetMethod(string name) =>
            typeof(EnterpriseDbDbHelper).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);

        [TestMethod]
        public void TestEnterpriseDbDbHelperReaderToDbFieldGetsTheProviderOfTheDbSetting()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();
            var helper = DbHelperMapper.Get<EDBConnection>();

            // Act
            var actual = (DbField)GetMethod("ReaderToDbField").Invoke(helper, new object[] { new FakeDbDataReader() });

            // Assert
            Assert.IsNotNull(setting.Provider);
            Assert.AreEqual(setting.Provider, actual.Provider);
        }

        [TestMethod]
        public async Task TestEnterpriseDbDbHelperReaderToDbFieldAsyncGetsTheProviderOfTheDbSetting()
        {
            // Setup
            var setting = DbSettingMapper.Get<EDBConnection>();
            var helper = DbHelperMapper.Get<EDBConnection>();

            // Act
            var actual = await (Task<DbField>)GetMethod("ReaderToDbFieldAsync").Invoke(helper, new object[] { new FakeDbDataReader(), CancellationToken.None });

            // Assert
            Assert.IsNotNull(setting.Provider);
            Assert.AreEqual(setting.Provider, actual.Provider);
        }

        /// <summary>
        /// A reader whose values are valid for any column: the text is <c>text</c>, the raw values are <c>0</c>, nothing is <c>DBNull</c> and the other values are the default ones.
        /// </summary>
        private sealed class FakeDbDataReader : DbDataReader
        {
            public override int Depth => 0;

            public override int FieldCount => 20;

            public override bool HasRows => true;

            public override bool IsClosed => false;

            public override int RecordsAffected => 0;

            public override object this[int ordinal] => GetValue(ordinal);

            public override object this[string name] => GetValue(0);

            public override bool GetBoolean(int ordinal) => false;

            public override byte GetByte(int ordinal) => 0;

            public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length) => 0;

            public override char GetChar(int ordinal) => 'a';

            public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length) => 0;

            public override string GetDataTypeName(int ordinal) => "text";

            public override DateTime GetDateTime(int ordinal) => DateTime.MinValue;

            public override decimal GetDecimal(int ordinal) => 0;

            public override double GetDouble(int ordinal) => 0;

            public override IEnumerator GetEnumerator() => new object[0].GetEnumerator();

            public override Type GetFieldType(int ordinal) => typeof(string);

            public override T GetFieldValue<T>(int ordinal) => typeof(T) == typeof(string) ? (T)(object)"text" : typeof(T) == typeof(object) ? (T)(object)0 : default;

            public override float GetFloat(int ordinal) => 0;

            public override Guid GetGuid(int ordinal) => Guid.Empty;

            public override short GetInt16(int ordinal) => 0;

            public override int GetInt32(int ordinal) => 0;

            public override long GetInt64(int ordinal) => 0;

            public override string GetName(int ordinal) => "text";

            public override int GetOrdinal(string name) => 0;

            public override string GetString(int ordinal) => "text";

            public override object GetValue(int ordinal) => 0;

            public override int GetValues(object[] values) => 0;

            public override bool IsDBNull(int ordinal) => false;

            public override bool NextResult() => false;

            public override bool Read() => false;
        }
    }
}
