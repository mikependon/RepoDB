using System;
using System.Data.Common;
using System.Linq.Expressions;

namespace RepoDb.Enumerations
{
    /// <summary>
    /// An enumeration that is used to define the conversion logic when converting an instance of <see cref="DbDataReader"/> into a .NET CLR class.
    /// </summary>
    public enum ConversionType
    {
        /// <summary>
        /// The conversion is strict. There is no additional implied logic in used during the conversion of the <see cref="DbDataReader"/> object into its destination .NET CLR type.
        /// </summary>
        Default = 1,

        /// <summary>
        /// The conversion is not strict (or automatic). An additional logic from the <see cref="Expression.Convert(Expression, Type)"/> and/or <see cref="Convert"/> objects
        /// will be used to properly map the instance of <see cref="DbDataReader"/> object into its destination .NET CLR type. The operation is compiled ahead-of-time (AOT) and 
        /// will only succeed if the data types of both objects are convertible.
        /// </summary>
        Automatic = 2
    }
}