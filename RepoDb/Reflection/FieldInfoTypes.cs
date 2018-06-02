using RepoDb.Attributes;

namespace RepoDb.Reflection
{
    /// <summary>
    /// An enumeration for type of <i>System.Reflection.FieldInfo</i>.
    /// </summary>
    public enum FieldInfoTypes : short
    {
        /// <summary>
        /// A <i>System.DBNull.Value</i> field.
        /// </summary>
        [CreateFieldInfo(TypeTypes.DBNull, "Value")]
        DbNullValue,
    }
}
