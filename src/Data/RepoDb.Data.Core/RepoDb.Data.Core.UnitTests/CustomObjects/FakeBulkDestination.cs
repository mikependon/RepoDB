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
using RepoDb.Data.Core.UnitTests.Fake.BulkOperations;
using RepoDb.Interfaces;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    /// <summary>
    /// Creates the destination connections of the copy of the data: connections that have a fake bulk insert and bulk merge, and whose setting, helper and statement builder are mapped.
    /// </summary>
    public static class FakeBulkDestination
    {
        static FakeBulkDestination()
        {
            Map<FakeBulkKeyedDbConnection>(new FakeDbHelper());
            Map<FakeBulkIdentityOnlyDbConnection>(new FakeFieldsDbHelper(
                new DbField("Id", false, true, false, typeof(int), null, null, null, null),
                new DbField("Name", false, false, true, typeof(string), null, null, null, null),
                new DbField("Birthday", false, false, true, typeof(DateTime), null, null, null, null)));
            Map<FakeBulkKeylessDbConnection>(new FakeFieldsDbHelper(
                new DbField("Id", false, false, false, typeof(int), null, null, null, null),
                new DbField("Name", false, false, true, typeof(string), null, null, null, null),
                new DbField("Birthday", false, false, true, typeof(DateTime), null, null, null, null)));
        }

        /// <summary>
        /// Creates a destination connection whose table has a primary key (that is the identity too).
        /// </summary>
        public static FakeBulkKeyedDbConnection Create() =>
            new FakeBulkKeyedDbConnection();

        /// <summary>
        /// Creates a destination connection whose table has an identity column, and no primary key.
        /// </summary>
        public static FakeBulkIdentityOnlyDbConnection CreateIdentityOnly() =>
            new FakeBulkIdentityOnlyDbConnection();

        /// <summary>
        /// Creates a destination connection whose table has neither a primary key nor an identity column.
        /// </summary>
        public static FakeBulkKeylessDbConnection CreateKeyless() =>
            new FakeBulkKeylessDbConnection();

        private static void Map<TConnection>(IDbHelper helper)
            where TConnection : IDbConnection
        {
            DbSettingMapper.Add<TConnection>(new FakeDbSetting(), true);
            DbHelperMapper.Add<TConnection>(helper, true);
            StatementBuilderMapper.Add<TConnection>(new FakeStatementBuilder(), true);
        }
    }

    /// <summary>
    /// A helper that gets the specified fields for every table.
    /// </summary>
    public class FakeFieldsDbHelper : IDbHelper
    {
        private readonly DbField[] _fields;

        public FakeFieldsDbHelper(params DbField[] fields)
        {
            _fields = fields;
        }

        public IResolver<string, Type> DbTypeResolver { get; set; }

        public IEnumerable<DbField> GetFields(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null) =>
            _fields.ToList();

        public Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<DbField>>(_fields.ToList());

        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null) =>
            default;

        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<T>(default);

        public void DynamicHandler<TEventInstance>(TEventInstance instance,
            string key)
        {
        }
    }
}
