#if !NET
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace MatFileHandler
{
    internal sealed class ZLibStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly CompressionMode _mode;

        private DeflateStream? _deflateStream;
        private bool _doneFirstRead;
        private bool _disposed;

        private const uint BigPrime = 0xFFF1;
        private uint s1 = 1;
        private uint s2;

        public ZLibStream(Stream stream, CompressionMode mode, bool leaveOpen)
        {
            _baseStream = stream;
            _mode = mode;

            if (mode == CompressionMode.Decompress)
            {
                CanRead = stream.CanRead;
            }
            else
            {
                Debug.Assert(mode == CompressionMode.Compress);
                CanWrite = stream.CanWrite;
            }

            Debug.Assert(leaveOpen, "leaveOpen parameter is expected to be true always");
        }

        public override bool CanRead { get; }

        public override bool CanSeek => false;

        public override bool CanWrite { get; }

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override void Flush()
        {
            _deflateStream?.Flush();
            _baseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!_doneFirstRead)
            {
                // Skip the 2 bytes header
                _baseStream.ReadExactly(new byte[2], 0, 2);

                _doneFirstRead = true;

                _deflateStream = new DeflateStream(_baseStream, CompressionMode.Decompress, leaveOpen: true);
            }

            // One might expect to have to read the 4 byte CRC once _deflateStream.Read
            // returns 0, but it appears that DeflateStream handles it already.
            // Whether it does or not does not matter for us since a) we aren't validating
            // the CRC; and b) compressed data is prefixed with a length in the MATLAB file
            // format and we always ensure we've read the full amount during parsing.

            return _deflateStream!.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (_deflateStream is null)
            {
                _baseStream.WriteByte(0x78);
                _baseStream.WriteByte(0x9c);

                _deflateStream = new DeflateStream(_baseStream, CompressionMode.Compress, leaveOpen: true);
            }

            for (var i = offset; i < offset + count; i++)
            {
                s1 = (s1 + buffer[i]) % BigPrime;
                s2 = (s2 + s1) % BigPrime;
            }

            _deflateStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (disposing && _deflateStream is not null)
            {
                _deflateStream.Dispose();

                if (_mode == CompressionMode.Compress)
                {
                    var crc = new byte[4];

                    BinaryPrimitives.WriteUInt32BigEndian(crc, (s2 << 16) | s1);

                    _baseStream.Write(crc, 0, crc.Length);
                }
            }

            base.Dispose(disposing);
        }
    }
}
#endif
