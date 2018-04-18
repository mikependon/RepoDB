using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection.Emit;

namespace RepoDb.Reflection
{
    public static class DbDataReaderMapper
    {
        private delegate TEntity MapEntityDataReaderDelegate<TEntity>(DbDataReader dataReader) where TEntity : IDataEntity;
        private static Dictionary<Type, Delegate> _caches = new Dictionary<Type, Delegate>();

        public static IEnumerable<TEntity> ToEnumerable<TEntity>(DbDataReader reader)
            where TEntity : IDataEntity
        {
            var @delegate = (MapEntityDataReaderDelegate<TEntity>)Get<TEntity>();
            var list = new List<TEntity>();
            while (reader.Read())
            {
                list.Add(@delegate(reader));
            }
            return list;
        }

        private static Delegate Get<TEntity>()
            where TEntity : IDataEntity
        {
            var type = typeof(TEntity);
            if (!_caches.ContainsKey(type))
            {
                var @delegate = CreateDelegate<TEntity>();
                _caches.Add(type, @delegate);
            }
            return _caches[type];
        }

        private static Delegate CreateDelegate<TEntity>()
            where TEntity : IDataEntity
        {
            var dataReaderType = typeof(DbDataReader);
            var entityType = typeof(TEntity);
            var converType = typeof(Convert);
            var stringTypes = new[] { typeof(string) };
            var objectTypes = new[] { typeof(object) };
            var readerItemMethod = dataReaderType.GetProperty("Item", stringTypes).GetMethod;
            var convertToStringMethod = converType.GetMethod($"ToString", objectTypes);
            var dynamicMethod = new DynamicMethod("EntityMapper", entityType, new[] { dataReaderType });
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            var entity = ilGenerator.DeclareLocal(entityType);

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc, entity);

            // Iterate every properties
            PropertyCache.Get<TEntity>(Command.Select)
                .ToList()
                .ForEach(property =>
                {
                    // Load the TEntity instance
                    ilGenerator.Emit(OpCodes.Ldloc, entity);
                    ilGenerator.Emit(OpCodes.Ldarg, entity);

                    // Load the value base on the Mapped Name for the first local variable
                    ilGenerator.Emit(OpCodes.Ldstr, property.GetMappedName());

                    // Call the reader[] method
                    ilGenerator.Emit(OpCodes.Callvirt, readerItemMethod);

                    // Switch which method of Convert are going to used
                    var convertMethod = converType.GetMethod($"To{property.PropertyType.Name}", objectTypes) ?? convertToStringMethod;

                    // Call the Convert.Get<Method>
                    ilGenerator.Emit(OpCodes.Call, convertMethod);

                    // Set the TEntity property
                    ilGenerator.Emit(OpCodes.Call, property.SetMethod);
                });

            // Return the TEntity instance value
            ilGenerator.Emit(OpCodes.Ldloc, entity);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return dynamicMethod.CreateDelegate(typeof(MapEntityDataReaderDelegate<TEntity>));
        }
    }
}
