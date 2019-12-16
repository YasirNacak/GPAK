namespace GPAK
{
    public class GPakEntry
    {
        public int NameLength;
        public string Name;
        public int SizeInBytes;
        public byte[] Content;

        public GPakEntry(int nameLength, string name, int sizeInBytes, byte[] content)
        {
            NameLength = nameLength;
            Name = name;
            SizeInBytes = sizeInBytes;
            Content = content;
        }
    }
}