namespace GPAK
{
    public class GPakEntry
    {
        public int NameLength { get; }
        public string Name { get; }
        public int SizeInBytes { get; }
        public byte[] Content { get; }
        public bool IsCompressed { get; }

        public GPakEntry(int nameLength, string name, int sizeInBytes, byte[] content, bool isCompressed)
        {
            NameLength = nameLength;
            Name = name;
            SizeInBytes = sizeInBytes;
            Content = content;
            IsCompressed = isCompressed;
        }
    }
}