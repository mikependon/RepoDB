using System.Data;
using System.Data.Common;

namespace RepoDb.Schema.UnitTestHelpers
{
    /// <summary>
    /// Provides an in-memory catalog test fixture.
    /// </summary>
    public sealed class CatalogParameter : DbParameter
    {
        #region Properties

        /// <summary>
        /// Gets or sets the DbType value.
        /// </summary>
        public override DbType DbType { get; set; }

        /// <summary>
        /// Gets or sets the Direction value.
        /// </summary>
        public override ParameterDirection Direction { get; set; }

        /// <summary>
        /// Gets or sets the IsNullable value.
        /// </summary>
        public override bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets the ParameterName value.
        /// </summary>
        public override string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the SourceColumn value.
        /// </summary>
        public override string SourceColumn { get; set; }

        /// <summary>
        /// Gets or sets the Value value.
        /// </summary>
        public override object Value { get; set; }

        /// <summary>
        /// Gets or sets the SourceColumnNullMapping value.
        /// </summary>
        public override bool SourceColumnNullMapping { get; set; }

        /// <summary>
        /// Gets or sets the Size value.
        /// </summary>
        public override int Size { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the parameter type.
        /// </summary>
        public override void ResetDbType() => DbType = DbType.String;

        #endregion
    }
}
