using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Extensions;
using RepoDb.Reflection.Delegates;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static factor class used to create a delegate a custom delegate.
    /// </summary>
    public static class DelegateFactory
    {
        /// <summary>
        /// Gets a delegate that is used to convert the <i>System.Data.Common.DbDataReader</i> object into <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The <i>RepoDb.DataEntity</i> object to convert to.</typeparam>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An instance of <i>RepoDb.DataEntity</i> object.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>(DbDataReader reader) where TEntity : DataEntity
        {
            var entityType = typeof(TEntity);
            var dynamicMethod = new DynamicMethod(StringConstant.DynamicMethod,
                entityType,
                new[] { typeof(DbDataReader) },
                typeof(Assembly).GetTypeInfo().Module,
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(entityType);
            ilGenerator.DeclareLocal(typeof(object));

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, entityType.GetTypeInfo().GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc, 0);

            // Matching the fields
            var fields = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();
            var matchedCount = 0;

            // Iterate the properties
            DataEntityExtension.GetPropertiesFor<TEntity>(Command.Query)
                .Where(property => property.CanWrite)
                .ToList()
                .ForEach(property =>
                {
                    var ordinal = fields.IndexOf(property.GetMappedName());
                    if (ordinal >= 0)
                    {
                        EmitDataReaderToDataEntityMapping<TEntity>(ilGenerator, ordinal, property);
                        matchedCount++;
                    }
                });

            // Throw an error if there are no matching atleast one
            if (matchedCount == 0)
            {
                throw new NoMatchedFieldsException($"There is no matching fields between the result set of the data reader and the type '{typeof(TEntity).FullName}'.");
            }

            // Return the TEntity instance value
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return (DataReaderToDataEntityDelegate<TEntity>)dynamicMethod.CreateDelegate(typeof(DataReaderToDataEntityDelegate<TEntity>));
        }

        private static void EmitDataReaderToDataEntityMapping<TEntity>(ILGenerator ilGenerator, int ordinal, PropertyInfo property) where TEntity : DataEntity
        {
            // Get the property type
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
            var propertyType = underlyingType ?? property.PropertyType;

            // Variables for ending this property emitting
            var endLabel = ilGenerator.DefineLabel();

            // Load the data DataReader instance from argument 0
            ilGenerator.Emit(OpCodes.Ldarg, 0);

            // Load the value base on the ordinal
            ilGenerator.Emit(OpCodes.Ldc_I4, ordinal);
            ilGenerator.Emit(OpCodes.Callvirt, typeof(DbDataReader).GetTypeInfo().GetMethod("get_Item", new[] { typeof(int) }));
            ilGenerator.Emit(OpCodes.Stloc, 1);

            // Load the resulted Value and DBNull.Value for comparisson
            ilGenerator.Emit(OpCodes.Ldloc, 1);
            ilGenerator.Emit(OpCodes.Ldsfld, typeof(DBNull).GetTypeInfo().GetField("Value"));
            ilGenerator.Emit(OpCodes.Ceq);
            ilGenerator.Emit(OpCodes.Brtrue_S, endLabel);

            // Load the DataEntity instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ldloc, 1);

            // Switch which method of Convert are going to used
            var convertMethod = typeof(Convert).GetTypeInfo().GetMethod($"To{propertyType.Name}", new[] { typeof(object) }) ??
                typeof(Convert).GetTypeInfo().GetMethod($"ToString", new[] { typeof(object) });
            ilGenerator.Emit(OpCodes.Call, convertMethod);

            // Parse if it is Guid
            if (propertyType == typeof(Guid))
            {
                ilGenerator.Emit(OpCodes.Call, typeof(Guid).GetTypeInfo().GetMethod("Parse", new[] { typeof(string) }));
            }

            // Check for nullable based on the underlying type
            if (underlyingType != null)
            {
                // Get the type of Nullable<T> object
                var nullableType = typeof(Nullable<>).MakeGenericType(propertyType);

                // Create a new instance of Nullable<T> object
                ilGenerator.Emit(OpCodes.Newobj, nullableType.GetTypeInfo().GetConstructor(new[] { propertyType }));
            }

            // Call the (Property.Set) or (Property = Value)
            ilGenerator.Emit(OpCodes.Call, property.GetSetMethod());

            // End label for DBNull.Value checking
            ilGenerator.MarkLabel(endLabel);
        }

        /// <summary>
        /// Gets a delegate that is used to convert the <i>System.Data.Common.DbDataReader</i> object into <i>RepoDb.DataEntity</i> object.
        /// </summary>
        /// <param name="reader">The <i>System.Data.Common.DbDataReader</i> to be converted.</param>
        /// <returns>An instance of <i>RepoDb.DataEntity</i> object.</returns>
        public static DataReaderToExpandoObjectDelegate GetDataReaderToExpandoObjectDelegate(DbDataReader reader)
        {
            var returnType = typeof(ExpandoObject);
            var dynamicMethod = new DynamicMethod(StringConstant.DynamicMethod,
                returnType,
                new[] { typeof(DbDataReader) },
                typeof(Assembly),
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(returnType);
            ilGenerator.DeclareLocal(typeof(string));
            ilGenerator.DeclareLocal(typeof(object));

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, returnType.GetTypeInfo().GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc, 0);

            // Iterate the fields
            for (var ordinal = 0; ordinal < reader.FieldCount; ordinal++)
            {
                EmitDataReaderToExpandoObjectMapping(ilGenerator, ordinal);
            }

            // Return the TEntity instance value
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return (DataReaderToExpandoObjectDelegate)dynamicMethod.CreateDelegate(typeof(DataReaderToExpandoObjectDelegate));
        }

        private static void EmitDataReaderToExpandoObjectMapping(ILGenerator ilGenerator, int ordinal)
        {
            // Load the data DataReader instance from argument 0
            ilGenerator.Emit(OpCodes.Ldarg, 0);

            // Get the column name by ordinal
            ilGenerator.Emit(OpCodes.Ldc_I4, ordinal);

            ilGenerator.Emit(OpCodes.Callvirt, typeof(DbDataReader).GetTypeInfo().GetMethod("GetName", new[] { typeof(int) }));
            ilGenerator.Emit(OpCodes.Stloc, 1);

            // Call the reader[] method
            ilGenerator.Emit(OpCodes.Ldarg, 0);
            ilGenerator.Emit(OpCodes.Ldc_I4, ordinal);
            ilGenerator.Emit(OpCodes.Callvirt, typeof(DbDataReader).GetTypeInfo().GetMethod("get_Item", new[] { typeof(int) }));
            ilGenerator.Emit(OpCodes.Call, typeof(ObjectConverter).GetTypeInfo().GetMethod("DbNullToNull", new[] { typeof(object) }));
            ilGenerator.Emit(OpCodes.Stloc, 2);

            // Load the object instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ldloc, 1);
            ilGenerator.Emit(OpCodes.Ldloc, 2);

            // Add the value
            ilGenerator.Emit(OpCodes.Call, typeof(IDictionary<string, object>).GetTypeInfo().GetMethod("Add", new[] { typeof(string), typeof(object) }));
        }
    }
}
