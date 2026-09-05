using System;
using System.Data;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// Query field that uses exactl what is passed.
    /// </summary>
    /// <example>
    /// See sample code below that uses a BETWEEN function.
    /// <code>
    ///     var where = new LiteralQueryField("ColumnName BETWEEN 1 AND 10");
    ///     var result = connection.Query&lt;Entity&gt;(where);
    /// </code>
    /// </example>
	public class LiteralQueryField : QueryField
    {
        #region Properties

        public string Literal { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of <see cref="FunctionalQueryField"/> object.
        /// </summary>
        /// <param name="literal">The content of the where as is.</param>
        public LiteralQueryField(string literal)
            // Both of the parameters are actually ignored.
            : base("fake", Operation.Equal, null, null)
        {
            Literal = literal;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the string representations (column-value pairs) of the current <see cref="QueryField"/> object with the formatted-function transformations.
        /// </summary>
        /// <param name="index">The target index.</param>
        /// <param name="dbSetting">The database setting currently in used.</param>
        /// <returns>The string representations of the current <see cref="QueryField"/> object using the LOWER function.</returns>
        public override string GetString(int index,
            IDbSetting dbSetting) =>
            Literal;

        #endregion
    }
}

