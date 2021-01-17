# FilterBuilder #

## Before we continue ##
First of all: this tool looks horrible. It's an eye-sore.

That being said, while part of **CmcScriptNet.WPF**, `FilterBuilder` is actually a standalone component that you can use in any other .Net solution, including PowerShell.

## What is it? ##
It is a visual aide that will help you create the DDE syntax to use in a Commence RM API filter. It mimics the *Filter* dialog in Commence. Filter syntax is easily the hardest part of programming with the Commence RM API.

It should be noted that using by [Vovin.CmcLibNet](http://cmclibnet.vovin.nl) you should never have to worry about that syntax, because `Vovin.CmcLibNet` offers an object-based approach to filtering, rather than a string approach.

However, there may be cases in which you do not want to reference `Vovin.CmcLibNet` in your code. `FilterBuilder` will construct the string representation of the filter used in the `ICommenceCursor`'s `SetFilter(..)` method.

## Requirements ##
* `FilterBuilder.dll` requires that `Vovin.CmcLibNet.dll` be present in the same path.
* Pass a (valid!) Commence category name to the constructor.
* Commence must be running.

## Usage ##

**CSharp** (Console app)
```cs
class Program
{
	[STAThread] // required
	static void Main(string[] args)
	{
		// CategoryX must be a valid Commence category name
		CmcScriptNet.FilterBuilder.FilterBuilderWindow fb = new CmcScriptNet.FilterBuilder.FilterBuilderWindow("CategoryX"); 
		fb.ShowDialog(); // requires a few additional references
		Console.WriteLine(fb.Result); // the generated syntax is stored in the Result property
		Console.ReadLine();
	}
}
```

**PowerShell**
```powershell
using module '.\FilterBuilder.dll' # substitute with your path
$fb = New-Object -TypeName CmcScriptNet.FilterBuilder.FilterBuilderWindow CategoryX # CategoryX must be a valid Commence category name
$fb.ShowDialog()
'Filter syntax is: ' + $fb.Result # the generated syntax is stored in the Result property
```