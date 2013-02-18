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
