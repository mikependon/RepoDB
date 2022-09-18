using System.Data;
using System.Data.Common;

namespace RepoDb.Options
{
    /// <summary>
    /// An option class that is containing the optional values during the hydration process of the property.
    /// </summary>
    public sealed class PropertyHandlerGetOptions : PropertyHandlerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="property"></param>
        private PropertyHandlerGetOptions(DbDataReader reader,
            ClassProperty property)
            : base(property)
        {
            DataReader = reader;
        }

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="IDataReader"/> object in used during the push operation.
        /// </summary>
        public DbDataReader DataReader { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static PropertyHandlerOptions Create(DbDataReader reader,
            ClassProperty property) =>
            new PropertyHandlerGetOptions(reader, property);

        #endregion
    }
}
