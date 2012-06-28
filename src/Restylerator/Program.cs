using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Restylerator
{
    class Program
    {
        //Need to check for CPU-bits
        private static string File = @"C:\Program Files (x86)\Windows Kits\8.0\Include\winrt\xaml\design\generic.xaml";

        private static readonly List<string> Brushes = new List<string>
                          {
                              "ComboBoxItemSelectedBackgroundThemeBrush",
                              "ComboBoxItemSelectedPointerOverBackgroundThemeBrush",
                              "ComboBoxSelectedBackgroundThemeBrush",
                              "ComboBoxSelectedPointerOverBackgroundThemeBrush",
                              "HyperlinkForegroundThemeBrush",
                              "HyperlinkPointerOverForegroundThemeBrush",
                              "HyperlinkPressedForegroundThemeBrush",
                              "ListBoxItemSelectedBackgroundThemeBrush",
                              "ListBoxItemSelectedPointerOverBackgroundThemeBrush",
                              "ListViewItemCheckHintThemeBrush",
                              "ListViewItemCheckSelectingThemeBrush",
                              "ListViewItemDragBackgroundThemeBrush",
                              "ListViewItemSelectedBackgroundThemeBrush",
                              "ListViewItemSelectedPointerOverBackgroundThemeBrush",
                              "ListViewItemSelectedPointerOverBorderThemeBrush",
                              "ProgressBarForegroundThemeBrush",
                              "ProgressBarIndeterminateForegroundThemeBrush",
                              "SliderTrackDecreaseBackgroundThemeBrush",
                              "SliderTrackDecreasePointerOverBackgroundThemeBrush",
                              "SliderTrackDecreasePressedBackgroundThemeBrush",
                              "ToggleSwitchCurtainBackgroundThemeBrush",
                              "ToggleSwitchCurtainPointerOverBackgroundThemeBrush",
                              "ToggleSwitchCurtainPressedBackgroundThemeBrush"
                          };

        static void Main(string[] args)
        {
            File = @"generic.xaml";
            XNamespace namespace1 = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            XNamespace namespace2 = "http://schemas.microsoft.com/winfx/2006/xaml";
            XDocument doc = XDocument.Load(File);
            var themeResources = (XElement)doc.Element(namespace1 + "ResourceDictionary").FirstNode;
            foreach (var x in themeResources.Elements())
            {
                var key = x.Attribute(namespace2 + "Key").Value;
                if (key == "HighContrast")
                    continue;

                Console.WriteLine(key);

                foreach (var brush in Brushes)
                {
                    var y = x.Elements().FirstOrDefault(i => i.Attribute(namespace2 + "Key").Value == brush);
                    if (y != null)
                        y.Attribute("Color").SetValue("#FFCACACA");
                    //Console.WriteLine(y.Attribute("Color").Value);
                }
            }

            doc.Save(@"new.xaml");

            Console.ReadKey();
        }
    }
}
