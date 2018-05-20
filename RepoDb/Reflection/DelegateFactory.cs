using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection.Delegates;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A class used to create a Delegate based on the needs.
    /// </summary>
    public static class DelegateFactory
    {
        /// <summary>
        /// Gets a delegate that is used to convert the System.Data.Common.DbDataReader object to RepoDb.Interfaces.IDataEntity object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity object to convert to.</typeparam>
        /// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        /// <returns>An instance of RepoDb.Interfaces.IDataEntity object.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>(DbDataReader reader) where TEntity : IDataEntity
        {
            var entityType = typeof(TEntity);
            var dynamicMethod = new DynamicMethod(Constant.DynamicMethod,
                entityType,
                TypeArrayCache.Get(TypeTypes.DbDataReader),
                TypeCache.Get(TypeTypes.Assembly),
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(entityType);
            ilGenerator.DeclareLocal(TypeCache.Get(TypeTypes.Object));

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc, 0);

            // Iterate the properties
            PropertyCache.Get<TEntity>(Command.Query)
                .Where(property => property.CanWrite)
                .ToList()
                .ForEach(property =>
                {
                    var ordinal = reader.GetOrdinal(property.GetMappedName());
                    if (ordinal >= 0)
                    {
                        EmitDataReaderToDataEntityMapping<TEntity>(ilGenerator, ordinal, property);
                    }
                });

            // Return the TEntity instance value
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return (DataReaderToDataEntityDelegate<TEntity>)dynamicMethod.CreateDelegate(typeof(DataReaderToDataEntityDelegate<TEntity>));
        }

        private static void EmitDataReaderToDataEntityMapping<TEntity>(ILGenerator ilGenerator, int ordinal, PropertyInfo property) where TEntity : IDataEntity
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
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataReaderIntGetIndexer));
            ilGenerator.Emit(OpCodes.Stloc, 1);
            //ilGenerator.Emit(OpCodes.Ldc_I4, ordinal);
            //var method = typeof(DbDataReader).GetMethod($"Get{propertyType.Name}", new[] { typeof(int) });
            //ilGenerator.Emit(OpCodes.Callvirt, method);
            //ilGenerator.Emit(OpCodes.Stloc, 1);

            // Load the resulted Value and DBNull.Value for comparisson
            ilGenerator.Emit(OpCodes.Ldloc, 1);
            ilGenerator.Emit(OpCodes.Ldsfld, FieldInfoCache.Get(FieldInfoTypes.DbNullValue));

            // Check for DBNull.Value True equality
            ilGenerator.Emit(OpCodes.Ceq);
            ilGenerator.Emit(OpCodes.Brtrue_S, endLabel);

            // Load the DataEntity instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ldloc, 1);

            // Switch which method of Convert are going to used
            var convertMethod = MethodInfoCache.GetConvertTo(propertyType) ?? MethodInfoCache.Get(MethodInfoTypes.ConvertToString);
            ilGenerator.Emit(OpCodes.Call, convertMethod);

            // Parse if it is Guid
            if (propertyType == typeof(Guid))
            {
                // Create a new instance of Nullable<T> object
                //ilGenerator.Emit(OpCodes.Newobj, ConstructorInfoCache.Get(TypeCache.Get(TypeTypes.Guid), TypeArrayCache.Get(TypeTypes.String)));
                //ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.GuidParse));
                ilGenerator.Emit(OpCodes.Ldstr, "D");
                ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.GuidParseExact));
            }

            // Check for nullable based on the underlying type
            if (underlyingType != null)
            {
                // Get the type of Nullable<T> object
                var nullableType = TypeCache.Get(TypeTypes.NullableGeneric).MakeGenericType(propertyType);

                // Create a new instance of Nullable<T> object
                ilGenerator.Emit(OpCodes.Newobj, ConstructorInfoCache.Get(nullableType, propertyType));
            }

            // Call the (Property.Set) or (Property = Value)
            ilGenerator.Emit(OpCodes.Call, property.SetMethod);

            // End label for DBNull.Value checking
            ilGenerator.MarkLabel(endLabel);
        }

        /// <summary>
        /// Gets a delegate that is used to convert the System.Data.Common.DbDataReader object to RepoDb.Interfaces.IDataEntity object.
        /// </summary>
        /// <param name="reader">The System.Data.Common.DbDataReader to be converted.</param>
        /// <returns>An instance of RepoDb.Interfaces.IDataEntity object.</returns>
        public static DataReaderToExpandoObjectDelegate GetDataReaderToExpandoObjectDelegate(DbDataReader reader)
        {
            var returnType = TypeCache.Get(TypeTypes.ExpandoObject);
            var dynamicMethod = new DynamicMethod(Constant.DynamicMethod,
                returnType,
                TypeArrayCache.Get(TypeTypes.DbDataReader),
                TypeCache.Get(TypeTypes.Assembly),
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(returnType);
            ilGenerator.DeclareLocal(TypeCache.Get(TypeTypes.String));
            ilGenerator.DeclareLocal(TypeCache.Get(TypeTypes.Object));

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
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataReaderGetName));
            ilGenerator.Emit(OpCodes.Stloc, 1);

            // Call the reader[] method
            ilGenerator.Emit(OpCodes.Ldarg, 0);
            ilGenerator.Emit(OpCodes.Ldc_I4, ordinal);
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataReaderIntGetIndexer));
            ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.ObjectConverterDbNullToNull));
            ilGenerator.Emit(OpCodes.Stloc, 2);

            // Load the object instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ldloc, 1);
            ilGenerator.Emit(OpCodes.Ldloc, 2);

            // Add the value
            ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.DictionaryStringObjectAdd));
        }

        /// <summary>
        /// Gets a delegate that is used to convert the RepoDb.Interfaces.IDataEntity object to System.Data.DataRow object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity object to convert to.</typeparam>
        /// <returns>An instance of System.Data.DataRow object containing the converted values.</returns>
        public static DataEntityToDataRowDelegate<TEntity> GetDataEntityToDataRowDelegate<TEntity>() where TEntity : IDataEntity
        {
            var returnType = TypeCache.Get(TypeTypes.DataRow);
            var entityType = typeof(TEntity);
            var dynamicMethod = new DynamicMethod(Constant.DynamicMethod,
                returnType,
                new Type[] { entityType, TypeCache.Get(TypeTypes.DataTable) },
                TypeCache.Get(TypeTypes.Assembly),
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(returnType);
            ilGenerator.DeclareLocal(TypeCache.Get(TypeTypes.Type));

            // Get the DataEntity type
            ilGenerator.Emit(OpCodes.Ldarg, 0);
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.TypeGetType));
            ilGenerator.Emit(OpCodes.Stloc, 1);

            // Create a new DataRow
            ilGenerator.Emit(OpCodes.Ldarg, 1);
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataTableNewRow));
            ilGenerator.Emit(OpCodes.Stloc, 0);

            // Iterate every properties
            PropertyCache.Get<TEntity>(Command.None)?
                .Where(property => property.CanRead)
                .ToList()
                .ForEach(property =>
                {
                    EmitDataEntityToDataRowMapping<TEntity>(ilGenerator, property);
                });

            // Load and return the DataRow instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return (DataEntityToDataRowDelegate<TEntity>)dynamicMethod.CreateDelegate(typeof(DataEntityToDataRowDelegate<TEntity>));
        }

        private static void EmitDataEntityToDataRowMapping<TEntity>(ILGenerator ilGenerator, PropertyInfo property) where TEntity : IDataEntity
        {
            var propertyName = property.GetMappedName();

            // Load the DataRow object
            ilGenerator.Emit(OpCodes.Ldloc, 0);

            // Load the target property for DataRow set indexer
            ilGenerator.Emit(OpCodes.Ldstr, propertyName);

            // Get the target PropertyInfo
            ilGenerator.Emit(OpCodes.Ldloc, 1);
            ilGenerator.Emit(OpCodes.Ldstr, propertyName);
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.TypeGetProperty));

            // Load the DataEntity object
            ilGenerator.Emit(OpCodes.Ldarg, 0);

            // Call the PropertyInfo.GetValue method
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.PropertyInfoGetValue));

            // Call the DataRow[..] set indexer
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataRowStringSetIndexer));
        }
    }
}
