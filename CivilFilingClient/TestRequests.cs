using System;
using System.IO;

namespace CivilFilingClient
{
    static class TestRequests
    {
        /// <summary>
        /// TestCorp2Corp - Generates a Sample Message for posting to the NJ Courts
        /// </summary>
        /// <returns></returns>
        public static CivilFilingServiceReference.civilFilingRequest TestCorp2Corp()
        {
            // Begining of Test Code
            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.caseAction = "028";
            caseData.courtSection = "SCP";
            caseData.defendantCaption = "Defendant Caption";
            caseData.demandAmount = Convert.ToDecimal(5400);
            caseData.demandAmountSpecified = true;
            caseData.docketDetailsForOtherCourt = "docketDetailsForOtherCourt";
            caseData.juryDemand = "N";
            caseData.lawFirmCaseId = "TESTDAN";
            caseData.otherCourtActions = "N";
            caseData.plaintiffCaption = "test plantiff caption";
            caseData.serviceMethod = "03";
            caseData.venue = "ATL";
            caseData.venueOfIncident = "CPM";

            CivilFilingServiceReference.attachment att = new CivilFilingServiceReference.attachment();
            string filePath = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\CivilFilingClient\TestFiles\Test.pdf";
            //TODO: how large are the files?
            byte[] bytes = File.ReadAllBytes(filePath);

            att.bytes = bytes;
            att.contentType = "application/pdf";
            att.docType = "pdf";
            att.documentCode = "CMPL";
            att.documentDescription = "Complaint";
            att.documentName = "Test";
            att.extention = ".pdf";

            CivilFilingServiceReference.fee fee = new CivilFilingServiceReference.fee();
            fee.accountNumber = "141375";
            fee.attorneyClientRefNumber = "1"; //numeric
            fee.attorneyFee = Convert.ToDecimal(0);
            fee.attorneyFeeSpecified = false;
            fee.feeExempt = "Y";
            fee.paymentType = "CG";

            CivilFilingServiceReference.address pAddress = new CivilFilingServiceReference.address();
            pAddress.addressLine1 = "123 Main Street";
            pAddress.city = "Parsippany";
            pAddress.stateCode = "NJ";
            pAddress.zipCode = "07054";

            CivilFilingServiceReference.party plantiff = new CivilFilingServiceReference.party();
            plantiff.adaAccommodationInd = "N";
            plantiff.address = pAddress;
            plantiff.corporationName = "Weyland-Yutani Corp.";
            plantiff.corporationType = "CO";
            plantiff.interpreterInd = "N";
            plantiff.partyAffiliation = "ADM";
            plantiff.partyDescription = "BUS";
            //plantiff.language = "SPA";
            //plantiff.accommodationType = "a";

            CivilFilingServiceReference.address dAddress = new CivilFilingServiceReference.address();
            dAddress.addressLine1 = "123 Main Street";
            dAddress.city = "Parsippany";
            dAddress.stateCode = "NJ";
            dAddress.zipCode = "07054";

            CivilFilingServiceReference.party defendant = new CivilFilingServiceReference.party();
            defendant.adaAccommodationInd = "N";
            defendant.address = dAddress;
            defendant.corporationName = "Initech";
            defendant.corporationType = "CO";
            defendant.interpreterInd = "N";
            defendant.partyAffiliation = "ADM";
            defendant.partyDescription = "BUS";
            //defendant.accommodationType = "a";

            CivilFilingServiceReference.bulkFilingPacket packet = new CivilFilingServiceReference.bulkFilingPacket();
            //takes and array of attachment[] we just have attachment
            CivilFilingServiceReference.attachment[] attachments = new CivilFilingServiceReference.attachment[1];
            attachments[0] = att;
            packet.attachmentList = attachments;
            packet.attorneyId = "888888005";            //Required
            packet.attorneyFirmId = "F88888003";        //Required

            packet.civilCase = caseData;
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            packet.defendantList = defendants;
            packet.documentRedactionInd = "Y";
            packet.fee = fee;

            // REQUIRED Branch Id - need the 
            CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute();
            attr.name = "branchId";
            attr.value = "0001";
            CivilFilingServiceReference.attribute[] attrs = new CivilFilingServiceReference.attribute[1];
            attrs[0] = attr;
            packet.attributes = attrs;

            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plantiff;
            packet.plaintiffList = plantiffs;

            CivilFilingServiceReference.civilFilingRequest filingRequest =
                new CivilFilingServiceReference.civilFilingRequest();
            filingRequest.bulkFilingPacket = packet;

            return filingRequest;
        }

        /// <summary>
        /// TestCorp2Individual -> TestPlantiff2Defendant
        /// </summary>
        /// <returns></returns>
        public static CivilFilingServiceReference.civilFilingRequest TestCorp2Individual()
        {
            // bulkFilingPacket REQ
            CivilFilingServiceReference.bulkFilingPacket packet = new CivilFilingServiceReference.bulkFilingPacket();
            packet.attorneyId = "888888005";            //Required
            packet.attorneyFirmId = "F88888003";        //Required

            // REQUIRED Branch Id - need the 
            CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute();
            attr.name = "branchId";
            attr.value = "0001";
            CivilFilingServiceReference.attribute[] attrs = new CivilFilingServiceReference.attribute[1];
            attrs[0] = attr;
            packet.attributes = attrs;

            // Begining of Test Code
            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.courtSection = "SCP";       //REQ
            caseData.venue = "ATL";              //REQ
            caseData.otherCourtActions = "Y";    //REQ
            caseData.caseAction = "028";         //REQ
            caseData.demandAmount = Convert.ToDecimal(5400);  //REQ
            caseData.demandAmountSpecified = true;
            caseData.juryDemand = "N";           //REQ
            caseData.serviceMethod = "03";       //REQ
            caseData.lawFirmCaseId = "CorpVsIndividual"; //Code: ECCV110 Description: Law Firm Case Id should be alphanumeric 
            caseData.venueOfIncident = "CPM";    //REQ
            caseData.plaintiffCaption = "plaintiffCaption not required";
            caseData.defendantCaption = "defendantCaption not required";
            caseData.docketDetailsForOtherCourt = "docketDetailsForOtherCourt";

            // PLANTIFF LIST
            CivilFilingServiceReference.party plaintiff = new CivilFilingServiceReference.party();
            plaintiff.partyDescription = "BUS";      //REQ
            plaintiff.partyAffiliation = "ADM";      //not required
            plaintiff.corporationType = "CO";        //REQ if plantiff.partyDescription = "BUS";
            plaintiff.corporationName = "Massive Dynamic";//REQ if plantiff.partyDescription = "BUS";
            //plantiff.phoneNumber = "1112223333";  // not required
            plaintiff.interpreterInd = "N";          //REQ
            //plantiff.language = "";               //REQ if interpreterInd = "Y", see code tables
            plaintiff.adaAccommodationInd = "N";     //REQ
            //plantiff.accommodationType = "";      //REQ if accomodationInd = "Y", see code tables
            //plaintiff.additionalAccommodationDetails = "";//MAX 50 Chars

            // REQ if plantiff.partyDescription = "IND";
            //CivilFilingServiceReference.name plaintiffName = new CivilFilingServiceReference.name();
            //plaintiffName.firstName = "";
            //plaintiffName.middleName = "";
            //plaintiffName.lastName = "";
            //plantiff.name = plaintiffName;

            // PLAINTIFF ADDRESS REQ
            CivilFilingServiceReference.address pAddress = new CivilFilingServiceReference.address();
            pAddress.addressLine1 = "123 Main Street";
            //pAddress.addressLine2 = "Unit 24"; // not required
            pAddress.city = "Parsippany";
            pAddress.stateCode = "NJ";
            pAddress.zipCode = "07054";
            //pAddress.zipCodeExt = ""; // not required
            plaintiff.address = pAddress;

            // PLAINTIFF ALIAS LIST - NOT REQUIRED
            //CivilFilingServiceReference.partyAlias partyAliasList = new CivilFilingServiceReference.partyAlias();
            //partyAliasList.alternateTypeCode = "No";
            //partyAliasList.alternateName = "Max 65 Characters only text";

            // DEFENDANT
            CivilFilingServiceReference.party defendant = new CivilFilingServiceReference.party();
            defendant.partyDescription = "IND";     //REQ
            defendant.partyAffiliation = "HEI";     // not required, heir
            //defendant.corporationType = "CO";       //REQ if defendant.partyDescription = "BUS";
            //defendant.corporationName = "T Corp";   //REQ if defendant.partyDescription = "BUS";
            //defendant.phoneNumber = "1112223333";   // Max 10 digits

            // defendant.name REQ if defendant.partyDescription = "IND" 
            CivilFilingServiceReference.name defendantName = new CivilFilingServiceReference.name();
            defendantName.firstName = "Oliva";
            defendantName.middleName = "K";
            defendantName.lastName = "Dunham";
            defendant.name = defendantName;         //REQ if defendant.partyDescription = "IND";

            // defendant address is not REQ
            //CivilFilingServiceReference.address dAddress = new CivilFilingServiceReference.address();
            //dAddress.addressLine1 = "123 Main Street";
            //dAddress.addressLine1 = "Unit 42";
            //dAddress.city = "Parsippany";
            //dAddress.stateCode = "NJ";
            //dAddress.zipCode = "07054";
            //dAddress.zipCodeExt = "01";
            //defendant.address = dAddress;

            // DEFENDANT ALIAS LIST - NOT REQUIRED
            // CivilFilingServiceReference.partyAlias partyAliasList = new CivilFilingServiceReference.partyAlias();
            // partyAliasList is already used above just reusing the code for this test stub
            //partyAliasList.alternateTypeCode = "No";
            //partyAliasList.alternateName = "Max 65 Characters only text";

            // ATTACHMENT
            CivilFilingServiceReference.attachment att = new CivilFilingServiceReference.attachment();
            string filePath = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\CivilFilingClient\TestFiles\Test.pdf";
            //TODO: how large are the files?
            byte[] bytes = File.ReadAllBytes(filePath);

            att.documentCode = "CMPL";          //REQ this is really the documentType
            att.docType = "pdf";                //REQ
            att.contentType = "application/pdf";//REQ
            att.documentName = "Test";          //REQ
            att.documentDescription = "Complaint"; //REQ
            att.extention = ".pdf";             //REQ
            att.bytes = bytes;                  //REQ and referenced as document bytes

            //FEE
            CivilFilingServiceReference.fee fee = new CivilFilingServiceReference.fee();
            fee.attorneyFee = Convert.ToDecimal(0); //not required
            fee.paymentType = "CG";         //REQ is feeExempt is No
            fee.accountNumber = "141375";   //REQ is feeExempt is No
            fee.attorneyClientRefNumber = "1"; //numeric, not required
            fee.feeExempt = "Y";            //REQ
            fee.exemptionReason = "CO";     //REQ is feeExempt is Yes, see code tables

            // Add everything we just created to the packet (bulkFilingPacket)
            // takes and array of attachment[] we just have attachment

            //caseData
            packet.civilCase = caseData;

            // plantiffs
            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plaintiff;
            packet.plaintiffList = plantiffs;

            //defendants
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            packet.defendantList = defendants;

            //attachments
            CivilFilingServiceReference.attachment[] attachments = new CivilFilingServiceReference.attachment[1];
            attachments[0] = att;
            packet.attachmentList = attachments;

            //fees
            packet.fee = fee;

            // misc
            packet.documentRedactionInd = "Y"; //Code: ECCV100 Description: Document Redaction Indicator should be Y

            // create a request to send ... and assign the packet we just created
            CivilFilingServiceReference.civilFilingRequest filingRequest =
                new CivilFilingServiceReference.civilFilingRequest();
            filingRequest.bulkFilingPacket = packet;

            return filingRequest;
        }

        /// <summary>
        /// TestIndividual2Corp
        /// </summary>
        /// <returns></returns>
        public static CivilFilingServiceReference.civilFilingRequest TestIndividual2Corp()
        {
            // bulkFilingPacket REQ
            CivilFilingServiceReference.bulkFilingPacket packet = new CivilFilingServiceReference.bulkFilingPacket();
            packet.attorneyId = "888888005";            //Required
            packet.attorneyFirmId = "F88888003";        //Required

            // REQUIRED Branch Id - need the 
            CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute();
            attr.name = "branchId";
            attr.value = "0001";
            CivilFilingServiceReference.attribute[] attrs = new CivilFilingServiceReference.attribute[1];
            attrs[0] = attr;
            packet.attributes = attrs;

            // Begining of Test Code
            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.courtSection = "SCP";       //REQ
            caseData.venue = "ATL";              //REQ
            caseData.otherCourtActions = "Y";    //REQ
            caseData.caseAction = "028";         //REQ
            caseData.demandAmount = Convert.ToDecimal(5400);  //REQ
            caseData.demandAmountSpecified = true;
            caseData.juryDemand = "N";           //REQ
            caseData.serviceMethod = "03";       //REQ
            caseData.lawFirmCaseId = "IndividualvsCorp"; //Code: ECCV110 Description: Law Firm Case Id should be alphanumeric 
            caseData.venueOfIncident = "CPM";    //REQ
            caseData.plaintiffCaption = "plaintiffCaption not required";
            caseData.defendantCaption = "defendantCaption not required";
            caseData.docketDetailsForOtherCourt = "docketDetailsForOtherCourt";

            // PLANTIFF LIST
            CivilFilingServiceReference.party plaintiff = new CivilFilingServiceReference.party();
            plaintiff.partyDescription = "IND";      //REQ
            //plaintiff.partyAffiliation = "ADM";      //not required
            //plaintiff.corporationType = "CO";        //REQ if plantiff.partyDescription = "BUS";
            //plaintiff.corporationName = "Massive Dynamic";//REQ if plantiff.partyDescription = "BUS";
            //plantiff.phoneNumber = "1112223333";  // not required
            plaintiff.interpreterInd = "N";          //REQ
            //plantiff.language = "";                //REQ if interpreterInd = "Y", see code tables
            plaintiff.adaAccommodationInd = "N";     //REQ
            //plantiff.accommodationType = "";      //REQ if accomodationInd = "Y", see code tables
            //plaintiff.additionalAccommodationDetails = "";//MAX 50 Chars

            // REQ if plantiff.partyDescription = "IND";
            CivilFilingServiceReference.name plaintiffName = new CivilFilingServiceReference.name();
            plaintiffName.firstName = "Mitch";
            plaintiffName.middleName = "M";
            plaintiffName.lastName = "McDeere";
            plaintiff.name = plaintiffName;

            // PLAINTIFF ADDRESS REQ
            CivilFilingServiceReference.address pAddress = new CivilFilingServiceReference.address();
            pAddress.addressLine1 = "123 Main Street";
            //pAddress.addressLine2 = "Unit 24"; // not required
            pAddress.city = "Memphis";
            pAddress.stateCode = "TN";
            pAddress.zipCode = "37501";
            //pAddress.zipCodeExt = ""; // not required
            plaintiff.address = pAddress;

            // PLAINTIFF ALIAS LIST - NOT REQUIRED
            //CivilFilingServiceReference.partyAlias partyAliasList = new CivilFilingServiceReference.partyAlias();
            //partyAliasList.alternateTypeCode = "No";
            //partyAliasList.alternateName = "Max 65 Characters only text";

            // DEFENDANT
            CivilFilingServiceReference.party defendant = new CivilFilingServiceReference.party();
            defendant.partyDescription = "BUS";     //REQ
            //defendant.partyAffiliation = "HEI";     // not required, heir
            defendant.corporationType = "CO";       //REQ if defendant.partyDescription = "BUS";
            defendant.corporationName = "BL&L";   //REQ if defendant.partyDescription = "BUS";
            //defendant.phoneNumber = "1112223333";   // Max 10 digits

            // defendant.name REQ if defendant.partyDescription = "IND" 
            //CivilFilingServiceReference.name defendantName = new CivilFilingServiceReference.name();
            //defendantName.firstName = "Oliva";
            //defendantName.middleName = "K";
            //defendantName.lastName = "Dunham";
            //defendant.name = defendantName;         //REQ if defendant.partyDescription = "IND";

            // defendant address is not REQ
            //CivilFilingServiceReference.address dAddress = new CivilFilingServiceReference.address();
            //dAddress.addressLine1 = "123 Main Street";
            //dAddress.addressLine1 = "Unit 42";
            //dAddress.city = "Parsippany";
            //dAddress.stateCode = "NJ";
            //dAddress.zipCode = "07054";
            //dAddress.zipCodeExt = "01";
            //defendant.address = dAddress;

            // DEFENDANT ALIAS LIST - NOT REQUIRED
            // CivilFilingServiceReference.partyAlias partyAliasList = new CivilFilingServiceReference.partyAlias();
            // partyAliasList is already used above just reusing the code for this test stub
            //partyAliasList.alternateTypeCode = "No";
            //partyAliasList.alternateName = "Max 65 Characters only text";


            // ATTACHMENT
            CivilFilingServiceReference.attachment att = new CivilFilingServiceReference.attachment();
            string filePath = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\CivilFilingClient\TestFiles\Test.pdf";
            //TODO: how large are the files?
            byte[] bytes = File.ReadAllBytes(filePath);

            att.documentCode = "CMPL";          //REQ this is really the documentType
            att.docType = "pdf";                //REQ
            att.contentType = "application/pdf";//REQ
            att.documentName = "Test";          //REQ
            att.documentDescription = "Complaint"; //REQ
            att.extention = ".pdf";             //REQ
            att.bytes = bytes;                  //REQ and referenced as document bytes

            //FEE
            CivilFilingServiceReference.fee fee = new CivilFilingServiceReference.fee();
            fee.attorneyFee = Convert.ToDecimal(0); //not required
            fee.paymentType = "CG";         //REQ is feeExempt is No
            fee.accountNumber = "141375";   //REQ is feeExempt is No
            fee.attorneyClientRefNumber = "1"; //numeric, not required
            fee.feeExempt = "Y";            //REQ
            fee.exemptionReason = "CO";     //REQ is feeExempt is Yes, see code tables

            // Add everything we just created to the packet (bulkFilingPacket)
            // takes and array of attachment[] we just have attachment
            //caseData
            packet.civilCase = caseData;

            // plantiffs
            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plaintiff;
            packet.plaintiffList = plantiffs;

            //defendants
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            packet.defendantList = defendants;

            //attachments
            CivilFilingServiceReference.attachment[] attachments = new CivilFilingServiceReference.attachment[1];
            attachments[0] = att;
            packet.attachmentList = attachments;

            //fees
            packet.fee = fee;

            // misc
            packet.documentRedactionInd = "Y"; //Code: ECCV100 Description: Document Redaction Indicator should be Y

            // create a request to send ... and assign the packet we just created
            CivilFilingServiceReference.civilFilingRequest filingRequest =
                new CivilFilingServiceReference.civilFilingRequest();
            filingRequest.bulkFilingPacket = packet;

            return filingRequest;
        }

        /// <summary>
        /// TestIndividual2Individual
        /// </summary>
        /// <returns></returns>
        public static CivilFilingServiceReference.civilFilingRequest TestIndividual2Individual()
        {
            // bulkFilingPacket REQ
            CivilFilingServiceReference.bulkFilingPacket packet = new CivilFilingServiceReference.bulkFilingPacket();
            packet.attorneyId = "888888005";            //Required
            packet.attorneyFirmId = "F88888003";        //Required

            // REQUIRED Branch Id - need the 
            CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute();
            attr.name = "branchId";
            attr.value = "0001";
            CivilFilingServiceReference.attribute[] attrs = new CivilFilingServiceReference.attribute[1];
            attrs[0] = attr;
            packet.attributes = attrs;

            // Begining of Test Code
            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.courtSection = "SCP";       //REQ
            caseData.venue = "ATL";              //REQ
            caseData.otherCourtActions = "Y";    //REQ
            caseData.caseAction = "028";         //REQ
            caseData.demandAmount = Convert.ToDecimal(5400);  //REQ
            caseData.demandAmountSpecified = true;
            caseData.juryDemand = "N";           //REQ
            caseData.serviceMethod = "03";       //REQ
            caseData.lawFirmCaseId = "IndvsInd"; //Code: ECCV110 Description: Law Firm Case Id should be alphanumeric 
            caseData.venueOfIncident = "CPM";    //REQ
            caseData.plaintiffCaption = "plaintiffCaption not required";
            caseData.defendantCaption = "defendantCaption not required";
            caseData.docketDetailsForOtherCourt = "docketDetailsForOtherCourt";

            // PLANTIFF LIST
            CivilFilingServiceReference.party plaintiff = new CivilFilingServiceReference.party();
            plaintiff.partyDescription = "IND";      //REQ
            //plaintiff.partyAffiliation = "ADM";      //not required
            //plaintiff.corporationType = "CO";        //REQ if plantiff.partyDescription = "BUS";
            //plaintiff.corporationName = "Massive Dynamic";//REQ if plantiff.partyDescription = "BUS";
            //plantiff.phoneNumber = "1112223333";  // not required
            plaintiff.interpreterInd = "N";          //REQ
            //plantiff.language = "";                //REQ if interpreterInd = "Y", see code tables
            plaintiff.adaAccommodationInd = "N";     //REQ
            //plantiff.accommodationType = "";      //REQ if accomodationInd = "Y", see code tables
            //plaintiff.additionalAccommodationDetails = "";//MAX 50 Chars

            // REQ if plantiff.partyDescription = "IND";
            CivilFilingServiceReference.name plaintiffName = new CivilFilingServiceReference.name();
            plaintiffName.firstName = "Kanye";
            plaintiffName.middleName = "Omari";
            plaintiffName.lastName = "West";
            plaintiff.name = plaintiffName;

            // PLAINTIFF ADDRESS REQ
            CivilFilingServiceReference.address pAddress = new CivilFilingServiceReference.address();
            pAddress.addressLine1 = "123 Main Street";
            pAddress.addressLine2 = "Unit 24"; // not required
            pAddress.city = "Memphis";
            pAddress.stateCode = "TN";
            pAddress.zipCode = "37501";
            //pAddress.zipCodeExt = ""; // not required
            plaintiff.address = pAddress;

            // PLAINTIFF ALIAS LIST - NOT REQUIRED
            //CivilFilingServiceReference.partyAlias partyAliasList = new CivilFilingServiceReference.partyAlias();
            //partyAliasList.alternateTypeCode = "No";
            //partyAliasList.alternateName = "Max 65 Characters only text";

            // DEFENDANT
            CivilFilingServiceReference.party defendant = new CivilFilingServiceReference.party();
            defendant.partyDescription = "IND";     //REQ
            defendant.partyAffiliation = "USA";     // not required, heir
            //defendant.corporationType = "CO";       //REQ if defendant.partyDescription = "BUS";
            //defendant.corporationName = "T Corp";   //REQ if defendant.partyDescription = "BUS";
            //defendant.phoneNumber = "1112223333";   // Max 10 digits

            // DEFENDANT
            // defendant.name REQ if defendant.partyDescription = "IND" 
            CivilFilingServiceReference.name defendantName = new CivilFilingServiceReference.name();
            defendantName.firstName = "Taylor";
            defendantName.middleName = "Alison";
            defendantName.lastName = "Swift";
            defendant.name = defendantName;         //REQ if defendant.partyDescription = "IND";

            // defendant address is not REQ

            // DEFENDANT ALIAS LIST - NOT REQUIRED

            // ATTACHMENT
            CivilFilingServiceReference.attachment att = new CivilFilingServiceReference.attachment();
            string filePath = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\CivilFilingClient\TestFiles\Test.pdf";
            //TODO: how large are the files?
            byte[] bytes = File.ReadAllBytes(filePath);

            att.documentCode = "CMPL";          //REQ this is really the documentType
            att.docType = "pdf";                //REQ
            att.contentType = "application/pdf";//REQ
            att.documentName = "Test";          //REQ
            att.documentDescription = "Complaint"; //REQ
            att.extention = ".pdf";             //REQ
            att.bytes = bytes;                  //REQ and referenced as document bytes

            //FEE
            CivilFilingServiceReference.fee fee = new CivilFilingServiceReference.fee();
            fee.attorneyFee = Convert.ToDecimal(0); //not required
            fee.paymentType = "CG";         //REQ is feeExempt is No
            fee.accountNumber = "141375";   //REQ is feeExempt is No
            fee.attorneyClientRefNumber = "1"; //numeric, not required
            fee.feeExempt = "Y";            //REQ
            fee.exemptionReason = "CO";     //REQ is feeExempt is Yes, see code tables

            // Add everything we just created to the packet (bulkFilingPacket)
            // takes and array of attachment[] we just have attachment

            //caseData
            packet.civilCase = caseData;

            // plantiffs
            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plaintiff;
            packet.plaintiffList = plantiffs;

            //defendants
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            packet.defendantList = defendants;

            //attachments
            CivilFilingServiceReference.attachment[] attachments = new CivilFilingServiceReference.attachment[1];
            attachments[0] = att;
            packet.attachmentList = attachments;

            //fees
            packet.fee = fee;

            // misc
            packet.documentRedactionInd = "Y"; //Code: ECCV100 Description: Document Redaction Indicator should be Y

            // create a request to send ... and assign the packet we just created
            CivilFilingServiceReference.civilFilingRequest filingRequest =
                new CivilFilingServiceReference.civilFilingRequest();
            filingRequest.bulkFilingPacket = packet;

            return filingRequest;
        }
    }
}
