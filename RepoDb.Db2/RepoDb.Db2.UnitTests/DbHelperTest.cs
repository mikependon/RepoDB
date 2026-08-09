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

        // NOTE: unlike Oracle (which has no session-wide "last identity" construct at all - its
        // OracleDbHelper.GetScopeIdentity/Async unconditionally throw NotSupportedException, and
        // this test file used to assert the same thing here, having been originally copied from
        // OracleDbHelper's test suite), Db2 *does* have one: IDENTITY_VAL_LOCAL(). Db2DbHelper's
        // GetScopeIdentity/GetScopeIdentityAsync are real, working implementations that delegate to
        // "SELECT IDENTITY_VAL_LOCAL() FROM SYSIBM.SYSDUMMY1" via connection.ExecuteScalar(Async).
        // Neither method special-cases a null connection, so calling either with one still fails -
        // just for the ordinary "nothing to execute against" reason
        // (NullReferenceException, from IDbConnection.EnsureOpen() accessing connection.State),
        // not because the feature itself is unsupported. That's exactly what these two tests now
        // confirm, in place of the stale NotSupportedException expectation.

        [TestMethod]
        public void TestDb2DbHelperGetScopeIdentityThrowsNullReferenceExceptionForANullConnection()
        {
            // Setup
            var helper = new Db2DbHelper();

            // Act - the method throws synchronously (not inside a Task) since it is not declared
            // with the 'async' keyword; the exception surfaces immediately upon invocation.
            Assert.Throws<NullReferenceException>(() =>
                helper.GetScopeIdentity<int>(connection: null));
        }

        [TestMethod]
        public async Task TestDb2DbHelperGetScopeIdentityAsyncThrowsNullReferenceExceptionForANullConnection()
        {
            // Setup
            var helper = new Db2DbHelper();

            // Act - unlike the sync overload, the underlying implementation is a genuine 'async'
            // method, so the NullReferenceException is captured into the returned Task's fault
            // state rather than thrown synchronously at the point of invocation; it only surfaces
            // once the task is awaited.
            await Assert.ThrowsAsync<NullReferenceException>(() =>
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
