using System;
using System.IO;

namespace MatFileHandler.Tests;

internal static class MatFileReadingMethods
{
    public static IMatFile ReadMatFile(MatFileReadingMethod method, string fullFileName)
    {
        using var stream = File.OpenRead(fullFileName);
        switch (method)
        {
            case MatFileReadingMethod.NormalStream:
                return ReadFromStream(stream);
            case MatFileReadingMethod.PartialStream:
            {
                using var wrapper = new PartialUnseekableReadStream(stream);
                return ReadFromStream(wrapper);
            }
            case MatFileReadingMethod.UnalignedStream:
            {
                using var ms =  new MemoryStream();
                ms.Seek(3, SeekOrigin.Begin);
                stream.CopyTo(ms);
                ms.Seek(3, SeekOrigin.Begin);
                return ReadFromStream(ms);
            }
            default:
                throw new NotImplementedException();
        }
    }

    private static IMatFile ReadFromStream(Stream stream)
    {
        var reader = new MatFileReader(stream);
        return reader.Read();
    }
}
