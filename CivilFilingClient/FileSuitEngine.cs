using NLog;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;

namespace CivilFilingClient
{
    public class FileSuitEngine
    {
        /// <summary>
        /// Initialize NLog logger
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        //Constructor
        public FileSuitEngine(string username, string password, string endpoint, string xmlFilePath, List<string> responses)
        {
            Username = username;
            Password = password;
            Endpoint = endpoint;
            XmlFilePath = xmlFilePath;
            Responses = responses;
        }

        public FileSuitEngine(string username, string password, string endpoint, string xmlFilePath, string pdfFilePath, List<string> responses)
        {
            Username = username;
            Password = password;
            Endpoint = endpoint;
            XmlFilePath = xmlFilePath;
            PdfFilePath = pdfFilePath;
            Responses = responses;
        }

        private string Username { get; set; }
        private string Password { get; set; }
        private string Endpoint { get; set; }
        private string XmlFilePath { get; set; }
        private string PdfFilePath { get; set; }
        private List<string> Responses { get; set; }

        /// <summary>
        /// FileSuit uses the XML file to process a complaint / suit
        /// </summary>
        /// <returns></returns>
        public bool FileSuit()
        {
            bool IsSuccess = false;
            // 6 chars with leading zeros when response is a success
            // set to Error when not a success
            string strDocketNumber = string.Empty;
            List<CourtCaseFiles> files = new List<CourtCaseFiles>();

            string fileMsg = "XML File:" + XmlFilePath;
            Responses.Add(fileMsg);
            _logger.Info(fileMsg);

            string pdfFilepath = string.Empty;
            // This is the complicated part...  If I have a list of attachements they
            // are in a list, we can also accept the xmlFilePath and pdfFilePath
            var bfp = Util.ReadXmlfile(XmlFilePath, Responses, files, out pdfFilepath);
            if (bfp == null)
            {
                Responses.Add("Invalid formatted file:" + Path.GetFileName(XmlFilePath));
                _logger.Info("Invalid formatted file:" + Path.GetFileName(XmlFilePath));
                return false;
            }

            var filingRequest = new CivilFilingServiceReference.civilFilingRequest();
            filingRequest.bulkFilingPacket = bfp;

            var address = new EndpointAddress(Endpoint);
            // Create a new client with the endpoint we want to send the request
            var client = new CivilFilingServiceReference.CivilFilingWSClient("CivilFilingWSPort", address);
            // Create the response outside of the using since instantiation inside of using limits the scope of the variable
            CivilFilingServiceReference.civilFilingResponse filingReponse = null;
            using (new OperationContextScope(client.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("feinsuch", Username, Password));
                string message = "Attempting to send the web request to:" + client.Endpoint.Address.ToString();
                Responses.Add(message);
                _logger.Warn(message);
                try
                {
                    // Web Service Can timeout.  We should still write a log file of Automation tracking.
                    filingReponse = client.submitCivilFiling(filingRequest);
                }
                catch(System.Exception ex)
                {
                    // TODO: Colors these red
                    Responses.Add("eCourts " + Endpoint + " error :" + message);
                    _logger.Error("eCourts " + Endpoint + " error :" + message);

                    if (ex.InnerException != null)
                    {
                        Responses.Add("eCourts " + Endpoint + " error :" + ex.InnerException);
                        _logger.Error("eCourts " + Endpoint + " error :" + ex.InnerException);
                    }
                    Util.SaveResponseToFile(Responses, "Failed", XmlFilePath,  pdfFilepath);
                    // throw the error back to the UI so they know the timeout occured
                    // note this will be logged 2x because we are throwing it up the stack
                    throw new System.ArgumentException("eCourts " + Endpoint + " error :", ex.Message); ;
                }
            }
            if (filingReponse.messages != null)
            {
                foreach (var msg in filingReponse.messages)
                {
                    string filingMsg = "eCourts | Code: " + msg.code + " Description: " + msg.description;
                    Responses.Add(filingMsg);
                    _logger.Warn(filingMsg);
                }
            }
            if (filingReponse.efilingNumber != null)
            {
                //TODO: Color these blue
                string eFilingNumberMsg = "eCourts | Efiling Sequence Number :" +
                    filingReponse.efilingNumber.efilingCourtDiv.ToString() +
                    filingReponse.efilingNumber.efilingCourtYr.ToString() +
                    filingReponse.efilingNumber.efilingSeqNo.ToString();
                Responses.Add(eFilingNumberMsg);
                _logger.Info(eFilingNumberMsg);
            }
            if (filingReponse.docketNumber != null)
            {
                // docketSeqNum padded front with zero, max of 6 chars
                string dockerSeqNum = filingReponse.docketNumber.docketSeqNum.ToString().PadLeft(6, '0');
                string strDocketCode = filingReponse.docketNumber.docketVenue
                    + "-" + filingReponse.docketNumber.docketTypeCode
                    + "-" + dockerSeqNum
                    + "-" + filingReponse.docketNumber.docketCourtYear;

                strDocketNumber = strDocketCode;
                string docketNumberMsg = "eCourts | Docket Number :" +
                    strDocketCode;

                Responses.Add(docketNumberMsg);
                _logger.Info(docketNumberMsg);
            }
            Responses.Add("End of Submission");
            _logger.Info("End of Submission");

            //When no docket number is received the transaction might have failed
            //If we have a eFilingNumber the message has been queued.
            if (strDocketNumber.Length == 0)
            {
                strDocketNumber = "Failed";
                string failedMesssage = "Failed. Please review the eCourts | Code above.";
                Responses.Add(failedMesssage);
                _logger.Info(failedMesssage);
            }
            else
            {
                IsSuccess = true;
            }
            // Save the data out to a file, in this reality it should be taking one xml and one pdf file only
            // The problem here is we need the full path to the pdf... which should be the same DirectoryName
            // with the PDF name tagged onto the directory...  Need to clean this up
            Util.SaveResponseToFile(Responses, strDocketNumber, XmlFilePath,  pdfFilepath);

            return IsSuccess;
        }

        /// <summary>
        /// FileSuitCSV uses the csv to process and send a complaint, one complaint per file.
        /// </summary>
        /// <returns></returns>
        public bool FileSuitCSV()
        {
            bool IsSuccess = false;
            // 6 chars with leading zeros when response is a success
            // set to Error when not a success
            string strDocketNumber = string.Empty;
            List<CourtCaseFiles> files = new List<CourtCaseFiles>();

            // Note the message has been changed to CSV File but the XmlFilePath variable is still the old variable name which holds a csv path for this method.
            // We can change the file path to be less descriptive later after testing
            string fileMsg = "CSV File:" + XmlFilePath;
            Responses.Add(fileMsg);
            _logger.Info(fileMsg);

            string pdfFilepath = string.Empty;

            List<ComplaintCsv> values = File.ReadAllLines(XmlFilePath)
                               .Skip(1)
                               .Select(v => ComplaintCsv.FromCsv(v))
                               .ToList();

            // This is the complicated part...  If I have a list of attachements they
            // are in a list, we can also accept the xmlFilePath and pdfFilePath
            //var bfp = Util.ReadXmlfile(XmlFilePath, Responses, files, out pdfFilepath);

            foreach(var value in values)
            {
                value.ToString();
            }
            //Create the soap message from scratch this time
            CivilFilingServiceReference.bulkFilingPacket bfp = new CivilFilingServiceReference.bulkFilingPacket();
            bfp.attorneyId = "";            //Required
            bfp.attorneyFirmId = "F88888003";        //Required

            // REQUIRED Branch Id - need the 
            CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute();
            attr.name = "branchId";
            attr.value = "0001";
            CivilFilingServiceReference.attribute[] attrs = new CivilFilingServiceReference.attribute[1];
            attrs[0] = attr;
            bfp.attributes = attrs;

            // Begining of Test Code
            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.courtSection = "SCP";       //REQ
            caseData.venue = "ATL";              //REQ
            caseData.otherCourtActions = "Y";    //REQ
            caseData.caseAction = "028";         //REQ
            caseData.demandAmount = System.Convert.ToDecimal(5400);  //REQ
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
            fee.attorneyFee = System.Convert.ToDecimal(0); //not required
            fee.paymentType = "CG";         //REQ is feeExempt is No
            fee.accountNumber = "141375";   //REQ is feeExempt is No
            fee.attorneyClientRefNumber = "1"; //numeric, not required
            fee.feeExempt = "Y";            //REQ
            fee.exemptionReason = "CO";     //REQ is feeExempt is Yes, see code tables

            // Add everything we just created to the packet (bulkFilingPacket)
            // takes and array of attachment[] we just have attachment

            //caseData
            bfp.civilCase = caseData;

            // plantiffs
            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plaintiff;
            bfp.plaintiffList = plantiffs;

            //defendants
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            bfp.defendantList = defendants;

            //attachments
            CivilFilingServiceReference.attachment[] attachments = new CivilFilingServiceReference.attachment[1];
            attachments[0] = att;
            bfp.attachmentList = attachments;

            //fees
            bfp.fee = fee;

            // misc
            bfp.documentRedactionInd = "Y"; //Code: ECCV100 Description: Document Redaction Indicator should be Y


            if (bfp == null)
            {
                Responses.Add("Invalid formatted file:" + Path.GetFileName(XmlFilePath));
                _logger.Info("Invalid formatted file:" + Path.GetFileName(XmlFilePath));
                return false;
            }

            var filingRequest = new CivilFilingServiceReference.civilFilingRequest();
            filingRequest.bulkFilingPacket = bfp;

            var address = new EndpointAddress(Endpoint);
            // Create a new client with the endpoint we want to send the request
            var client = new CivilFilingServiceReference.CivilFilingWSClient("CivilFilingWSPort", address);
            // Create the response outside of the using since instantiation inside of using limits the scope of the variable
            CivilFilingServiceReference.civilFilingResponse filingReponse = null;
            using (new OperationContextScope(client.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("feinsuch", Username, Password));
                string message = "Attempting to send the web request to:" + client.Endpoint.Address.ToString();
                Responses.Add(message);
                _logger.Warn(message);
                try
                {
                    // Web Service Can timeout.  We should still write a log file of Automation tracking.
                    filingReponse = client.submitCivilFiling(filingRequest);
                }
                catch (System.Exception ex)
                {
                    // TODO: Colors these red
                    Responses.Add("eCourts " + Endpoint + " error :" + message);
                    _logger.Error("eCourts " + Endpoint + " error :" + message);

                    if (ex.InnerException != null)
                    {
                        Responses.Add("eCourts " + Endpoint + " error :" + ex.InnerException);
                        _logger.Error("eCourts " + Endpoint + " error :" + ex.InnerException);
                    }
                    Util.SaveResponseToFile(Responses, "Failed", XmlFilePath, pdfFilepath);
                    // throw the error back to the UI so they know the timeout occured
                    // note this will be logged 2x because we are throwing it up the stack
                    throw new System.ArgumentException("eCourts " + Endpoint + " error :", ex.Message); ;
                }
            }
            if (filingReponse.messages != null)
            {
                foreach (var msg in filingReponse.messages)
                {
                    string filingMsg = "eCourts | Code: " + msg.code + " Description: " + msg.description;
                    Responses.Add(filingMsg);
                    _logger.Warn(filingMsg);
                }
            }
            if (filingReponse.efilingNumber != null)
            {
                //TODO: Color these blue
                string eFilingNumberMsg = "eCourts | Efiling Sequence Number :" +
                    filingReponse.efilingNumber.efilingCourtDiv.ToString() +
                    filingReponse.efilingNumber.efilingCourtYr.ToString() +
                    filingReponse.efilingNumber.efilingSeqNo.ToString();
                Responses.Add(eFilingNumberMsg);
                _logger.Info(eFilingNumberMsg);
            }
            if (filingReponse.docketNumber != null)
            {
                // docketSeqNum padded front with zero, max of 6 chars
                string dockerSeqNum = filingReponse.docketNumber.docketSeqNum.ToString().PadLeft(6, '0');
                string strDocketCode = filingReponse.docketNumber.docketVenue
                    + "-" + filingReponse.docketNumber.docketTypeCode
                    + "-" + dockerSeqNum
                    + "-" + filingReponse.docketNumber.docketCourtYear;

                strDocketNumber = strDocketCode;
                string docketNumberMsg = "eCourts | Docket Number :" +
                    strDocketCode;

                Responses.Add(docketNumberMsg);
                _logger.Info(docketNumberMsg);
            }
            Responses.Add("End of Submission");
            _logger.Info("End of Submission");

            //When no docket number is received the transaction might have failed
            //If we have a eFilingNumber the message has been queued.
            if (strDocketNumber.Length == 0)
            {
                strDocketNumber = "Failed";
                string failedMesssage = "Failed. Please review the eCourts | Code above.";
                Responses.Add(failedMesssage);
                _logger.Info(failedMesssage);
            }
            else
            {
                IsSuccess = true;
            }
            // Save the data out to a file, in this reality it should be taking one xml and one pdf file only
            // The problem here is we need the full path to the pdf... which should be the same DirectoryName
            // with the PDF name tagged onto the directory...  Need to clean this up
            Util.SaveResponseToFile(Responses, strDocketNumber, XmlFilePath, pdfFilepath);

            return IsSuccess;

        }
    }
}