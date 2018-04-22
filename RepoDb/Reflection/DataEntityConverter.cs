using RepoDb.Enumerations;
using RepoDb.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A mapper used to convert the RepoDb.Interfaces.IDataEntity to certain objects (Dynamics, DataTable etc).
    /// </summary>
    public static class DataEntityConverter
    {
        /// <summary>
        /// Converts the list of RepoDb.Interfaces.IDataEntity object into System.DataTable object.
        /// </summary>
        /// <typeparam name="TEntity">The type of RepoDb.Interfaces.IDataEntity object.</typeparam>
        /// <param name="entities">The list of RepoDb.Interfaces.IDataEntity object to be converted.</param>
        /// <returns>An instance of System.Data.DataTable object containing the converted values.</returns>
        public static DataTable ToDataTable<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : IDataEntity
        {
            var @delegate = DelegateCache.GetDataEntityToDataRowDelegate<TEntity>();
            var table = new DataTable(typeof(TEntity).Name);
            PropertyCache.Get<TEntity>(Command.None)?
                .Where(property => property.CanRead)
                .ToList()
                .ForEach(property =>
                {
                    var underlyingType = Nullable.GetUnderlyingType(property.PropertyType) ??
                        property.PropertyType;
                    table.Columns.Add(new DataColumn(property.Name, typeof(string)));
                });
            foreach(var entity in entities)
            {
                var row = @delegate(entity, table);
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
