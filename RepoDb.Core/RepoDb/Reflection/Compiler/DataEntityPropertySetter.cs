using System;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="field">The target <see cref="Field"/>.</param>
        /// <returns>A compiled function that is used to set the data entity object property value.</returns>
        public static Action<TEntity, object> CompileDataEntityPropertySetter<TEntity>(Field field)
            where TEntity : class
        {
            // Variables for type
            var typeOfEntity = typeof(TEntity);

            // Get the entity property
            var property = (typeOfEntity.GetProperty(field.Name) ?? typeOfEntity.GetMappedProperty(field.Name)?.PropertyInfo);

            // Return the function
            return CompileDataEntityPropertySetter<TEntity>(property, field.Type);
        }

        /// <summary>
        /// Gets a compiled function that is used to set the data entity object property value.
        /// </summary>
        /// <typeparam name="TEntity">The type of the data entity.</typeparam>
        /// <param name="property">The target <see cref="PropertyInfo"/>.</param>
        /// <param name="targetType">The target .NET CLR type.</param>
        /// <returns>A compiled function that is used to set the data entity object property value.</returns>
        public static Action<TEntity, object> CompileDataEntityPropertySetter<TEntity>(PropertyInfo property,
            Type targetType)
            where TEntity : class
        {
            // Check the property first
            if (property == null)
            {
                return null;
            }

            // Variables for type
            var typeOfEntity = typeof(TEntity);
            var underlyingType = targetType.GetUnderlyingType();

            // Variables for argument
            var entityParameter = Expression.Parameter(typeOfEntity, "entity");
            var valueParameter = Expression.Parameter(StaticType.Object, "value");

            // Get the converter
            var toTypeMethod = StaticType
                .Converter
                .GetMethod("ToType", new[] { StaticType.Object })
                .MakeGenericMethod(underlyingType);

            // Assign the value into DataEntity.Property
            var propertyAssignment = Expression.Call(entityParameter, property.SetMethod,
                Expression.Convert(Expression.Call(toTypeMethod, valueParameter), underlyingType));

            // Return function
            return Expression.Lambda<Action<TEntity, object>>(propertyAssignment,
                entityParameter, valueParameter).Compile();
        }
    }
}
