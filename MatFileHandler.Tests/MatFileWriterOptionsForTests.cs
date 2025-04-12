namespace MatFileHandler.Tests;

/// <summary>
/// Options to give to MatFileWriter constructor for testing it.
/// </summary>
public enum MatFileWriterOptionsForTests
{
    /// <summary>
    /// Undefined.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// No options.
    /// </summary>
    None,

    /// <summary>
    /// Option to always use compression.
    /// </summary>
    Always,

    /// <summary>
    /// Option to never use compression.
    /// </summary>
    Never,
}
