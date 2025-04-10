// Copyright 2017-2018 Alexander Luzgarev

using System;

namespace MatFileHandler
{
    /// <summary>
    /// Helpers for working with Matlab data types.
    /// </summary>
    internal static class DataTypeExtensions
    {
        /// <summary>
        /// Get data type size in bytes.
        /// </summary>
        /// <param name="type">A data type.</param>
        /// <returns>Size in bytes.</returns>
        public static int Size(this DataType type)
        {
            return type switch
            {
                DataType.MiInt8 or DataType.MiUInt8 => 1,
                DataType.MiInt16 or DataType.MiUInt16 => 2,
                DataType.MiInt32 or DataType.MiUInt32 => 4,
                DataType.MiSingle => 4,
                DataType.MiDouble => 8,
                DataType.MiInt64 or DataType.MiUInt64 => 8,
                DataType.MiMatrix => 0,
                DataType.MiCompressed => 0,
                DataType.MiUtf8 => 1,
                DataType.MiUtf16 => 2,
                DataType.MiUtf32 => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }
}
