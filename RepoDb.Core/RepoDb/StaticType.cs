using RepoDb.Attributes;
using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Reflection;

namespace RepoDb
{
    /// <summary>
    /// A static class that contains all the .NET CLR types needed within the library.
    /// </summary>
    internal static class StaticType
    {
        /// <summary>
        /// Gets a type of the <see cref="System.ReflectionBindingFlags"/> .NET CLR type.
        /// </summary>
        public static Type BindingFlags => typeof(BindingFlags);

        /// <summary>
        /// Gets a type of the <see cref="bool"/> .NET CLR type.
        /// </summary>
        public static Type Boolean => typeof(bool);

        /// <summary>
        /// Gets a type of the <see cref="byte"/> .NET CLR type.
        /// </summary>
        public static Type Byte => typeof(Byte);

        /// <summary>
        /// Gets a type of the <see cref="byte"/> (array) .NET CLR type.
        /// </summary>
        public static Type ByteArray => typeof(byte[]);

        /// <summary>
        /// Gets a type of the <see cref="char"/> (array) .NET CLR type.
        /// </summary>
        public static Type CharArray => typeof(char[]);

        /// <summary>
        /// Gets a type of the <see cref="RepoDb.ClassProperty"/> .NET CLR type.
        /// </summary>
        public static Type ClassProperty => typeof(ClassProperty);

        /// <summary>
        /// Gets a type of the <see cref="System.ComponentModel.DataAnnotations.Schema.ColumnAttribute"/> .NET CLR type.
        /// </summary>
        public static Type ColumnAttribute => typeof(ColumnAttribute);

        /// <summary>
        /// Gets a type of the <see cref="System.Convert"/> .NET CLR type.
        /// </summary>
        public static Type Convert => typeof(Convert);

        /// <summary>
        /// Gets a type of the <see cref="RepoDb.Converter"/> .NET CLR type.
        /// </summary>
        public static Type Converter => typeof(Converter);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.DataTable"/> .NET CLR type.
        /// </summary>
        public static Type DataTable => typeof(System.Data.DataTable);

        /// <summary>
        /// Gets a type of the <see cref="System.DateTime"/> .NET CLR type.
        /// </summary>
        public static Type DateTime => typeof(DateTime);

        /// <summary>
        /// Gets a type of the <see cref="System.DateTimeOffset"/> .NET CLR type.
        /// </summary>
        public static Type DateTimeOffset => typeof(DateTimeOffset);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.Common.DbCommand"/> .NET CLR type.
        /// </summary>
        public static Type DbCommand => typeof(DbCommand);

        /// <summary>
        /// Gets a type of the <see cref="Extensions.DbCommandExtension"/> .NET CLR type.
        /// </summary>
        public static Type DbCommandExtension => typeof(DbCommandExtension);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.Common.DbConnection"/> .NET CLR type.
        /// </summary>
        public static Type DbConnection => typeof(DbConnection);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.Common.DbDataReader"/> .NET CLR type.
        /// </summary>
        public static Type DbDataReader => typeof(DbDataReader);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.Common.DbParameter"/> .NET CLR type.
        /// </summary>
        public static Type DbParameter => typeof(DbParameter);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.Common.DbParameterCollection"/> .NET CLR type.
        /// </summary>
        public static Type DbParameterCollection => typeof(DbParameterCollection);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.DbType"/> (array) .NET CLR type.
        /// </summary>
        public static Type DbType => typeof(DbType);

        /// <summary>
        /// Gets a type of the <see cref="decimal"/> .NET CLR type.
        /// </summary>
        public static Type Decimal => typeof(decimal);

        /// <summary>
        /// Gets a type of the <see cref="Dictionary{TKey, TValue}"/> .NET CLR type.
        /// </summary>
        public static Type Dictionary => typeof(Dictionary<,>);

        /// <summary>
        /// Gets a type of the <see cref="Dictionary{TKey, TValue}"/> (with string/object key-value-pair) .NET CLR type.
        /// </summary>
        public static Type DictionaryStringObject => typeof(Dictionary<string, object>);

        /// <summary>
        /// Gets a type of the <see cref="double"/> .NET CLR type.
        /// </summary>
        public static Type Double => typeof(double);

        /// <summary>
        /// Gets a type of the <see cref="System.Enum"/> .NET CLR type.
        /// </summary>
        public static Type Enum => typeof(Enum);

        /// <summary>
        /// Gets a type of the <see cref="System.DynamicExpandoObject"/> .NET CLR type.
        /// </summary>
        public static Type ExpandoObject => typeof(ExpandoObject);

        /// <summary>
        /// Gets a type of the <see cref="System.Guid"/> .NET CLR type.
        /// </summary>;
        public static Type Guid => typeof(Guid);

        /// <summary>
        /// Gets a type of the <see cref="IClassHandler"/> .NET CLR type.
        /// </summary>
        public static Type IClassHandler => typeof(IClassHandler<>);

        /// <summary>
        /// Gets a type of the <see cref="System.Data.IDbCommand"/> .NET CLR type.
        /// </summary>
        public static Type IDbCommand => typeof(IDbCommand);

        /// <summary>
        /// Gets a type of the <see cref="Nullable{T}"/> (of type <see cref="DbType"/>) .NET CLR type.
        /// </summary>
        public static Type DbTypeNullable => typeof(Nullable<DbType>);

        /// <summary>
        /// Gets a type of the <see cref="Attributes.IdentityAttribute"/> .NET CLR type.
        /// </summary>
        public static Type IdentityAttribute => typeof(IdentityAttribute);

        /// <summary>
        /// Gets a type of the <see cref="System.Collections.IEnumerable"/> .NET CLR type.
        /// </summary>
        public static Type IEnumerable => typeof(System.Collections.IEnumerable);

        /// <summary>
        /// Gets a type of the <see cref="IDictionary{TKey, TValue}"/> .NET CLR type.
        /// </summary>
        public static Type IDictionary => typeof(IDictionary<,>);

        /// <summary>
        /// Gets a type of the <see cref="IDictionary{TKey, TValue}"/> (with string/object key-value-pair) .NET CLR type.
        /// </summary>
        public static Type IDictionaryStringObject => typeof(IDictionary<string, object>);

        /// <summary>
        /// Gets a type of the <see cref="short"/> .NET CLR type.
        /// </summary>
        public static Type Int16 => typeof(short);

        /// <summary>
        /// Gets a type of the <see cref="int"/> .NET CLR type.
        /// </summary>
        public static Type Int32 => typeof(int);

        /// <summary>
        /// Gets a type of the <see cref="long"/> .NET CLR type.
        /// </summary>
        public static Type Int64 => typeof(long);

        /// <summary>
        /// Gets a type of the <see cref="IPropertyHandler"/> .NET CLR type.
        /// </summary>
        public static Type IPropertyHandler => typeof(IPropertyHandler<,>);

        /// <summary>
        /// Gets a type of the <see cref="Interfaces.IStatementBuilder"/> .NET CLR type.
        /// </summary>
        public static Type IStatementBuilder => typeof(IStatementBuilder);

        /// <summary>
        /// Gets a type of the <see cref="Attributes.MapAttribute"/> .NET CLR type.
        /// </summary>
        public static Type MapAttribute => typeof(MapAttribute);

        /// <summary>
        /// Gets a type of the <see cref="System.Nullable"/> .NET CLR type.
        /// </summary>
        public static Type Nullable => typeof(Nullable<>);

        /// <summary>
        /// Gets a type of the <see cref="object"/> .NET CLR type.
        /// </summary>
        public static Type Object => typeof(object);

        /// <summary>
        /// Gets a type of the <see cref="Enumerations.Operation"/> .NET CLR type.
        /// </summary>
        public static Type Operation => typeof(Operation);

        /// <summary>
        /// Gets a type of the <see cref="Attributes.PropertyHandlerAttribute"/> .NET CLR type.
        /// </summary>
        public static Type PropertyHandlerAttribute => typeof(PropertyHandlerAttribute);

        /// <summary>
        /// Gets a type of the <see cref="System.Reflection.PropertyInfo"/> .NET CLR type.
        /// </summary>
        public static Type PropertyInfo => typeof(PropertyInfo);

        /// <summary>
        /// Gets a type of the <see cref="RepoDb.PropertyValue"/> .NET CLR type.
        /// </summary>
        public static Type PropertyValue => typeof(PropertyValue);

        /// <summary>
        /// Gets a type of the <see cref="List{T}"/> (of <see cref="RepoDb.PropertyValue"/>) .NET CLR type.
        /// </summary>
        public static Type PropertyValueList => typeof(List<PropertyValue>);

        /// <summary>
        /// Gets a type of the <see cref="RepoDb.QueryField"/> .NET CLR type.
        /// </summary>
        public static Type QueryField => typeof(QueryField);

        /// <summary>
        /// Gets a type of the <see cref="RepoDb.QueryGroup"/> .NET CLR type.
        /// </summary>
        public static Type QueryGroup => typeof(QueryGroup);

        /// <summary>
        /// Gets a type of the <see cref="float"/> .NET CLR type.
        /// </summary>
        public static Type Single => typeof(float);

        /// <summary>
        /// Gets a type of the <see cref="string"/> .NET CLR type.
        /// </summary>
        public static Type String => typeof(string);

        /// <summary>
        /// Gets a type of the <see cref="Types.SqlVariant"/> .NET CLR type.
        /// </summary>
        public static Type SqlVariant => typeof(SqlVariant);

        /// <summary>
        /// Gets a type of the <see cref="System.ComponentModel.DataAnnotations.Schema.TableAttribute"/> .NET CLR type.
        /// </summary>
        public static Type TableAttribute => typeof(TableAttribute);

        /// <summary>
        /// Gets a type of the <see cref="System.TimeSpan"/> .NET CLR type.
        /// </summary>
        public static Type TimeSpan => typeof(TimeSpan);

        /// <summary>
        /// Gets a type of the <see cref="System.Type"/> .NET CLR type.
        /// </summary>
        public static Type Type => typeof(Type);

        /// <summary>
        /// Gets a type of the <see cref="Attributes.TypeMapAttribute"/> .NET CLR type.
        /// </summary>
        public static Type TypeMapAttribute => typeof(TypeMapAttribute);

        /// <summary>
        /// Gets a type of the <see cref="ushort"/> .NET CLR type.
        /// </summary>
        public static Type UInt16 => typeof(ushort);

        /// <summary>
        /// Gets a type of the <see cref="uint"/> .NET CLR type.
        /// </summary>
        public static Type UInt32 => typeof(uint);

        /// <summary>
        /// Gets a type of the <see cref="ulong"/> .NET CLR type.
        /// </summary>
        public static Type UInt64 => typeof(ulong);
    }
}
