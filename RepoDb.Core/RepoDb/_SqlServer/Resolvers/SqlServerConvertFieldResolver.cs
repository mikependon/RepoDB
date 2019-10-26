using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Data;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the <see cref="Field"/> name conversion for SQL Server.
    /// </summary>
    public class SqlServerConvertFieldResolver : IResolver<Field, IDbSetting, string>
    {
        #region Properties

        /// <summary>
        /// Gets the resolver that is being used to resolve the .NET CLR Type and <see cref="DbType"/>.
        /// </summary>
        private static ClientTypeToDbTypeResolver DbTypeResolver => new ClientTypeToDbTypeResolver();

        /// <summary>
        /// Gets the resolver that is being used to resolve the <see cref="DbType"/> and the database type string name.
        /// </summary>
        private static DbTypeToSqlServerStringNameResolver StringNameResolver => new DbTypeToSqlServerStringNameResolver();

        #endregion

        #region Methods

        /// <summary>
        /// Returns the converted name of the <see cref="Field"/> object for SQL Server.
        /// </summary>
        /// <param name="field">The instance of the <see cref="Field"/> to be converted..</param>
        /// <param name="dbSetting">The current in used <see cref="IDbSetting"/> object.</param>
        /// <returns>The converted name of the <see cref="Field"/> object for SQL Server.</returns>
        public string Resolve(Field field,
            IDbSetting dbSetting)
        {
            if (field != null && field.Type != null)
            {
                var dbType = DbTypeResolver.Resolve(field.Type);
                if (dbType != null)
                {
                    var dbTypeName = StringNameResolver.Resolve(dbType.Value).ToUpper().AsQuoted(dbSetting);
                    return string.Concat("CONVERT(", dbTypeName, ", ", field.Name.AsField(dbSetting), ")");
                }
            }
            return field?.Name?.AsField(dbSetting);
        }

        #endregion
    }
}
