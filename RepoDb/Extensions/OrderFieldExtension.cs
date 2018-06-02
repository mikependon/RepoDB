using System.Collections.Generic;
using RepoDb.Interfaces;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>RepoDb.Interfaces.IOrderField</i> object.
    /// </summary>
    public static class OrderFieldExtension
    {
        /// <summary>
        /// Converts an instance of order field into an enumerable list of order fields.
        /// </summary>
        /// <param name="orderField">The order field instance to be converted.</param>
        /// <returns>An enumerable list of order fields.</returns>
        public static IEnumerable<IOrderField> AsEnumerable(this IOrderField orderField)
        {
            return new[] { orderField };
        }

        /// <summary>
        /// Converts an instance of order field into an enumerable list of fields.
        /// </summary>
        /// <param name="orderField">The order field instance to be converted.</param>
        /// <returns>An enumerable list of fields.</returns>
        internal static string AsField(this IOrderField orderField)
        {
            return $"[{orderField.Name}] {orderField.GetOrderText()}";
        }

        /// <summary>
        /// Converts an instance of order field into an stringified alias-formatted string.
        /// </summary>
        /// <param name="orderField">The order field to be converted.</param>
        /// <param name="alias">The alias to be used for conversion.</param>
        /// <returns>A string value for the stringified alias-formatted converted string.</returns>
        internal static string AsAliasField(this IOrderField orderField, string alias)
        {
            return $"{alias}.[{orderField.Name}] {orderField.GetOrderText()}";
        }
    }
}
 