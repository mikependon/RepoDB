using System;
using System.Collections.Generic;

namespace RepoDb.Extensions
{
    /// <summary>
    /// An extension class for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// Retrieve or add a value to dictionary
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key value</param>
        /// <param name="value">value</param>
        /// <returns>value</returns>
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, TV value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }

            return dictionary[key];
        }

        /// <summary>
        /// Retrieve or add a value to dictionary via add function.
        /// </summary>
        /// <typeparam name="TK">key type</typeparam>
        /// <typeparam name="TV">value type</typeparam>
        /// <param name="dictionary">dictionary</param>
        /// <param name="key">key value</param>
        /// <param name="addFunc">add function returning the value</param>
        /// <returns>value</returns>
        public static TV GetOrAdd<TK, TV>(this IDictionary<TK, TV> dictionary, TK key, Func<TV> addFunc)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, addFunc());
            }

            return dictionary[key];
        }
    }
}
