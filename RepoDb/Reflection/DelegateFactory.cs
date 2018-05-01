using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using RepoDb.Reflection.Delegates;
using System;
using System.Data;
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
        /// <returns>An instance of RepoDb.Interfaces.IDataEntity object.</returns>
        public static DataReaderToDataEntityDelegate<TEntity> GetDataReaderToDataEntityDelegate<TEntity>() where TEntity : IDataEntity
        {
            var entityType = typeof(TEntity);
            var objectConverter = TypeCache.Get(TypeTypes.ObjectConverter);
            var dynamicMethod = new DynamicMethod(Constant.DynamicMethod,
                entityType,
                TypeArrayCache.Get(TypeTypes.DbDataReader),
                TypeCache.Get(TypeTypes.Assembly),
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(entityType);

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc, 0);

            // Iterate every properties
            PropertyCache.Get<TEntity>(Command.Query)
                .Where(property => property.CanWrite)
                .ToList()
                .ForEach(property =>
                {
                    EmitDataReaderToDataEntityMapping<TEntity>(ilGenerator, property);
                });

            // Return the TEntity instance value
            ilGenerator.Emit(OpCodes.Ldloc, 0);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return (DataReaderToDataEntityDelegate<TEntity>)dynamicMethod.CreateDelegate(typeof(DataReaderToDataEntityDelegate<TEntity>));
        }

        private static void EmitDataReaderToDataEntityMapping<TEntity>(ILGenerator ilGenerator, PropertyInfo property) where TEntity : IDataEntity
        {
            // Load the DataEntity instance
            ilGenerator.Emit(OpCodes.Ldloc, 0);

            // Load the data DataReader instance from argument 0
            ilGenerator.Emit(OpCodes.Ldarg, 0);

            // Load the value base on the MappedName
            ilGenerator.Emit(OpCodes.Ldstr, property.GetMappedName());

            // Call the reader[] method
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataReaderStringGetIndexer));

            // Convert the value if it is equals to DbNull
            ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.ObjectConverterDbNullToNull));

            // Get the property type
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
            var propertyType = underlyingType ?? property.PropertyType;

            // Switch which method of Convert are going to used
            var convertMethod = MethodInfoCache.GetConvertTo(propertyType) ?? MethodInfoCache.Get(MethodInfoTypes.ConvertToString);

            // Call the Convert.Get<Method>
            ilGenerator.Emit(OpCodes.Call, convertMethod);

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
        }

        /// <summary>
        /// Gets a delegate that is used to convert the RepoDb.Interfaces.IDataEntity object to System.Data.DataRow object.
        /// </summary>
        /// <typeparam name="TEntity">The RepoDb.Interfaces.IDataEntity object to convert to.</typeparam>
        /// <returns>An instance of System.Data.DataRow object containing the converted values.</returns>
        public static DataEntityToDataRowDelegate<TEntity> GetDataEntityToDataRowDelegate<TEntity>() where TEntity : IDataEntity
        {
            var dataRowType = TypeCache.Get(TypeTypes.DataRow);
            var entityType = typeof(TEntity);
            var dynamicMethod = new DynamicMethod(Constant.DynamicMethod,
                dataRowType,
                new Type[] { entityType, TypeCache.Get(TypeTypes.DataTable) },
                TypeCache.Get(TypeTypes.Assembly),
                true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(dataRowType);
            ilGenerator.DeclareLocal(entityType);
            ilGenerator.DeclareLocal(TypeCache.Get(TypeTypes.Object));

            // Load the DataTable
            ilGenerator.Emit(OpCodes.Ldarg, 1);

            // Invoke the DataTable.NewRow(), and set to location 0
            ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.DataTableNewRow));
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
            // TODO: Fix the Object Null Reference Exception

            // Load the DataEntity instance
            ilGenerator.Emit(OpCodes.Ldarg, 0);

            // Get the DataEntity.GetType()
            ilGenerator.Emit(OpCodes.Ldloc, 1);

            // Get the Type.GetProperty()
            ilGenerator.Emit(OpCodes.Ldstr, property.GetMappedName());
            ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.TypeGetProperty));

            // Call the PropertyInfo.GetValue(object)
            ilGenerator.Emit(OpCodes.Call, MethodInfoCache.Get(MethodInfoTypes.PropertyInfoGetValue));
            ilGenerator.Emit(OpCodes.Stloc, 2);

            // Load the DataRow from the location 0
            ilGenerator.Emit(OpCodes.Ldloc, 0);

            // Load the value of property.GetMappedName() stands as the columnname for DataRow indexer
            ilGenerator.Emit(OpCodes.Ldstr, property.GetMappedName());

            // Load the value of location 2 for the DataRow indexer value
            ilGenerator.Emit(OpCodes.Ldloc, 2);

            // Call the DataRow[..] set indexer
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataRowStringSetIndexer));
        }
    }
}
