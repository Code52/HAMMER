Hyper-Awesome, Malicious, Markup Enabled Reality.
====================

HAMMER.Pants
------------

So we started with [this](http://stackoverflow.com/questions/10292166/can-i-change-rebind-the-default-winrt-system-brushes-palette-colors-to-match-m) little conundrum when restyling Win8 XAML apps: 

 > To brand WinRT/XAML apps, you need to override ALL control styles. This sucks if you just want to change the colours to match your theme.

HAMMER.Pants solves this, by auto-modifying all the coloured brushes so you don't have to pick through them. Currently, you provide a "base" colour, and HAMMER.Pants modifies that based on the luminance variation found in the original styles. That is, if you pick red, all the purples will be replaced with reds, but they'll change in brightness slightly just like the default purples do.

**Usage**
	
	HAMMER.Pants.exe /colour=value [/inputfile=value] [/outputfile=value]
	
	 /colour hex (without the #) value for the "base" - only supports RGB values currently
	 /inputfile (optional if on Windows 8)
	 /outputfile (optional, will default to generic.xaml in the Hammer.Pants directory)

**TODO**

* Provide a wider colour pallete rather than auto-generated HSB values
* Project-build script so that it's done at build time, no external app required.

HAMMER.Touch
------
You shouldn't touch this.