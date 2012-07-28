using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace HAMMER.Pants.Tests
{
    public class ResourceFileParser
    {
        public static readonly XNamespace XamlPresentationNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        public static readonly XNamespace XamlRootNamespace = "http://schemas.microsoft.com/winfx/2006/xaml";

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



        private readonly XDocument _document;

        public ResourceFileParser(XDocument document)
        {
            _document = document;
        }

        public XDocument Update(string baseColourHex)
        {
            var output = new XDocument(_document);

            var baseColour = new HSLColor(baseColourHex);
            var dictionary = output.Element(XamlPresentationNamespace + "ResourceDictionary");

            var themeResources = (XElement)dictionary.FirstNode;
            foreach (var x in themeResources.Elements())
            {
                var brushKey = x.Attribute(XamlRootNamespace + "Key");
                if (brushKey == null)
                    continue;

                var key = brushKey.Value;
                if (key == "HighContrast")
                    continue;

                foreach (var brush in Brushes[key])
                {
                    var y = x.Elements().FirstOrDefault(i => i.Attribute(XamlRootNamespace + "Key").Value == brush.Key);
                    if (y == null)
                        continue;

                    var attribute = y.Attribute("Color");
                    if (attribute == null)
                        continue;

                    var newColour = new HSLColor(baseColour.Hue, baseColour.Saturation, baseColour.Luminosity + brush.Value);
                    attribute.SetValue(newColour.ToHex());
                }
            }

            return output;
        }
    }
}