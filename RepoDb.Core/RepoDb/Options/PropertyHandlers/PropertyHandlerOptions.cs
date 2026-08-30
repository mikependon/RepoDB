#region Copyright Attributions

// Copyright (c) 2022 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

namespace RepoDb.Options
{
    /// <summary>
    /// An option class that is containing the optional values during the hydration process or when pushing the property values towards the database.
    /// </summary>
    public abstract class PropertyHandlerOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        internal PropertyHandlerOptions(ClassProperty property)
        {
            ClassProperty = property;
        }

        #region Properties

        /// <summary>
        /// Gets the instance of <see cref="RepoDb.ClassProperty"/> that is being handled.
        /// </summary>
        public ClassProperty ClassProperty { get; }

        #endregion
    }
}
