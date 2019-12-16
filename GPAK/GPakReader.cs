using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GPAK
{
    public class GPakReader
    {
        private readonly string _packageFilename;

        private BinaryReader _fileReader;

        private bool _isPackageValid;

        private byte[] _packageBytes;

        private const int _bufferSize = 4096;

        private List<GPakEntry> _entries;


        public GPakReader(string filename)
        {
            _packageFilename = GPakUtil.GetPackageFileName(filename);

            if (File.Exists(_packageFilename))
            {
                _fileReader = new BinaryReader(File.Open(_packageFilename, System.IO.FileMode.Open));
                _packageBytes = ReadAllBytes();
                _fileReader.Close();

                _isPackageValid = IsHeaderValid();

                if (_isPackageValid)
                {
                    _entries = new List<GPakEntry>();
                    ReadEntries();
                }
            }
            else
            {
                throw new FileNotFoundException(_packageFilename + " does not found.");
            }
        }

        public void DumpInfo()
        {
            Console.WriteLine($@"File Name: {_packageFilename}");
            Console.WriteLine($@"File Size: {_packageBytes.Length} bytes");
            Console.WriteLine($@"Number of Entries: {_entries.Count}");

            Console.WriteLine("Entries:");

            for (int i = 0; i < _entries.Count; i++)
            {
                Console.WriteLine($@"ENTRY #{i + 1}");
                Console.WriteLine($@"Name: {_entries[i].Name}");
                Console.WriteLine($@"Size: {_entries[i].SizeInBytes} bytes");

            }
        }

        public void Extract()
        {
            foreach (var entry in _entries)
            {
                var writer = new BinaryWriter(File.Open(entry.Name, System.IO.FileMode.Create));
                writer.Write(entry.Content);
                writer.Close();
            }
        }

        private byte[] ReadAllBytes()
        {
            var memStream = new MemoryStream();
            var buffer = new byte[_bufferSize];

            int count;
            while ((count = _fileReader.Read(buffer, 0, buffer.Length)) != 0)
            {
                memStream.Write(buffer, 0, count);
            }

            return memStream.ToArray();
        }

        private string GetByteRangeAsString(int startByte, int endByte)
        {
            var result = "";

            for (int i = startByte; i <= endByte; i++)
            {
                result += (char)_packageBytes[i];
            }

            return result;
        }

        private int GetByteRangeAsInteger(int startByte, int endByte)
        {
            byte[] bytesToConvert = new byte[4];

            int toConvertIndex = 0;

            for (int i = startByte; i <= endByte; i++)
            {
                bytesToConvert[toConvertIndex] = _packageBytes[i];
                toConvertIndex++;
            }

            return BitConverter.ToInt32(bytesToConvert, 0);
        }

        private byte[] RangeSubset(int startIndex, int length)
        {
            byte[] subset = new byte[length];

            Array.Copy(_packageBytes, startIndex, subset, 0, length);

            return subset;
        }

        private bool IsHeaderValid()
        {
            bool isFormatNameCorrect = GetByteRangeAsString(0, 3).Equals(GPakUtil.GetExtension());
            bool isMagicNumberCorrect = GetByteRangeAsInteger(4, 4).Equals(GPakUtil.MagicNumber);
            bool isVersionCorrect = GetByteRangeAsInteger(5, 5).Equals(GPakUtil.Version);

            if (!isFormatNameCorrect)
            {
                throw new Exception("Format Name chunk is wrong.");
            }

            if (!isMagicNumberCorrect)
            {
                throw new Exception("Magic Number chunk is wrong.");
            }

            if (!isVersionCorrect)
            {
                throw new Exception("Incompatible GPAK version.");
            }

            return true;
        }

        private void ReadEntries()
        {
            for (int i = GPakUtil.FirstEntryOffset; i < _packageBytes.Length; i++)
            {
                var entryNameLength = GetByteRangeAsInteger(i, i);
                var entryName = GetByteRangeAsString(i + 1, i + entryNameLength);
                var entryContentSize = GetByteRangeAsInteger(i + entryNameLength + 1, i + entryNameLength + 4);
                var entryContent = RangeSubset(i + entryNameLength + 5, entryContentSize);

                // TODO (yasir): remove when you are sure that reading works correctly
                if(false)
                {
                    Console.WriteLine($@"Entry Name Length is at Bytes {i} - {i}");
                    Console.WriteLine($@"Entry Name is at Bytes {i + 1} - {i + entryNameLength}");
                    Console.WriteLine($@"Entry Content Length is at Bytes {i + entryNameLength + 1} - {i + entryNameLength + 4}");
                    Console.WriteLine($@"Entry Content is at Bytes {i + entryNameLength + 5} - {i + entryNameLength + 5 + entryContentSize - 1}");
                    Console.WriteLine("=============================================");
                    Console.WriteLine($@"Entry Name Length: {entryNameLength}");
                    Console.WriteLine($@"Entry Name: {entryName}");
                    Console.WriteLine($@"Entry Content Size: {entryContentSize}");
                    Console.WriteLine("=================END ENTRY===================");

                    if (entryContentSize < 50)
                    {
                        Console.Write("!!!");
                        for (int j = 0; j < entryContent.Length; j++)
                        {
                            Console.Write((char)entryContent[j]);
                        }
                        Console.Write("!!!");
                        Console.WriteLine();
                    }
                }

                i = i + entryNameLength + 4 + entryContentSize;

                _entries.Add(new GPakEntry(entryNameLength, entryName, entryContentSize, entryContent));
            }
        }
    }
}