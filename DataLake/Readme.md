# DataLake

## GLEIF.USQLClass

This project contains the custom developed User Defined Objects (UDO) for processing Xml at the Azure Data Lake.

Note that the `XmlApplier` is actually copied from [U-SQL Examples - Data Formats](https://github.com/Azure/usql/tree/master/Examples/DataFormats), but as the Pull Request [Adding missing XML Namespace support to XmlApplier](https://github.com/Azure/usql/pull/145) for  extending this UDO with namespace support hasn't been merged yet, this code is copied here as well.

## GLEIF.USQLDB

This project contains the declaritve USQL database development hosting the Azure Data Lake Analytics (ADLA) logic in USQL Procedures. 

Declaritive database development is achieved by always dropping & reacrating the USQL-DB, which contains logic only, as data is persisted on the Data Lake itself.

## GLEIF.USQL

This project contains sample USQL scripts for calling the USQL Procedures