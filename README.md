Series Manager (seriesmanager)
=============

Series Manager is a WinRT app currently targeting Windows 8.1.


-----------------------------------------

Code Conventions (examples):

Make use of 'this' for accessing properties and functions in your class and 'base' for accessing those in the base class.

Start boolean properties - if possible - with the prefix Is...

-- Properties --

public bool IsActive
{
   get 
   { 
      return this.isActive;
   }
   private set 
   {
      base.SetProperty(ref this.isActive, value);
   }
}

-- Auto Properties --

public bool IsActive
{
   get;
   private set;
}



Make use of the readonly pattern (whereever possible)!!!
Use same property names in the constructor while initializing. Distinguish with 'this' keyword. Also use new language features like 'default properties'.

-- Readonly Properties, default properties and settings from constructor --

private readonly int id;
### Constructor ###
public ClassName(int id = 6)
{
   this.id = id;
}
