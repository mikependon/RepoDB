namespace RepoDb.Enumerations
{
    /// <summary>
    /// 
    /// </summary>
    public enum EnumHandling
    {
        /// <summary>
        /// Throw an error when encountering non defined enum values. For enums decorated with a <see cref="FlagsAttribute"/> no value check is performed.
        /// </summary>
        ThrowError = 0,
        /// <summary>
        /// Use the default (0) value of the enum when encountering non defined enum values, For enums decorated with a <see cref="FlagsAttribute"/> no value check is performed.
        /// </summary>
        UseDefault = 1,
        /// <summary>
        /// Assumes all matched strings and integer values are valid.
        /// </summary>
        Cast = 2
    }
}
