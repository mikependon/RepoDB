using RepoDb.Interfaces;
using System.Data.SqlClient;

namespace RepoDb.UnitTests.Setup
{
    /// <summary>
    /// A helper class for the Unit Tests.
    /// </summary>
    public static class Helper
    {
        static Helper()
        {
            StatementBuilder = StatementBuilderMapper.Get<SqlConnection>();
            DbSetting = new CustomDbSetting();
            DbValidator = new CustomDbValidator();
        }

        /// <summary>
        /// Gets the instance of <see cref="IStatementBuilder"/> object.
        /// </summary>
        public static IStatementBuilder StatementBuilder { get; }

        /// <summary>
        /// Gets the instance of <see cref="IDbSetting"/> object.
        /// </summary>
        public static IDbSetting DbSetting { get; }

        /// <summary>
        /// Gets the instance of <see cref="IDbValidator"/> object.
        /// </summary>
        public static IDbValidator DbValidator { get; }
    }
}
