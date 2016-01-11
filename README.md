# Build On Save

A Visual Studio 2015 extension to build your solution upon saving a document.

RedGate's [.NET Demon](https://www.red-gate.com/products/dotnet-development/dotnet-demon/) used to perform this task for us, but unfortunately they've retired the project due to the release of the new ["Roslyn" Compiler](https://en.wikipedia.org/wiki/.NET_Compiler_Platform).  While a lot of the features .NET Demon provided were rolled into Roslyn, there is one important feature that they missed.  That feature is the ability to have your solution build upon saving one or many documents.  This extension adds that functionality back into your workflow.


### Installation
To install this extension, just grab [`BuildOnSave.vsix`](/dist/BuildOnSave.vsix?raw=true) from the `dist` folder and install it.


### Configuration
This extension provides an options page located under `Tools -> Options -> Build On Save`.

**Options**: [(**&lt;default&gt;**) &lt;description&gt;]
- **Enabled**: (**true**)  Determines whether a build should be triggered upon saving a document.
- **Build Entire Solution**: (**true**)  Determines whether the entire solution should be built or just the project containing the modified document.
- **Extensions**: (**cs, config**)  Document extensions which trigger a build upon saving a document.
