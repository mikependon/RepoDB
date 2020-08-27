using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace RepoDb.Reflection
{
    internal partial class Compiler
    {
        #region SqlServer (System)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariable"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterSystemSqlDbTypeAssignmentExpression(ParameterExpression parameterVariable,
            ClassProperty classProperty)
        {
            var attribute = GetSystemSqlServerTypeMapAttribute(classProperty);
            if (attribute == null)
            {
                return null;
            }

            var dbType = GetSystemSqlServerDbTypeFromAttribute(attribute);
            var parameterType = GetSystemSqlServerParameterTypeFromAttribute(attribute);
            var setMethod = GetSystemSqlServerDbTypeFromAttributeSetMethod(attribute);

            return Expression.Call(ConvertValueExpressionToTypeExpression(parameterVariable, parameterType),
                setMethod,
                Expression.Constant(dbType));
        }

        /// <summary>
        /// Gets the SystemSqlServerTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of SystemSqlServerTypeMapAttribute.</returns>
        internal static Attribute GetSystemSqlServerTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.SystemSqlServerTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the SystemSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of SystemSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the SystemSqlServerTypeMapAttribute.DbType property.</returns>
        internal static object GetSystemSqlServerDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of System.Data.SqlClient.SqlParameter represented by SystemSqlServerTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of SystemSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The type of System.Data.SqlClient.SqlParameter represented by SystemSqlServerTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetSystemSqlServerParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the SystemSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of SystemSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the SystemSqlServerTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetSystemSqlServerDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetSystemSqlServerParameterTypeFromAttribute(attribute)?
                .GetProperty("SqlDbType")?
                .SetMethod;
        }

        #endregion

        #region SqlServer (Microsoft)

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariable"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterMicrosoftSqlDbTypeAssignmentExpression(ParameterExpression parameterVariable,
            ClassProperty classProperty)
        {
            var attribute = GetMicrosoftSqlServerTypeMapAttribute(classProperty);
            if (attribute == null)
            {
                return null;
            }

            var dbType = GetMicrosoftSqlServerDbTypeFromAttribute(attribute);
            var parameterType = GetMicrosoftSqlServerParameterTypeFromAttribute(attribute);
            var setMethod = GetMicrosoftSqlServerDbTypeFromAttributeSetMethod(attribute);

            return Expression.Call(ConvertValueExpressionToTypeExpression(parameterVariable, parameterType),
                setMethod,
                Expression.Constant(dbType));
        }

        /// <summary>
        /// Gets the MicrosoftSqlServerTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of MicrosoftSqlServerTypeMapAttribute.</returns>
        internal static Attribute GetMicrosoftSqlServerTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.MicrosoftSqlServerTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MicrosoftSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.</returns>
        internal static object GetMicrosoftSqlServerDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of Microsoft.Data.SqlClient.SqlParameter represented by MicrosoftSqlServerTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of MicrosoftSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The type of Microsoft.Data.SqlClient.SqlParameter represented by MicrosoftSqlServerTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetMicrosoftSqlServerParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MicrosoftSqlServerTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the MicrosoftSqlServerTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetMicrosoftSqlServerDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetMicrosoftSqlServerParameterTypeFromAttribute(attribute)?
                .GetProperty("SqlDbType")?
                .SetMethod;
        }

        #endregion

        #region MySql

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariable"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterMySqlDbTypeAssignmentExpression(ParameterExpression parameterVariable,
            ClassProperty classProperty)
        {
            var attribute = GetMySqlDbTypeTypeMapAttribute(classProperty);
            if (attribute == null)
            {
                return null;
            }

            var dbType = GetMySqlDbTypeFromAttribute(attribute);
            var parameterType = GetMySqlParameterTypeFromAttribute(attribute);
            var setMethod = GetMySqlDbTypeFromAttributeSetMethod(attribute);

            return Expression.Call(ConvertValueExpressionToTypeExpression(parameterVariable, parameterType),
                setMethod,
                Expression.Constant(dbType));
        }

        /// <summary>
        /// Gets the MySqlTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of MySqlTypeMapAttribute.</returns>
        internal static Attribute GetMySqlDbTypeTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.MySqlTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the MySqlTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MySqlTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the MySqlTypeMapAttribute.DbType property.</returns>
        internal static object GetMySqlDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of MySql.Data.MySqlClient.MySqlParameter represented by MySqlTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of MySqlTypeMapAttribute to extract.</param>
        /// <returns>The type of MySql.Data.MySqlClient.MySqlParameter represented by MySqlTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetMySqlParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the MySqlTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of MySqlTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the MySqlTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetMySqlDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetMySqlParameterTypeFromAttribute(attribute)?
                .GetProperty("MySqlDbType")?
                .SetMethod;
        }

        #endregion

        #region Npgsql

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterVariable"></param>
        /// <param name="classProperty"></param>
        /// <returns></returns>
        internal static MethodCallExpression GetDbParameterNpgsqlDbTypeAssignmentExpression(ParameterExpression parameterVariable,
            ClassProperty classProperty)
        {
            var attribute = GetNpgsqlDbTypeTypeMapAttribute(classProperty);
            if (attribute == null)
            {
                return null;
            }

            var dbType = GetNpgsqlDbTypeFromAttribute(attribute);
            var parameterType = GetNpgsqlParameterTypeFromAttribute(attribute);
            var setMethod = GetNpgsqlDbTypeFromAttributeSetMethod(attribute);

            return Expression.Call(ConvertValueExpressionToTypeExpression(parameterVariable, parameterType),
                setMethod,
                Expression.Constant(dbType));
        }

        /// <summary>
        /// Gets the NpgsqlDbTypeMapAttribute if present.
        /// </summary>
        /// <param name="property">The instance of propery to inspect.</param>
        /// <returns>The instance of NpgsqlDbTypeMapAttribute.</returns>
        internal static Attribute GetNpgsqlDbTypeTypeMapAttribute(ClassProperty property)
        {
            return property?
                .PropertyInfo
                .GetCustomAttributes()?
                .FirstOrDefault(e =>
                    e.GetType().FullName.Equals("RepoDb.Attributes.NpgsqlTypeMapAttribute"));
        }

        /// <summary>
        /// Gets the value represented by the NpgsqlDbTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of NpgsqlDbTypeMapAttribute to extract.</param>
        /// <returns>The value represented by the NpgsqlDbTypeMapAttribute.DbType property.</returns>
        internal static object GetNpgsqlDbTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            var type = attribute.GetType();
            return type
                .GetProperty("DbType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the system type of NpgsqlTypes.NpgsqlParameter represented by NpgsqlDbTypeMapAttribute.ParameterType property.
        /// </summary>
        /// <param name="attribute">The instance of NpgsqlDbTypeMapAttribute to extract.</param>
        /// <returns>The type of NpgsqlTypes.NpgsqlParameter represented by NpgsqlDbTypeMapAttribute.ParameterType property.</returns>
        internal static Type GetNpgsqlParameterTypeFromAttribute(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return (Type)attribute
                .GetType()
                .GetProperty("ParameterType")?
                .GetValue(attribute);
        }

        /// <summary>
        /// Gets the instance of <see cref="MethodInfo"/> represented by the NpgsqlDbTypeMapAttribute.DbType property.
        /// </summary>
        /// <param name="attribute">The instance of NpgsqlDbTypeMapAttribute to extract.</param>
        /// <returns>The instance of <see cref="MethodInfo"/> represented by the NpgsqlDbTypeMapAttribute.DbType property.</returns>
        internal static MethodInfo GetNpgsqlDbTypeFromAttributeSetMethod(Attribute attribute)
        {
            if (attribute == null)
            {
                return null;
            }
            return GetNpgsqlParameterTypeFromAttribute(attribute)?
                .GetProperty("NpgsqlDbType")?
                .SetMethod;
        }

        #endregion
    }
}
