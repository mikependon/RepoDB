#region Copyright Attributions

// Copyright (c) 2023 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using System;
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
            // The handler is invoked through a statically constructed delegate, as the generic method cannot be
            // constructed at runtime when publishing with NativeAOT.
            var handler = (Action<System.Data.Common.DbParameter, string>)dbHelper.DynamicHandler<System.Data.Common.DbParameter>;
            return Expression.Call(Expression.Constant(handler),
                typeof(Action<System.Data.Common.DbParameter, string>).GetMethod(nameof(Action.Invoke)),
                ConvertExpressionToTypeExpression(dbCommandExpression, StaticType.DbParameter),
                Expression.Constant("RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]"));
        }
    }
}
