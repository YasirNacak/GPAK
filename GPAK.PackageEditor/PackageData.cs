using System.IO;

namespace GPAK.PackageEditor
{
    public static class PackageData
    {
        public static GPakReader Reader;
        public static GPakWriter Writer;

        public static void LoadPackage(string path)
        {
            Reader = new GPakReader(path);
            Writer = new GPakWriter(path);
        }
    }
}