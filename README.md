NuGet Me
========
Install-Package ConfigInCode

Config In Code
==============

Too many times xml configurations become a meta-programming language (yes MsBuild, I'm looking at you) that
requires developers to jump through arbitrary hurdles to accomplish logic based tasks.  Scripting languages
have already solved this, everything is a config file!  Let's steal some great ideas

Enter Config In Code.

Config In Code provides a convention based loading of IronRuby implimentations of .NET interfaces.  So...

What does it look like
======================
````
var config = new Configuration<IMyInterface>();
var c = config.GetConfiguration("RubyClassName");
````

Yep, that is it.  You now have an instance of IMyInterface that is actually a Ruby class.  Seriously.

Creating the interface
======================
Any old interface will do.  What are you trying to accomplish?
````
public interface IRespond
{
    string Request();
}
````

Now add a Ruby implimentation in your output folder with the same filename as the interface (by convention bin\IRespond.rb)
Make sure this file is encoded in Codepage 1252 - Western European(Windows)
````
class RubyResponder
    include IRespond
    def Request
        "Hello World"
    end    
end
````

Convention
==========
Much of this magic is based on convention, so here are the default conventions:
* The Ruby class name is the first parameter passed to GetConfiguration
* The Ruby class must "inherit" from the .NET interface.  Do this with "include Full::Namespace::And:Interface"
* The Ruby file that gets loaded will be InterfaceName.rb
* The Ruby file should be located in the same directory as the executable or in \Configuration
* For Web apps the Ruby file may be in \bin\Configuration

Tips
====
* Put your ruby files in your project under a folder /Configuration.  Set the .rb files to "Copy if newer".  This will place the file in bin/Configuration/ during build.
* You can include ruby gems!!  To install gems you need to use 'igem install gemname' at the command line.  For this to work you will have to install IronRuby (recommend 1.0)
    * Put "require 'rubygems'" before any custom gem requires
* The dynamic runtime wants files to be encoded in Codepage 1252.
    * In VS you can set this under File -> Advanced Save Options when you have the file open
