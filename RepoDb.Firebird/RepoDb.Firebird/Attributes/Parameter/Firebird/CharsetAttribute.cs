using FirebirdSql.Data.FirebirdClient;

namespace RepoDb.Attributes.Parameter.Firebird
{
    /// <summary>
    /// An attribute used to define a value to the <see cref="FbParameter.Charset"/>
    /// property via an entity property before the actual execution.
    /// </summary>
    public class CharsetAttribute : PropertyValueAttribute
    {
        /// <summary>
        /// Creates a new instance of <see cref="CharsetAttribute"/> class.
        /// </summary>
        /// <param name="charset">A target <see cref="global::FirebirdSql.Data.FirebirdClient.FbCharset"/> value.</param>
        public CharsetAttribute(FbCharset charset)
            : base(typeof(FbParameter), nameof(FbParameter.Charset), charset)
        { }

        /// <summary>
        /// Gets the mapped <see cref="global::FirebirdSql.Data.FirebirdClient.FbCharset"/> value of the parameter.
        /// </summary>
        public FbCharset Charset => (FbCharset)Value;
    }
}
