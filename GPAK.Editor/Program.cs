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
                if (args.Length != 4)
                {
                    PrintWriteModeInfo();
                    return;
                }

                if (args[1] != "-c" && args[1] != "-n")
                {
                    PrintWriteModeInfo();
                    return;
                }

                bool shouldCompress = args[1] == "-c";

                var packageFileName = args[2];

                var inputFileName = args[3];

                var gpWriter = new GPakWriter(packageFileName);
                gpWriter.AddEntry(inputFileName, shouldCompress);
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
                var packageFilename = args[1];
                var gpReader = new GPakReader(packageFilename);
                var dirToExtract = "";

                if (args.Length == 3)
                {
                    dirToExtract = args[2];
                }

                if (args.Length == 3 || args.Length == 2)
                {
                    gpReader.ExtractAll(dirToExtract);
                }
                else
                {
                    PrintExtractModeInfo();
                }
            }
            else
            {
                PrintHelp();
            }

            Console.WriteLine("Operation Finished Successfully.");
            Console.ReadKey();
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
            Console.WriteLine("[-w] [-c (compress)/-n (no compression)] [package filename] [input filename] For writing input file to package file.");
        }

        private static void PrintReadModeInfo()
        {
            Console.WriteLine("[-r] [package filename] For reading package file and dumping summary of it's contents.");
        }

        private static void PrintExtractModeInfo()
        {
            Console.WriteLine("[-x] [package filename] [directory to extract (can be empty)] For extracting package file to the current or given directory.");
        }
    }
}
