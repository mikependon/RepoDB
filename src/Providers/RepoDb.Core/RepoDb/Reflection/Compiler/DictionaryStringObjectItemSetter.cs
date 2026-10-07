#region Copyright Attributions

// Copyright (c) 2020 Michael Camara Pendon.
// Portions copyright their respective RepoDB contributors.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

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
        /// <param name="entityType"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        internal static Action<object, object> CompileDictionaryStringObjectItemSetter(Type entityType,
            Field field)
        {
            // Check the property first
            if (field == null)
            {
                return null;
            }

            // Variables for type
            var valueParameter = Expression.Parameter(StaticType.Object, "value");
            var targetType = TypeCache.Get(field.Type)?.GetUnderlyingType();
            var valueExpression = (Expression)valueParameter;

            if (targetType != null)
            {
                // Conversion (if needed)
                valueExpression = ConvertExpressionToTypeExpression(GetConverterToTypeExpression(valueParameter, targetType), targetType);
            }

            // Property Handler
            valueExpression = ConvertExpressionToPropertyHandlerSetExpression(valueExpression, parameterExpression: null, classProperty: null, targetType);

            // Assign the value into DataEntity.Property
            var dictionaryParameter = Expression.Parameter(StaticType.Object, "entity");
            var itemIndexMethod = typeof(System.Collections.Generic.IDictionary<string, object>).GetMethod("set_Item", new[]
            {
                StaticType.String,
                StaticType.Object
            });
            var itemAssignment = Expression.Call(ConvertExpressionToTypeExpression(dictionaryParameter, entityType),
                itemIndexMethod,
                Expression.Constant(field.Name),
                ConvertExpressionToTypeExpression(valueExpression, StaticType.Object));

            // Return function
            return Expression
                .Lambda<Action<object, object>>(itemAssignment,
                    dictionaryParameter, valueParameter)
                .Compile();
        }
    }
}
