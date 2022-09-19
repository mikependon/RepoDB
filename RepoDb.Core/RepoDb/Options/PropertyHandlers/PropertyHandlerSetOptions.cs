using System.Data;

namespace RepoDb.Options
{
    /// <summary>
    /// An option class that is containing the optional values when pushing the property values towards the database.
    /// </summary>
    public sealed class PropertyHandlerSetOptions : PropertyHandlerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="property"></param>
        private PropertyHandlerSetOptions(IDbDataParameter parameter,
            ClassProperty property)
            : base(property)
        {
            DbParameter = parameter;
        }

        #region Properties

        /// <summary>
        /// Gets the associated <see cref="IDbDataParameter"/> object in used during the push operation.
        /// </summary>
        public IDbDataParameter DbParameter { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        internal static PropertyHandlerSetOptions Create(IDbDataParameter parameter,
            ClassProperty property) =>
            new PropertyHandlerSetOptions(parameter, property);

        #endregion
    }
}
