namespace GPAK
{
    public static class GPakUtil
    {
        public static readonly char[] FormatName = { 'G', 'P', 'A', 'K' };
        public const byte MagicNumber = 78;
        public const byte Version = 1;
        public const byte FirstEntryOffset = 6;

        public static string GetExtension()
        {
            var result = "";
            result += FormatName[0];
            result += FormatName[1];
            result += FormatName[2];
            result += FormatName[3];
            return result;
        }

        public static string GetPackageFileName(string filename)
        {
            if (!filename.EndsWith(GetExtension()))
            {
                return filename + "." + GetExtension();
            }

            return filename;
        }
    }
}