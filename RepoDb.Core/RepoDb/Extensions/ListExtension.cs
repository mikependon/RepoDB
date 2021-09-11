using System.Collections.Generic;
using System.Linq;

namespace RepoDb.Extensions
{
    /// <summary>
    /// An extension class for <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        /// Adds an item into the <see cref="IList{T}"/> if not null.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="list">The instance of the list.</param>
        /// <param name="item">The item to be evaulated and added.</param>
        public static void AddIfNotNull<T>(this IList<T> list,
            T item)
        {
            if (item != null)
            {
                list.Add(item);
            }
        }

        /// <summary>
        /// Adds an item into the <see cref="IList{T}"/> if not null.
        /// </summary>
        /// <typeparam name="T">The type of the item.</typeparam>
        /// <param name="list">The instance of the list.</param>
        /// <param name="items">The items to be evaulated and added.</param>
        public static void AddRangeIfNotNullOrNotEmpty<T>(this List<T> list,
            IEnumerable<T> items)
        {
            if (items?.Any() == true)
            {
                list.AddRange(items);
            }
        }
    }
}
