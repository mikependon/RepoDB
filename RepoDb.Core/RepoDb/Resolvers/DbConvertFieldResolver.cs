using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the <see cref="Field"/> name conversion.
    /// </summary>
    public class DbConvertFieldResolver : IResolver<Field, IDbSetting, string>
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbConvertFieldResolver"/> class.
        /// </summary>
        public DbConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
        {
            DbTypeResolver = dbTypeResolver;
            StringNameResolver = stringNameResolver;
        }

        #region Properties

        /// <summary>
        /// Gets the resolver that is being used to resolve the .NET CLR type and <see cref="DbType"/>.
        /// </summary>
        public IResolver<Type, DbType?> DbTypeResolver { get; }

        /// <summary>
        /// Gets the resolver that is being used to resolve the <see cref="DbType"/> and the database type string name.
        /// </summary>
        public IResolver<DbType, string> StringNameResolver { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the converted name of the <see cref="Field"/> object.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> to be converted.</param>
        /// <param name="dbSetting">The current in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The converted name of the <see cref="Field"/> object.</returns>
        public virtual string Resolve(Field field,
            IDbSetting dbSetting)
        {
            if (field != null && field.Type != null)
            {
                var dbType = DbTypeResolver.Resolve(field.Type);
                if (dbType != null)
                {
                    var dbTypeName = StringNameResolver.Resolve(dbType.Value).ToUpper();
                    return string.Concat("CAST(", field.Name.AsField(dbSetting), " AS ", dbTypeName.AsQuoted(dbSetting), ")");
                }
            }
            return field?.Name?.AsQuoted(true, true, dbSetting);
        }

        #endregion
    }
}
