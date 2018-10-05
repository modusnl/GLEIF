// 
// Copyright (c) Microsoft and contributors.  All rights reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the License for the specific language governing permissions and
// limitations under the License.
// 
using Microsoft.Analytics.Interfaces;
using System.Collections.Generic;
using System.IO.Compression;
using System.Xml;
using System.Xml.Linq;

namespace GLEIF.USQLClass
{
    /// <summary>Reader XML extractor</summary>
    /// <remarks>Extractors inherit from IExtractor and optionally include 
    /// the SqlUserDefinedExtractor attribute.
    /// 
    /// They convert a sequence of bytes into a sequence of SQLIP rows.
    /// This extractor reads XML incrementally to avoid loading the whole
    /// document into memory. 
    /// 
    /// XmlReader scales indefinetly as it's memory consumption is not related to the file size.
    /// therefore this is the recommended approach for XML files > 1GB
    /// 
    /// the output is 1 Xml-Line as string per Element
    /// this way a multi-line Xml-File can be converted to a file with single-line Xml-strings, 
    /// which can be futher processed using single-line Xml approaches.
    /// to come to a single-line, the following formatting applied:
    ///     Replace Double Qoute (")      by Singe Quote (') within the XML to ensure proper enclosing string delimiter "
    ///     Replace CRLF character (\r\n) by space ( )       within the XML to ensure the string fits in 1 row
    /// </remarks>

    [SqlUserDefinedExtractor(AtomicFileProcessing = true)]
    public class XmlReaderExtractor : IExtractor
    {
        private readonly string elementName;

        /// <summary>New instances are constructed at least once per vertex</summary>
        /// <param name="elementName">defines the element to write as Xml-line to 1 row</param>
        /// <remarks>Do not rely on static fields because their values are not shared across vertices.</remarks>
        public XmlReaderExtractor(string elementName)
        {
            this.elementName = elementName;
        }

        /// https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-u-sql-programmability-guide#use-user-defined-extractors
        /// <summary>Extract is called at least once per vertex</summary>
        /// <param name="input">Wrapper for a Stream</param>
        /// <param name="output">IUpdatableRow uses a mutable builder pattern -- 
        /// set individual fields with IUpdatableRow.Set, 
        /// then build an immutable IRow by calling IUpdatableRow.AsReadOnly.</param>
        /// <returns>A sequence of IRows.</returns>
        public override IEnumerable<IRow> Extract(IUnstructuredReader input, IUpdatableRow output)
        {
            // use XML Reader for streaming the XML to keep memory usage to a minimum
            using (XmlReader reader = XmlReader.Create(input.BaseStream))
            {
                reader.MoveToContent();

                // forward reader to next available Element
                while (reader.ReadToFollowing(this.elementName))
                {
                    // decouple from reader position with new subtreeReader
                    // this prevents reader.ReadToFollowing() from skipping rows as its not forwarded now by ReadOuterXml()
                    using (XmlReader subtreeReader = reader.ReadSubtree())
                    {
                        subtreeReader.MoveToContent();

                        // Replace CRLF & CR & LF character (\r\n) by space ( ) within the XML to ensure the string fits in 1 row
                        output.Set<string>(0,
                            XElement.Parse(subtreeReader.ReadOuterXml()).
                            ToString(SaveOptions.DisableFormatting).
                            Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' '));

                        // then call output.AsReadOnly to build an immutable IRow.
                        yield return output.AsReadOnly();
                    }
                }
            }
        }
    }

    /// <summary>
    /// the same as XmlReaderExtractor, only then looping through all the files in the zip 
    /// an reading the zipstream as xmlstream without the need of unpacking the zip.
    /// </summary>
    [SqlUserDefinedExtractor(AtomicFileProcessing = true)]
    public class ZipXmlReaderExtractor : IExtractor
    {
        private readonly string elementName;

        /// <summary>New instances are constructed at least once per vertex</summary>
        /// <param name="elementName">defines the element to write as Xml-line to 1 row</param>
        /// <remarks>Do not rely on static fields because their values are not shared across vertices.</remarks>
        public ZipXmlReaderExtractor(string elementName)
        {
            this.elementName = elementName;
        }

        /// https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-u-sql-programmability-guide#use-user-defined-extractors
        /// <summary>Extract is called at least once per vertex</summary>
        /// <param name="input">Wrapper for a Stream</param>
        /// <param name="output">IUpdatableRow uses a mutable builder pattern -- 
        /// set individual fields with IUpdatableRow.Set, 
        /// then build an immutable IRow by calling IUpdatableRow.AsReadOnly.</param>
        /// <returns>A sequence of IRows.</returns>
        public override IEnumerable<IRow> Extract(IUnstructuredReader input, IUpdatableRow output)
        {
            using (ZipArchive archive = new ZipArchive(input.BaseStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // use XML Reader for streaming the XML to keep memory usage to a minimum
                    using (XmlReader reader = XmlReader.Create(entry.Open()))
                    {
                        reader.MoveToContent();

                        // forward reader to next available Element
                        while (reader.ReadToFollowing(this.elementName))
                        {
                            // decouple from reader position with new subtreeReader
                            // this prevents reader.ReadToFollowing() from skipping rows as its not forwarded now by ReadOuterXml()
                            using (XmlReader subtreeReader = reader.ReadSubtree())
                            {
                                subtreeReader.MoveToContent();

                                // Replace CRLF & CR & LF character (\r\n) by space ( ) within the XML to ensure the string fits in 1 row
                                output.Set<string>(0,
                                    XElement.Parse(subtreeReader.ReadOuterXml()).
                                    ToString(SaveOptions.DisableFormatting).
                                    Replace("\r\n", " ").Replace('\n', ' ').Replace('\r', ' '));

                                // then call output.AsReadOnly to build an immutable IRow.
                                yield return output.AsReadOnly();
                            }
                        }
                    }
                }
            }
        }
    }
}
