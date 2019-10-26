using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class used to resolve the .NET CLR Type into its averageable .NET CLR Type.
    /// </summary>
    public class ClientTypeToAverageableClientTypeResolver : IResolver<Type, Type>
    {
        /// <summary>
        /// Returns the avergeable .NET CLR Type.
        /// </summary>
        /// <param name="type">The .NET CLR Type.</param>
        /// <returns>The avergeable .NET CLR Type.</returns>
        public Type Resolve(Type type)
        {
            if (type == null)
            {
                throw new NullReferenceException("The type must not be null.");
            }

            // Get the type
            type = type?.GetUnderlyingType();

            // Only convert those numerics
            if (type == typeof(short) ||
               type == typeof(int) ||
               type == typeof(long) ||
               type == typeof(UInt16) ||
               type == typeof(UInt32) ||
               type == typeof(UInt64))
            {
                type = typeof(double);
            }

            // Return the type
            return type;
        }
    }
}
