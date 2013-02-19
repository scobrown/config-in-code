NuGet Me
========
Install-Package ConfigInCode
Install-Package ConfigInCode.StructureMap

https://nuget.org/packages/ConfigInCode/1.1

Config In Code
==============

Too many times xml configurations become a meta-programming language (yes MsBuild, I'm looking at you) that
requires developers to jump through arbitrary hurdles to accomplish logic based tasks.  Scripting languages
have already solved this, everything is a config file!  Let's steal some great ideas

Enter Config In Code.

Config In Code provides a convention based loading of IronRuby and/or IronPython implimentations of .NET interfaces.  So...

What does it look like
======================
````
var config = new Configuration<IMyInterface>();
var c = config.GetDefaultConfiguration();
````

Yep, that is it.  You now have an instance of IMyInterface that is actually a Ruby class.  Seriously.

If you use a container like StructureMap you can simply
````
ObjectFactory.Initialize(x => x.WithDefaultConfiguration<IMyInterface>());
var instance = ObjectFactory.GetInstance<IMyInterface>();
````

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
Ruby
====
````
class Respond
    include IRespond
    def Request
        "Hello World"
    end    
end
````
Python
======
````
import IRespond

class TestPython(IRespond):
    def Request(self):
		return "Hello World"
````

Convention
==========
Much of this magic is based on convention, so here are the default conventions:
* The DLR class name is the first parameter passed to GetConfiguration
* The DLR class must "inherit" from the .NET interface.  
    * For Ruby "include Full::Namespace::And:Interface"
    * For Python "PythonClass(NETClass)" and add an import for the namespace
* The DLR class name must match the interface name without the prefix I
* The Ruby file that gets loaded will be InterfaceName.rb
* The Python file that gets loaded will be InterfaceName.py
* The DLR file should be located in the same directory as the executable or in \Configuration
* For Web apps the DLR file may be in \bin\Configuration

Tips
====
* Put your dlr files in your project under a folder /Configuration.  Set the .rb and .py files to "Copy if newer".  This will place the file in bin/Configuration/ during build.
* You can include ruby gems!!  To install gems you need to use 'igem install gemname' at the command line.  
    * For this to work you will have to install IronRuby (recommend 1.0)
    * Put "require 'rubygems'" before any custom gem requires
* The dynamic runtime wants files to be encoded in Codepage 1252.
    * In VS you can set this under File -> Advanced Save Options when you have the file open
* The script gets loaded every call to GetConfiguration, so this can be used to allow runtime changes (but see the caveat below)

Caveats
=======
* ~5MB dll!!  Both python and ruby are included by default and get embedded
* The script gets loaded and executed every time GetConfiguration is called.  This is intended to be "bootstrapp-ish" code and not something for a tight loop
* May not hold up well under high load
* Subject to all the limitations of the DLR implementations
