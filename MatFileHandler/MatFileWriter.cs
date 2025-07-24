using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace MatFileHandler
{
    /// <summary>
    /// Class for writing .mat files.
    /// </summary>
    public class MatFileWriter
    {
        private readonly MatFileWriterOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileWriter"/> class with a stream and default options.
        /// </summary>
        /// <param name="stream">Output stream.</param>
        public MatFileWriter(Stream stream)
        {
            Stream = stream;
            _options = MatFileWriterOptions.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MatFileWriter"/> class with a stream.
        /// </summary>
        /// <param name="stream">Output stream.</param>
        /// <param name="options">Options to use for file writing.</param>
        public MatFileWriter(Stream stream, MatFileWriterOptions options)
        {
            Stream = stream;
            _options = options;
        }

        private Stream Stream { get; }

        /// <summary>
        /// Writes a .mat file.
        /// </summary>
        /// <param name="file">A file to write.</param>
        public void Write(IMatFile file)
        {
            var header = Header.CreateNewHeader();
            using var writer = new BinaryWriter(Stream);
            WriteHeader(writer, header);
            foreach (var variable in file.Variables)
            {
                switch (_options.UseCompression)
                {
                    case CompressionUsage.Always:
                        if (Stream.CanSeek)
                        {
                            WriteCompressedVariableToSeekableStream(writer, variable);
                        }
                        else
                        {
                            WriteCompressedVariableToUnseekableStream(writer, variable);
                        }

                        break;
                    case CompressionUsage.Never:
                        WriteVariable(writer, variable);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private static uint CalculateAdler32Checksum(Stream stream)
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

        private static void WriteHeader(BinaryWriter writer, Header header)
        {
            writer.Write(Encoding.UTF8.GetBytes(header.Text));
            writer.Write(header.SubsystemDataOffset);
            writer.Write((short)header.Version);
            writer.Write((short)19785); // Magic number, 'IM'.
        }

        private static void WriteTag(BinaryWriter writer, Tag tag)
        {
            writer.Write((int)tag.Type);
            writer.Write(tag.Length);
        }

        private static void WriteShortTag(BinaryWriter writer, Tag tag)
        {
            writer.Write((short)tag.Type);
            writer.Write((short)tag.Length);
        }

        private static void WriteDataElement<T>(BinaryWriter writer, DataType type, T[] data)
            where T : unmanaged
        {
            unsafe
            {
                Debug.Assert(type.Size() == sizeof(T));
            }

            ReadOnlySpan<byte> bytes = MemoryMarshal.AsBytes<T>(data);

            if (bytes.Length > 4)
            {
                WriteTag(writer, new Tag(type, bytes.Length));
                writer.Write(bytes);
                var rem = bytes.Length % 8;
                if (rem > 0)
                {
                    var padding = new byte[8 - rem];
                    writer.Write(padding);
                }
            }
            else
            {
                WriteShortTag(writer, new Tag(type, bytes.Length));
                writer.Write(bytes);
                if (bytes.Length < 4)
                {
                    var padding = new byte[4 - bytes.Length];
                    writer.Write(padding);
                }
            }
        }

        private static void WriteDimensions(BinaryWriter writer, int[] dimensions)
        {
            WriteDataElement(writer, DataType.MiInt32, dimensions);
        }

        private static (T[] real, T[] imaginary) ConvertToPairOfArrays<T>(ComplexOf<T>[] data)
            where T : struct
        {
            return (data.Select(x => x.Real).ToArray(), data.Select(x => x.Imaginary).ToArray());
        }

        private static (double[] real, double[] imaginary) ConvertToPairOfArrays(Complex[] data)
        {
            return (data.Select(x => x.Real).ToArray(), data.Select(x => x.Imaginary).ToArray());
        }

        private static void WriteComplexValues<T>(BinaryWriter writer, DataType type, (T[] real, T[] complex) data)
            where T : unmanaged
        {
            WriteDataElement(writer, type, data.real);
            WriteDataElement(writer, type, data.complex);
        }

        private static void WriteArrayFlags(BinaryWriter writer, ArrayFlags flags)
        {
            var flag = (byte)flags.Variable;
            WriteTag(writer, new Tag(DataType.MiUInt32, 8));
            writer.Write((byte)flags.Class);
            writer.Write(flag);
            writer.Write([0, 0, 0, 0, 0, 0]);
        }

        private static void WriteSparseArrayFlags(BinaryWriter writer, SparseArrayFlags flags)
        {
            var flag = (byte)flags.ArrayFlags.Variable;
            WriteTag(writer, new Tag(DataType.MiUInt32, 8));
            writer.Write((byte)flags.ArrayFlags.Class);
            writer.Write(flag);
            writer.Write([0, 0]);
            writer.Write(flags.NzMax);
        }

        private static void WriteName(BinaryWriter writer, string name)
        {
            var nameBytes = Encoding.ASCII.GetBytes(name);
            WriteDataElement(writer, DataType.MiInt8, nameBytes);
        }

        private static void WriteNumericalArrayValues(BinaryWriter writer, IArray value)
        {
            switch (value)
            {
                case IArrayOf<sbyte> sbyteArray:
                    WriteDataElement(writer, DataType.MiInt8, sbyteArray.Data);
                    break;
                case IArrayOf<byte> byteArray:
                    WriteDataElement(writer, DataType.MiUInt8, byteArray.Data);
                    break;
                case IArrayOf<short> shortArray:
                    WriteDataElement(writer, DataType.MiInt16, shortArray.Data);
                    break;
                case IArrayOf<ushort> ushortArray:
                    WriteDataElement(writer, DataType.MiUInt16, ushortArray.Data);
                    break;
                case IArrayOf<int> intArray:
                    WriteDataElement(writer, DataType.MiInt32, intArray.Data);
                    break;
                case IArrayOf<uint> uintArray:
                    WriteDataElement(writer, DataType.MiUInt32, uintArray.Data);
                    break;
                case IArrayOf<long> longArray:
                    WriteDataElement(writer, DataType.MiInt64, longArray.Data);
                    break;
                case IArrayOf<ulong> ulongArray:
                    WriteDataElement(writer, DataType.MiUInt64, ulongArray.Data);
                    break;
                case IArrayOf<float> floatArray:
                    WriteDataElement(writer, DataType.MiSingle, floatArray.Data);
                    break;
                case IArrayOf<double> doubleArray:
                    WriteDataElement(writer, DataType.MiDouble, doubleArray.Data);
                    break;
                case IArrayOf<bool> boolArray:
                    WriteDataElement(writer, DataType.MiUInt8, boolArray.Data);
                    break;
                case IArrayOf<ComplexOf<sbyte>> complexSbyteArray:
                    WriteComplexValues(writer, DataType.MiInt8, ConvertToPairOfArrays(complexSbyteArray.Data));
                    break;
                case IArrayOf<ComplexOf<byte>> complexByteArray:
                    WriteComplexValues(writer, DataType.MiUInt8, ConvertToPairOfArrays(complexByteArray.Data));
                    break;
                case IArrayOf<ComplexOf<short>> complexShortArray:
                    WriteComplexValues(writer, DataType.MiInt16, ConvertToPairOfArrays(complexShortArray.Data));
                    break;
                case IArrayOf<ComplexOf<ushort>> complexUshortArray:
                    WriteComplexValues(writer, DataType.MiUInt16, ConvertToPairOfArrays(complexUshortArray.Data));
                    break;
                case IArrayOf<ComplexOf<int>> complexIntArray:
                    WriteComplexValues(writer, DataType.MiInt32, ConvertToPairOfArrays(complexIntArray.Data));
                    break;
                case IArrayOf<ComplexOf<uint>> complexUintArray:
                    WriteComplexValues(writer, DataType.MiUInt32, ConvertToPairOfArrays(complexUintArray.Data));
                    break;
                case IArrayOf<ComplexOf<long>> complexLongArray:
                    WriteComplexValues(writer, DataType.MiInt64, ConvertToPairOfArrays(complexLongArray.Data));
                    break;
                case IArrayOf<ComplexOf<ulong>> complexUlongArray:
                    WriteComplexValues(writer, DataType.MiUInt64, ConvertToPairOfArrays(complexUlongArray.Data));
                    break;
                case IArrayOf<ComplexOf<float>> complexFloatArray:
                    WriteComplexValues(writer, DataType.MiSingle, ConvertToPairOfArrays(complexFloatArray.Data));
                    break;
                case IArrayOf<Complex> complexDoubleArray:
                    WriteComplexValues(writer, DataType.MiDouble, ConvertToPairOfArrays(complexDoubleArray.Data));
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        private static ArrayFlags GetArrayFlags(IArray array, bool isGlobal)
        {
            var variableFlags = isGlobal ? Variable.IsGlobal : 0;
            return array switch
            {
                IArrayOf<sbyte> => new ArrayFlags(ArrayType.MxInt8, variableFlags),
                IArrayOf<byte> => new ArrayFlags(ArrayType.MxUInt8, variableFlags),
                IArrayOf<short> => new ArrayFlags(ArrayType.MxInt16, variableFlags),
                IArrayOf<ushort> => new ArrayFlags(ArrayType.MxUInt16, variableFlags),
                IArrayOf<int> => new ArrayFlags(ArrayType.MxInt32, variableFlags),
                IArrayOf<uint> => new ArrayFlags(ArrayType.MxUInt32, variableFlags),
                IArrayOf<long> => new ArrayFlags(ArrayType.MxInt64, variableFlags),
                IArrayOf<ulong> => new ArrayFlags(ArrayType.MxUInt64, variableFlags),
                IArrayOf<float> => new ArrayFlags(ArrayType.MxSingle, variableFlags),
                IArrayOf<double> => new ArrayFlags(ArrayType.MxDouble, variableFlags),
                IArrayOf<bool> => new ArrayFlags(ArrayType.MxUInt8, variableFlags | Variable.IsLogical),
                IArrayOf<ComplexOf<sbyte>> => new ArrayFlags(ArrayType.MxInt8, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<byte>> => new ArrayFlags(ArrayType.MxUInt8, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<short>> => new ArrayFlags(ArrayType.MxInt16, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<ushort>> => new ArrayFlags(ArrayType.MxUInt16, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<int>> => new ArrayFlags(ArrayType.MxInt32, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<uint>> => new ArrayFlags(ArrayType.MxUInt32, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<long>> => new ArrayFlags(ArrayType.MxInt64, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<ulong>> => new ArrayFlags(ArrayType.MxUInt64, variableFlags | Variable.IsComplex),
                IArrayOf<ComplexOf<float>> => new ArrayFlags(ArrayType.MxSingle, variableFlags | Variable.IsComplex),
                IArrayOf<Complex> => new ArrayFlags(ArrayType.MxDouble, variableFlags | Variable.IsComplex),
                IStructureArray => new ArrayFlags(ArrayType.MxStruct, variableFlags),
                ICellArray => new ArrayFlags(ArrayType.MxCell, variableFlags),
                _ => throw new NotSupportedException(),
            };
        }

        private static SparseArrayFlags GetSparseArrayFlags<T>(ISparseArrayOf<T> array, bool isGlobal, uint nonZero)
            where T : struct
        {
            var flags = GetArrayFlags(array, isGlobal);
            return new SparseArrayFlags
            {
                ArrayFlags = new ArrayFlags
                {
                    Class = ArrayType.MxSparse,
                    Variable = flags.Variable,
                },
                NzMax = nonZero,
            };
        }

        private static ArrayFlags GetCharArrayFlags(bool isGlobal)
        {
            return new ArrayFlags(ArrayType.MxChar, isGlobal ? Variable.IsGlobal : 0);
        }

        private static void WriteWrappingContents<T>(
            BinaryWriter writer,
            T array,
            Action<FakeWriter> lengthCalculator,
            Action<BinaryWriter> writeContents)
            where T : IArray
        {
            if (array.IsEmpty)
            {
                WriteTag(writer, new Tag(DataType.MiMatrix, 0));
                return;
            }

            var fakeWriter = new FakeWriter();
            lengthCalculator(fakeWriter);
            var calculatedLength = fakeWriter.Position;
            WriteTag(writer, new Tag(DataType.MiMatrix, calculatedLength));
            writeContents(writer);
        }

        private static void WriteNumericalArrayContents(BinaryWriter writer, IArray array, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetArrayFlags(array, isGlobal));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteNumericalArrayValues(writer, array);
        }

        private static void WriteNumericalArray(
            BinaryWriter writer,
            IArray numericalArray,
            string name = "",
            bool isGlobal = false)
        {
            WriteWrappingContents(
                writer,
                numericalArray,
                fakeWriter => fakeWriter.WriteNumericalArrayContents(numericalArray, name),
                contentsWriter => { WriteNumericalArrayContents(contentsWriter, numericalArray, name, isGlobal); });
        }

        private static void WriteCharArrayContents(BinaryWriter writer, ICharArray charArray, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetCharArrayFlags(isGlobal));
            WriteDimensions(writer, charArray.Dimensions);
            WriteName(writer, name);
            WriteDataElement(writer, DataType.MiUtf16, charArray.String.ToCharArray());
        }

        private static void WriteCharArray(BinaryWriter writer, ICharArray charArray, string name, bool isGlobal)
        {
            WriteWrappingContents(
                writer,
                charArray,
                fakeWriter => fakeWriter.WriteCharArrayContents(charArray, name),
                contentsWriter => { WriteCharArrayContents(contentsWriter, charArray, name, isGlobal); });
        }

        private static void WriteSparseArrayValues<T>(
            BinaryWriter writer, int[] rows, int[] columns, T[] data)
          where T : unmanaged
        {
            WriteDataElement(writer, DataType.MiInt32, rows);
            WriteDataElement(writer, DataType.MiInt32, columns);
            if (data is double[])
            {
                WriteDataElement(writer, DataType.MiDouble, data);
            }
            else if (data is Complex[] complexData)
            {
                WriteDataElement(
                    writer,
                    DataType.MiDouble,
                    complexData.Select(c => c.Real).ToArray());
                WriteDataElement(
                    writer,
                    DataType.MiDouble,
                    complexData.Select(c => c.Imaginary).ToArray());
            }
            else if (data is bool[] boolData)
            {
                WriteDataElement(
                    writer,
                    DataType.MiUInt8,
                    boolData);
            }
        }

        private static (int[] rowIndex, int[] columnIndex, T[] data, uint nonZero) PrepareSparseArrayData<T>(
            ISparseArrayOf<T> array)
            where T : struct, IEquatable<T>
        {
            var dict = array.Data;
            var keys = dict.Keys.ToArray();
            var rowIndexList = new List<int>();
            var valuesList = new List<T>();
            var numberOfColumns = array.Dimensions[1];
            var columnIndex = new int[numberOfColumns + 1];
            columnIndex[0] = 0;
            for (var column = 0; column < numberOfColumns; column++)
            {
                var column1 = column;
                var thisColumn = keys.Where(pair => pair.column == column1 && !dict[pair].Equals(default));
                var thisRow = thisColumn.Select(pair => pair.row).OrderBy(x => x).ToArray();
                rowIndexList.AddRange(thisRow);
                valuesList.AddRange(thisRow.Select(row => dict[(row, column1)]));
                columnIndex[column + 1] = rowIndexList.Count;
            }
            return (rowIndexList.ToArray(), columnIndex, valuesList.ToArray(), (uint)rowIndexList.Count);
        }

        private static void WriteSparseArrayContents<T>(
            BinaryWriter writer,
            ISparseArrayOf<T> array,
            string name,
            bool isGlobal)
            where T : unmanaged, IEquatable<T>
        {
            (var rows, var columns, var data, var nonZero) = PrepareSparseArrayData(array);
            WriteSparseArrayFlags(writer, GetSparseArrayFlags(array, isGlobal, nonZero));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteSparseArrayValues(writer, rows, columns, data);
        }

        private static void WriteSparseArray<T>(BinaryWriter writer, ISparseArrayOf<T> sparseArray, string name, bool isGlobal)
            where T : unmanaged, IEquatable<T>
        {
            WriteWrappingContents(
                writer,
                sparseArray,
                fakeWriter => fakeWriter.WriteSparseArrayContents(sparseArray, name),
                contentsWriter => { WriteSparseArrayContents(contentsWriter, sparseArray, name, isGlobal); });
        }

        private static void WriteFieldNames(BinaryWriter writer, IEnumerable<string> fieldNames)
        {
            var fieldNamesArray = fieldNames.Select(name => Encoding.ASCII.GetBytes(name)).ToArray();
            var maxFieldName = fieldNamesArray.Max(name => name.Length) + 1;
            WriteDataElement(writer, DataType.MiInt32, [maxFieldName]);
            var buffer = new byte[fieldNamesArray.Length * maxFieldName];
            var startPosition = 0;
            foreach (var name in fieldNamesArray)
            {
                for (var i = 0; i < name.Length; i++)
                {
                    buffer[startPosition + i] = name[i];
                }
                startPosition += maxFieldName;
            }
            WriteDataElement(writer, DataType.MiInt8, buffer);
        }

        private void WriteStructureArrayValues(BinaryWriter writer, IStructureArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                foreach (var name in array.FieldNames)
                {
                    WriteArray(writer, array[name, i]);
                }
            }
        }

        private void WriteStructureArrayContents(BinaryWriter writer, IStructureArray array, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetArrayFlags(array, isGlobal));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteFieldNames(writer, array.FieldNames);
            WriteStructureArrayValues(writer, array);
        }

        private void WriteStructureArray(
            BinaryWriter writer,
            IStructureArray structureArray,
            string name,
            bool isGlobal)
        {
            WriteWrappingContents(
                writer,
                structureArray,
                fakeWriter => fakeWriter.WriteStructureArrayContents(structureArray, name),
                contentsWriter => { WriteStructureArrayContents(contentsWriter, structureArray, name, isGlobal); });
        }

        private void WriteCellArrayValues(BinaryWriter writer, ICellArray array)
        {
            for (var i = 0; i < array.Count; i++)
            {
                WriteArray(writer, array[i]);
            }
        }

        private void WriteCellArrayContents(BinaryWriter writer, ICellArray array, string name, bool isGlobal)
        {
            WriteArrayFlags(writer, GetArrayFlags(array, isGlobal));
            WriteDimensions(writer, array.Dimensions);
            WriteName(writer, name);
            WriteCellArrayValues(writer, array);
        }

        private void WriteCellArray(BinaryWriter writer, ICellArray cellArray, string name, bool isGlobal)
        {
            WriteWrappingContents(
                writer,
                cellArray,
                fakeWriter => fakeWriter.WriteCellArrayContents(cellArray, name),
                contentsWriter => { WriteCellArrayContents(contentsWriter, cellArray, name, isGlobal); });
        }

        private void WriteArray(BinaryWriter writer, IArray array, string variableName = "", bool isGlobal = false)
        {
            switch (array)
            {
                case ICharArray charArray:
                    WriteCharArray(writer, charArray, variableName, isGlobal);
                    break;
                case ISparseArrayOf<double> doubleSparseArray:
                    WriteSparseArray(writer, doubleSparseArray, variableName, isGlobal);
                    break;
                case ISparseArrayOf<Complex> complexSparseArray:
                    WriteSparseArray(writer, complexSparseArray, variableName, isGlobal);
                    break;
                case ISparseArrayOf<bool> boolSparseArray:
                    WriteSparseArray(writer, boolSparseArray, variableName, isGlobal);
                    break;
                case ICellArray cellArray:
                    WriteCellArray(writer, cellArray, variableName, isGlobal);
                    break;
                case IStructureArray structureArray:
                    WriteStructureArray(writer, structureArray, variableName, isGlobal);
                    break;
                default:
                    WriteNumericalArray(writer, array, variableName, isGlobal);
                    break;
            }
        }

        private void WriteVariable(BinaryWriter writer, IVariable variable)
        {
            WriteArray(writer, variable.Value, variable.Name, variable.IsGlobal);
        }

        private void WriteCompressedVariableToSeekableStream(BinaryWriter writer, IVariable variable)
        {
            var position = writer.BaseStream.Position;
            WriteTag(writer, new Tag(DataType.MiCompressed, 0));

            int compressedLength;
            var before = writer.BaseStream.Position;
            using (var compressionStream = new ZLibStream(writer.BaseStream, CompressionMode.Compress, leaveOpen: true))
            using (var internalWriter = new BinaryWriter(compressionStream, Encoding.UTF8, leaveOpen: true))
            {
                WriteVariable(internalWriter, variable);
            }

            var after = writer.BaseStream.Position;
            compressedLength = (int)(after - before);

            writer.BaseStream.Position = position;
            WriteTag(writer, new Tag(DataType.MiCompressed, compressedLength));
            writer.BaseStream.Seek(0, SeekOrigin.End);
        }

        private void WriteCompressedVariableToUnseekableStream(BinaryWriter writer, IVariable variable)
        {
            using var compressedStream = new MemoryStream();
            uint crc;
            using (var originalStream = new MemoryStream())
            {
                using var internalWriter = new BinaryWriter(originalStream);
                WriteVariable(internalWriter, variable);
                originalStream.Position = 0;
                crc = CalculateAdler32Checksum(originalStream);
                originalStream.Position = 0;
                using var compressionStream = new DeflateStream(
                    compressedStream,
                    CompressionMode.Compress,
                    leaveOpen: true);
                originalStream.CopyTo(compressionStream);
            }
            compressedStream.Position = 0;
            WriteTag(writer, new Tag(DataType.MiCompressed, (int)(compressedStream.Length + 6)));
            writer.Write((byte)0x78);
            writer.Write((byte)0x9c);
            compressedStream.CopyTo(writer.BaseStream);
            writer.Write(BitConverter.GetBytes(crc).Reverse().ToArray());
        }
    }
}
