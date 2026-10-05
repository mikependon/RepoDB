#region Copyright Attributions

// Copyright (c) 2026 Michael Camara Pendon.
// Licensed under the Apache License, Version 2.0.
// See the LICENSE file in the project root for full license information.

#endregion

using Moq;
using RepoDb.Interfaces;

namespace RepoDb.Schema.Core.UnitTests.CustomObjects
{
    public static class CustomDbSetting
    {
        /// <summary>
        /// Maps the setting of the <see cref="CustomDbConnection"/>, with the given default schema (<c>null</c> if the database has no default schema).
        /// </summary>
        /// <param name="defaultSchema">The default schema.</param>
        public static void Map(string defaultSchema)
        {
            var setting = new Mock<IDbSetting>();
            setting.SetupGet(s => s.DefaultSchema).Returns(defaultSchema);
            DbSettingMapper.Add<CustomDbConnection>(setting.Object, true);
        }
    }
}
