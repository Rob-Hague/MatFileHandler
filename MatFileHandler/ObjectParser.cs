namespace MatFileHandler;

/// <summary>
/// Parser for object data.
/// </summary>
internal static class ObjectParser
{
    /// <summary>
    /// Parse object data.
    /// </summary>
    /// <param name="uintArray">Opaque link array.</param>
    /// <param name="subsystemData">Current subsystem data.</param>
    /// <returns>Parsed object.</returns>
    public static IArray ParseObject(MatNumericalArrayOf<uint> uintArray, SubsystemData subsystemData)
    {
        var (dimensions, indexToObjectId, classIndex) = DataElementReader.ParseOpaqueData(uintArray.Data);
        return new OpaqueLink(
            uintArray.Name,
            string.Empty,
            string.Empty,
            dimensions,
            uintArray,
            indexToObjectId,
            classIndex,
            subsystemData);
    }
}
