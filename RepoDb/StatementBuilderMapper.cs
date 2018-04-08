using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace RepoDb
{
    public static class StatementBuilderMapper
    {
        private static readonly object _syncLock;
        private static readonly IList<IStatementBuilderMap> _maps;

        static StatementBuilderMapper()
        {
            // Properties
            _syncLock = new object();
            _maps = new List<IStatementBuilderMap>();

            // Default for SqlDbConnection
            Map(typeof(SqlConnection), new SqlDbStatementBuilder());
        }

        public static IStatementBuilderMap Get(Type type)
        {
            return _maps.FirstOrDefault(m => m.DbConnectionType == type);
        }

        public static void Map(Type dbConnectionType, IStatementBuilder statementBuilder)
        {
            lock (_syncLock)
            {
                var map = Get(dbConnectionType);
                if (map != null)
                {
                    map.StatementBuilder = statementBuilder;
                }
                else
                {
                    _maps.Add(new StatementBuilderMap(dbConnectionType, statementBuilder));
                }
            }
        }
    }
}
