using System;
using System.Data.Common;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define the conversion logic when converting an instance of <see cref="DbDataReader"/> into a .NET CLR class.
    /// </summary>
    public enum ConversionType
    {
        /// <summary>
        /// The conversion is strict and there is no additional implied logic during the conversion of <see cref="DbDataReader"/> object into its
        /// destination .NET CLR type.
        /// </summary>
        Default = 1,
        /// <summary>
        /// The data type conversion is not strict. An additional logic from <see cref="Convert"/> object will be used to properly map the <see cref="DbDataReader"/> data type
        /// into its destination .NET CLR type. The operation will only succeed if the data types are convertible.
        /// </summary>
        Automatic = 2
    }
}