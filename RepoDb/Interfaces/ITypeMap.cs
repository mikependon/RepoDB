using System;
using System.Data;

namespace RepoDb.Interfaces
{
    public interface ITypeMap
    {
        Type Type { get; }

        DbType DbType { get; }
    }
}
