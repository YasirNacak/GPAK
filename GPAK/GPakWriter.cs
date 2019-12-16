using System;
using System.Data;
using System.IO;
using System.Text;

namespace GPAK
{
    public class GPakWriter
    {
        private readonly string _packageFilename;

        private BinaryWriter _fileWriter;

        public GPakWriter(string filename)
        {
            _packageFilename = GPakUtil.GetPackageFileName(filename);

            bool doesFileExist = File.Exists(_packageFilename);

            StartWriting();

            if (!doesFileExist)
            {
                WriteHeader();
            }

            EndWriting();
        }

        private void StartWriting()
        {
            _fileWriter = new BinaryWriter(File.Open(_packageFilename, System.IO.FileMode.Append));
        }

        private void EndWriting()
        {
            _fileWriter.Close();
        }

        private void WriteHeader()
        {
            _fileWriter.Write(GPakUtil.FormatName);
            _fileWriter.Write(GPakUtil.MagicNumber);
            _fileWriter.Write(GPakUtil.Version);
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