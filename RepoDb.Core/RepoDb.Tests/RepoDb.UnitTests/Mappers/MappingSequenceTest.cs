using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace RepoDb.UnitTests.Mappers
{
    /*
     * DbType, Decide in the following logical order
     *   1. PropertyHandler.Set method's ReturnType, use Primitive mapping.
     *   2. Property Type, use TypeMapCache.
     *   3. Property value's Type, use TypeMapCache.
     *   4. Property Type, use Primitive mapping.
     *   5. Specialized enum, use Converter.EnumDefaultDatabaseType.
     */

    [TestClass]
    public class MappingSequenceTest
    {
        public class SequenceModel
        {
            public SequenceEnum? V { get; set; }
        }

        public enum SequenceEnum
        {
            A, B
        }

        public class SequencePropertyHandler : IPropertyHandler<decimal?, SequenceEnum?>
        {
            public SequenceEnum? Get(decimal? input, ClassProperty property)
                => throw new NotImplementedException();

            public decimal? Set(SequenceEnum? input, ClassProperty property)
                => input switch
                {
                    SequenceEnum.A => 100,
                    SequenceEnum.B => 200,
                    null => null,
                    _ => throw new NotImplementedException()
                };
        }

        [TestMethod]
        public void ParameterDbType_DecideOrder()
        {
            if (PropertyHandlerMapper.Get<object>(typeof(SequenceEnum)) == null)
            {
                // PropertyHandlerMapper.Add(typeof(SequenceEnum), new SequencePropertyHandler(), true);
            }

            using var connection = new CustomDbConnection();
            var sql = "SELECT @V";

            //var command = connection.CreateDbCommandForExecution(sql, new SequenceModel { V = null }, skipCommandArrayParametersCheck: false);
            var command = connection.CreateDbCommandForExecution(sql, new { V = (SequenceEnum?)null }, skipCommandArrayParametersCheck: false);

            // Assert.AreEqual(DBNull.Value, command.Parameters["V"].Value);
            // Assert.AreEqual(DbType.AnsiString, command.Parameters["V"].DbType);
        }
    }
}
