using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
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
        public static DataReaderToEntityMapperDelegate<TEntity> GetDataReaderToEntity<TEntity>() where TEntity : IDataEntity
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

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc_0);

            // Iterate every properties
            PropertyCache.Get<TEntity>(Command.Select)
                .Where(property => property.CanWrite)
                .ToList()
                .ForEach(property =>
                {
                    EmitDataReaderToPropertyMapping(ilGenerator, property);
                });

            // Return the TEntity instance value
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return (DataReaderToEntityMapperDelegate<TEntity>)dynamicMethod.CreateDelegate(typeof(DataReaderToEntityMapperDelegate<TEntity>));
        }

        // Private Methods

        private static void EmitDataReaderToPropertyMapping(ILGenerator ilGenerator, PropertyInfo property)
        {
            // Load the TEntity instance
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Load the value base on the MappedName
            ilGenerator.Emit(OpCodes.Ldstr, property.GetMappedName());

            // Call the reader[] method
            ilGenerator.Emit(OpCodes.Callvirt, MethodInfoCache.Get(MethodInfoTypes.DataReaderStringIndexer));

            // Get the property type
            var underlyingType = Nullable.GetUnderlyingType(property.PropertyType);
            var propertyType = underlyingType ?? property.PropertyType;

            // Switch which method of Convert are going to used
            var convertMethod = MethodInfoCache.GetConvertTo(propertyType) ??
                MethodInfoCache.Get(MethodInfoTypes.ConvertToString);

            // Call the Convert.Get<Method>
            ilGenerator.Emit(OpCodes.Call, convertMethod);

            // Check for nullable based on the underlying type
            if (underlyingType != null)
            {
                // Get the type of Nullable<T> object
                var nullableType = TypeCache.Get(TypeTypes.NullableGeneric).MakeGenericType(underlyingType);
                var constructor = ConstructorInfoCache.Get(nullableType, underlyingType);

                // Create an instance of Nullable<T> object
                ilGenerator.Emit(OpCodes.Newobj, constructor);
            }
            
            // Call the (Property.Set) or (Property = Value)
            ilGenerator.Emit(OpCodes.Call, property.SetMethod);
        }
    }
}
