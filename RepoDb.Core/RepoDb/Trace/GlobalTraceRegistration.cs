using System.Collections.Generic;
using RepoDb.Extensions;
using RepoDb.Interfaces;

namespace RepoDb
{
    /// <summary>
    /// A class that is being used to register a default trace object (see <see cref="ITrace"/>) to be used globally by the library.
    /// </summary>
    public static class GlobalTraceRegistration
    {
        #region Privates

        private readonly static IList<ITrace> _defaultTracers;

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        static GlobalTraceRegistration()
        {
            _defaultTracers = new List<ITrace>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers a trace object to be part of the default tracer of the library.
        /// </summary>
        /// <param name="trace">The instance of <see cref="ITrace"/> object to be assigned as the default tracer.</param>
        public static void Register(ITrace trace) =>
            _defaultTracers.Add(trace);

        /// <summary>
        /// Gets the current default tracer that has been assigned globally.
        /// </summary>
        /// <returns>The list of the default <see cref="ITrace"/> objects.</returns>
        public static IList<ITrace> GetTracers() =>
            _defaultTracers.AsList();

        #endregion
    }
}
