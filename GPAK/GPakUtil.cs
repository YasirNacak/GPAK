using System;
using System.IO;
using System.IO.Compression;

namespace GPAK
{
    public static class GPakUtil
    {
        public static readonly char[] FormatName = { 'G', 'P', 'A', 'K' };
        public const byte MagicNumber = 78;
        public const byte Version = 1;
        public const byte FirstEntryOffset = 6;

        public static string GetExtension()
        {
            var result = "";
            result += FormatName[0];
            result += FormatName[1];
            result += FormatName[2];
            result += FormatName[3];
            return result;
        }

        public static string GetPackageFileName(string filename)
        {
            if (!filename.EndsWith(GetExtension()))
            {
                return filename + "." + GetExtension();
            }

            return filename;
        }

        public static string GetByteRangeAsString(byte[] data, int startByte, int endByte)
        {
            var result = "";

            for (int i = startByte; i <= endByte; i++)
            {
                result += (char)data[i];
            }

            return result;
        }

        public static int GetByteRangeAsInteger(byte[] data, int startByte, int endByte)
        {
            byte[] bytesToConvert = new byte[4];

            int toConvertIndex = 0;

            for (int i = startByte; i <= endByte; i++)
            {
                bytesToConvert[toConvertIndex] = data[i];
                toConvertIndex++;
            }

            return BitConverter.ToInt32(bytesToConvert, 0);
        }

        public static byte[] GetByteRangeSubset(byte[] data, int startIndex, int length)
        {
            byte[] subset = new byte[length];

            Array.Copy(data, startIndex, subset, 0, length);

            return subset;
        }

        public static byte[] Compress(byte[] data)
        {
            var output = new MemoryStream();

            using (var dStream = new DeflateStream(output, CompressionLevel.Optimal))
            {
                dStream.Write(data, 0, data.Length);
            }

            return output.ToArray();
        }

        public static byte[] Decompress(byte[] data)
        {
            var input = new MemoryStream(data);
            var output = new MemoryStream();

            using (var dStream = new DeflateStream(input, CompressionMode.Decompress))
            {
                dStream.CopyTo(output);
            }

            return output.ToArray();
        }
    }
}
