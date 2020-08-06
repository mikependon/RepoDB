using RepoDb.Enumerations;
using RepoDb.Exceptions;
using RepoDb.Reflection.Delegates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A static factory class used to create a delegate a custom delegate.
    /// </summary>
    public static class DelegateFactory
    {
        /// <summary>
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <typeparam name="TEntity">The data entity object to convert to.</typeparam>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An instance of data entity object.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>(DbDataReader reader)
            where TEntity : class
        {
            var entityType = typeof(TEntity);
            var dynamicMethod = new DynamicMethod(StringConstant.DynamicMethod,
                entityType,
                new[] { typeof(DbDataReader) },
                typeof(Assembly).Module,
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();
            var fieldDefinitions = FieldDefinitionCache.Get<TEntity>();

            // Declare IL Variables
            ilGenerator.DeclareLocal(entityType);
            ilGenerator.DeclareLocal(typeof(object));

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc, 0);

            // Matching the fields
            var fields = Enumerable.Range(0, reader.FieldCount)
                .Select(reader.GetName)
                .Select(n => n.ToLowerInvariant())
                .ToList();
            var matchedCount = 0;

            // Iterate the properties
            PropertyCache.Get<TEntity>()
                .Where(property => property.PropertyInfo.CanWrite)
                .ToList()
                .ForEach(property =>
                {
                    var mappedName = property.GetMappedName().ToLowerInvariant();
                    var fieldDefinition = fieldDefinitions?.FirstOrDefault(fd => fd.Name.ToLowerInvariant() == mappedName);
                    var ordinal = fields.IndexOf(mappedName);
                    if (ordinal >= 0)
                    {
                        EmitDataReaderToDataEntityMapping<TEntity>(ilGenerator, ordinal, property, fieldDefinition);
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

        private static void EmitDataReaderToDataEntityMapping<TEntity>(ILGenerator ilGenerator, int ordinal, ClassProperty property, FieldDefinition fieldDefinition)
            where TEntity : class
        {
            // Get the property type
            var isNullable = fieldDefinition == null || fieldDefinition?.IsNullable == true;
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyInfo.PropertyType);
            var propertyType = underlyingType ?? property.PropertyInfo.PropertyType;

            // Variables for ending this property emitting
            var endLabel = ilGenerator.DefineLabel();

            // Load the data DataReader instance from argument 0
            ilGenerator.Emit(OpCodes.Ldarg, 0);

            // Load the value base on the ordinal
            ilGenerator.Emit(OpCodes.Ldc_I4, ordinal);
            ilGenerator.Emit(OpCodes.Callvirt, typeof(DbDataReader).GetMethod("get_Item", new[] { typeof(int) }));
            ilGenerator.Emit(OpCodes.Stloc, 1);

            // Check if nullable in the actual definition
            if (isNullable == true)
            {
                // Load the resulted Value and DBNull.Value for comparisson
                ilGenerator.Emit(OpCodes.Ldloc, 1);
                ilGenerator.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value"));
                ilGenerator.Emit(OpCodes.Ceq);
                ilGenerator.Emit(OpCodes.Brtrue_S, endLabel);
            }

            // Load the DataEntity instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ldloc, 1);

            // Switch which method of Convert are going to used
            if (propertyType != typeof(byte[]))
            {
                // Get the proper convert method
                var convertMethod = typeof(Convert).GetMethod(string.Concat("To", propertyType.Name), new[] { typeof(object) }) ??
                    typeof(Convert).GetMethod("ToString", new[] { typeof(object) });

                // Convert the value
                ilGenerator.Emit(OpCodes.Call, convertMethod);

                // Parse if it is Guid
                var parseMethod = (MethodInfo)null;

                // Switch to the correct parser method
                if (propertyType == typeof(Guid) ||
                    propertyType == typeof(DateTimeOffset) ||
                    propertyType == typeof(TimeSpan))
                {
                    parseMethod = propertyType.GetMethod("Parse", new[] { typeof(string) });
                }

                // Do the parse if set
                if (parseMethod != null)
                {
                    ilGenerator.Emit(OpCodes.Call, parseMethod);
                }
            }

            // Check for nullable based on the underlying type
            if (underlyingType != null)
            {
                // Get the type of Nullable<T> object
                var nullableType = typeof(Nullable<>).MakeGenericType(propertyType);

                // Create a new instance of Nullable<T> object
                ilGenerator.Emit(OpCodes.Newobj, nullableType.GetConstructor(new[] { propertyType }));
            }

            // Call the (Property.Set) or (Property = Value)
            ilGenerator.Emit(OpCodes.Call, property.PropertyInfo.GetSetMethod());

            // End label for DBNull.Value checking
            ilGenerator.MarkLabel(endLabel);
        }

        /// <summary>
        /// Gets a delegate that is used to convert the <see cref="DbDataReader"/> object into data entity object.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> to be converted.</param>
        /// <returns>An instance of data entity object.</returns>
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
            ilGenerator.Emit(OpCodes.Newobj, returnType.GetConstructor(Type.EmptyTypes));
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

            ilGenerator.Emit(OpCodes.Callvirt, typeof(DbDataReader).GetMethod("GetName", new[] { typeof(int) }));
            ilGenerator.Emit(OpCodes.Stloc, 1);

            // Call the reader[] method
            ilGenerator.Emit(OpCodes.Ldarg, 0);
            ilGenerator.Emit(OpCodes.Ldc_I4, ordinal);
            ilGenerator.Emit(OpCodes.Callvirt, typeof(DbDataReader).GetMethod("get_Item", new[] { typeof(int) }));
            ilGenerator.Emit(OpCodes.Call, typeof(ObjectConverter).GetMethod("DbNullToNull", new[] { typeof(object) }));
            ilGenerator.Emit(OpCodes.Stloc, 2);

            // Load the object instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ldloc, 1);
            ilGenerator.Emit(OpCodes.Ldloc, 2);

            // Add the value
            ilGenerator.Emit(OpCodes.Call, typeof(IDictionary<string, object>).GetMethod("Add", new[] { typeof(string), typeof(object) }));
        }
    }
}
