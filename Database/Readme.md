# Database

## Recommendation

### Small Xml

1. OpenRowset might be handy for extracting some records straight off the Xml file as 1 Xml Document
1. use XQuery for Cross Apply Nodes() to help you explode the Nodes() into  records off the Xml Document. Note that perfomance gets bad with larger Xml files, this approach doesn't scale

### Large Xml

1. As OpenRowset with Cross Apply Nodes() doesn't scale properly, use a Streaming approach instead: Stream the XmlLines upfront into a file (per target table) with 1 Xml line per row. (see ConsoleApp and/or DataLake solutions in this repository, those .NET snippets can be run from SSIS / Azure Functions, Data Factory as well)
1. use BulkInsert for loading the Xml lines into a table with Xml column
1. use XQuery for parsing the column values off the Xml Record into columns