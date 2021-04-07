using RepoDb.Interfaces;
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
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>An enumerable list of fields.</returns>
        public static string AsField(this OrderField orderField,
            IDbSetting dbSetting) =>
            AsField(orderField, null, dbSetting);

        /// <summary>
        /// Converts an instance of order field into an enumerable list of fields.
        /// </summary>
        /// <param name="orderField">The order field instance to be converted.</param>
        /// <param name="alias">The alias to be used for conversion.</param>
        /// <param name="dbSetting">The currently in used <see cref="IDbSetting"/> object.</param>
        /// <returns>An enumerable list of fields.</returns>
        public static string AsField(this OrderField orderField,
            string alias,
            IDbSetting dbSetting)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                return string.Concat(orderField.Name.AsField(dbSetting), " ", orderField.Order.GetText());
            }
            else
            {
                return string.Concat(orderField.Name.AsAliasField(alias, dbSetting), " ", orderField.Order.GetText());
            }
        }
    }
}
