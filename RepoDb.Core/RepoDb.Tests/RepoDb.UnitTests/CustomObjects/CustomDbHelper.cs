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

        public object GetScopeIdentity(IDbConnection connection,
            IDbTransaction transaction = null)
        {
            return 0;
        }

        public Task<object> GetScopeIdentityAsync(IDbConnection connection,
            IDbTransaction transaction = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult((object)0);
        }
    }

}
