namespace MatFileHandler.Tests;

/// <summary>
/// Method of writing .mat files for testing.
/// </summary>
public enum MatFileWritingMethod
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
    /// A stream that cannot be seeked (like a deflate stream).
    /// </summary>
    UnseekableStream,

    /// <summary>
    /// Unaligned stream (what happens if the data don't start at the beginning?).
    /// </summary>
    UnalignedStream,
}
