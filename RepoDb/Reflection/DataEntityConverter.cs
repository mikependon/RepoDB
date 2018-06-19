using RepoDb.Enumerations;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RepoDb.Reflection
{
    /// <summary>
    /// A mapper used to convert the <i>RepoDb.Interfaces.DataEntity</i> to certain objects (Dynamics, DataTable etc).
    /// </summary>
    public static class DataEntityConverter
    {
        /// <summary>
        /// Converts the list of <i>RepoDb.Interfaces.DataEntity</i> object into <i>System.DataTable</i> object.
        /// </summary>
        /// <typeparam name="TEntity">The type of <i>RepoDb.Interfaces.DataEntity</i> object.</typeparam>
        /// <param name="entities">The list of <i>RepoDb.Interfaces.DataEntity</i> object to be converted.</param>
        /// <returns>An instance of <i>System.Data.DataTable</i> object containing the converted values.</returns>
        public static DataTable ToDataTable<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : DataEntity
        {
            var @delegate = DelegateCache.GetDataEntityToDataRowDelegate<TEntity>();
            var table = new DataTable(typeof(TEntity).Name);
            PropertyCache.Get<TEntity>(Command.None)?
                .Where(property => property.CanRead)
                .ToList()
                .ForEach(property =>
                {
                    table.Columns.Add(new DataColumn(property.Name, typeof(string)));
                });
            foreach (var entity in entities)
            {
                var row = @delegate(entity, table);
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
