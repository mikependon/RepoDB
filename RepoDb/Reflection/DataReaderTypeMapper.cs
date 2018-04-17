using RepoDb.Extensions;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Emit;

namespace RepoDb.Reflection
{
    public class DbDataReaderMapper<T> where T : IDataEntity
    {
        // https://improve.dk/mapping-datareader-to-objects-using-reflection-emit/
        private delegate T MapEntityDelegage(DbDataReader dr);
        private static Dictionary<Type, Delegate> _caches = new Dictionary<Type, Delegate>();
        private readonly DbDataReader _dataReader;

        public DbDataReaderMapper(DbDataReader dataReader)
        {
            _dataReader = dataReader;
        }

        public IEnumerable<T> AsEnumerable()
        {
            if (!_caches.ContainsKey(typeof(T)))
            {
                var args = new []{ typeof(DbDataReader) };
                var dynamicMethod = new DynamicMethod("DataReaderMap", typeof(T), args, Assembly.GetExecutingAssembly().GetType().Module);
                var ilGenerator = dynamicMethod.GetILGenerator();

                ilGenerator.DeclareLocal(typeof(T));
                ilGenerator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
                ilGenerator.Emit(OpCodes.Stloc_0);

                foreach (PropertyInfo pi in typeof(T).GetProperties())
                {
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    ilGenerator.Emit(OpCodes.Ldstr, pi.GetMappedName());
                    ilGenerator.Emit(OpCodes.Callvirt, typeof(DbDataReader).GetMethod("get_Item", new Type[] { typeof(string) }));
                    switch (pi.PropertyType.Name)
                    {
                        case "Int16":
                            ilGenerator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToInt16", new Type[] { typeof(object) }));
                            break;
                        case "Int32":
                            ilGenerator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToInt32", new Type[] { typeof(object) }));
                            break;
                        case "Int64":
                            ilGenerator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToInt64", new Type[] { typeof(object) }));
                            break;
                        case "Boolean":
                            ilGenerator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToBoolean", new Type[] { typeof(object) }));
                            break;
                        case "String":
                            ilGenerator.Emit(OpCodes.Callvirt, typeof(string).GetMethod("ToString", new Type[] { }));
                            break;
                        case "DateTime":
                            ilGenerator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToDateTime", new Type[] { typeof(object) }));
                            break;
                        case "Decimal":
                            ilGenerator.Emit(OpCodes.Call, typeof(Convert).GetMethod("ToDecimal", new Type[] { typeof(object) }));
                            break;
                        default:
                            continue;
                    }
                    ilGenerator.Emit(OpCodes.Callvirt, typeof(T).GetMethod("set_" + pi.Name, new Type[] { pi.PropertyType }));
                }
                ilGenerator.Emit(OpCodes.Ldloc_0);
                ilGenerator.Emit(OpCodes.Ret);
                _caches.Add(typeof(T), dynamicMethod.CreateDelegate(typeof(MapEntityDelegage)));
            }

            var @delegate = (MapEntityDelegage)_caches[typeof(T)];
            var list = new List<T>();
            while (_dataReader.Read())
            {
                list.Add(@delegate(_dataReader));
            }
            return list;
        }
    }
}
