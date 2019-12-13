using System;
using System.Data;
using System.IO;
using System.Text;

namespace GPAK
{
    public class GPakFile
    {
        public char[] FormatName = {'G', 'P', 'A', 'K'};
        public const byte MagicNumber = 78;
        public const byte Version = 1;

        private readonly string _packageFilename;
        private byte[] _fileContent;

        private BinaryWriter _fileWriter;

        public GPakFile(string filename, FileMode mode)
        {
            _packageFilename = filename + ".GPAK";

            switch (mode)
            {
                case FileMode.Read:
                    ReadFile();
                    break;
                case FileMode.Write:
                    CreateFile();
                    break;
            }
        }

        private void ReadFile()
        {
            _fileContent = File.ReadAllBytes(_packageFilename);
            ParseContent();
        }

        private void ParseContent()
        {

        }

        private void StartWriting()
        {
            _fileWriter = new BinaryWriter(File.Open(_packageFilename, System.IO.FileMode.Append));
        }

        private void EndWriting()
        {
            _fileWriter.Close();
        }

        private void CreateFile()
        {
            bool doesFileExist = File.Exists(_packageFilename);

            StartWriting();

            if (!doesFileExist)
            {
                _fileWriter.Write(FormatName);
                _fileWriter.Write(MagicNumber);
                _fileWriter.Write(Version);
            }

            EndWriting();
        }

        public void AddEntry(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(filename + " could not be opened!");
            }

            if (filename.Length > byte.MaxValue)
            {
                throw new ConstraintException("file name length can't be greater than " + byte.MaxValue);
            }

            StartWriting();

            var fi = new FileInfo(filename);

            var fnLength = Convert.ToByte(Encoding.ASCII.GetByteCount(fi.Name));
            var fnContent = Encoding.ASCII.GetBytes(fi.Name);
            var fSize = Convert.ToUInt32(fi.Length);
            var fContent = File.ReadAllBytes(filename);

            _fileWriter.Write(fnLength);
            _fileWriter.Write(fnContent);
            _fileWriter.Write(fSize);
            _fileWriter.Write(fContent);

            EndWriting();
        }
    }
}