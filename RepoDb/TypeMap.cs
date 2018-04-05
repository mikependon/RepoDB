using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb
{
    public class TypeMap : ITypeMap
    {
        public TypeMap(Type type, DbType dbType)
        {
            Type = type;
            DbType = dbType;
        }

        public Type Type { get; set; }

        public DbType DbType { get; set; }
    }
}
