namespace GPAK
{
    public class GPakEntry
    {
        public byte NameLength;
        public byte[] Name;
        public uint SizeInBytes;
        public byte[] Content;

        public GPakEntry(byte nameLength, byte[] name, uint sizeInBytes, byte[] content)
        {
            NameLength = nameLength;
            Name = name;
            SizeInBytes = sizeInBytes;
            Content = content;
        }
    }
}