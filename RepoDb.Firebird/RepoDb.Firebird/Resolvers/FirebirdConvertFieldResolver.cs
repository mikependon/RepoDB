using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Field"/> name conversion for Firebird.
    /// </summary>
    /// <remarks>
    /// Firebird's AVG() returns a value of the same exact-numeric type as its argument (e.g.
    /// AVG(INTEGER) is itself INTEGER), truncating any fractional part instead of widening to a
    /// floating-point type the way MySQL/SQL Server do (see
    /// https://github.com/FirebirdSQL/firebird/issues/6845). <see cref="BaseStatementBuilder"/>'s
    /// CreateAverage/CreateAverageAll already widen exact-numeric field types to <see cref="double"/>
    /// via its AverageableClientTypeResolver before this resolver ever runs (see
    /// ClientTypeToAverageableClientTypeResolver); this resolver's job is only to turn that
    /// (possibly-widened) field type into the actual "CAST(field AS type)" SQL text - the same
    /// approach Oracle/PostgreSql use here.
    /// </remarks>
    public class FirebirdConvertFieldResolver : DbConvertFieldResolver
    {
        /// <summary>
        /// Creates a new instance of <see cref="FirebirdConvertFieldResolver"/> class.
        /// </summary>
        public FirebirdConvertFieldResolver()
            : this(new ClientTypeToDbTypeResolver(),
                 new DbTypeToFirebirdStringNameResolver())
        { }

        /// <summary>
        /// Creates a new instance of <see cref="FirebirdConvertFieldResolver"/> class.
        /// </summary>
        public FirebirdConvertFieldResolver(IResolver<Type, DbType?> dbTypeResolver,
            IResolver<DbType, string> stringNameResolver)
            : base(dbTypeResolver,
                  stringNameResolver)
        { }

        #region Methods

        /// <summary>
        /// Returns the converted name of the <see cref="Field"/> object for Firebird.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> to be converted.</param>
        /// <param name="dbSetting">The current in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The converted name of the <see cref="Field"/> object for Firebird.</returns>
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
