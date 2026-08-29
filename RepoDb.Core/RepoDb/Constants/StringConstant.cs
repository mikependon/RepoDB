namespace RepoDb
{
    /// <summary>
    /// A class that contains all the constant strings.
    /// </summary>
    internal static class StringConstant
    {
        /// <summary>
        /// The text being prepended to the parameter name whenever a <see cref="QueryField"/> (or <see cref="QueryGroup"/>)
        /// is being used for an 'Update' operation (see <see cref="QueryField.IsForUpdate"/> and <see cref="QueryGroup.IsForUpdate"/>).
        /// </summary>
        internal const string UpdateParameterPrefix = "m_";
    }
}
