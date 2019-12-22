using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace GPAK
{
    public static class GPakUtil
    {
        public const string FormatName = "GPAK";

        public const string Header = "GP78";

        public const byte FirstEntryOffset = 4;

        public static string GetPackageFileName(string filename)
        {
            if (!filename.ToUpper().EndsWith(FormatName))
            {
                return filename + "." + FormatName;
            }

            return filename;
        }

        public static byte[] GetBytesFromFile(BinaryReader reader, int offset, int byteCount)
        {
            var result = new byte[byteCount];

            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            reader.Read(result, 0, byteCount);

            return result;
        }

        public static string GetByteRangeAsString(byte[] data)
        {
            return data.Aggregate("", (current, t) => current + (char) t);
        }

        public static int GetByteRangeAsInteger(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
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
