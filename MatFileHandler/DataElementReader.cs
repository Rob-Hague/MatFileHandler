using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MatFileHandler
{
    /// <summary>
    /// Functions for reading data elements from a .mat file.
    /// </summary>
    internal class DataElementReader
    {
        private readonly SubsystemData subsystemData;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataElementReader"/> class.
        /// </summary>
        /// <param name="subsystemData">Reference to file's SubsystemData.</param>
        public DataElementReader(SubsystemData subsystemData)
        {
            this.subsystemData = subsystemData ?? throw new ArgumentNullException(nameof(subsystemData));
        }

        /// <summary>
        /// Read a data element.
        /// </summary>
        /// <param name="reader">Input reader.</param>
        /// <returns>Data element.</returns>
        public DataElement? Read(BinaryReader reader)
        {
            var maybeTagPair = ReadTag(reader);
            if (maybeTagPair is not { } tagPair)
            {
                return null;
            }

            var (dataReader, tag) = tagPair;

            var result = tag.Type switch
            {
                DataType.MiInt8 => ReadNum<sbyte>(tag, dataReader),
                DataType.MiUInt8 or DataType.MiUtf8 => ReadNum<byte>(tag, dataReader),
                DataType.MiInt16 => ReadNum<short>(tag, dataReader),
                DataType.MiUInt16 or DataType.MiUtf16 => ReadNum<ushort>(tag, dataReader),
                DataType.MiInt32 => ReadNum<int>(tag, dataReader),
                DataType.MiUInt32 => ReadNum<uint>(tag, dataReader),
                DataType.MiSingle => ReadNum<float>(tag, dataReader),
                DataType.MiDouble => ReadNum<double>(tag, dataReader),
                DataType.MiInt64 => ReadNum<long>(tag, dataReader),
                DataType.MiUInt64 => ReadNum<ulong>(tag, dataReader),
                DataType.MiMatrix => ReadMatrix(tag, dataReader),
                DataType.MiCompressed => ReadCompressed(tag, dataReader),
                _ => throw new NotSupportedException("Unknown element."),
            };

            if (tag.Type != DataType.MiCompressed)
            {
                var position = reader.BaseStream.Position;
                if (position % 8 != 0)
                {
                    reader.ReadBytes(8 - (int)(position % 8));
                }
            }

            return result;
        }

        /// <summary>
        /// Parse opaque link data.
        /// </summary>
        /// <param name="data">Opaque link data.</param>
        /// <returns>Dimensions array, links array, class index.</returns>
        internal static (int[] dimensions, int[] links, int classIndex) ParseOpaqueData(uint[] data)
        {
            var nDims = data[1];
            var dimensions = new int[nDims];
            var position = 2;
            for (var i = 0; i < nDims; i++)
            {
                dimensions[i] = (int)data[position];
                position++;
            }

            var count = dimensions.NumberOfElements();
            var links = new int[count];
            for (var i = 0; i < count; i++)
            {
                links[i] = (int)data[position];
                position++;
            }

            var classIndex = (int)data[position];

            return (dimensions, links, classIndex);
        }

        private static ArrayFlags ReadArrayFlags(DataElement element)
        {
            var flagData = (element as MiNum<uint>)?.Data ??
                throw new HandlerException("Unexpected type in array flags.");
            var class_ = (ArrayType)(flagData[0] & 0xff);
            var variableFlags = (flagData[0] >> 8) & 0x0e;
            return new ArrayFlags
            {
                Class = class_,
                Variable = (Variable)variableFlags,
            };
        }

        private static DataElement ReadData(DataElement element)
        {
            return element;
        }

        private static int[] ReadDimensionsArray(MiNum<int> element)
        {
            return element.Data;
        }

        private static string[] ReadFieldNames(MiNum<sbyte> element, int fieldNameLength)
        {
            var numberOfFields = element.Data.Length / fieldNameLength;
            var result = new string[numberOfFields];
            for (var i = 0; i < numberOfFields; i++)
            {
                var list = new List<byte>();
                var position = i * fieldNameLength;
                while (element.Data[position] != 0)
                {
                    list.Add((byte)element.Data[position]);
                    position++;
                }

                result[i] = Encoding.ASCII.GetString(list.ToArray());
            }

            return result;
        }

        private static string ReadName(MiNum<sbyte> element)
        {
            return Encoding.ASCII.GetString(element.Data.Select(x => (byte)x).ToArray());
        }

        private static MiNum<T> ReadNum<T>(Tag tag, BinaryReader reader)
            where T : unmanaged
        {
            T[] result;

            unsafe
            {
                Debug.Assert(tag.ElementSize == sizeof(T));
                result = new T[tag.Length / sizeof(T)];
            }

            reader.BaseStream.ReadExactly(MemoryMarshal.AsBytes<T>(result));
            return new MiNum<T>(result);
        }

        private static SparseArrayFlags ReadSparseArrayFlags(DataElement element)
        {
            var arrayFlags = ReadArrayFlags(element);
            var flagData = (element as MiNum<uint>)?.Data ??
                           throw new HandlerException("Unexpected type in sparse array flags.");
            var nzMax = flagData[1];
            return new SparseArrayFlags
            {
                ArrayFlags = arrayFlags,
                NzMax = nzMax,
            };
        }

        private static int? TryReadInt32(BinaryReader reader)
        {
            var buffer = new byte[4];
            var position = 0;
            while (position < 4)
            {
                var actually = reader.BaseStream.Read(buffer, position, 4 - position);
                if (actually == 0)
                {
                    return null;
                }
                position += actually;
            }

            return BitConverter.ToInt32(buffer, 0);
        }

        private static (BinaryReader reader, Tag tag)? ReadTag(BinaryReader reader)
        {
            var maybeType = TryReadInt32(reader);
            if (maybeType is not int type)
            {
                return null;
            }

            var typeHi = type >> 16;
            if (typeHi == 0)
            {
                var length = reader.ReadInt32();
                return (reader, new Tag((DataType)type, length));
            }
            else
            {
                var length = typeHi;
                type &= 0xffff;
                var smallReader = new BinaryReader(new MemoryStream(reader.ReadBytes(4)));
                return (smallReader, new Tag((DataType)type, length));
            }
        }

        private MatCellArray ContinueReadingCellArray(
            BinaryReader reader,
            ArrayFlags flags,
            int[] dimensions,
            string name)
        {
            var numberOfElements = dimensions.NumberOfElements();
            var elements = new List<IArray>();
            for (var i = 0; i < numberOfElements; i++)
            {
                var element = Read(reader) as IArray
                    ?? throw new HandlerException("Unable to read cell array.");
                elements.Add(element);
            }

            return new MatCellArray(flags, dimensions, name, elements);
        }

        private DataElement ContinueReadingOpaque(BinaryReader reader)
        {
            var nameElement = Read(reader) as MiNum<sbyte> ??
                              throw new HandlerException("Unexpected type in object name.");
            var name = ReadName(nameElement);
            var anotherElement = Read(reader) as MiNum<sbyte> ??
                                 throw new HandlerException("Unexpected type in object type description.");
            var typeDescription = ReadName(anotherElement);
            var classNameElement = Read(reader) as MiNum<sbyte> ??
                                   throw new HandlerException("Unexpected type in class name.");
            var className = ReadName(classNameElement);
            var dataElement = Read(reader) ?? throw new HandlerException("Missing opaque data element.");
            var data = ReadData(dataElement);
            if (data is MatNumericalArrayOf<uint> linkElement)
            {
                var (dimensions, indexToObjectId, classIndex) = ParseOpaqueData(linkElement.Data);
                return new OpaqueLink(
                    name,
                    typeDescription,
                    className,
                    dimensions,
                    data,
                    indexToObjectId,
                    classIndex,
                    subsystemData);
            }
            else
            {
                return new Opaque(name, typeDescription, className, Array.Empty<int>(), data, subsystemData);
            }
        }

        private MatArray ContinueReadingSparseArray(
            BinaryReader reader,
            DataElement firstElement,
            int[] dimensions,
            string name)
        {
            var sparseArrayFlags = ReadSparseArrayFlags(firstElement);
            var rowIndex = Read(reader) as MiNum<int> ??
                           throw new HandlerException("Unexpected type in row indices of a sparse array.");
            var columnIndex = Read(reader) as MiNum<int> ??
                              throw new HandlerException("Unexpected type in column indices of a sparse array.");
            var data = Read(reader) ?? throw new HandlerException("Missing sparse array data.");
            if (sparseArrayFlags.ArrayFlags.Variable.HasFlag(Variable.IsLogical))
            {
                return DataElementConverter.ConvertToMatSparseArrayOf<bool>(
                    sparseArrayFlags,
                    dimensions,
                    name,
                    rowIndex.Data,
                    columnIndex.Data,
                    data);
            }

            if (sparseArrayFlags.ArrayFlags.Variable.HasFlag(Variable.IsComplex))
            {
                var imaginaryData = Read(reader) ?? throw new HandlerException("Missing imaginary part of sparse array data.");
                return DataElementConverter.ConvertToMatSparseArrayOfComplex(
                    sparseArrayFlags,
                    dimensions,
                    name,
                    rowIndex.Data,
                    columnIndex.Data,
                    data,
                    imaginaryData);
            }

            return data switch
            {
                MiNum<double> => DataElementConverter.ConvertToMatSparseArrayOf<double>(
                    sparseArrayFlags,
                    dimensions,
                    name,
                    rowIndex.Data,
                    columnIndex.Data,
                    data),
                _ => throw new NotSupportedException("Only double and logical sparse arrays are supported."),
            };
        }

        private MatStructureArray ContinueReadingStructure(
            BinaryReader reader,
            ArrayFlags flags,
            int[] dimensions,
            string name,
            int fieldNameLength)
        {
            var element = Read(reader) as MiNum<sbyte>
                ?? throw new HandlerException("Unable to parse structure field names.");
            var fieldNames = ReadFieldNames(element, fieldNameLength);
            var fields = new Dictionary<string, List<IArray>>();
            foreach (var fieldName in fieldNames)
            {
                fields[fieldName] = new List<IArray>();
            }

            var numberOfElements = dimensions.NumberOfElements();
            for (var i = 0; i < numberOfElements; i++)
            {
                foreach (var fieldName in fieldNames)
                {
                    var field = Read(reader) as IArray
                        ?? throw new HandlerException("Unable to parse field name.");
                    fields[fieldName].Add(field);
                }
            }

            return new MatStructureArray(flags, dimensions, name, fields);
        }

        private DataElement ReadCompressed(Tag tag, BinaryReader reader)
        {
            DataElement element;

            using (var substream = new Substream(reader.BaseStream, tag.Length))
            {
                using (var deflateStream = new ZLibStream(substream, CompressionMode.Decompress, leaveOpen: true))
                using (var bufferedStream = new BufferedStream(deflateStream))
                using (var positionTrackingStream = new PositionTrackingStream(bufferedStream))
                using (var innerReader = new BinaryReader(positionTrackingStream))
                {
                    element = Read(innerReader) ?? throw new HandlerException("Missing compressed data.");
                }

                if (substream.Position != substream.Length)
                {
                    // In the pathological case that the deflate stream did not read the full
                    // length, then read out the rest manually (normally 1 byte).
                    reader.ReadBytes((int)(substream.Length - substream.Position));
                }
            }

            return element;
        }

        private DataElement ReadMatrix(Tag tag, BinaryReader reader)
        {
            if (tag.Length == 0)
            {
                return MatArray.Empty();
            }

            var element1 = Read(reader) ?? throw new HandlerException("Missing matrix data.");
            var flags = ReadArrayFlags(element1);
            if (flags.Class == ArrayType.MxOpaque)
            {
                return ContinueReadingOpaque(reader);
            }

            var element2 = Read(reader) as MiNum<int> ??
                           throw new HandlerException("Unexpected type in array dimensions data.");
            var dimensions = ReadDimensionsArray(element2);
            var element3 = Read(reader) as MiNum<sbyte> ?? throw new HandlerException("Unexpected type in array name.");
            var name = ReadName(element3);
            if (flags.Class == ArrayType.MxCell)
            {
                return ContinueReadingCellArray(reader, flags, dimensions, name);
            }

            if (flags.Class == ArrayType.MxSparse)
            {
                return ContinueReadingSparseArray(reader, element1, dimensions, name);
            }

            var element4 = Read(reader) ?? throw new HandlerException("Missing matrix data.");
            var data = ReadData(element4);
            DataElement? imaginaryData = null;
            if (flags.Variable.HasFlag(Variable.IsComplex))
            {
                var element5 = Read(reader) ?? throw new HandlerException("Missing complex matrix data.");
                imaginaryData = ReadData(element5);
            }

            if (flags.Class == ArrayType.MxStruct)
            {
                var fieldNameLengthElement = data as MiNum<int> ??
                                             throw new HandlerException(
                                                 "Unexpected type in structure field name length.");
                return ContinueReadingStructure(reader, flags, dimensions, name, fieldNameLengthElement.Data[0]);
            }

            switch (flags.Class)
            {
                case ArrayType.MxChar:
                    return data switch
                    {
                        MiNum<byte> => DataElementConverter.ConvertToMatNumericalArrayOf<byte>(
                            flags,
                            dimensions,
                            name,
                            data,
                            imaginaryData),
                        MiNum<ushort> => DataElementConverter.ConvertToMatNumericalArrayOf<ushort>(
                            flags,
                            dimensions,
                            name,
                            data,
                            imaginaryData),
                        _ => throw new NotSupportedException(
                            $"This type of char array ({data.GetType()}) is not supported."),
                    };
                case ArrayType.MxInt8:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<sbyte>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxUInt8:
                    if (flags.Variable.HasFlag(Variable.IsLogical))
                    {
                        return DataElementConverter.ConvertToMatNumericalArrayOf<bool>(
                            flags,
                            dimensions,
                            name,
                            data,
                            imaginaryData);
                    }

                    return DataElementConverter.ConvertToMatNumericalArrayOf<byte>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxInt16:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<short>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxUInt16:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<ushort>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxInt32:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<int>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxUInt32:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<uint>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxInt64:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<long>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxUInt64:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<ulong>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxSingle:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<float>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                case ArrayType.MxDouble:
                    return DataElementConverter.ConvertToMatNumericalArrayOf<double>(
                        flags,
                        dimensions,
                        name,
                        data,
                        imaginaryData);
                default:
                    throw new HandlerException("Unknown data type.");
            }
        }
    }
}
