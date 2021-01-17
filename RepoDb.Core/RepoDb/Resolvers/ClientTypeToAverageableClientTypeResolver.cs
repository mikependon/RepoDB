using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the .NET CLR type into its averageable .NET CLR type.
    /// </summary>
    public class ClientTypeToAverageableClientTypeResolver : IResolver<Type, Type>
    {
        /// <summary>
        /// Returns the averageable .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type.</param>
        /// <returns>The averageable .NET CLR type.</returns>
        public Type Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }

            // Get the type
            type = type.GetUnderlyingType();

            // Only convert those numerics
            if (type == StaticType.Int16 ||
               type == StaticType.Int32 ||
               type == StaticType.Int64 ||
               type == StaticType.UInt16 ||
               type == StaticType.UInt32 ||
               type == StaticType.UInt64)
            {
                type = StaticType.Double;
            }

            // Return the type
            return type;
        }
    }
}
