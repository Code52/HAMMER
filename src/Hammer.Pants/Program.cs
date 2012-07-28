using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MadProps.AppArgs;

namespace HAMMER.Pants
{
    class Program
    {
        // NOTE: this is included when you install VS2012 - so you can run this from Windows 7
        const string DefaultInputFile = @"\Windows Kits\8.0\Include\winrt\xaml\design\generic.xaml";
        static readonly XNamespace XamlPresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        static void Main(string[] args)
        {
            // verify the input arguments
            var argsWithColourSpecified = ProcessInputRequiringColour(args);
            if (argsWithColourSpecified.Exception == null)
            {
                Process(argsWithColourSpecified.InputFile, argsWithColourSpecified.Colour, argsWithColourSpecified.OutputFile);
            }
            else
            {
                Console.WriteLine(argsWithColourSpecified.Exception.Message);
                Console.WriteLine();
                Console.WriteLine(AppArgs.HelpFor<PantsArgs>());
                WaitForInput();
                return;
            }

            var argsWithAppSpecified = ProcessInputRequiringFile(args);
            if (argsWithAppSpecified.Exception == null)
            {
                 Process(argsWithColourSpecified.InputFile, argsWithColourSpecified.Colour, argsWithColourSpecified.OutputFile);
            }
            else
            {
                Console.WriteLine(argsWithAppSpecified.Exception.Message);
                Console.WriteLine();
                Console.WriteLine(AppArgs.HelpFor<PantsArgs>());
                WaitForInput();
                return;
            }

            WaitForInput();
        }

        private static void Process(string inputFile, string baseColourHex, string outputFile)
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Default generic.xaml file '{0}' could not be found.", inputFile);
                Console.WriteLine("You must specify a value for the /inputfile parameter to continue.");
                Console.WriteLine("You can grab a version from https://raw.github.com/Code52/HAMMER/master/SampleData/generic.xaml and add it to this folder...");
                return;
            }

            var doc = XDocument.Load(inputFile);
            var dictionary = doc.Element(XamlPresentationNamespace + "ResourceDictionary");
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

        static PantsArgs ProcessInputRequiringColour(string[] args)
        {
            const string output = "Generic.xaml";
            try
            {
                var resolvedInputFilePath = ProgramFilesx86() + DefaultInputFile;
                var appArgs = args.As<PantsArgs>();
                if (string.IsNullOrEmpty(appArgs.InputFile))
                {
                    appArgs.InputFile = resolvedInputFilePath;
                }
                if (string.IsNullOrEmpty(appArgs.OutputFile))
                {
                    appArgs.OutputFile = output;
                }
                return appArgs;
            }
            catch (ArgumentException ex)
            {
                return new PantsArgs(ex);
            }
        }

        static PantsArgs ProcessInputRequiringFile(string[] args)
        {
            const string output = "Themes/Generic.xaml";
            try
            {
                var resolvedInputFilePath = ProgramFilesx86() + DefaultInputFile;
                var configArgs = args.As<PantsWithConfigArgs>();

                var file = XDocument.Load(configArgs.ConfigFile);

                var appArgs = new PantsArgs
                                  {
                                      Colour = file.Descendants("Colour").First().Value,
                                  };

                if (string.IsNullOrEmpty(appArgs.InputFile))
                {
                    appArgs.InputFile = resolvedInputFilePath;
                }
                if (string.IsNullOrEmpty(appArgs.OutputFile))
                {
                    appArgs.OutputFile = output;
                }
                return appArgs;
            }
            catch (ArgumentException ex)
            {
                return new PantsArgs(ex);
            }
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