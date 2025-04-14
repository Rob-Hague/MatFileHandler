using System;

namespace MatFileHandler
{
    /// <summary>
    /// Functions for extracting values from data elements.
    /// </summary>
    internal static class DataExtraction
    {
        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Double values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Double.</returns>
        public static double[] GetDataAsDouble(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToDouble(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToDouble(byteElement.Data),
                MiNum<int> intElement => IntToDouble(intElement.Data),
                MiNum<uint> uintElement => UintToDouble(uintElement.Data),
                MiNum<short> shortElement => ShortToDouble(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToDouble(ushortElement.Data),
                MiNum<long> longElement => LongToDouble(longElement.Data),
                MiNum<ulong> ulongElement => UlongToDouble(ulongElement.Data),
                MiNum<float> floatElement => FloatToDouble(floatElement.Data),
                MiNum<double> doubleElement => doubleElement.Data,
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to double, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Single values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Single.</returns>
        public static float[] GetDataAsSingle(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToSingle(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToSingle(byteElement.Data),
                MiNum<int> intElement => IntToSingle(intElement.Data),
                MiNum<uint> uintElement => UintToSingle(uintElement.Data),
                MiNum<short> shortElement => ShortToSingle(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToSingle(ushortElement.Data),
                MiNum<long> longElement => LongToSingle(longElement.Data),
                MiNum<ulong> ulongElement => UlongToSingle(ulongElement.Data),
                MiNum<float> floatElement => floatElement.Data,
                MiNum<double> doubleElement => DoubleToSingle(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to float, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int8 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int8.</returns>
        public static sbyte[] GetDataAsInt8(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => sbyteElement.Data,
                MiNum<byte> byteElement => ByteToSByte(byteElement.Data),
                MiNum<int> intElement => IntToSByte(intElement.Data),
                MiNum<uint> uintElement => UintToSByte(uintElement.Data),
                MiNum<short> shortElement => ShortToSByte(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToSByte(ushortElement.Data),
                MiNum<long> longElement => LongToSByte(longElement.Data),
                MiNum<ulong> ulongElement => UlongToSByte(ulongElement.Data),
                MiNum<float> floatElement => SingleToSByte(floatElement.Data),
                MiNum<double> doubleElement => DoubleToSByte(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to int8, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Uint8 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt8.</returns>
        public static byte[] GetDataAsUInt8(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToByte(sbyteElement.Data),
                MiNum<byte> byteElement => byteElement.Data,
                MiNum<int> intElement => IntToByte(intElement.Data),
                MiNum<uint> uintElement => UintToByte(uintElement.Data),
                MiNum<short> shortElement => ShortToByte(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToByte(ushortElement.Data),
                MiNum<long> longElement => LongToByte(longElement.Data),
                MiNum<ulong> ulongElement => UlongToByte(ulongElement.Data),
                MiNum<float> floatElement => SingleToByte(floatElement.Data),
                MiNum<double> doubleElement => DoubleToByte(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to uint8, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int16 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int16.</returns>
        public static short[] GetDataAsInt16(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToInt16(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToInt16(byteElement.Data),
                MiNum<int> intElement => IntToInt16(intElement.Data),
                MiNum<uint> uintElement => UintToInt16(uintElement.Data),
                MiNum<short> shortElement => shortElement.Data,
                MiNum<ushort> ushortElement => UshortToInt16(ushortElement.Data),
                MiNum<long> longElement => LongToInt16(longElement.Data),
                MiNum<ulong> ulongElement => UlongToInt16(ulongElement.Data),
                MiNum<float> floatElement => SingleToInt16(floatElement.Data),
                MiNum<double> doubleElement => DoubleToInt16(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to int16, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt16 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt16.</returns>
        public static ushort[] GetDataAsUInt16(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToUInt16(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToUInt16(byteElement.Data),
                MiNum<int> intElement => IntToUInt16(intElement.Data),
                MiNum<uint> uintElement => UintToUInt16(uintElement.Data),
                MiNum<short> shortElement => ShortToUInt16(shortElement.Data),
                MiNum<ushort> ushortElement => ushortElement.Data,
                MiNum<long> longElement => LongToUInt16(longElement.Data),
                MiNum<ulong> ulongElement => UlongToUInt16(ulongElement.Data),
                MiNum<float> floatElement => SingleToUInt16(floatElement.Data),
                MiNum<double> doubleElement => DoubleToUInt16(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to uint16, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int32 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int32.</returns>
        public static int[] GetDataAsInt32(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToInt32(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToInt32(byteElement.Data),
                MiNum<int> intElement => intElement.Data,
                MiNum<uint> uintElement => UintToInt32(uintElement.Data),
                MiNum<short> shortElement => ShortToInt32(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToInt32(ushortElement.Data),
                MiNum<long> longElement => LongToInt32(longElement.Data),
                MiNum<ulong> ulongElement => UlongToInt32(ulongElement.Data),
                MiNum<float> floatElement => SingleToInt32(floatElement.Data),
                MiNum<double> doubleElement => DoubleToInt32(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to int32, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt32 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt32.</returns>
        public static uint[] GetDataAsUInt32(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToUInt32(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToUInt32(byteElement.Data),
                MiNum<int> intElement => IntToUInt32(intElement.Data),
                MiNum<uint> uintElement => uintElement.Data,
                MiNum<short> shortElement => ShortToUInt32(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToUInt32(ushortElement.Data),
                MiNum<long> longElement => LongToUInt32(longElement.Data),
                MiNum<ulong> ulongElement => UlongToUInt32(ulongElement.Data),
                MiNum<float> floatElement => SingleToUInt32(floatElement.Data),
                MiNum<double> doubleElement => DoubleToUInt32(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to uint32, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of Int64 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to Int64.</returns>
        public static long[] GetDataAsInt64(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToInt64(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToInt64(byteElement.Data),
                MiNum<int> intElement => IntToInt64(intElement.Data),
                MiNum<uint> uintElement => UintToInt64(uintElement.Data),
                MiNum<short> shortElement => ShortToInt64(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToInt64(ushortElement.Data),
                MiNum<long> longElement => longElement.Data,
                MiNum<ulong> ulongElement => UlongToInt64(ulongElement.Data),
                MiNum<float> floatElement => SingleToInt64(floatElement.Data),
                MiNum<double> doubleElement => DoubleToInt64(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to int64, found {element.GetType()}."),
            };
        }

        /// <summary>
        /// Convert the contents of the Matlab data element to a sequence of UInt64 values.
        /// </summary>
        /// <param name="element">Data element.</param>
        /// <returns>Contents of the elements, converted to UInt64.</returns>
        public static ulong[] GetDataAsUInt64(DataElement element)
        {
            return element switch
            {
                MiNum<sbyte> sbyteElement => SbyteToUInt64(sbyteElement.Data),
                MiNum<byte> byteElement => ByteToUInt64(byteElement.Data),
                MiNum<int> intElement => IntToUInt64(intElement.Data),
                MiNum<uint> uintElement => UintToUInt64(uintElement.Data),
                MiNum<short> shortElement => ShortToUInt64(shortElement.Data),
                MiNum<ushort> ushortElement => UshortToUInt64(ushortElement.Data),
                MiNum<long> longElement => LongToUInt64(longElement.Data),
                MiNum<ulong> ulongElement => ulongElement.Data,
                MiNum<float> floatElement => SingleToUInt64(floatElement.Data),
                MiNum<double> doubleElement => DoubleToUInt64(doubleElement.Data),
                _ => throw new HandlerException(
                    $"Expected data element that would be convertible to uint64, found {element.GetType()}."),
            };
        }

        // * to double

        /// <summary>
        /// Convert an array of signed bytes to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] SbyteToDouble(sbyte[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of bytes to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] ByteToDouble(byte[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of shorts to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] ShortToDouble(short[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of unsigned shorts to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] UshortToDouble(ushort[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of integers to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] IntToDouble(int[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of unsigned integers to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] UintToDouble(uint[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of longs to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] LongToDouble(long[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of unsigned longs to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] UlongToDouble(ulong[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of floats to an array of doubles.
        /// </summary>
        /// <param name="source">Source array.</param>
        /// <returns>Converted array.</returns>
        public static double[] FloatToDouble(float[] source)
        {
            var result = new double[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToDouble(source[i]);
            }

            return result;
        }

        // * to single
        private static float[] SbyteToSingle(sbyte[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] ByteToSingle(byte[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] ShortToSingle(short[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] UshortToSingle(ushort[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] IntToSingle(int[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] UintToSingle(uint[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] LongToSingle(long[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] UlongToSingle(ulong[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        private static float[] DoubleToSingle(double[] source)
        {
            var result = new float[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSingle(source[i]);
            }

            return result;
        }

        // * to sbyte
        private static sbyte[] ByteToSByte(byte[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] ShortToSByte(short[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] UshortToSByte(ushort[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] IntToSByte(int[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] UintToSByte(uint[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] LongToSByte(long[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] UlongToSByte(ulong[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] SingleToSByte(float[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        private static sbyte[] DoubleToSByte(double[] source)
        {
            var result = new sbyte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToSByte(source[i]);
            }

            return result;
        }

        // * to byte
        private static byte[] SbyteToByte(sbyte[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] SingleToByte(float[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] ShortToByte(short[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] UshortToByte(ushort[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] IntToByte(int[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] UintToByte(uint[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] LongToByte(long[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] UlongToByte(ulong[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        private static byte[] DoubleToByte(double[] source)
        {
            var result = new byte[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToByte(source[i]);
            }

            return result;
        }

        // * to int16
        private static short[] SbyteToInt16(sbyte[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] ByteToInt16(byte[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] UshortToInt16(ushort[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] IntToInt16(int[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] UintToInt16(uint[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] LongToInt16(long[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] UlongToInt16(ulong[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] SingleToInt16(float[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        private static short[] DoubleToInt16(double[] source)
        {
            var result = new short[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt16(source[i]);
            }

            return result;
        }

        // * to uint16
        private static ushort[] SbyteToUInt16(sbyte[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] ByteToUInt16(byte[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] ShortToUInt16(short[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] IntToUInt16(int[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] UintToUInt16(uint[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] LongToUInt16(long[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] UlongToUInt16(ulong[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] SingleToUInt16(float[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        private static ushort[] DoubleToUInt16(double[] source)
        {
            var result = new ushort[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt16(source[i]);
            }

            return result;
        }

        // * to int32
        private static int[] SbyteToInt32(sbyte[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] ByteToInt32(byte[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] ShortToInt32(short[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] UshortToInt32(ushort[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] UintToInt32(uint[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] LongToInt32(long[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] UlongToInt32(ulong[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] SingleToInt32(float[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        private static int[] DoubleToInt32(double[] source)
        {
            var result = new int[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt32(source[i]);
            }

            return result;
        }

        // * to uint32
        private static uint[] SbyteToUInt32(sbyte[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] ByteToUInt32(byte[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] ShortToUInt32(short[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] UshortToUInt32(ushort[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] IntToUInt32(int[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] LongToUInt32(long[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] UlongToUInt32(ulong[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] SingleToUInt32(float[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        private static uint[] DoubleToUInt32(double[] source)
        {
            var result = new uint[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt32(source[i]);
            }

            return result;
        }

        // * to int64
        private static long[] SbyteToInt64(sbyte[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] ByteToInt64(byte[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] ShortToInt64(short[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] UshortToInt64(ushort[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] IntToInt64(int[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] UintToInt64(uint[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] UlongToInt64(ulong[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] SingleToInt64(float[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        private static long[] DoubleToInt64(double[] source)
        {
            var result = new long[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToInt64(source[i]);
            }

            return result;
        }

        // * to uint64
        private static ulong[] SbyteToUInt64(sbyte[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] ByteToUInt64(byte[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] ShortToUInt64(short[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] UshortToUInt64(ushort[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] IntToUInt64(int[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] UintToUInt64(uint[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] LongToUInt64(long[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] SingleToUInt64(float[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }

        private static ulong[] DoubleToUInt64(double[] source)
        {
            var result = new ulong[source.Length];
            for (var i = 0; i < source.Length; i++)
            {
                result[i] = Convert.ToUInt64(source[i]);
            }

            return result;
        }
    }
}
