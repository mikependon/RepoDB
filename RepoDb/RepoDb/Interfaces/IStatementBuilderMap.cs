using System;

namespace RepoDb.Interfaces
{
    public interface IStatementBuilderMap
    {
        Type DbConnectionType { get; }
        IStatementBuilder StatementBuilder { get; set; }
    }
}