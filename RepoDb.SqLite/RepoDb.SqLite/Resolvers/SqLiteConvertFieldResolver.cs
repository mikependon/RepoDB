using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Field"/> name conversion for SqLite.
    /// </summary>
    public class SqLiteConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="SqLiteConvertFieldResolver"/> class.
        /// </summary>
        public SqLiteConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToSqLiteStringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="SqLiteConvertFieldResolver"/> class.
        /// </summary>
        public SqLiteConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }
    }
}
