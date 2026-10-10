#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using RepoDb.Interfaces;

namespace RepoDb.Data.Core.UnitTests.CustomObjects
{
    public class FakeDbHelper : IDbHelper
    {
        private static readonly DbField[] Fields =
        {
            new DbField("Id", true, true, false, typeof(int), null, null, null, null),
            new DbField("Name", false, false, true, typeof(string), null, null, null, null),
            new DbField("Birthday", false, false, true, typeof(DateTime), null, null, null, null)
        };

        public IResolver<string, Type> DbTypeResolver { get; set; }

        public IEnumerable<DbField> GetFields(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null) =>
            Fields;

        public Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IEnumerable<DbField>>(Fields);

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
