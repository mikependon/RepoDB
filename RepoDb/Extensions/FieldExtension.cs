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
    public static class FieldExtension
    {
        // AsEnumerable
        public static IEnumerable<IField> AsEnumerable(this IField field)
        {
            return new[] { field };
        }

        // AsField
        internal static string AsField(this IField field)
        {
            return $"[{field.Name}]";
        }

        // AsJoinQualifier
        internal static string AsJoinQualifier(this IField field, string leftAlias, string rightAlias)
        {
            return $"{leftAlias}.[{field.Name}] = {rightAlias}.[{field.Name}]";
        }

        // AsFieldAndAliasField
        internal static string AsFieldAndAliasField(this IField field, string alias)
        {
            return $"{AsField(field)} = {alias}.{AsField(field)}";
        }
    }
}
 