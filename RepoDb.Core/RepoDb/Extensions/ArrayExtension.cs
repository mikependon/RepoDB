using System;
using System.Collections.Generic;

namespace RepoDb.Extensions
{
    /// <summary>
    /// Contains the extension methods for <i>System.Array</i> object.
    /// </summary>
    public static class ArrayExtension
    {
        internal static IEnumerable<object> AsEnumerable(this Array array)
        {
            var objects = new List<object>();
            if (array != null)
            {
                foreach (var obj in array)
                {
                    objects.Add(obj);
                }
            }
            return objects;
        }
    }
}