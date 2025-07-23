#if !NET
using System;
using System.Buffers;
using System.IO;

namespace MatFileHandler
{
    /// <summary>
    /// Polyfills for methods that do not exist on lower targets.
    /// </summary>
    internal static class PolyfillExtensions
    {
        public static void ReadExactly(this Stream stream, Span<byte> buffer)
        {
            var array = ArrayPool<byte>.Shared.Rent(buffer.Length);

            stream.ReadExactly(array, 0, buffer.Length);

            array.AsSpan(0, buffer.Length).CopyTo(buffer);

            ArrayPool<byte>.Shared.Return(array);
        }

        public static void ReadExactly(this Stream stream, byte[] buffer, int offset, int count)
        {
            var totalRead = 0;

            while (totalRead < count)
            {
                var read = stream.Read(buffer, offset + totalRead, count - totalRead);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                totalRead += read;
            }
        }

        public static void Write(this BinaryWriter writer, ReadOnlySpan<byte> buffer)
        {
            var array = ArrayPool<byte>.Shared.Rent(buffer.Length);

            buffer.CopyTo(array);

            writer.Write(array, 0, buffer.Length);

            ArrayPool<byte>.Shared.Return(array);
        }
    }
}
#endif