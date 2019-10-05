using System.Collections.Generic;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <see cref="OrderField"/> object.
    /// </summary>
    public static class OrderFieldExtension
    {
        /// <summary>
        /// Converts an instance of order field into an enumerable list of order fields.
        /// </summary>
        /// <param name="orderField">The order field instance to be converted.</param>
        /// <returns>An enumerable list of order fields.</returns>
        public static IEnumerable<OrderField> AsEnumerable(this OrderField orderField)
        {
            yield return orderField;
        }

        /// <summary>
        /// Converts an instance of order field into an enumerable list of fields.
        /// </summary>
        /// <param name="orderField">The order field instance to be converted.</param>
        /// <returns>An enumerable list of fields.</returns>
        public static string AsField(this OrderField orderField)
        {
            return string.Concat(orderField.Name, " ", orderField.GetOrderText());
        }

        /// <summary>
        /// Converts an instance of order field into an stringified alias-formatted string.
        /// </summary>
        /// <param name="orderField">The order field to be converted.</param>
        /// <param name="alias">The alias to be used for conversion.</param>
        /// <returns>A string value for the stringified alias-formatted converted string.</returns>
        public static string AsAliasField(this OrderField orderField, string alias)
        {
            return string.Concat(alias, ".", orderField.Name, " ", orderField.GetOrderText());
        }
    }
}
