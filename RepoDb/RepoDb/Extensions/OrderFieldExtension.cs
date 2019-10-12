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
            return AsField(orderField, null);
        }

        /// <summary>
        /// Converts an instance of order field into an enumerable list of fields.
        /// </summary>
        /// <param name="orderField">The order field instance to be converted.</param>
        /// <param name="alias">The alias to be used for conversion.</param>
        /// <returns>An enumerable list of fields.</returns>
        public static string AsField(this OrderField orderField,
            string alias)
        {
            if (string.IsNullOrEmpty(alias))
            {
                return string.Concat(orderField.Name, " ", orderField.GetOrderText());
            }
            else
            {
                return string.Concat(alias, ".", orderField.Name, " ", orderField.GetOrderText());
            }
        }
    }
}
