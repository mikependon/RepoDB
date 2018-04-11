using System;
using RepoDb.Interfaces;
using System.Data;
using System.Data.Common;

namespace RepoDb
{
    public class StatementBuilderMap : IStatementBuilderMap
    {
        public StatementBuilderMap(Type dbConnectionType, IStatementBuilder statementBuilder)
        {
            if (!dbConnectionType.IsSubclassOf(typeof(IDbConnection)) && !dbConnectionType.IsSubclassOf(typeof(DbConnection)))
            {
                throw new ArgumentException($"Argument 'dbConnectionType' must be a sub class of '{typeof(DbConnection).FullName}'.");
            }
            DbConnectionType = dbConnectionType;
            StatementBuilder = statementBuilder;
        }

        public Type DbConnectionType { get; }

        public IStatementBuilder StatementBuilder { get; set; }
    }
}
