using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oracle.ManagedDataAccess.Client;
using RepoDb.DbHelpers;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Threading.Tasks;

namespace RepoDb.Oracle.UnitTests
{
    [TestClass]
    public class DbHelperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseOracle();
        }

        #region DbTypeResolver

        [TestMethod]
        public void TestOracleDbHelperDefaultConstructorUsesOracleDbTypeNameToClientTypeResolver()
        {
            // Act
            var helper = new OracleDbHelper();

            // Assert
            Assert.IsInstanceOfType(helper.DbTypeResolver, typeof(OracleDbTypeNameToClientTypeResolver));
        }

        [TestMethod]
        public void TestOracleDbHelperConstructorAcceptsACustomDbTypeResolver()
        {
            // Setup
            var resolver = new CustomDbTypeResolver();

            // Act
            var helper = new OracleDbHelper(resolver);

            // Assert
            Assert.AreSame(resolver, helper.DbTypeResolver);
        }

        [TestMethod]
        public void TestOracleDbHelperMapperUsesOracleDbHelper()
        {
            // Setup
            var helper = DbHelperMapper.Get<OracleConnection>();

            // Assert
            Assert.IsInstanceOfType(helper, typeof(OracleDbHelper));
        }

        #endregion

        #region GetScopeIdentity

        [TestMethod]
        public void ThrowExceptionOnOracleDbHelperGetScopeIdentitySinceOracleHasNoSessionWideScopeIdentity()
        {
            // Setup
            var helper = new OracleDbHelper();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                helper.GetScopeIdentity<int>(connection: null));
        }

        [TestMethod]
        public void ThrowExceptionOnOracleDbHelperGetScopeIdentityAsyncSinceOracleHasNoSessionWideScopeIdentity()
        {
            // Setup
            var helper = new OracleDbHelper();

            // Act - the method throws synchronously (not inside the returned Task) since it is not
            // declared with the 'async' keyword; the exception surfaces immediately upon invocation.
            Assert.Throws<NotSupportedException>(() =>
                helper.GetScopeIdentityAsync<int>(connection: null));
        }

        #endregion

        #region DynamicHandler

        [TestMethod]
        public void TestOracleDbHelperDynamicHandlerDoesNothingForTheAfterCreateDbParameterKey()
        {
            // Setup
            var helper = new OracleDbHelper();
            var parameter = new OracleParameter("Field1", 1);

            // Act - should not throw; the current implementation is a documented no-op placeholder
            helper.DynamicHandler(parameter, "RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]");
        }

        [TestMethod]
        public void TestOracleDbHelperDynamicHandlerDoesNothingForAnUnrecognizedKey()
        {
            // Setup
            var helper = new OracleDbHelper();
            var parameter = new OracleParameter("Field1", 1);

            // Act - unrecognized keys are silently ignored (no cast, no invocation)
            helper.DynamicHandler(parameter, "RepoDb.Internal.Compiler.Events[SomeOtherEvent]");
        }

        #endregion

        #region Helper Classes

        private class CustomDbTypeResolver : IResolver<string, Type>
        {
            public Type Resolve(string input) => typeof(object);
        }

        #endregion
    }
}
