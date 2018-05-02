using RepoDb.Attributes;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A type of System.Reflection.FieldInfo being cached.
    /// </summary>
    public enum FieldInfoTypes : short
    {
        /// <summary>
        /// A System.DBNull.Value field.
        /// </summary>
        [CreateFieldInfo(TypeTypes.DBNull, "Value")]
        DbNullValue,
    }
}
