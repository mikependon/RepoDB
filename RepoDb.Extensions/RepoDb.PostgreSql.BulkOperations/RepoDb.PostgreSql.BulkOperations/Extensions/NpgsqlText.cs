using RepoDb.Enumerations;
using RepoDb.Extensions;
using RepoDb.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace RepoDb
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class NpgsqlConnectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceTableName"></param>
        /// <param name="destinationTableName"></param>
        /// <param name="fields"></param>
        /// <param name="identityField"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetInsertCommand(string sourceTableName,
            string destinationTableName,
            IEnumerable<Field> fields,
            Field identityField,
            IDbSetting dbSetting)
        {
            if (identityField != null)
            {
                fields = fields
                    .Where(field =>
                        !string.Equals(field.Name, identityField.Name, System.StringComparison.OrdinalIgnoreCase));
            }

            var builder = new QueryBuilder();

            builder
                .Clear()
                .Insert()
                .Into()
                .TableNameFrom(destinationTableName, dbSetting)
                .OpenParen()
                .FieldsFrom(fields, dbSetting)
                .CloseParen()
                .Select()
                .FieldsFrom(fields, dbSetting)
                .From()
                .TableNameFrom(sourceTableName, dbSetting)
                .OrderByFrom(GetOderColumnOrderField().AsEnumerable(), dbSetting);

            if (identityField != null)
            {
                builder
                    .Returning()
                    .WriteText(identityField.Name.AsQuoted(true, dbSetting));
            }

            return builder
                .End()
                .ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dbSetting"></param>
        /// <returns></returns>
        private static string GetBinaryInsertPseudoTableName(string tableName,
            IDbSetting dbSetting) =>
            $"_RepoDb_BinaryBulkInsert_{tableName.AsUnquoted(true, dbSetting)}";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static OrderField GetOderColumnOrderField() =>
            new OrderField("__RepoDb_OrderColumn", Order.Ascending);
    }
}
