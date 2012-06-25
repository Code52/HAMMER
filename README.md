bearded-octo-nemesis
====================

So we started with this little conundrum when restyling Win8 XAML apps:

http://stackoverflow.com/questions/10292166/can-i-change-rebind-the-default-winrt-system-brushes-palette-colors-to-match-m

Which lead to this discussion:

**6:23:48 PM  Paul Jenkins**

What I'm thinking - and its likely I/you will write more apps in the future - is that we could write an app that finds those files, you give it a bunch of colours for the "Accent", then it replaces where required and outputs a new generic.xaml And perhaps an option to choose what controls you want to include a more minimal set of XAML resources is always easier to work with  

The default palette is various shades of grey, black, white, and various shades of purple. Thats one of the big problems, its not just a single brush to replace, but about 30. What we could do is have two modes: * select a single base colour, and it adjusts up/down for each brush * get user to provide all the "Accent" values  

http://i.imgur.com/hlUGt.png example of the different names for very slightly different shades 

**6:27:03 PM  Brendan Forster**

ok, so i'm thinking from a consumer side - an ideal thing for me would be to have a "config" file in the csproj where i pick out the simple/medium config. i then build the solution to get the necessary resource file 

i'd rather a simple file than anything too advanced at this stage 

but yes, i like the "just tweak the shades for me" convention 

**6:28:49 PM  Paul Jenkins**

I think there is probably calls for both styles - a more flexible, but slower/more configurable app that generates it, and the auto-build-via-config. The former means you can do further tweaking/restyling, the latter is "I don't want fucking purple" 

**6:29:26 PM  Brendan Forster**

why not ship both, with the simple one uncommented and the complex one commented out 

**6:30:28 PM  Paul Jenkins**

not saying dont' do both, but it comes down to effort-reward/effort-resources (you, me, quandtm) available :P 
