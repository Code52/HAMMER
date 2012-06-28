using System;
using System.Drawing;

namespace Restylerator
{
    //Source: http://richnewman.wordpress.com/about/code-listings-and-diagrams/hslcolor-class/
    public class HSLColor
    {
        // Private data members below are on scale 0-1
        // They are scaled for use externally based on scale
        private double _hue = 1.0;
        private double _saturation = 1.0;
        private double _luminosity = 1.0;

        private const double Scale = 240.0;

        public double Hue
        {
            get { return _hue * Scale; }
            set { _hue = CheckRange(value / Scale); }
        }

        public double Saturation
        {
            get { return _saturation * Scale; }
            set { _saturation = CheckRange(value / Scale); }
        }

        public double Luminosity
        {
            get { return _luminosity * Scale; }
            set { _luminosity = CheckRange(value / Scale); }
        }

        private double CheckRange(double value)
        {
            if (value < 0.0)
                value = 0.0;
            else if (value > 1.0)
                value = 1.0;
            return value;
        }

        public override string ToString()
        {
            return String.Format("H: {0:#0.##} S: {1:#0.##} L: {2:#0.##}", Hue, Saturation, Luminosity);
        }

        public string ToRGBString()
        {
            var color = (Color)this;
            return String.Format("R: {0:#0.##} G: {1:#0.##} B: {2:#0.##}", color.R, color.G, color.B);
        }

        public static implicit operator Color(HSLColor hslColor)
        {
            double r = 0, g = 0, b = 0;
            if (hslColor._luminosity != 0)
            {
                if (hslColor._saturation == 0)
                    r = g = b = hslColor._luminosity;
                else
                {
                    double temp2 = GetTemp2(hslColor);
                    double temp1 = 2.0 * hslColor._luminosity - temp2;

                    r = GetColorComponent(temp1, temp2, hslColor._hue + 1.0 / 3.0);
                    g = GetColorComponent(temp1, temp2, hslColor._hue);
                    b = GetColorComponent(temp1, temp2, hslColor._hue - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            temp3 = MoveIntoRange(temp3);
            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;

            if (temp3 < 0.5)
                return temp2;

            if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);

            return temp1;
        }

        private static double MoveIntoRange(double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;
            return temp3;
        }

        private static double GetTemp2(HSLColor hslColor)
        {
            double temp2;
            if (hslColor._luminosity < 0.5)  //<=??
                temp2 = hslColor._luminosity * (1.0 + hslColor._saturation);
            else
                temp2 = hslColor._luminosity + hslColor._saturation - (hslColor._luminosity * hslColor._saturation);
            return temp2;
        }

        public static implicit operator HSLColor(Color color)
        {
            HSLColor hslColor = new HSLColor
                                    {
                                        _hue = color.GetHue() / 360.0,
                                        _luminosity = color.GetBrightness(),
                                        _saturation = color.GetSaturation()
                                    };
            // we store hue as 0-1 as opposed to 0-360 
            return hslColor;
        }

        public void SetRGB(int red, int green, int blue)
        {
            HSLColor hslColor = Color.FromArgb(red, green, blue);
            _hue = hslColor._hue;
            _saturation = hslColor._saturation;
            _luminosity = hslColor._luminosity;
        }

        public HSLColor()
        {
            
        }

        public HSLColor(Color color)
        {
            SetRGB(color.R, color.G, color.B);
        }

        public HSLColor(string hex)
        {
            var r = Convert.ToInt16(hex.Substring(0, 2), 16);
            var g = Convert.ToInt16(hex.Substring(2, 2), 16);
            var b = Convert.ToInt16(hex.Substring(4, 2), 16);

            SetRGB(r,g,b);
        }

        public HSLColor(int red, int green, int blue)
        {
            SetRGB(red, green, blue);
        }

        public HSLColor(double hue, double saturation, double luminosity)
        {
            Hue = hue;
            Saturation = saturation;
            Luminosity = luminosity;
        }

        public string ToHex()
        {
            var color = (Color)this;
            return String.Format("#FF{0:X}{1:X}{2:X}", color.R, color.G, color.B);
        }
    }
}