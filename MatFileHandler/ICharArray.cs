// Copyright 2017-2018 Alexander Luzgarev

namespace MatFileHandler
{
    /// <summary>
    /// Matlab's character array.
    /// </summary>
    public interface ICharArray : IArrayOf<char>
    {
        /// <summary>
        /// Gets the contained string.
        /// </summary>
#pragma warning disable CA1716, CA1720
        string String { get; }
#pragma warning restore CA1716, CA1720
    }
}
