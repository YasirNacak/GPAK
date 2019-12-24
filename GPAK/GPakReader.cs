using System;
using System.Collections.Generic;
using System.IO;

namespace GPAK
{
    public class GPakReader
    {
        public string PackagePath { get; }

        public string PackageName { get; }

        public int PackageFileSize { get; }

        public int EntryCount { get; private set; }

        private readonly BinaryReader _packageFileReader;

        /// <summary>
        /// For quick access when an entry is requested
        /// </summary>
        private readonly Dictionary<string, GPakEntry> _entryTable;

        public GPakReader(string filename)
        {
            PackagePath = GPakUtil.GetPackageFileName(filename);
            PackageName = Path.GetFileName(PackagePath);

            if (File.Exists(PackagePath))
            {
                PackageFileSize = (int)new FileInfo(PackagePath).Length;
                _packageFileReader = new BinaryReader(File.Open(PackagePath, System.IO.FileMode.Open));

                if (IsHeaderValid())
                {
                    _entryTable = new Dictionary<string, GPakEntry>();
                    ReadEntries();
                }
                else
                {
                    throw new Exception("Invalid Header");
                }
            }
            else
            {
                throw new FileNotFoundException(PackagePath + " does not found.");
            }
        }

        private bool IsHeaderValid()
        {
            var header = GPakUtil.GetBytesFromFile(_packageFileReader, 0, GPakUtil.FirstEntryOffset);
            return GPakUtil.GetByteRangeAsString(header).Equals(GPakUtil.Header);
        }

        private void ReadEntries()
        {
            int i = GPakUtil.FirstEntryOffset;
            while (i < PackageFileSize)
            {
                // Read 4 bytes to find the length of the entry's name
                var entryNameLength = 
                    GPakUtil.GetByteRangeAsInteger(GPakUtil.GetBytesFromFile(_packageFileReader, i, 4));
                i += 4;

                // Read ENTRY_NAME bytes to obtain it's name
                var entryName = 
                    GPakUtil.GetByteRangeAsString(GPakUtil.GetBytesFromFile(_packageFileReader, i,entryNameLength));
                i += entryName.Length;

                // Read 1 byte to check if the entry is compressed or not ("C" for compressed, "N" for not compressed)
                var compressedFlag =
                    GPakUtil.GetByteRangeAsString(GPakUtil.GetBytesFromFile(_packageFileReader, i, 1));
                i += 1;
                var isCompressed = compressedFlag == "C";

                // Read 4 bytes to get the size of the entry in the package
                var entryContentSize =
                    GPakUtil.GetByteRangeAsInteger(GPakUtil.GetBytesFromFile(_packageFileReader, i, 4));
                i += 4;

                // Create a new entry based on the previously gathered info
                _entryTable.Add(entryName, new GPakEntry(entryContentSize, isCompressed, i));

                i += entryContentSize;

                EntryCount++;
            }

            _packageFileReader.Close();
        }

        public byte[] GetEntryContent(string entryName)
        {
            var entry = _entryTable[entryName];

            if (entry == null)
            {
                return null;
            }

            var extractReader = new BinaryReader(File.Open(PackagePath, System.IO.FileMode.Open));

            var result = entry.IsCompressed
                ? GPakUtil.Decompress(GPakUtil.GetBytesFromFile(extractReader, entry.ContentOffset, entry.SizeInBytes))
                : GPakUtil.GetBytesFromFile(extractReader, entry.ContentOffset, entry.SizeInBytes);

            extractReader.Close();

            return result;
        }

        public List<string> GetAllEntryNames()
        {
            var result = new List<string>();

            foreach (var (name, entry) in _entryTable)
            {
                result.Add(name);
            }

            return result;
        }

        public void DumpInfo()
        {
            Console.WriteLine($@"File Name: {PackagePath}");
            Console.WriteLine($@"Number of Entries: {_entryTable.Count}");

            Console.WriteLine("Entries:");

            foreach (var (name, entry) in _entryTable)
            {
                Console.WriteLine($@"ENTRY");
                Console.WriteLine($@"Name: {name}");
                Console.WriteLine($@"Is Compressed: {entry.IsCompressed}");
                Console.WriteLine($@"Size: {entry.SizeInBytes} bytes");
                Console.WriteLine($@"Content Offset in Package: {entry.ContentOffset}");
            }
        }

        public void ExtractAll(string toDirectory = "")
        {
            if (toDirectory != "" && !Directory.Exists(toDirectory))
            {
                Directory.CreateDirectory(toDirectory);
            }

            var extractReader = new BinaryReader(File.Open(PackagePath, System.IO.FileMode.Open));

            foreach (var (name, entry) in _entryTable)
            {
                var writer = new BinaryWriter(File.Open(Path.Combine(toDirectory, name), System.IO.FileMode.Create));

                writer.Write(entry.IsCompressed
                    ? GPakUtil.Decompress(GPakUtil.GetBytesFromFile(extractReader, entry.ContentOffset, entry.SizeInBytes))
                    : GPakUtil.GetBytesFromFile(extractReader, entry.ContentOffset, entry.SizeInBytes));

                writer.Close();
            }

            extractReader.Close();
        }

        public void ExtractOne(string entryName, string toDirectory = "")
        {
            if (toDirectory != "" && !Directory.Exists(toDirectory))
            {
                Directory.CreateDirectory(toDirectory);
            }

            var entryContent = GetEntryContent(entryName);

            if (entryContent == null) return;
            
            var writer = new BinaryWriter(File.Open(Path.Combine(toDirectory, entryName), System.IO.FileMode.Create));

            writer.Write(entryContent);

            writer.Close();
        }
    }
}