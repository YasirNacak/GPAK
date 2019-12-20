using System;
using System.Collections.Generic;
using System.IO;

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
                Console.WriteLine($@"Is Compressed: {_entries[i].IsCompressed}");
                Console.WriteLine($@"Size: {_entries[i].SizeInBytes} bytes");
            }
        }

        public void Extract(string toDirectory = "")
        {
            if (toDirectory != "" && !Directory.Exists(toDirectory))
            {
                Directory.CreateDirectory(toDirectory);
            }

            foreach (var entry in _entries)
            {
                var writer = new BinaryWriter(File.Open(Path.Combine(toDirectory, entry.Name), System.IO.FileMode.Create));

                writer.Write(entry.IsCompressed ? GPakUtil.Decompress(entry.Content) : entry.Content);

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

        private bool IsHeaderValid()
        {
            bool isFormatNameCorrect = GPakUtil.GetByteRangeAsString(_packageBytes, 0, 3).Equals(GPakUtil.GetExtension());
            bool isMagicNumberCorrect = GPakUtil.GetByteRangeAsInteger(_packageBytes, 4, 4).Equals(GPakUtil.MagicNumber);
            bool isVersionCorrect = GPakUtil.GetByteRangeAsInteger(_packageBytes, 5, 5).Equals(GPakUtil.Version);

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
                var entryNameLength = 
                    GPakUtil.GetByteRangeAsInteger(_packageBytes, i, i);
                
                var entryName = 
                    GPakUtil.GetByteRangeAsString(_packageBytes, i + 1, i + entryNameLength);
                
                var compressedFlag =
                    GPakUtil.GetByteRangeAsString(_packageBytes, i + entryNameLength + 1, i + entryNameLength + 1);

                var entryContentSize = 
                    GPakUtil.GetByteRangeAsInteger(_packageBytes, i + entryNameLength + 2, i + entryNameLength + 5);
                
                var entryContent = 
                    GPakUtil.GetByteRangeSubset(_packageBytes, i + entryNameLength + 6, entryContentSize);

                i = i + entryNameLength + 5 + entryContentSize;

                var isCompressed = compressedFlag == "C";

                _entries.Add(new GPakEntry(entryNameLength, entryName, entryContentSize, entryContent, isCompressed));
            }
        }
    }
}