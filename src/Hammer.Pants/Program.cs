using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MadProps.AppArgs;

namespace HAMMER.Pants
{
    class Program
    {
        //Need to check for CPU-bits
        private static string File = @"C:\Program Files (x86)\Windows Kits\8.0\Include\winrt\xaml\design\generic.xaml";
        private static double baseLuminosity = 95.5294132232666;
        private static readonly Dictionary<string, Dictionary<string, double>> Brushes = new Dictionary<string, Dictionary<string, double>>
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

        static void Main(string[] args)
        {
            HSLColor baseColour;
            PantsArgs appargs;
            string output = "Generic.xaml";
            try
            {
                appargs = args.As<PantsArgs>();
                if (!string.IsNullOrEmpty(appargs.InputFile))
                    File = appargs.InputFile;

                if (!string.IsNullOrEmpty(appargs.OutputFile))
                    output = appargs.OutputFile;

                baseColour = new HSLColor(appargs.Colour);

            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine(AppArgs.HelpFor<PantsArgs>());

                return;
            }

            XNamespace namespace1 = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XNamespace namespace2 = "http://schemas.microsoft.com/winfx/2006/xaml";
            XDocument doc = XDocument.Load(File);
            var themeResources = (XElement)doc.Element(namespace1 + "ResourceDictionary").FirstNode;
            foreach (var x in themeResources.Elements())
            {
                var key = x.Attribute(namespace2 + "Key").Value;
                if (key == "HighContrast")
                    continue;

                /* Generate dictionary code */
                if (appargs.Generate)
                    Console.WriteLine(string.Format("{{\"{0}\", new Dictionary<string, double>{{", key));

                foreach (var brush in Brushes[key])
                {
                    var y = x.Elements().FirstOrDefault(i => i.Attribute(namespace2 + "Key").Value == brush.Key);
                    if (y != null)
                    {
                        //y.Attribute("Color").SetValue(baseColour);
                        
                        /* Generate dictionary code */
                        if (appargs.Generate)
                        {
                            var c = new HSLColor(y.Attribute("Color").Value.Substring(3));
                            Console.WriteLine(string.Format("{{\"{0}\", {1}}},", brush, c.Luminosity - baseLuminosity));
                        }

                        var newColour = new HSLColor(baseColour.Hue, baseColour.Saturation, baseColour.Luminosity + brush.Value);
                        y.Attribute("Color").SetValue(newColour.ToHex());
                    }
                }

                /* Generate dictionary code */
                if (appargs.Generate)
                    Console.WriteLine("}}");
            }

            doc.Save(output);

            Console.WriteLine("Complete, press any key");
            Console.ReadKey();
        }
    }
}
