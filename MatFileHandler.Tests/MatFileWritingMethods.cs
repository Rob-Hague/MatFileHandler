using System;
using System.IO;

namespace MatFileHandler.Tests;

internal static class MatFileWritingMethods
{
    public static byte[] WriteMatFile(MatFileWritingMethod method, MatFileWriterOptionsForTests options, IMatFile matFile)
    {
        switch (method)
        {
            case MatFileWritingMethod.NormalStream:
            {
                using var memoryStream = new MemoryStream();
                var matFileWriter = CreateWriter(options, memoryStream);
                matFileWriter.Write(matFile);
                return memoryStream.ToArray();
            }
            case MatFileWritingMethod.UnseekableStream:
            {
                using var memoryStream = new MemoryStream();
                using var unseekableStream = new UnseekableWriteStream(memoryStream);
                var matFileWriter = CreateWriter(options, unseekableStream);
                matFileWriter.Write(matFile);
                return memoryStream.ToArray();
            }
            case MatFileWritingMethod.UnalignedStream:
            {
                using var memoryStream = new MemoryStream();
                memoryStream.Seek(3, SeekOrigin.Begin);
                var matFileWriter = CreateWriter(options, memoryStream);
                matFileWriter.Write(matFile);
                var fullArray = memoryStream.ToArray();
                var length = fullArray.Length - 3;
                var result = new byte[length];
                Buffer.BlockCopy(fullArray, 3, result, 0, length);
                return result;
            }
            default:
                throw new NotImplementedException();
        }
    }

    private static MatFileWriter CreateWriter(MatFileWriterOptionsForTests options, Stream stream)
    {
        return options switch
        {
            MatFileWriterOptionsForTests.None => new MatFileWriter(stream),
            MatFileWriterOptionsForTests.Always => new MatFileWriter(
                stream,
                new MatFileWriterOptions { UseCompression = CompressionUsage.Always }),
            MatFileWriterOptionsForTests.Never => new MatFileWriter(
                stream,
                new MatFileWriterOptions { UseCompression = CompressionUsage.Never }),
            _ => throw new NotImplementedException(),
        };
    }
}
