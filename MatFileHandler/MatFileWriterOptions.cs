namespace MatFileHandler
{
    /// <summary>
    /// Options for writing .mat files.
    /// </summary>
    public struct MatFileWriterOptions
    {
        /// <summary>
        /// Gets default options.
        /// </summary>
        public static MatFileWriterOptions Default => new()
        {
            UseCompression = CompressionUsage.Always,
        };

        /// <summary>
        /// Gets or sets a value indicating whether to compress all variables when writing the file.
        /// </summary>
        public CompressionUsage UseCompression { get; set; }
    }
}
