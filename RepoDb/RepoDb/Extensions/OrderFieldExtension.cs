using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using RepoDb.Attributes;
using RepoDb.Interfaces;
using RepoDb.Enumerations;

namespace RepoDb.Extensions
{
    public static class OrderFieldExtension
    {
        // AsEnumerable
        public static IEnumerable<IOrderField> AsEnumerable(this IOrderField orderField)
        {
            return new[] { orderField };
        }

        // AsField
        internal static string AsField(this IOrderField orderField)
        {
            return $"[{orderField.Name}] {orderField.GetOrderText()}";
        }

        // AsAliasField
        internal static string AsAliasField(this IOrderField orderField, string alias)
        {
            return $"{alias}.[{orderField.Name}] {orderField.GetOrderText()}";
        }
    }
}
 