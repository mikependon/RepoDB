using RepoDb.Attributes;
using RepoDb.Interfaces;
using System;
using System.Reflection;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is being used to resolve the equivalent <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.
    /// </summary>
    public class ClassHandlerResolver : IResolver<Type, object>
    {
        /// <summary>
        /// Resolves the equivalent <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type</param>
        /// <returns>The equivalent <see cref="IClassHandler{TEntity}"/> object of the .NET CLR type.</returns>
        public object Resolve(Type type)
        {
            var classHandler = (object)null;

            // Attribute
            var attribute = type.GetCustomAttribute<ClassHandlerAttribute>();
            if (attribute != null)
            {
                classHandler = Converter.ToType<object>(Activator.CreateInstance(attribute.HandlerType));
            }

            // Type Level
            if (classHandler == null)
            {
                classHandler = ClassHandlerMapper.Get<object>(type);
            }

            // Return the value
            return classHandler;
        }
    }
}
