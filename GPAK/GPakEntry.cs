namespace GPAK
{
    public class GPakEntry
    {
        public int SizeInBytes { get; }
        public bool IsCompressed { get; }
        public int ContentOffset { get; }

        public GPakEntry(int sizeInBytes, bool isCompressed, int contentOffset)
        {
            SizeInBytes = sizeInBytes;
            IsCompressed = isCompressed;
            ContentOffset = contentOffset;
        }
    }
}