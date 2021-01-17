using System;
using System.Linq.Expressions;
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
        internal static Action<TEntity, object> CompileDictionaryStringObjectItemSetter<TEntity>(Field field)
            where TEntity : class
        {
            // Check the property first
            if (field == null)
            {
                return null;
            }

            // Variables for type
            var typeOfEntity = typeof(TEntity);
            var valueParameter = Expression.Parameter(StaticType.Object, "value");
            var targetType = field.Type?.GetUnderlyingType();
            var valueExpression = (Expression)valueParameter;

            if (targetType != null)
            {
                // Get the converter
                var toTypeMethod = StaticType
                    .Converter
                    .GetMethod("ToType", new[] { StaticType.Object })
                    .MakeGenericMethod(field.Type?.GetUnderlyingType());

                // Conversion (if needed)
                valueExpression = ConvertExpressionToTypeExpression(Expression.Call(toTypeMethod, valueParameter), targetType);
            }

            // Property Handler
            valueExpression = ConvertExpressionToPropertyHandlerSetExpression(valueExpression, null, targetType);

            // Assign the value into DataEntity.Property
            var dictionaryParameter = Expression.Parameter(typeOfEntity, "entity");
            var itemIndexMethod = StaticType.IDictionaryStringObject.GetMethod("set_Item", new[]
            {
                StaticType.String,
                StaticType.Object
            });
            var itemAssignment = Expression.Call(dictionaryParameter, itemIndexMethod,
                Expression.Constant(field.Name), ConvertExpressionToTypeExpression(valueExpression, StaticType.Object));

            // Return function
            return Expression.Lambda<Action<TEntity, object>>(itemAssignment,
                dictionaryParameter, valueParameter).Compile();
        }
    }
}
