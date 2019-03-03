using System.Data.Common;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enum used to define the conversion strictness when converting the instance of <see cref="DbDataReader"/> into a class.
    /// </summary>
    public enum ConversionType : short
    {
        /// <summary>
        /// The conversion is strict and there are no additional implied logics during the conversion.
        /// </summary>
        Default = 1,
        /// <summary>
        /// The data type conversion is not strict. An additional logic will be used to properly map the proper <see cref="DbDataReader"/> data type
        /// into its destination class property type. The operation will only succeed if the data types are convertible.
        /// </summary>
        Ample = 2
    }
}
