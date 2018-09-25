using System;
using System.IO;
using System.Xml.Schema;
using System.Collections.Generic;

namespace GLEIF.ConsoleApp
{
    // https://docs.microsoft.com/en-us/dotnet/standard/data/xml/xml-schema-xsd-validation-with-xmlschemaset
    class Validator
    {
        private int _validationCount = 0;
        private string _validationString;
        private List<string> _validationList = new List<string>();

        // Need to consolidate all Asynchonous EventHandler calls into private List<> first
        public void XmlReaderValidationEventHandler(object sender, ValidationEventArgs e)
        {
            // Update private class variables
            _validationCount++;
            _validationString = string.Format("{0} {1}: {2}", e.Severity.ToString(), _validationCount.ToString(), e.Message);
            _validationList.Add(_validationString);

            // Write to Console
            Console.ForegroundColor = e.Severity == XmlSeverityType.Warning ? ConsoleColor.DarkYellow : ConsoleColor.Red;
            Console.WriteLine(_validationString);
            Console.ForegroundColor = ConsoleColor.White;
        }

        // Flush final List<> to file at once
        public void WriteValidationExceptions(string validationFileName)
        {
            File.WriteAllLines(validationFileName, _validationList);
        }
}
}
