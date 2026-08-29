using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Field"/> name conversion for Vertica.
    /// </summary>
    public class VerticaConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="VerticaConvertFieldResolver"/> class.
        /// </summary>
        public VerticaConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToVerticaStringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="VerticaConvertFieldResolver"/> class.
        /// </summary>
        public VerticaConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }

        #region Methods

        /// <summary>
        /// Returns the converted name of the <see cref="Field"/> object for Vertica.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> to be converted.</param>
        /// <param name="dbSetting">The current in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The converted name of the <see cref="Field"/> object for Vertica.</returns>
        public override string Resolve(Field field,
            IDbSetting dbSetting)
        {
            if (field?.Type != null)
            {
                var dbType = DbTypeResolver.Resolve(field.Type);
                if (dbType != null)
                {
                    var dbTypeName = StringNameResolver.Resolve(dbType.Value).ToUpperInvariant();
                    return string.Concat("CAST(", field.Name.AsField(dbSetting), " AS ", dbTypeName, ")");
                }
            }
            return field?.Name?.AsField(dbSetting);
        }

        #endregion
    }
}
