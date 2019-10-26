using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the SqLite Database Types into its equivalent .NET CLR Types.
    /// </summary>
    public class SqLiteTypeNameToClientTypeResolver : IResolver<string, Type>
    {
        /// <summary>
        /// Returns the equivalent .NET CLR Types of the Database Type.
        /// </summary>
        /// <param name="dbTypeName">The name of the database type.</param>
        /// <returns>The equivalent .NET CLR type.</returns>
        public Type Resolve(string dbTypeName)
        {
            if (dbTypeName == null)
            {
                throw new NullReferenceException("The DB Type name must not be null.");
            }
            switch (dbTypeName.ToLower())
            {
                case "bigint":
                    return typeof(long);
                case "blob":
                    return typeof(byte[]);
                case "boolean":
                    return typeof(bool);
                case "char":
                case "string":
                case "text":
                case "varchar":
                    return typeof(string);
                case "date":
                case "datetime":
                    return typeof(DateTime);
                case "decimal":
                case "numeric":
                    return typeof(decimal);
                case "double":
                    return typeof(double);
                case "integer":
                case "int":
                    return typeof(int);
                case "none":
                    return typeof(object);
                case "real":
                    return typeof(float);
                case "time":
                    return typeof(TimeSpan);
                default:
                    return typeof(object);
            }
        }
    }
}
