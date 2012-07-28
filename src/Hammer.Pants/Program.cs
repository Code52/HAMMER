using System;
using System.Collections.Generic;
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
        const double BaseLuminosity = 95.5294132232666;
        static readonly XNamespace XamlPresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        static readonly XNamespace XamlRootNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

        static readonly Dictionary<string, Dictionary<string, double>> Brushes = new Dictionary<string, Dictionary<string, double>>
                          {
                              {"Default", new Dictionary<string, double>{
                                            {"ComboBoxItemSelectedBackgroundThemeBrush", 0},
                                            {"ComboBoxItemSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"ComboBoxSelectedBackgroundThemeBrush", 0},
                                            {"ComboBoxSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"HyperlinkForegroundThemeBrush", 78.1176424026489},
                                            {"HyperlinkPointerOverForegroundThemeBrush", 78.1176424026489},
                                            {"HyperlinkPressedForegroundThemeBrush", 78.1176424026489},
                                            {"ListBoxItemSelectedBackgroundThemeBrush", 0},
                                            {"ListBoxItemSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"ListViewItemCheckHintThemeBrush", 144.470586776733},
                                            {"ListViewItemCheckSelectingThemeBrush", 144.470586776733},
                                            {"ListViewItemDragBackgroundThemeBrush", 0},
                                            {"ListViewItemSelectedBackgroundThemeBrush", 0},
                                            {"ListViewItemSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"ListViewItemSelectedPointerOverBorderThemeBrush", 19.7647047042847},
                                            {"ProgressBarForegroundThemeBrush", 18.8235282897949},
                                            {"ProgressBarIndeterminateForegroundThemeBrush", 65.4117679595947},
                                            {"SliderTrackDecreaseBackgroundThemeBrush", 18.8235282897949},
                                            {"SliderTrackDecreasePointerOverBackgroundThemeBrush", 36.2352991104126},
                                            {"SliderTrackDecreasePressedBackgroundThemeBrush", 55.529408454895},
                                            {"ToggleSwitchCurtainBackgroundThemeBrush", 14.5882344245911},
                                            {"ToggleSwitchCurtainPointerOverBackgroundThemeBrush", 32.4705934524536},
                                            {"ToggleSwitchCurtainPressedBackgroundThemeBrush", 52.7058792114258},
                            }},
                            {"Light", new Dictionary<string, double>{
                                            {"ComboBoxItemSelectedBackgroundThemeBrush", 0},
                                            {"ComboBoxItemSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"ComboBoxSelectedBackgroundThemeBrush", 0},
                                            {"ComboBoxSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"HyperlinkForegroundThemeBrush", 12.2352933883667},
                                            {"HyperlinkPointerOverForegroundThemeBrush", 12.2352933883667},
                                            {"HyperlinkPressedForegroundThemeBrush", 12.2352933883667},
                                            {"ListBoxItemSelectedBackgroundThemeBrush", 0},
                                            {"ListBoxItemSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"ListViewItemCheckHintThemeBrush", 0},
                                            {"ListViewItemCheckSelectingThemeBrush", 0},
                                            {"ListViewItemDragBackgroundThemeBrush", 0},
                                            {"ListViewItemSelectedBackgroundThemeBrush", 0},
                                            {"ListViewItemSelectedPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"ListViewItemSelectedPointerOverBorderThemeBrush", 19.7647047042847},
                                            {"ProgressBarForegroundThemeBrush", 0},
                                            {"ProgressBarIndeterminateForegroundThemeBrush", 0},
                                            {"SliderTrackDecreaseBackgroundThemeBrush", 0},
                                            {"SliderTrackDecreasePointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"SliderTrackDecreasePressedBackgroundThemeBrush", 42.3529386520386},
                                            {"ToggleSwitchCurtainBackgroundThemeBrush", 0},
                                            {"ToggleSwitchCurtainPointerOverBackgroundThemeBrush", 19.7647047042847},
                                            {"ToggleSwitchCurtainPressedBackgroundThemeBrush", 42.3529386520386},
                                            }}
                          };

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
            HSLColor baseColour;
            PantsArgs appArgs;

            string inputFile;
            var resolvedInputFilePath = ProgramFilesx86() + DefaultInputFile;

            // verify the input arguments
            var output = "Generic.xaml";
            try
            {
                appArgs = args.As<PantsArgs>();
                inputFile = string.IsNullOrEmpty(appArgs.InputFile)
                                        ? resolvedInputFilePath
                                        : appArgs.InputFile;

                if (!string.IsNullOrEmpty(appArgs.OutputFile))
                    output = appArgs.OutputFile;

                baseColour = new HSLColor(appArgs.Colour);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(AppArgs.HelpFor<PantsArgs>());
                WaitForInput();

                return;
            }

            // check that the input file exists
            if (!File.Exists(inputFile))
            {
                Console.WriteLine("Default generic.xaml file '{0}' could not be found.", inputFile);
                Console.WriteLine("You must specify a value for the /inputfile parameter to continue.");
                Console.WriteLine("You can grab a version from https://raw.github.com/Code52/HAMMER/master/SampleData/generic.xaml and add it to this folder...");
                WaitForInput();

                return;
            }

            var doc = XDocument.Load(inputFile);
            var dictionary = doc.Element(XamlPresentationNamespace + "ResourceDictionary");

            if (dictionary == null)
            {
                Console.WriteLine("Error: Input file '{0}' does not contain a resource dictionary", inputFile);
                WaitForInput();

                return;
            }

            var themeResources = (XElement)dictionary.FirstNode;
            foreach (var x in themeResources.Elements())
            {
                var brushKey = x.Attribute(XamlRootNamespace + "Key");
                if (brushKey == null)
                    continue;

                var key = brushKey.Value;
                if (key == "HighContrast")
                    continue;

                /* Generate dictionary code */
                if (appArgs.Generate)
                    Console.WriteLine("{{\"{0}\", new Dictionary<string, double>{{", key);

                foreach (var brush in Brushes[key])
                {
                    var y = x.Elements().FirstOrDefault(i => i.Attribute(XamlRootNamespace + "Key").Value == brush.Key);
                    if (y == null)
                        continue;

                    var attribute = y.Attribute("Color");
                    if (attribute == null)
                        continue;

                    /* Generate dictionary code */
                    if (appArgs.Generate)
                    {
                        var c = new HSLColor(attribute.Value.Substring(3));
                        Console.WriteLine("{{\"{0}\", {1}}},", brush, c.Luminosity - BaseLuminosity);
                    }

                    var newColour = new HSLColor(baseColour.Hue, baseColour.Saturation, baseColour.Luminosity + brush.Value);
                    attribute.SetValue(newColour.ToHex());
                }

                /* Generate dictionary code */
                if (appArgs.Generate)
                    Console.WriteLine("}}");
            }

            doc.Save(output);

            Console.WriteLine("Completed");
            Console.WriteLine("Output file is at '{0}'", Path.GetFullPath(output));
            Console.WriteLine("Press any key to finish");
            WaitForInput();
        }

        private static void WaitForInput()
        {
#if DEBUG
            Console.WriteLine();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
#endif
        }
    }
}
