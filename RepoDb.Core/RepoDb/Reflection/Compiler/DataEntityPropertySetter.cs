using System;
using System.Linq.Expressions;
using System.Reflection;
using RepoDb.Extensions;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static Action<TEntity, object> CompileDataEntityPropertySetter<TEntity>(Field field)
            where TEntity : class
        {
            // Variables for type
            var typeOfEntity = typeof(TEntity);

            // Get the entity property
            var property = typeOfEntity.GetMappedProperty(field.Name)?.PropertyInfo;

            // Return the function
            return CompileDataEntityPropertySetter<TEntity>(property, property?.PropertyType ?? field.Type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="property"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        internal static Action<TEntity, object> CompileDataEntityPropertySetter<TEntity>(PropertyInfo property,
            Type targetType)
            where TEntity : class
        {
            // Check the property first
            if (property == null)
            {
                return null;
            }

            // Make sure we can write
            if (property.CanWrite == false)
            {
                return null;
            }

            // Variables for type
            var typeOfEntity = typeof(TEntity);

            // Variables for argument
            var valueParameter = Expression.Parameter(StaticType.Object, "value");

            // Get the converter
            var toTypeMethod = StaticType
                .Converter
                .GetMethod("ToType", new[] { StaticType.Object })
                .MakeGenericMethod(targetType.GetUnderlyingType());

            // Conversion (if needed)
            var valueExpression = ConvertExpressionToTypeExpression(Expression.Call(toTypeMethod, valueParameter), targetType);

            // Property Handler
            if (typeOfEntity.IsClassType())
            {
                var classProperty = PropertyCache.Get(typeOfEntity, property);
                valueExpression = ConvertExpressionToPropertyHandlerSetExpression(valueExpression,
                    classProperty, targetType ?? classProperty.PropertyInfo.PropertyType);
            }

            // Assign the value into DataEntity.Property
            var entityParameter = Expression.Parameter(typeOfEntity, "entity");
            var propertyAssignment = Expression.Call(entityParameter, property.SetMethod,
                valueExpression);

            // Return function
            return Expression.Lambda<Action<TEntity, object>>(propertyAssignment,
                entityParameter, valueParameter).Compile();
        }
    }
}
