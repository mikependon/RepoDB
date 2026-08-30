#region Copyright Attributions

// Copyright (c) 2019 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbHelper : IDbHelper
    {
        public IResolver<string, Type> DbTypeResolver { get; set; }

        public IEnumerable<DbField> GetFields(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
        {
            return new[]
            {
                new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                new DbField("Name", false, false, true, typeof(string), null, null, null, null)
            };
        }

        public Task<IEnumerable<DbField>> GetFieldsAsync(IDbConnection connection,
            string tableName,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<DbField>>(new[]
            {
                new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                new DbField("Name", false, false, true, typeof(string), null, null, null, null)
            });
        }

        public T GetScopeIdentity<T>(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            return default;
        }

        public Task<T> GetScopeIdentityAsync<T>(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult<T>(default);
        }
        public void DynamicHandler<TEventInstance>(TEventInstance instance,
            string key)
        {
            // Do nothing
        }
    }

}
