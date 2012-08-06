using System;
using System.IO;
using System.Xml.Linq;
using MadProps.AppArgs;

namespace HAMMER.Pants
{
    class Program
    {
        static void Main(string[] args)
        {
            // verify the input arguments
            var argsWithColourSpecified = InputProcessor.GetArgs(args);
            var argsWithAppSpecified = InputProcessor.GetArgsFromFile(args);

            if (argsWithColourSpecified.Exception != null 
                && argsWithAppSpecified.Exception != null)
            {
                Console.WriteLine(argsWithColourSpecified.Exception.Message);
                Console.WriteLine();
                Console.WriteLine(AppArgs.HelpFor<PantsArgs>());

                Console.WriteLine(argsWithAppSpecified.Exception.Message);
                Console.WriteLine();
                Console.WriteLine(AppArgs.HelpFor<PantsWithConfigArgs>());
                WaitForInput();
                return;
            }

            if (argsWithColourSpecified.Exception == null)
            {
                Process(argsWithColourSpecified.InputFile, argsWithColourSpecified.Colour, argsWithColourSpecified.OutputFile);
            }
            else if (argsWithAppSpecified.Exception == null)
            {
                Process(argsWithAppSpecified.InputFile, argsWithAppSpecified.Colour, argsWithAppSpecified.OutputFile);
            }
            
            WaitForInput();
        }

        private static void Process(string inputFile, string baseColourHex, string outputFile)
        {
            var directory = Directory.GetCurrentDirectory();
            Console.WriteLine("Working from directory '{0}'", directory);
            var path = Path.GetFullPath(inputFile);

            if (!File.Exists(path))
            {
                Console.WriteLine("Input resource file '{0}' could not be found.", path);
                Console.WriteLine("You must specify a value for the /inputfile parameter to continue.");
                Console.WriteLine("You can grab a version from https://raw.github.com/Code52/HAMMER/master/SampleData/generic.xaml and add it to this folder...");
                return;
            }

            var doc = XDocument.Load(inputFile);
            var dictionary = doc.Element(ResourceFileParser.XamlPresentationNamespace + "ResourceDictionary");
            if (dictionary == null)
            {
                Console.WriteLine("Error: Input file '{0}' does not contain a resource dictionary", inputFile);
                return;
            }

            var parser = new ResourceFileParser(doc);
            var postProcessedFile = parser.Update(baseColourHex);
            postProcessedFile.Save(outputFile);

            Console.WriteLine("Completed");
            Console.WriteLine("Output file is at '{0}'", Path.GetFullPath(outputFile));
            Console.WriteLine("Press any key to finish");
        }

        static void WaitForInput()
        {
#if DEBUG
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
#endif
        }
    }
}