#region Copyright Attributions

// Copyright (c) 2023 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System.Linq.Expressions;
using RepoDb.Interfaces;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbCommandExpression"></param>
        /// <param name="dbHelper"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetCompilerDbParameterPostCreationExpression(ParameterExpression dbCommandExpression,
            IDbHelper dbHelper)
        {
            var method = StaticType.IDbHelper.GetMethod(nameof(IDbHelper.DynamicHandler))
                .MakeGenericMethod(dbCommandExpression.Type);
            return Expression.Call(Expression.Constant(dbHelper),
                method, dbCommandExpression, Expression.Constant("RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]"));
        }
    }
}
