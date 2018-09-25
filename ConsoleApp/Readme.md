Generate .NET classes from XSD schema using XSD.EXE:
* xsd.exe /c 2017-03-21_lei-cdf-v2-1.xsd /namespace:GLEIF.lei
* xsd.exe /c 2017-03-16_rr-cdf-v1-1.xsd /namespace:GLEIF.rr

override generated file with the following
* Add 'using System.Collections.Generic;'
* change LEIRecordType[] to List<LEIRecord>
* add line 1445 [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.gleif.org/data/schema/leidata/2016", ElementName = "LEIRecord", IsNullable = true)]


// see https://www.gleif.org/en/lei-data/gleif-concatenated-file/download-the-concatenated-file
// download zip 110 mb .zip file manually, i.e. https://leidata.gleif.org/api/v1/concatenated-files/lei2/20180313/zip
// using LEI Level 1 data CDF v2.1
// see https://www.gleif.org/en/about-lei/common-data-file-format/lei-cdf-format/lei-cdf-format-version-2-1