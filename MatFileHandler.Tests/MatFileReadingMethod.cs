namespace MatFileHandler.Tests;

/// <summary>
/// Method of reading .mat files for testing.
/// </summary>
public enum MatFileReadingMethod
{
    /// <summary>
    /// Undefined.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Normal stream (like memory or file stream).
    /// </summary>
    NormalStream,

    /// <summary>
    /// Partial stream (only is capable of reading one byte at a time).
    /// </summary>
    PartialStream,

    /// <summary>
    /// Unaligned stream (what happens if the data don't start at the beginning?).
    /// </summary>
    UnalignedStream,
}
