using System;

namespace GPAK.Editor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            if (args[0].Equals("-h"))
            {
                PrintHelp();
                return;
            }

            if (args[0].Equals("-w"))
            {
                if (args.Length != 3)
                {
                    PrintWriteModeInfo();
                    return;
                }

                var packageFileName = args[1];

                var inputFileName = args[2];

                var gpWriter = new GPakWriter(packageFileName);
                gpWriter.AddEntry(inputFileName);
            }
            else if (args[0].Equals("-r"))
            {
                if (args.Length != 2)
                {
                    PrintReadModeInfo();
                    return;
                }

                var packageFileName = args[1];

                var gpReader = new GPakReader(packageFileName);
                gpReader.DumpInfo();
            }
            else if (args[0].Equals("-x"))
            {
                if (args.Length != 2)
                {
                    PrintExtractModeInfo();
                    return;
                }

                var packageFileName = args[1];

                var gpReader = new GPakReader(packageFileName);
                gpReader.Extract();
            }
            else
            {
                PrintHelp();
                return;
            }

            Console.WriteLine("Operation Finished Successfully.");
        }

        private static void PrintHelp()
        {
            Console.WriteLine("[-h] for viewing this information");
            PrintReadModeInfo();
            PrintExtractModeInfo();
            PrintWriteModeInfo();
        }

        private static void PrintWriteModeInfo()
        {
            Console.WriteLine("[-w] [package filename] [input filename] For writing input file to package file.");
        }

        private static void PrintReadModeInfo()
        {
            Console.WriteLine("[-r] [package filename] For reading package file and dumping summary of it's contents.");
        }

        private static void PrintExtractModeInfo()
        {
            Console.WriteLine("[-x] [package filename] For extracting package file to the current directory.");
        }
    }
}
