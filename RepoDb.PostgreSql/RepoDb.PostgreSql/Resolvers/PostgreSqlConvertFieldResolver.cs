using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Field"/> name conversion for PostgreSql.
    /// </summary>
    public class PostgreSqlConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlConvertFieldResolver"/> class.
        /// </summary>
        public PostgreSqlConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToPostgreSqlStringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="PostgreSqlConvertFieldResolver"/> class.
        /// </summary>
        public PostgreSqlConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }

        #region Methods

        /// <summary>
        /// Returns the converted name of the <see cref="Field"/> object for PostgreSql.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> to be converted.</param>
        /// <param name="dbSetting">The current in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The converted name of the <see cref="Field"/> object for PostgreSql.</returns>
        public override string Resolve(Field field,
            IDbSetting dbSetting)
        {
            if (field?.Type != null)
            {
                var dbType = DbTypeResolver.Resolve(field.Type);
                if (dbType != null)
                {
                    var dbTypeName = StringNameResolver.Resolve(dbType.Value).ToUpper();
                    return string.Concat("CAST(", field.Name.AsField(dbSetting), " AS ", dbTypeName, ")");
                }
            }
            return field?.Name?.AsQuoted(true, true, dbSetting);
        }

        #endregion
    }
}
