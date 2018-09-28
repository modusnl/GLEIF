# GLEIF (multi GB Xml-handling)

This project demonstrates various processing approaches for handling large Xml files, using the publicly available GLEIF dataset.

## Projects

See [ConsoleApp](/ConsoleApp/Readme.md) for the .NET Core CommandLineApp as a starting point for deriving some smaller files off the big Xml and lots of valuable .NET methods for Reading, Writing, Validating, Serializing Xml.

See [Database](/Database/Readme.md) for the T-SQL approaches

See [DataLake](/DataLake/Readme.md) for the U-SQL approaches

See [Databricks](/Databricks/Readme.md) for the Spark approaches

## Data

using [LEI Level 1 data CDF v2.1](https://www.gleif.org/en/about-lei/common-data-file-format/lei-cdf-format/lei-cdf-format-version-2-1)

Run [Download-LEI2.ps1](/Data/Download-LEI2.ps1) for downloading the 155 mb Zip file and extracting the 2.6 GB Xml file

## GLEIF
The Global Legal Entity Identifier Foundation (GLEIF) is tasked to support the implementation and use of the Legal Entity Identifier (LEI). The Legal Entity Identifier (LEI) enables clear and unique identification of legal entities engaging in financial transactions. LEI data is a good open data source for processing large XMLs. Download the GLEIF Concatenated File in zipped XML format for advanced users.

## References

- https://www.gleif.org/en/lei-data/gleif-concatenated-file/download-the-concatenated-file