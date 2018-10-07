# Console App 

This .NET Core Console app hosts losts of code snippets. Core function is crunching the big Xml file into several smaller files for rapid development purposes.

Besides that, it is mainly used for design time exploration of usable code snippets, which might end-up eventually somewhere in a data pipeline on a production system.

## Main

Make sure the ../Data folder contains the data file as refered to in `string fileName`. 
The following methods can be toggled on/off based on your needs. Dive into the methods for more details.

1. `WriteSubsets(fileName)` will write Subsets based on the serialized Full Xml. This method actually only needs to run once and can be commented-out after initial run.
Going forward, the TopN subsets can be used for quick & consumable feedback on the various processing approaches.
2. `XPathExtension.WriteXPathFile(fileName);`
3. `Streaming.ValidateXmlStream(fileName);`
4. `Streaming.WriteXmlLines(fileName, "lei:LEIRecord")`
5. `Serialization.ReadXmlLines(fileName);`
6. `Streaming.ReadXmlLines(fileName);`

## Generate .NET classes from XSD schema using XSD.EXE

However this project already contains the generated .NET classes in [2017-03-21_lei-cdf-v2-1.cs](xsd/2017-03-21_lei-cdf-v2-1.cs), the following steps describe how they can be generated from the Xsd file [2017-03-21_lei-cdf-v2-1.xsd](xsd/2017-03-21_lei-cdf-v2-1.xsd)

Run the `Developer Command Prompt for VS 2017 (Enterprise)` through the shortcut in in the subfolder xsd.
Within the prompt, run the following commands for generating the .NET classes from the XSD schema
* `xsd.exe /c 2017-03-21_lei-cdf-v2-1.xsd /namespace:GLEIF.lei`
* `xsd.exe /c 2017-03-16_rr-cdf-v1-1.xsd /namespace:GLEIF.rr`

Alter the genrated .NET class for LEI as follows;
* Add `using System.Collections.Generic;`
* Change `LEIRecordType[]` to `List<LEIRecord>` for richer filter options
* add line 1445: `[System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.gleif.org/data/schema/leidata/2016", ElementName = "LEIRecord", IsNullable = true)]` for using the same .NET class for serializing just a LEIRecord in addition to the full LEIData object