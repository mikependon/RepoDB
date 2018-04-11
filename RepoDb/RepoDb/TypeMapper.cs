using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using RepoDb.Interfaces;
using RepoDb.Exceptions;

namespace RepoDb
{
    public static class TypeMapper
    {
        private static readonly IList<ITypeMap> _typeMaps = new List<ITypeMap>();

        static TypeMapper()
        {
            new List<ITypeMap>();
        }

        public static IEnumerable<ITypeMap> TypeMaps => _typeMaps;

        public static void AddMap(Type type, DbType dbType)
        {
            AddMap(new TypeMap(type, dbType));
        }

        public static void AddMap(ITypeMap typeMap)
        {
            var target = Get(typeMap.Type);
            if (target != null)
            {
                throw new DuplicateTypeMapException(target.Type);
            }
            _typeMaps.Add(typeMap);
        }

        public static ITypeMap Get(Type type)
        {
            return _typeMaps.FirstOrDefault(t => t.Type == type);
        }

        public static ITypeMap Get<T>()
        {
            return Get(typeof(T));
        }
    }
}
