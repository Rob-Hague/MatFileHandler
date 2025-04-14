using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace MatFileHandler.Tests
{
    /// <summary>
    /// Tests for the <see cref="ChecksumCalculatingStream"/> class.
    /// </summary>
    public class ChecksumCalculatingStreamTests
    {
        /// <summary>
        /// Test writing various things.
        /// </summary>
        /// <param name="bytes">Bytes to write.</param>
        [Theory]
        [MemberData(nameof(TestData))]
        public void Test(byte[] bytes)
        {
            using var stream = new MemoryStream();
            var sut = new ChecksumCalculatingStream(stream);
            sut.Write(bytes, 0, bytes.Length);
            var actual = sut.GetCrc();

            var expected = ReferenceCalculation(bytes);
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

        private static Action<Stream> BinaryWriterAction(Action<BinaryWriter> action)
        {
            return stream =>
            {
                using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
                action(writer);
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
