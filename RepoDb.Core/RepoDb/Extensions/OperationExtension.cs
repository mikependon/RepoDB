using System;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Operation"/>.
    /// </summary>
    public static class OperationExtension
    {
        /// <summary>
        /// Gets the text value is used to defined the <see cref="Operation"/>.
        /// </summary>
        public static string GetText(this Operation operation) => operation switch
        {
            Operation.Equal => "=",
            Operation.NotEqual => "<>",
            Operation.LessThan => "<",
            Operation.GreaterThan => ">",
            Operation.LessThanOrEqual => "<=",
            Operation.GreaterThanOrEqual => ">=",
            Operation.Like => "LIKE",
            Operation.NotLike => "NOT LIKE",
            Operation.Between => "BETWEEN",
            Operation.NotBetween => "NOT BETWEEN",
            Operation.In => "IN",
            Operation.NotIn => "NOT IN",
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
    }
}