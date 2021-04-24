using System;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="Order"/>.
    /// </summary>
    public static class OrderExtension
    {
        /// <summary>
        /// Gets the text value is used to defined the <see cref="Order"/>.
        /// </summary>
        public static string GetText(this Order order) => order switch
        {
            Order.Ascending => "ASC",
            Order.Descending => "DESC",
            _ => throw new ArgumentOutOfRangeException(nameof(order))
        };
    }
}