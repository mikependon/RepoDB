using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepoDb.Interfaces;
using RepoDb.Attributes;
using RepoDb.UnitTests.CustomObjects;
using System;
using System.Linq;
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
        private class PrivateDbConnection : CustomDbConnection { }

        [TestInitialize]
        public void Initialize()
        {
            Cleanup();
            DbSettingMapper.Add(typeof(PrivateDbConnection), new CustomDbSetting(), true);
        }

        [TestCleanup]
        public void Cleanup()
        {
            IdentityCache.Flush();
            IdentityMapper.Clear();
        }

        #region original

        public enum OriginalEnum { A, B }
        public class OriginalModel
        {
            public OriginalEnum? V { get; set; }
        }

        [TestMethod]
        public void ParameterDbType_DecideOrder_Original()
        {
            using var connection = new PrivateDbConnection();
            var sql = "SELECT @V";

            var command = connection.CreateDbCommandForExecution(sql, new OriginalModel { V = null }, skipCommandArrayParametersCheck: false);
            Assert.AreEqual(DBNull.Value, command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.String, command.Parameters["@V"].DbType);

            command = connection.CreateDbCommandForExecution(sql, new OriginalModel { V = OriginalEnum.B }, skipCommandArrayParametersCheck: false);
            Assert.IsInstanceOfType(command.Parameters["@V"].Value, typeof(OriginalEnum));
            Assert.AreEqual(OriginalEnum.B, (OriginalEnum)command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.String, command.Parameters["@V"].DbType);
        }

        #endregion

        #region attribute

        public enum WithAttributeEnum { A, B }
        public class WithAttributeModel
        {
            [TypeMap(DbType.Single)]
            public WithAttributeEnum? V { get; set; }
        }

        [TestMethod]
        public void ParameterDbType_DecideOrder_Attribute()
        {
            using var connection = new PrivateDbConnection();
            var sql = "SELECT @V";

            var command = connection.CreateDbCommandForExecution(sql, new WithAttributeModel { V = null }, skipCommandArrayParametersCheck: false);
            Assert.AreEqual(DBNull.Value, command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Single, command.Parameters["@V"].DbType);

            command = connection.CreateDbCommandForExecution(sql, new WithAttributeModel { V = WithAttributeEnum.B }, skipCommandArrayParametersCheck: false);
            Assert.IsInstanceOfType(command.Parameters["@V"].Value, typeof(WithAttributeEnum));
            Assert.AreEqual(WithAttributeEnum.B, (WithAttributeEnum)command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Single, command.Parameters["@V"].DbType);
        }

        #endregion

        #region property 

        public enum WithPropertyEnum { A, B }
        public class WithPropertyModel
        {
            public WithPropertyEnum? V { get; set; }
        }

        [TestMethod]
        public void ParameterDbType_DecideOrder_Property()
        {
            TypeMapper.Add<WithPropertyModel>(m => m.V, DbType.Double);

            using var connection = new PrivateDbConnection();
            var sql = "SELECT @V";

            var command = connection.CreateDbCommandForExecution(sql, new WithPropertyModel { V = null }, skipCommandArrayParametersCheck: false);
            Assert.AreEqual(DBNull.Value, command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Double, command.Parameters["@V"].DbType);

            command = connection.CreateDbCommandForExecution(sql, new WithPropertyModel { V = WithPropertyEnum.B }, skipCommandArrayParametersCheck: false);
            Assert.IsInstanceOfType(command.Parameters["@V"].Value, typeof(WithPropertyEnum));
            Assert.AreEqual(WithPropertyEnum.B, (WithPropertyEnum)command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Double, command.Parameters["@V"].DbType);
        }

        #endregion

        #region type

        public enum WithTypeEnum { A, B }
        public class WithTypeModel
        {
            public WithTypeEnum? V { get; set; }
        }

        [TestMethod]
        public void ParameterDbType_DecideOrder_Type()
        {
            TypeMapper.Add<WithTypeEnum>(DbType.Binary);

            using var connection = new PrivateDbConnection();
            var sql = "SELECT @V";

            var command = connection.CreateDbCommandForExecution(sql, new WithTypeModel { V = null }, skipCommandArrayParametersCheck: false);
            Assert.AreEqual(DBNull.Value, command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Binary, command.Parameters["@V"].DbType);

            command = connection.CreateDbCommandForExecution(sql, new WithTypeModel { V = WithTypeEnum.B }, skipCommandArrayParametersCheck: false);
            Assert.IsInstanceOfType(command.Parameters["@V"].Value, typeof(WithTypeEnum));
            Assert.AreEqual(WithTypeEnum.B, (WithTypeEnum)command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Binary, command.Parameters["@V"].DbType);
        }

        #endregion

        #region handler

        public enum WithHandlerEnum { A, B }
        public class WithHandlerModel
        {
            public WithHandlerEnum? V { get; set; }
        }
        public class WithHandlerPropertyHandler : IPropertyHandler<decimal?, WithHandlerEnum?>
        {
            public WithHandlerEnum? Get(decimal? input, ClassProperty property)
                => throw new NotImplementedException();

            public decimal? Set(WithHandlerEnum? input, ClassProperty property)
                => input switch
                {
                    WithHandlerEnum.A => 100,
                    WithHandlerEnum.B => 200,
                    null => null,
                    _ => throw new NotImplementedException()
                };
        }

        [TestMethod]
        public void ParameterDbType_DecideOrder_Handler()
        {
            PropertyHandlerMapper.Add<WithHandlerModel, WithHandlerPropertyHandler>(m => m.V);

            using var connection = new PrivateDbConnection();
            var sql = "SELECT @V";

            var command = connection.CreateDbCommandForExecution(sql, new WithHandlerModel { V = null }, skipCommandArrayParametersCheck: false);
            Assert.AreEqual(DBNull.Value, command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Decimal, command.Parameters["@V"].DbType);

            command = connection.CreateDbCommandForExecution(sql, new WithHandlerModel { V = WithHandlerEnum.B }, skipCommandArrayParametersCheck: false);
            Assert.IsInstanceOfType(command.Parameters["@V"].Value, typeof(decimal));
            Assert.AreEqual(200m, (decimal)command.Parameters["@V"].Value);
            Assert.AreEqual(DbType.Decimal, command.Parameters["@V"].DbType);
        }

        #endregion 
    }
}
