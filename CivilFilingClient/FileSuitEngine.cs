using Microsoft.VisualBasic.FileIO;
using NLog;
using System;
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
            filePath = xmlFilePath;
            Responses = responses;
        }

        public FileSuitEngine(string username, string password, string endpoint, string xmlFilePath, string pdfFilePath, List<string> responses)
        {
            Username = username;
            Password = password;
            Endpoint = endpoint;
            filePath = xmlFilePath;
            PdfFilePath = pdfFilePath;
            Responses = responses;
        }

        private string Username { get; set; }
        private string Password { get; set; }
        private string Endpoint { get; set; }
        private string filePath { get; set; }
        private string PdfFilePath { get; set; }
        private List<string> Responses { get; set; }

        /// <summary>
        /// FileSuitXml uses the XML file to process a complaint / suit
        /// </summary>
        /// <returns></returns>
        public bool FileSuitXml()
        {
            bool IsSuccess = false;
            // 6 chars with leading zeros when response is a success
            // set to Error when not a success
            string strDocketNumber = string.Empty;
            List<CourtCaseFiles> files = new List<CourtCaseFiles>();

            string fileMsg = "XML File:" + filePath;
            Responses.Add(fileMsg);
            _logger.Info(fileMsg);

            string pdfFilepath = string.Empty;
            // This is the complicated part...  If I have a list of attachements they
            // are in a list, we can also accept the xmlFilePath and pdfFilePath
            var bfp = Util.ReadXmlfile(filePath, Responses, files, out pdfFilepath);
            if (bfp == null)
            {
                Responses.Add("Invalid formatted file:" + Path.GetFileName(filePath));
                _logger.Info("Invalid formatted file:" + Path.GetFileName(filePath));
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
                    Util.SaveResponseToFile(Responses, "Failed", filePath, pdfFilepath);
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
            Util.SaveResponseToFile(Responses, strDocketNumber, filePath, pdfFilepath);

            return IsSuccess;
        }

        /// <summary>
        /// FileSuitCSV uses a csv format to process and send a complaint, one complaint per file.
        /// This format was created to help law firms easily create a file to use with this application instead
        /// of using the xml format which has been more complicated to implement for most law firms.
        /// </summary>
        /// <returns></returns>
        public bool FileSuitCSV()
        {
            var complaints = new List<ComplaintCsv>();

            bool IsSuccess = false;
            // 6 chars with leading zeros when response is a success
            // set to Error when not a success
            string strDocketNumber = string.Empty;


            // Note the message has been changed to CSV File but the XmlFilePath variable is still the old variable name which holds a csv path for this method.
            // We can change the file path to be less descriptive later after testing
            string fileMsg = "CSV File:" + this.filePath;
            Responses.Add(fileMsg);
            _logger.Info(fileMsg);

            string pdfFilepath = string.Empty;

            // Populate ComplaintCsv with the data from the csv file
            using (TextFieldParser fieldParser = new TextFieldParser(this.filePath))
            {
                // setting for skipping first line
                bool firstLine = true;

                // Configure the TextFieldParser
                // This tells the parser it is a Delimited text 
                fieldParser.TextFieldType = FieldType.Delimited;
                // This tells the parser that delimiter used is a ,
                fieldParser.Delimiters = new string[] { "," };
                // This tells the parser that some fields may have quotes around them
                fieldParser.HasFieldsEnclosedInQuotes = true;
                // Used to hold the fields after each read
                string[] fields;

                // Read and process the fields
                // Each Row should contain 58 Fields
                // 2nd Row contains the main data set and one Plaintiff and Defendant
                // All other rows will contain additional Plaintiffs and Defendants
                while (!fieldParser.EndOfData)
                {
                    var lineNumber = fieldParser.LineNumber;
                    // Parse the line just read into the array
                    fields = fieldParser.ReadFields();

                    // get the column headers
                    if (firstLine)
                    {
                        firstLine = false;
                        continue;
                    }

                    var fieldMsg = "Line Number: " + lineNumber.ToString() + ", Number of Columns: " + fields.Length.ToString();
                    Responses.Add(fieldMsg);
                    _logger.Info(fieldMsg);

                    if (fields.Length != 58)
                    {
                        var fieldErr = "Error Line Number: " + lineNumber.ToString() + ", Number of Columns: " + fields.Length.ToString() + " | Number of columns should be 58. File has an error. Review the file for missing or added data and fix and process again.";
                        Responses.Add(fieldErr);
                        _logger.Info(fieldErr);
                    }

                    // Process the fields into the list object
                    // Trim all the data.
                    // Need error checking here for index out of range checks and captures
                    ComplaintCsv data = new ComplaintCsv();
                    data.AttorneyId = fields[0].Trim();             // REQ - 9 digits
                    data.AttorneyFirmId = fields[1].Trim();         // REQ - 9 digits
                    data.BranchId = fields[2].Trim();               // REQ - 
                    data.CourtSection = fields[3].Trim();           // REQ - Defaults to SCP, not defined in Code Tables
                    data.Venue = fields[4].Trim();                  // REQ - ATL,BER,BUR,CAM,CPM,CUM,ESX,GLO,HUD,HNT,MER,MID,MON,MRS,OCN,PAS,SLM,SOM,SSX,UNN,WRN (See Venue in Code Tables)
                    data.CertifcationText = fields[5].Trim();       // REQ - Default to Y for Yes
                    data.Action = fields[6].Trim();                 // REQ - 28,175,32,37,41,33 - 3 digit numeric, Refer to the Case Action worksheet in the Code Tables
                    var demandAmount = fields[7].Trim();
                    // Customer is sending in $1,000.00 so we need to clean this up
                    // typically this is handled by the application export but we are helping others here instead
                    demandAmount = demandAmount.Replace("$", "");
                    demandAmount = demandAmount.Replace(",", "");
                    data.DemandAmount = demandAmount;               // REQ - Less than 15000. Accepts up to two decimal places
                    data.JuryDemand = fields[8].Trim();             // REQ - N-is None; S-is 6 Jurors
                    data.ServiceMethod = fields[9].Trim();          // REQ - Default to 03, no documentation exists
                    data.LawFirmCaseId = Util.SubStr(fields[10], 20); // NOT REQ - Alpha-numeric, Max 20 chars
                    data.CountyOfIncident = fields[11].Trim();      // REQ - 3 letter venue code. Refer to the County worksheet in the Code Tables spreadsheet
                    data.PlantiffCaption = Util.SubStr(fields[12], 123);  // NOT REQ
                    data.DefendantCaption = Util.SubStr(fields[13], 123); // NOT REQ

                    data.PartyDescription_Plaintiff = fields[14].Trim();    // REQ - BUS or IND (TODO Add check)
                    data.PartyAffiliation_Plaintiff = fields[15].Trim();    // REQ if BUS
                    data.CorporationType_Plaintiff = fields[16].Trim();     // REQ if BUS
                    data.CorpName_Plaintiff = Util.SubStr(fields[17], 30); // NOT REQ - Max 30 chars
                    data.Phone_Plaintiff = Util.SubStr(fields[18], 10);    // NOT REQ
                    data.InterpreterNeeded_Plaintiff = fields[19].Trim();   // REQ - Y or N
                    data.Language_Plaintiff = fields[20].Trim();            // REQ if InterpreterNeeded is Y
                    data.AccomodationNeeded_Plaintiff = fields[21].Trim();  // REQ - Y or N
                    data.AccomodationRequirement_Plaintiff = fields[22].Trim();
                    data.AdditionalAccomodationDetails_Plaintiff = Util.SubStr(fields[23], 50);

                    data.FirstName_Plaintiff = Util.SubStr(fields[24], 9);    // REQ
                    data.MiddleInitial_Plaintiff = Util.SubStr(fields[25], 1);// REQ
                    data.LastName_Plaintiff = Util.SubStr(fields[26], 20);    // REQ

                    data.AddressLine1_Plaintiff = Util.SubStr(fields[27], 36);// REQ
                    data.AddressLine2_Plaintiff = Util.SubStr(fields[28], 36);// NOT REQ
                    data.City_Plaintiff = Util.SubStr(fields[29], 16);// REQ
                    data.State_Plaintiff = Util.SubStr(fields[30], 2);// REQ
                    data.ZipCode_Plaintiff = Util.SubStr(fields[31], 5); // REQ
                    data.ZipCodeExt_Plaintiff = Util.SubStr(fields[32], 4); // NOT REQ

                    data.AlternateType_Plaintiff = fields[33].Trim(); // NOT REQ - Refer to Alternate worksheet in Code Tables spreadsheet
                    data.AlternateName_Plaintiff = Util.SubStr(fields[34], 65); // TODO: AlternateType_Plaintiff must have value if this field has a value, could always force to AK

                    data.PartyDescription_Defendant = fields[35].Trim(); // REQ - BUS - Business, IND - Individual, FIC - Fictious
                    data.PartyAffiliation_Defendant = fields[36].Trim(); // NOT REQ
                    data.CorporationType_Defendant = fields[37].Trim();
                    data.Name_Defendant = Util.SubStr(fields[38], 30);   // Max 30
                    data.Phone_Defendant = Util.SubStr(fields[39], 10);  // Max 10

                    data.FirstName_Defendant = Util.SubStr(fields[40], 9); // Max 9
                    data.MiddleInitial_Defendant = Util.SubStr(fields[41], 1); // Max 1
                    data.LastName_Defendant = Util.SubStr(fields[42], 20); // Max 20

                    data.AddressLine1_Defendant = Util.SubStr(fields[43], 36); // Max 36
                    data.AddressLine2_Defendant = Util.SubStr(fields[44], 36); // Max 36
                    data.City_Defendant = Util.SubStr(fields[45], 16); // Max 16
                    data.State_Defendant = Util.SubStr(fields[46], 2); // Max 2
                    data.ZipCode_Defendant = Util.SubStr(fields[47], 5); // Max 5
                    data.ZipCodeExt_Defendant = Util.SubStr(fields[48], 4); // Max 4

                    data.AlternateType_Defendant = fields[49].Trim();
                    data.AlternateName_Defendant = Util.SubStr(fields[50], 65); // Max 65

                    // PDF location must be reachable by this application
                    data.PDF_FileLocation = fields[51].Trim();

                    // Fee: BulkFilingPacket > Fee
                    var attorneyFee = fields[52].Trim();
                    attorneyFee = attorneyFee.Replace("$", "");
                    attorneyFee = attorneyFee.Replace(",", "");
                    data.AttorneyFee = attorneyFee;
                    data.PaymentMethod = fields[53].Trim(); // REQ if Fee exempt is N, default to CG
                    data.AccountNumber = fields[54].Trim();
                    data.AttorneyClientRefNumber = Util.SubStr(fields[55], 10); // Max 10
                    data.FeeExempt = fields[56].Trim();  // REQ  - Y or N
                    data.ReasonForFilingFeeExemption = fields[57].Trim();

                    complaints.Add(data);

                }
            }

            ////
            // As we build the bulkFilingPacket we will attempt to apply the Validation Rules
            // 
            ////

            // Create the soap message
            CivilFilingServiceReference.bulkFilingPacket bfp = new CivilFilingServiceReference.bulkFilingPacket
            {
                attorneyId = complaints[0].AttorneyId,          // REQ
                attorneyFirmId = complaints[0].AttorneyFirmId  // REQ
            };

            // REQUIRED Branch Id is an Attribute
            CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute
            {
                name = "branchId",                 // REQ
                value = complaints[0].BranchId     // REQ
            };

            // Populate the attributes
            CivilFilingServiceReference.attribute[] attrs = new CivilFilingServiceReference.attribute[1];
            attrs[0] = attr;
            bfp.attributes = attrs;

            // CASE DATA
            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.courtSection = complaints[0].CourtSection;             // REQ - Defaults to SCP
            if (!VenueCodeValidate(complaints[0].Venue))
            {
                //TODO: move list up or show list in this message
                Responses.Add("Venue Code is not correct: " + complaints[0].Venue);
                _logger.Info("Venue Code is not correct: " + complaints[0].Venue);
            }
            caseData.venue = complaints[0].Venue;                           // REQ - TODO - Wire Up Validation
            caseData.otherCourtActions = complaints[0].CertifcationText;    // REQ - Defaults to Y
            if (!CaseActionValidate(complaints[0].Action))
            {
                Responses.Add("Action Code is not correct: " + complaints[0].Action);
                _logger.Info("Action Code is not correct: " + complaints[0].Action);
            }
            caseData.caseAction = complaints[0].Action;                     // REQ
            var demandAmt = System.Convert.ToDecimal(complaints[0].DemandAmount);
            if(demandAmt > 15000)
            {
                Responses.Add("Demand Amount is over 15000");
                _logger.Info("Demand Amount is over 15000");
            }
            // If Demand Amount is not null or empty try to convert and flip on the specified flag
            if (!String.IsNullOrEmpty(complaints[0].DemandAmount.Trim()))
            {
                caseData.demandAmount = System.Convert.ToDecimal(complaints[0].DemandAmount);  //REQ, TODO: Must be less than 15000.00
                caseData.demandAmountSpecified = true; // REQ, TODO need to create a check to see if a value exists in DemandAmount
            }
            caseData.juryDemand = complaints[0].JuryDemand;                // REQ N for None, S for 6 Jurors
            caseData.serviceMethod = complaints[0].ServiceMethod;          // REQ - Defaults to '03' no documentation
            caseData.lawFirmCaseId = complaints[0].LawFirmCaseId;          // Code: ECCV110 Description: Law Firm Case Id should be alphanumeric 
            if (!VenueCodeValidate(complaints[0].CountyOfIncident))
            {
                //TODO: move list up or show list in this message
                Responses.Add("Venue Code is not correct for CountyOfIncident: " + complaints[0].CountyOfIncident);
                _logger.Info("Venue Code is not correct for CountyOfIncident: " + complaints[0].CountyOfIncident);
            }
            caseData.venueOfIncident = complaints[0].CountyOfIncident;     // REQ
            caseData.plaintiffCaption = complaints[0].PlantiffCaption;
            caseData.defendantCaption = complaints[0].DefendantCaption;
            caseData.docketDetailsForOtherCourt = "";                      // NOT LISTED IN DOCUMENTATION

            // PLANTIFF LIST
            CivilFilingServiceReference.party plaintiff = new CivilFilingServiceReference.party();
            plaintiff.partyDescription = complaints[0].PartyDescription_Plaintiff;          // REQ - BUS OR IND
            plaintiff.partyAffiliation = complaints[0].PartyAffiliation_Plaintiff;          // NOT REQ
            // TODO: REQ if BUS, these must have values
            if (complaints[0].PartyDescription_Plaintiff == "BUS")
            {
                plaintiff.corporationType = complaints[0].CorporationType_Plaintiff; // REQ: CO,LC,LP,OT,SP - CorporationTypeValidate
                if (!CorporationTypeValidate(complaints[0].CorporationType_Plaintiff))
                {
                    Responses.Add("Code is not correct for corporationType: " + complaints[0].CorporationType_Plaintiff);
                    _logger.Info("Code is not correct for corporationType: " + complaints[0].CorporationType_Plaintiff);
                }
                if(complaints[0].CorpName_Plaintiff.Trim().Length < 1) // REQ if PartyDescription_Plaintiff is BUS
                {
                    Responses.Add("CorpName_Plaintiff needs to have a value: " + complaints[0].CorpName_Plaintiff);
                    _logger.Info("CorpName_Plaintiff needs to have a value: " + complaints[0].CorpName_Plaintiff);
                }
                plaintiff.corporationName = complaints[0].CorpName_Plaintiff;        // Max 30 chars, already truncated above when loading ComplaintCsv
            } 
            plaintiff.phoneNumber = complaints[0].Phone_Plaintiff;                   // NOT REQ
            plaintiff.interpreterInd = complaints[0].InterpreterNeeded_Plaintiff;    // REQ Y or N
            if(complaints[0].InterpreterNeeded_Plaintiff == "Y")
            {
                plaintiff.language = complaints[0].Language_Plaintiff;  // REQ if InterpreterNeeded_Plaintiff is BUS
                if (complaints[0].Language_Plaintiff.Trim().Length < 1) 
                {
                    Responses.Add("Language_Plaintiff needs to have a value: " + complaints[0].Language_Plaintiff);
                    _logger.Info("Language_Plaintiff needs to have a value: " + complaints[0].Language_Plaintiff);
                }
            }
            plaintiff.adaAccommodationInd = complaints[0].AccomodationNeeded_Plaintiff;     // REQ
            if(complaints[0].AccomodationNeeded_Plaintiff == "Y")
            {
                plaintiff.accommodationType = complaints[0].AccomodationRequirement_Plaintiff;
                if (complaints[0].AccomodationRequirement_Plaintiff.Trim().Length < 1)
                {
                    Responses.Add("Language_Plaintiff needs to have a value: " + complaints[0].AccomodationRequirement_Plaintiff);
                    _logger.Info("Language_Plaintiff needs to have a value: " + complaints[0].AccomodationRequirement_Plaintiff);
                }
            }
            plaintiff.additionalAccommodationDetails = complaints[0].AdditionalAccomodationDetails_Plaintiff; // MAX 50 Chars

            // REQ if plantiff.partyDescription = "IND";
            if (complaints[0].PartyDescription_Plaintiff == "IND")
            {
                CivilFilingServiceReference.name plaintiffName = new CivilFilingServiceReference.name();
                plaintiffName.firstName = complaints[0].FirstName_Plaintiff;
                plaintiffName.middleName = complaints[0].MiddleInitial_Plaintiff;
                plaintiffName.lastName = complaints[0].LastName_Plaintiff;
                plaintiff.name = plaintiffName;
            }
            // PLAINTIFF ADDRESS REQ
            CivilFilingServiceReference.address pAddress = new CivilFilingServiceReference.address();
            pAddress.addressLine1 = complaints[0].AddressLine1_Plaintiff; // REQ
            pAddress.addressLine2 = complaints[0].AddressLine2_Plaintiff; // NOT REQ
            pAddress.city = complaints[0].City_Plaintiff;                 // REQ
            pAddress.stateCode = complaints[0].State_Plaintiff;           // REQ
            pAddress.zipCode = complaints[0].ZipCode_Plaintiff;           // REQ
            pAddress.zipCodeExt = complaints[0].ZipCodeExt_Plaintiff; // NOT REQ
            plaintiff.address = pAddress;

            // PLAINTIFF ALIAS LIST - NOT REQUIRED - used List and then swapped to array for fun, See defendant alias for List coding example below
            var plaintiffAliasList = new CivilFilingServiceReference.partyAlias[1];
            var plaintiffAlias = new CivilFilingServiceReference.partyAlias();
            plaintiffAlias.alternateTypeCode = complaints[0].AlternateType_Plaintiff;
            if (complaints[0].AlternateType_Plaintiff == "AK")
            {
                // TODO: Acceptable values: AK,DB,FK,NK,OB,SB,SU,TA
                plaintiffAlias.alternateName = complaints[0].AlternateName_Plaintiff;   // MAX 65 Chars
                plaintiffAliasList[0] = plaintiffAlias;
                plaintiff.partyAliasList = plaintiffAliasList;
            }

            // ADD PRIMARY PLAINTIFF - NOT ALL ROWS MAY HAVE A PLAINTIFF - Default to one plaintiff for now
            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plaintiff;

            // DEFENDANT
            CivilFilingServiceReference.party defendant = new CivilFilingServiceReference.party();
            defendant.partyDescription = complaints[0].PartyDescription_Defendant;  // REQ BUS or IND or FIC
            defendant.partyAffiliation = complaints[0].PartyAffiliation_Defendant;  // NOT REQ
            defendant.corporationType = complaints[0].CorporationType_Defendant;    // REQ  "BUS";
            defendant.corporationName = complaints[0].Name_Defendant;               // REQ  "BUS";
            defendant.phoneNumber = complaints[0].Phone_Defendant;                  // NOT REQ Max 10 digits

            // defendant.name REQ if defendant.partyDescription = "IND" 
            CivilFilingServiceReference.name defendantName = new CivilFilingServiceReference.name();
            defendantName.firstName = complaints[0].FirstName_Defendant;    // REQ
            defendantName.middleName = complaints[0].MiddleInitial_Defendant;// REQ
            defendantName.lastName = complaints[0].LastName_Defendant;// REQ
            defendant.name = defendantName;

            // DEFENDANT ADDRESS NOT REQ
            CivilFilingServiceReference.address dAddress = new CivilFilingServiceReference.address();
            dAddress.addressLine1 = complaints[0].AddressLine1_Defendant; // NOT REQ
            dAddress.addressLine2 = complaints[0].AddressLine2_Defendant;// NOT REQ
            dAddress.city = complaints[0].City_Defendant;// NOT REQ
            dAddress.stateCode = complaints[0].State_Defendant;// NOT REQ
            dAddress.zipCode = complaints[0].ZipCode_Defendant;// NOT REQ
            dAddress.zipCodeExt = complaints[0].ZipCodeExt_Defendant;// NOT REQ
            defendant.address = dAddress;

            // DEFENDANT ALIAS LIST - NOT REQUIRED
            var defendantAliasList = new List<CivilFilingServiceReference.partyAlias>();
            var defendantAlias = new CivilFilingServiceReference.partyAlias();
            if (complaints[0].AlternateType_Defendant == "AK")
            {
                // TODO: Acceptable values: AK,DB,FK,NK,OB,SB,SU,TA
                defendantAlias.alternateTypeCode = complaints[0].AlternateType_Defendant;
                defendantAlias.alternateName = complaints[0].AlternateName_Defendant;   // MAX 65 Chars
                defendantAliasList.Add(defendantAlias);
                defendant.partyAliasList = defendantAliasList.ToArray();
            }

            // ADD PRIMARY DEFENDANT - ALL ROWS WILL CONTAIN DEFENDANT, If not we gotta find another to initialize the defendant array
            int numberOfDefendants = complaints.Count;  // default to 2 for testing SelfGen_Test.csv
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant; // Default to zero for primary defendant 

            // ATTACHMENT - Supporting only one attachment this version
            CivilFilingServiceReference.attachment att = new CivilFilingServiceReference.attachment();
            string filePath = complaints[0].PDF_FileLocation;
            byte[] bytes = File.ReadAllBytes(filePath);

            att.documentCode = "CMPL";                      // REQ - CMPL - Complaint, can it take more?
            att.docType = "pdf";                            // REQ - pdf
            att.contentType = "application/pdf";            // REQ - application/pdf
            att.documentName = Path.GetFileName(filePath);  // REQ - check to see if using this doubles up the extension name and works on the website
            att.documentDescription = "Complaint";          // REQ - Max 256 chars
            att.extention = ".pdf";                         // REQ - .pdf
            att.bytes = bytes;                              // REQ - Byte array with Base 64 encoding
            
            // FEE
            CivilFilingServiceReference.fee fee = new CivilFilingServiceReference.fee();
            fee.attorneyFee = Convert.ToDecimal(complaints[0].AttorneyFee);      // NOT REQ, Numeric
            fee.attorneyFeeSpecified = true;                                     // TODO: Work in a check for this, if user supplied a attorneyFee
            fee.feeExempt = complaints[0].FeeExempt;                             // REQ  Y or N, usually a N.
            fee.paymentType = complaints[0].PaymentMethod;                       // REQ - 'CG' if feeExempt is N
            fee.accountNumber = complaints[0].AccountNumber;                     // REQ if feeExempt is N, Numeric
            fee.attorneyClientRefNumber = complaints[0].AttorneyClientRefNumber; // NOT REQ - Numeric
            fee.exemptionReason = complaints[0].ReasonForFilingFeeExemption;     // REQ if feeExempt is Y, see code table

            // Check for Additional Plaintiffs and Defendants
            // 1st line is Headers, 2nd line is the default complaint
            // All lines past the 2nd line are additional plaintiffs and defendants
            // Note: complaints list will not contain the header line from file so we can use a count > 1, the default complaint to start
            // the adding of additional plaintiffs and defendants
            if (complaints.Count > 1)
            {
                
                for (int line = 1; line < complaints.Count; line++)
                {
                    // PLAINTIFF

                    // DEFENDANT
                    CivilFilingServiceReference.party def = new CivilFilingServiceReference.party();
                    def.partyDescription = complaints[line].PartyDescription_Defendant;  // REQ
                    def.partyAffiliation = complaints[line].PartyAffiliation_Defendant;  // NOT REQ
                    def.corporationType = complaints[line].CorporationType_Defendant;    // REQ if defendant.partyDescription = "BUS";
                    def.corporationName = complaints[line].Name_Defendant;               // REQ if defendant.partyDescription = "BUS";
                    def.phoneNumber = complaints[line].Phone_Defendant;                  // Max 10 digits

                    // DEFENDANT.name REQ if defendant.partyDescription = "IND" 
                    CivilFilingServiceReference.name defName = new CivilFilingServiceReference.name();
                    defName.firstName = complaints[line].FirstName_Defendant;
                    defName.middleName = complaints[line].MiddleInitial_Defendant;
                    defName.lastName = complaints[line].LastName_Defendant;
                    def.name = defName;

                    // DEFENDANT address is not REQ
                    CivilFilingServiceReference.address defAddress = new CivilFilingServiceReference.address();
                    defAddress.addressLine1 = complaints[line].AddressLine1_Defendant;
                    defAddress.addressLine2 = complaints[line].AddressLine2_Defendant;
                    defAddress.city = complaints[line].City_Defendant;
                    defAddress.stateCode = complaints[line].State_Defendant;
                    defAddress.zipCode = complaints[line].ZipCode_Defendant;
                    defAddress.zipCodeExt = complaints[line].ZipCodeExt_Defendant;
                    def.address = defAddress;

                    // DEFENDANT ALIAS LIST - NOT REQUIRED
                    var defAliasList = new List<CivilFilingServiceReference.partyAlias>();
                    var defAlias = new CivilFilingServiceReference.partyAlias();
                    defAlias.alternateTypeCode = complaints[line].AlternateType_Defendant;
                    defAlias.alternateName = complaints[line].AlternateName_Defendant;
                    defAliasList.Add(defAlias);
                    def.partyAliasList = defAliasList.ToArray(); // Assign Alias back to this def (defendant)

                    defendants[line] = def; // Default to 1 for the second defendant in this SelfGen_Test.csv
                }
            }

            // Add everything we just created to bfp (bulkFilingPacket)
            // ADD caseData
            bfp.civilCase = caseData;

            // ADD PLAINTIFF
            bfp.plaintiffList = plantiffs;

            // ADD DEFENDANT
            bfp.defendantList = defendants;

            // ADD ATTACHMENTS - We are just supporting one attachment for this version
            // bulkFilingPacket takes an array of attachment[] we just have one attachment as noted above
            CivilFilingServiceReference.attachment[] attachments = new CivilFilingServiceReference.attachment[1];
            attachments[0] = att;
            bfp.attachmentList = attachments;

            // ADD FEE
            bfp.fee = fee;

            // MISC - Discovered when testing
            bfp.documentRedactionInd = "Y"; //Code: ECCV100 Description: Document Redaction Indicator should be Y


            if (bfp == null)
            {
                Responses.Add("Invalid formatted file:" + Path.GetFileName(this.filePath));
                _logger.Info("Invalid formatted file:" + Path.GetFileName(this.filePath));
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
                // Changed the 1st value of SecurityHeader from feinsuch to CivilFilingClient 8/26/2020
                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("CivilFilingClient", Username, Password));
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
                    // TODO: Color these red
                    Responses.Add("eCourts " + Endpoint + " error :" + message);
                    _logger.Error("eCourts " + Endpoint + " error :" + message);

                    if (ex.InnerException != null)
                    {
                        Responses.Add("eCourts " + Endpoint + " error :" + ex.InnerException);
                        _logger.Error("eCourts " + Endpoint + " error :" + ex.InnerException);
                    }
                    Util.SaveResponseToFile(Responses, "Failed", this.filePath, pdfFilepath);
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
            Util.SaveResponseToFile(Responses, strDocketNumber, this.filePath, pdfFilepath);

            return IsSuccess;
        }

        
        public bool VenueCodeValidate(string value)
        {
            var county = "ATL,BER,BUR,CAM,CPM,CUM,ESX,GLO,HUD,HNT,MER,MID,MON,MRS,OCN,PAS,SLM,SOM,SSX,UNN,WRN".Split(',');
            var countylist = county.Cast<string>().ToList();
            Responses.Add("VenueCodeValidate Codes: " + String.Join(", ", countylist));
            _logger.Info("VenueCodeValidate Codes: " + String.Join(", ", countylist));
            return countylist.Contains(value);
        }
        public bool CaseActionValidate(string value)
        {
            var actions = "28,175,32,37,41,33".Split(',');
            var actionList = actions.Cast<string>().ToList();
            Responses.Add("CaseActionValidate Codes: " + String.Join(", ", actionList));
            _logger.Info("CaseActionValidate Codes: " + String.Join(", ", actionList));
            return actionList.Contains(value);
        }
        public bool CorporationTypeValidate(string value)
        {
            var corpTypes = "CO,LC,LP,OT,SP".Split(',');          
            var corpTypeList = corpTypes.Cast<string>().ToList();
            Responses.Add("CorporationTypeValidate Codes: " + String.Join(", ", corpTypeList));
            _logger.Info("CorporationTypeValidate Codes: " + String.Join(", ", corpTypeList));
            return corpTypeList.Contains(value);
        }
        public bool StateValidate(string value)
        {
            var states = "AL,AK,AS,AZ,AR,CA,CN,CO,CT,DE,DC,FM,FL,FR,GA,GU,HI,ID,IL,IN,IA,KS,KY,LA,ME,MH,MD,MA,MI,MN,MS,MO,MT,NE,NV,NH,NJ,NM,NY,NC,ND,MP,OH,OK,OR,PW,PA,PR,RI,SC,SD,TN,TX,UT,VT,VI,VA,WA,WV,WI,WY".Split(',');
            var stateList = states.Cast<string>().ToList();
            Responses.Add("StateValidate Codes: " + String.Join(", ", stateList));
            _logger.Info("StateValidate Codes: " + String.Join(", ", stateList));
            return stateList.Contains(value);
        }
        public bool AltTypeValidate(string value)
        {
            var altTypes = "AK,DB,FK,NK,OB,SB,SU,TA".Split(',');
            var altTypeList = altTypes.Cast<string>().ToList();
            Responses.Add("AltTypeValidate Codes: " + String.Join(", ", altTypeList));
            _logger.Info("AltTypeValidate Codes: " + String.Join(", ", altTypeList));
            return altTypeList.Contains(value);
        }
        public bool AdaValidate(string value)
        {
            var adaCodes = "A,L,B,X,C,D,M,W,N,O,F,P,R,T,S,I".Split(',');
            var adaCodeList = adaCodes.Cast<string>().ToList();
            Responses.Add("AdaValidate Codes: " + String.Join(", ", adaCodeList));
            _logger.Info("AdaValidate Codes: " + String.Join(", ", adaCodeList));
            return adaCodeList.Contains(value);
        }
        public bool DocTypeValidate(string value)
        {
            var docTypes = "AF,BR,CE,EX,MC".Split(',');
            var docTypeList = docTypes.Cast<string>().ToList();
            Responses.Add("DocTypeValidate Codes: " + String.Join(", ", docTypeList));
            _logger.Info("DocTypeValidate Codes: " + String.Join(", ", docTypeList));
            return docTypeList.Contains(value);
        }
        public bool FeeExemptionValidate(string value)
        {
            var feeCodes = "CO,LS,SA,PD".Split(',');
            var feeCodeList = feeCodes.Cast<string>().ToList();
            Responses.Add("FeeExemptionValidate Codes: " + String.Join(", ", feeCodeList));
            _logger.Info("FeeExemptionValidate Codes: " + String.Join(", ", feeCodeList));
            return feeCodeList.Contains(value);
        }
        public bool PartyAffilValidate(string value)
        {
            var codes = "ADM,CTY,EST,EXE,HEI,JCR,USA".Split(',');
            var codeList = codes.Cast<string>().ToList();
            Responses.Add("PartyAffilValidate Codes: " + String.Join(", ", codeList));
            _logger.Info("PartyAffilValidate Codes: " + String.Join(", ", codeList));
            return codeList.Contains(value);
        }
    }
}