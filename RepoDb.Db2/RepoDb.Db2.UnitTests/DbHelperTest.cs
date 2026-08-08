using Microsoft.VisualStudio.TestTools.UnitTesting;
using IBM.Data.Db2;
using RepoDb.DbHelpers;
using RepoDb.Interfaces;
using RepoDb.Resolvers;
using System;
using System.Threading.Tasks;

namespace RepoDb.Db2.UnitTests
{
    [TestClass]
    public class DbHelperTest
    {
        [TestInitialize]
        public void Initialize()
        {
            GlobalConfiguration
                .Setup()
                .UseDb2();
        }

        #region DbTypeResolver

        [TestMethod]
        public void TestDb2DbHelperDefaultConstructorUsesDb2DbTypeNameToClientTypeResolver()
        {
            // Act
            var helper = new Db2DbHelper();

            // Assert
            Assert.IsInstanceOfType(helper.DbTypeResolver, typeof(Db2DbTypeNameToClientTypeResolver));
        }

        [TestMethod]
        public void TestDb2DbHelperConstructorAcceptsACustomDbTypeResolver()
        {
            // Setup
            var resolver = new CustomDbTypeResolver();

            // Act
            var helper = new Db2DbHelper(resolver);

            // Assert
            Assert.AreSame(resolver, helper.DbTypeResolver);
        }

        [TestMethod]
        public void TestDb2DbHelperMapperUsesDb2DbHelper()
        {
            // Setup
            var helper = DbHelperMapper.Get<DB2Connection>();

            // Assert
            Assert.IsInstanceOfType(helper, typeof(Db2DbHelper));
        }

        #endregion

        #region GetScopeIdentity

        [TestMethod]
        public void ThrowExceptionOnDb2DbHelperGetScopeIdentitySinceDb2HasNoSessionWideScopeIdentity()
        {
            // Setup
            var helper = new Db2DbHelper();

            // Act
            Assert.Throws<NotSupportedException>(() =>
                helper.GetScopeIdentity<int>(connection: null));
        }

        [TestMethod]
        public void ThrowExceptionOnDb2DbHelperGetScopeIdentityAsyncSinceDb2HasNoSessionWideScopeIdentity()
        {
            // Setup
            var helper = new Db2DbHelper();

            // Act - the method throws synchronously (not inside the returned Task) since it is not
            // declared with the 'async' keyword; the exception surfaces immediately upon invocation.
            Assert.Throws<NotSupportedException>(() =>
                helper.GetScopeIdentityAsync<int>(connection: null));
        }

        #endregion

        #region DynamicHandler

        [TestMethod]
        public void TestDb2DbHelperDynamicHandlerDoesNothingForTheAfterCreateDbParameterKey()
        {
            // Setup
            var helper = new Db2DbHelper();
            var parameter = new DB2Parameter("Field1", 1);

            // Act - should not throw; the current implementation is a documented no-op placeholder
            helper.DynamicHandler(parameter, "RepoDb.Internal.Compiler.Events[AfterCreateDbParameter]");
        }

        [TestMethod]
        public void TestDb2DbHelperDynamicHandlerDoesNothingForAnUnrecognizedKey()
        {
            // Setup
            var helper = new Db2DbHelper();
            var parameter = new DB2Parameter("Field1", 1);

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
