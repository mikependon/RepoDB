using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace RepoDb.Reflection
{
    public static class DbDataReaderMapper
    {
        private delegate TEntity MapEntityDataReaderDelegate<TEntity>(DbDataReader dataReader)
            where TEntity : IDataEntity;
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
            var executeAssemblyType = Assembly.GetExecutingAssembly().GetType();
            var dataReaderType = typeof(DbDataReader);
            var entityType = typeof(TEntity);
            var dynamicMethod = new DynamicMethod("DataReaderEntityMapper", entityType, new[] { dataReaderType },
                executeAssemblyType.Module, true);
            var ilGenerator = dynamicMethod.GetILGenerator();

            // Declare IL Variables
            ilGenerator.DeclareLocal(typeof(TEntity));

            // Create instance of the object type
            ilGenerator.Emit(OpCodes.Newobj, entityType.GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc_0);

            // Iterate every properties
            PropertyCache.Get<TEntity>(Command.Select)
                .ToList()
                .ForEach(property =>
                {
                    // Used the first local variable
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Ldarg_0);

                    // Load the value base on the Mapped Name for the first local variable
                    ilGenerator.Emit(OpCodes.Ldstr, property.GetMappedName());

                    // Get the dataReader[..] method as default
                    var dataReaderMethod = dataReaderType.GetMethod("get_Item", new[] { typeof(string) });

                    // Call the data reader method
                    ilGenerator.Emit(OpCodes.Callvirt, dataReaderMethod);

                    // Switch which method of Convert are going to used
                    var converType = typeof(Convert);
                    var convertMethod = converType.GetMethod($"To{property.PropertyType.Name}", new[] { typeof(object) }) ??
                                        converType.GetMethod($"ToString", new[] { typeof(object) });

                    // Call the Convert.Get<Method>
                    ilGenerator.Emit(OpCodes.Call, convertMethod);

                    // Set the entity property
                    ilGenerator.Emit(OpCodes.Callvirt, entityType.GetMethod($"set_{property.Name}", new[] { property.PropertyType }));
                });

            // Return the value
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            // Create a delegate
            return dynamicMethod.CreateDelegate(typeof(MapEntityDataReaderDelegate<TEntity>));
        }
    }
}
