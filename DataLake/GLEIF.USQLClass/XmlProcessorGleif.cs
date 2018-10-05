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
using Microsoft.Analytics.Diagnostics;
using Microsoft.Analytics.Interfaces;
using System.IO;
using System.Xml.Serialization;

namespace GLEIF.USQLClass
{
    /// <summary>
    /// https://docs.microsoft.com/en-us/azure/data-lake-analytics/data-lake-analytics-u-sql-programmability-guide#use-user-defined-processors
    /// </summary>
    [SqlUserDefinedProcessor]
    public class XmlProcessorGleif : IProcessor
    {
        public override IRow Process(IRow input, IUpdatableRow output)
        {
            // StringReader based on the first column (regardless of ColumnName)
            using (TextReader reader = new StringReader(input.Get<string>(0)))
            {
                // Deserialize XML string straight into LEIRecord Object (stream)
                XmlSerializer serializer = new XmlSerializer(typeof(LEIRecord));
                LEIRecord lEIRecord = (LEIRecord)serializer.Deserialize(reader);

                // write Log to Diagnostics stream ?!?
                //DiagnosticStream.WriteLine("Serializing LEI: " + lEIRecord.LEI);

                // Set Fixed output columns based on deserialized (nullable) LEIRecord objects
                output.Set<string>("Entity_AssociatedEntity_type", (lEIRecord?.Entity?.AssociatedEntity?.type)?.ToString());
                output.Set<string>("Entity_AssociatedEntity_AssociatedLei", (lEIRecord?.Entity?.AssociatedEntity?.Item)?.ToString());
                output.Set<string>("Entity_EntityExpirationDate", (lEIRecord?.Entity?.EntityExpirationDate)?.ToString());
                output.Set<string>("Entity_EntityExpirationReason", (lEIRecord?.Entity?.EntityExpirationReason)?.ToString());
                output.Set<string>("Entity_EntityStatus", (lEIRecord?.Entity?.EntityStatus)?.ToString());
                output.Set<string>("Entity_HeadquartersAddress_lang", (lEIRecord?.Entity?.HeadquartersAddress?.lang)?.ToString());
                output.Set<string>("Entity_HeadquartersAddress_AdditionalAddressLine", (lEIRecord?.Entity?.HeadquartersAddress?.AdditionalAddressLine?[0])?.ToString());
                output.Set<string>("Entity_HeadquartersAddress_City", (lEIRecord?.Entity?.HeadquartersAddress?.City)?.ToString());
                output.Set<string>("Entity_HeadquartersAddress_Country", (lEIRecord?.Entity?.HeadquartersAddress?.Country)?.ToString());
                output.Set<string>("Entity_HeadquartersAddress_FirstAddressLine", (lEIRecord?.Entity?.HeadquartersAddress?.FirstAddressLine)?.ToString());
                output.Set<string>("Entity_HeadquartersAddress_PostalCode", (lEIRecord?.Entity?.HeadquartersAddress?.PostalCode)?.ToString());
                output.Set<string>("Entity_HeadquartersAddress_Region", (lEIRecord?.Entity?.HeadquartersAddress?.Region)?.ToString());
                output.Set<string>("Entity_LegalAddress_lang", (lEIRecord?.Entity?.LegalAddress?.lang)?.ToString());
                output.Set<string>("Entity_LegalAddress_AdditionalAddressLine", (lEIRecord?.Entity?.LegalAddress?.AdditionalAddressLine?[0])?.ToString());
                output.Set<string>("Entity_LegalAddress_City", (lEIRecord?.Entity?.LegalAddress?.City)?.ToString());
                output.Set<string>("Entity_LegalAddress_Country", (lEIRecord?.Entity?.LegalAddress?.Country)?.ToString());
                output.Set<string>("Entity_LegalAddress_FirstAddressLine", (lEIRecord?.Entity?.LegalAddress?.FirstAddressLine)?.ToString());
                output.Set<string>("Entity_LegalAddress_PostalCode", (lEIRecord?.Entity?.LegalAddress?.PostalCode)?.ToString());
                output.Set<string>("Entity_LegalAddress_Region", (lEIRecord?.Entity?.LegalAddress?.Region)?.ToString());
                output.Set<string>("Entity_LegalForm_EntityLegalFormCode", (lEIRecord?.Entity?.LegalForm?.EntityLegalFormCode)?.ToString());
                output.Set<string>("Entity_LegalForm_OtherLegalForm", (lEIRecord?.Entity?.LegalForm?.OtherLegalForm)?.ToString());
                output.Set<string>("Entity_LegalJurisdiction", (lEIRecord?.Entity?.LegalJurisdiction)?.ToString());
                output.Set<string>("Entity_LegalName", (lEIRecord?.Entity?.LegalName?.Value)?.ToString());
                output.Set<string>("Entity_LegalName_lang", (lEIRecord?.Entity?.LegalName?.lang)?.ToString());
                output.Set<string>("Entity_NextVersion", (lEIRecord?.Entity?.NextVersion)?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_lang", (lEIRecord?.Entity?.OtherAddresses?[0]?.lang)?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_type", (lEIRecord?.Entity?.OtherAddresses?[0]?.type)?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_AdditionalAddressLine", (lEIRecord?.Entity?.OtherAddresses?[0]?.AdditionalAddressLine?[0])?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_City", (lEIRecord?.Entity?.OtherAddresses?[0]?.City)?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_Country", (lEIRecord?.Entity?.OtherAddresses?[0]?.Country)?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_FirstAddressLine", (lEIRecord?.Entity?.OtherAddresses?[0]?.FirstAddressLine)?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_PostalCode", (lEIRecord?.Entity?.OtherAddresses?[0]?.PostalCode)?.ToString());
                output.Set<string>("Entity_OtherAddresses_OtherAddress_Region", (lEIRecord?.Entity?.OtherAddresses?[0]?.Region)?.ToString());
                output.Set<string>("Entity_OtherEntityNames_OtherEntityName", (lEIRecord?.Entity?.OtherEntityNames?[0].Value)?.ToString());
                output.Set<string>("Entity_OtherEntityNames_OtherEntityName_lang", (lEIRecord?.Entity?.OtherEntityNames?[0]?.lang)?.ToString());
                output.Set<string>("Entity_RegistrationAuthority_OtherRegistrationAuthorityID", (lEIRecord?.Entity?.RegistrationAuthority?.OtherRegistrationAuthorityID)?.ToString());
                output.Set<string>("Entity_RegistrationAuthority_RegistrationAuthorityEntityID", (lEIRecord?.Entity?.RegistrationAuthority?.RegistrationAuthorityEntityID)?.ToString());
                output.Set<string>("Entity_RegistrationAuthority_RegistrationAuthorityID", (lEIRecord?.Entity?.RegistrationAuthority?.RegistrationAuthorityID)?.ToString());
                output.Set<string>("Entity_SuccessorEntity_SuccessorEntityName", (lEIRecord?.Entity?.SuccessorEntity?.Item)?.ToString());
                output.Set<string>("Entity_SuccessorEntity_SuccessorLei", (lEIRecord?.Entity?.SuccessorEntity?.Item)?.ToString());
                output.Set<string>("Extension", (lEIRecord?.Extension)?.ToString());
                output.Set<string>("Lei", (lEIRecord?.LEI)?.ToString());
                output.Set<string>("NextVersion", (lEIRecord?.NextVersion)?.ToString());
                output.Set<string>("Registration_InitialRegistrationDate", (lEIRecord?.Registration?.InitialRegistrationDate)?.ToString());
                output.Set<string>("Registration_LastUpdateDate", (lEIRecord?.Registration?.LastUpdateDate)?.ToString());
                output.Set<string>("Registration_ManagingLOU", (lEIRecord?.Registration?.ManagingLOU)?.ToString());
                output.Set<string>("Registration_NextRenewalDate", (lEIRecord?.Registration?.NextRenewalDate)?.ToString());
                output.Set<string>("Registration_NextVersion", (lEIRecord?.Registration?.NextVersion)?.ToString());
                output.Set<string>("Registration_RegistrationStatus", (lEIRecord?.Registration?.RegistrationStatus)?.ToString());
                output.Set<string>("Registration_ValidationAuthority_OtherValidationAuthorityID", (lEIRecord?.Registration?.ValidationAuthority?.OtherValidationAuthorityID)?.ToString());
                output.Set<string>("Registration_ValidationAuthority_ValidationAuthorityEntityID", (lEIRecord?.Registration?.ValidationAuthority?.ValidationAuthorityEntityID)?.ToString());
                output.Set<string>("Registration_ValidationAuthority_ValidationAuthorityID", (lEIRecord?.Registration?.ValidationAuthority?.ValidationAuthorityID)?.ToString());
                output.Set<string>("Registration_ValidationSources", (lEIRecord?.Registration?.ValidationSources)?.ToString());

                // then call output.AsReadOnly to build an immutable IRow.
                return output.AsReadOnly();
            }
        }
    }
}
