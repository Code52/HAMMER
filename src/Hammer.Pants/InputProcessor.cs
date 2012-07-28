using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MadProps.AppArgs;

namespace HAMMER.Pants
{
    public class InputProcessor
    {
        const string DefaultHammerConfigFile = "hammer.config";
        // NOTE: this is included when you install VS2012 - so you can run this from Windows 7
        const string DefaultInputFile = @"\Windows Kits\8.0\Include\winrt\xaml\design\generic.xaml";

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        public static PantsArgs GetArgs(string[] args)
        {
            const string output = "Generic.xaml";
            try
            {
                var resolvedInputFilePath = ProgramFilesx86() + DefaultInputFile;
                var appArgs = args.As<PantsArgs>();
                if (String.IsNullOrEmpty(appArgs.InputFile))
                {
                    appArgs.InputFile = resolvedInputFilePath;
                }
                if (String.IsNullOrEmpty(appArgs.OutputFile))
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

        public static PantsArgs GetArgsFromFile(string[] args)
        {
            try
            {
                var resolvedInputFilePath = ProgramFilesx86() + DefaultInputFile;
                var configArgs = args.As<PantsWithConfigArgs>();

                if (string.IsNullOrEmpty(configArgs.ConfigFile))
                {
                    if (File.Exists(DefaultHammerConfigFile))
                    {
                        configArgs.ConfigFile = DefaultHammerConfigFile;
                    }
                }

                var file = XDocument.Load(configArgs.ConfigFile);

                
                var settings = file.Descendants("appSettings");

                var appArgs = new PantsArgs
                {
                    Colour = GetNodeByKey(settings, "colour"),
                    InputFile = GetNodeByKey(settings, "input-file")
                };

                if (string.IsNullOrEmpty(appArgs.InputFile))
                {
                    appArgs.InputFile = resolvedInputFilePath;
                }
                if (string.IsNullOrEmpty(appArgs.OutputFile))
                {
                    appArgs.OutputFile = appArgs.InputFile;
                }
                return appArgs;
            }
            catch (Exception ex)
            {
                return new PantsArgs(ex);
            }
        }

        static string GetNodeByKey(IEnumerable<XElement> settings, string key)
        {
            return settings.Descendants().FirstOrDefault(x => x.Attribute("key").Value == key)
                .Attribute("value").Value;
        }
    }
}