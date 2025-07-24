using System;
using System.Buffers.Binary;
using System.IO;
using System.IO.Compression;
using Xunit;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests for the <see cref="ZLibStream"/> class.
    /// This is only useful while a .NET Framework target exists,
    /// otherwise it is just testing the .NET API.
    /// </summary>
    public class ZLibStreamTests
    {
        /// <summary>
        /// Test writing various things.
        /// </summary>
        /// <param name="bytes">Bytes to write.</param>
        [Theory]
        [MemberData(nameof(TestData))]
        public void Test(byte[] bytes)
        {
            var expectedCrc = ReferenceCalculation(bytes);

            using var stream = new MemoryStream();
            using (var sut = new ZLibStream(stream, CompressionMode.Compress, leaveOpen: true))
            {
                sut.Write(bytes, 0, bytes.Length);
            }

            var actualBytes = stream.ToArray();

            Assert.True(actualBytes.Length > 6);

            Assert.Equal(0x78, actualBytes[0]);
            Assert.Equal(0x9C, actualBytes[1]);

            var actualCrc = BinaryPrimitives.ReadUInt32BigEndian(actualBytes.AsSpan(actualBytes.Length - 4));

            Assert.Equal(expectedCrc, actualCrc);

            // Test round-trip
            stream.SetLength(0);
            using (var sut = new ZLibStream(new MemoryStream(actualBytes), CompressionMode.Decompress, leaveOpen: true))
            {
                sut.CopyTo(stream);
            }

            Assert.Equal(bytes, stream.ToArray());
        }

        /// <summary>
        /// Test data for <see cref="Test"/>.
        /// </summary>
        /// <returns>Test data.</returns>
        public static TheoryData<byte[]> TestData()
        {
            var empty = new byte[1234];
            var nonEmpty = new byte[12345];
            for (var i = 0; i < 1234; i++)
            {
                nonEmpty[i] = (byte)((i * i) % 256);
            }
            return new TheoryData<byte[]>()
            {
                new byte[] { 0x00 },
                new byte[] { 0x01 },
                new byte[] { 0xff },
                new byte[] { 0xff, 0xff },
                new byte[] { 0xff, 0xff, 0xff },
                new byte[] { 0xff, 0xff, 0xff, 0xff },
                new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff },
                new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
                new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
                new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
                new byte[] { 0x00, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff },
                new byte[] { 0x02, 0x03, 0x05, 0x07, 0x0b, 0x0d, 0x11, 0x13, 0x17, 0x1d },
                empty,
                nonEmpty,
            };
        }

        private static uint ReferenceCalculation(byte[] bytes)
        {
            using var stream = new MemoryStream();
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;
            return CalculateAdler32Checksum(stream);
        }

        private static uint CalculateAdler32Checksum(MemoryStream stream)
        {
            uint s1 = 1;
            uint s2 = 0;
            const uint bigPrime = 0xFFF1;
            const int bufferSize = 2048;
            var buffer = new byte[bufferSize];
            while (true)
            {
                var bytesRead = stream.Read(buffer, 0, bufferSize);
                for (var i = 0; i < bytesRead; i++)
                {
                    s1 = (s1 + buffer[i]) % bigPrime;
                    s2 = (s2 + s1) % bigPrime;
                }
                if (bytesRead < bufferSize)
                {
                    break;
                }
            }
            return (s2 << 16) | s1;
        }
    }
}
