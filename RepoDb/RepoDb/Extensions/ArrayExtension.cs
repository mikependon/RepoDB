using System;
using System.Collections.Generic;

namespace RepoDb.Extensions
{
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