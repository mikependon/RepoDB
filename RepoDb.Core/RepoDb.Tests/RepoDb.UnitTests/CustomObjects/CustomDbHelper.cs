using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RepoDb.UnitTests.CustomObjects
{
    public class CustomDbHelper : IDbHelper
    {
        public IResolver<string, Type> DbTypeResolver { get; set; }

        public IEnumerable<DbField> GetFields<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            return new[]
            {
                new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                new DbField("Name", false, false, true, typeof(string), null, null, null, null)
            };
        }

        public Task<IEnumerable<DbField>> GetFieldsAsync<TDbConnection>(TDbConnection connection,
            string tableName,
            IDbTransaction transaction = null)
            where TDbConnection : IDbConnection
        {
            return Task.FromResult<IEnumerable<DbField>>(new[]
            {
                new DbField("Id", true, true, false, typeof(int), null, null, null, null),
                new DbField("Name", false, false, true, typeof(string), null, null, null, null)
            });
        }
    }

}
