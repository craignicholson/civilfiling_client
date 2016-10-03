using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

/// <summary>
/// Authors: Brian Ketelsen, Craig Nicholson
/// CivilFilingClient sends data to the New Jeresy Courts - eCourt Filing System
/// All rights reserved.
/// </summary>
namespace CivilFilingClient
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// CourtCaseFiles will maintain a list of all files selected during submission process.
        /// We will process each of the XML files and search for the corresponding PDF files inside
        /// of the soapenvelope.  We will use the PDF in the soap envelope to find the file path
        /// for the PDF.  If the PDF is not found we will assume the XML directory will also have
        /// the PDF file located in the same directory as the PDF file.
        /// </summary>
        private class CourtCaseFiles
        {
            public string FileName { get; set; }
            public string FullFilePath { get; set; }
            public string FileExtension { get; set; }
            public string DirectoryName { get; set; }
            public bool IsSubmitted { get; set; }
            public CourtCaseFiles(
                string fileName,
                string fullFilePath,
                string fileExtension,
                string directoryName,
                bool isSubmitted)
            {
                FileName = fileName;
                FullFilePath = fullFilePath;
                FileExtension = fileExtension;
                DirectoryName = directoryName;
                IsSubmitted = isSubmitted;
            }
        }

        /// <summary>
        /// Initialize NLog logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// files will contain the full filePath and fileName for the files
        /// we need to load.  This will be one xml file and one pdf file per
        /// submission.
        /// </summary>
        List<CourtCaseFiles> files = new List<CourtCaseFiles>();

        /// <summary>
        /// Legacy clean this up later...
        /// </summary>
        string rootDirectory = string.Empty;

        /// <summary>
        /// responses will contain all of the messages returned from the web service
        /// and any errors.  These messages will be written to a file for the users.
        /// </summary>
        List<string> responses = new List<string>();

        public Form1()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
        }

        /// <summary>
        /// btnAttach_Click will allow the user to select multiple files which will
        /// be processed.  We exepect one xml and one pdf file.  If we have more files
        /// we should error out and write the output to the directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAttach_Click(object sender, EventArgs e)
        {
            // We will only attach the pdf... but we can have many pdf's
            // Use Case - we will file one case at a time.
            // xml file can have many plantiff and defendants
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                // Read all the files
                foreach (String file in openFileDialog1.FileNames)
                {
                    // Load the files into the List for processing.
                    try
                    {
                        richTextBox1.AppendText(Environment.NewLine + Path.GetFullPath(file));
                        richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(file));
                        rootDirectory = Path.GetDirectoryName(file);
                        files.Add(new CourtCaseFiles(Path.GetFileName(file),
                                                       file, 
                                                       Path.GetExtension(file).ToString(),
                                                       Path.GetDirectoryName(file),
                                                       false));
                        responses.Add("Attachment: " + file);
                        logger.Info("Attachment: " + file);
                    }
                    catch (SecurityException ex)
                    {
                        // The user lacks appropriate permissions to read files, discover paths, etc.
                        MessageBox.Show("Security error. Please contact your administrator for details.\n\n" +
                            "Error message: " + ex.Message + "\n\n" +
                            "Details (send to Support):\n\n" + ex.StackTrace
                        );
                    }
                    catch (System.Exception ex)
                    {
                        // Could not load the image - probably related to Windows file system permissions.
                        MessageBox.Show("Cannot display the image: " + file.Substring(file.LastIndexOf('\\'))
                            + ". You may not have permission to read the file, or " +
                            "it may be corrupt.\n\nReported error: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// InitializeOpenFileDialog so by default we only see pdf and xml files.
        /// </summary>
        private void InitializeOpenFileDialog()
        {
            // Set the file dialog to filter for graphics files.
            openFileDialog1.Filter =
                "Files (*.PDF;*.XML)|*.PDF;*.XML|" +
                "All files (*.*)|*.*";

            // Allow the user to select multiple images.
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "Civil Filing Client (Accepts xml and pdfs files)";
        }


        /// <summary>
        /// btnSend_Click processes the data in the xml file and attaches the pdf if we 
        /// have one to the web service request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSend_Click(object sender, EventArgs e)
        {
            // Disable the btn until the request has finished or error's out
            btnSend.Enabled = false;
            richTextBox1.AppendText(Environment.NewLine + "Attemping to send  request");
            responses.Add("Attemping to send  request");
            logger.Info("Attemping to send  request");

            // Create the proxy > Send Request > Wait for Response > Parse Response
            CivilFilingServiceReference.CivilFilingWSClient proxy =
                new CivilFilingServiceReference.CivilFilingWSClient();

            try
            {
                foreach (var item in files)
                {
                    if (item.FileExtension.ToUpper() == ".XML")
                    {
                        string fileMsg = "Reading File:" + item.FullFilePath;
                        richTextBox1.AppendText(Environment.NewLine + fileMsg);
                        responses.Add(fileMsg);
                        logger.Info(fileMsg);

                        //Parse XML file
                        var bfp = readXmlFileBFP(item.FullFilePath);
                        // If the deserialzation of the file fails we will get a null back
                        // and need to stop the processing
                        if (bfp == null)
                        {
                            richTextBox1.AppendText(Environment.NewLine + "Invalid formated file :" + item.FileName);
                            responses.Add("Invalid formated file:" + item.FileName);
                            logger.Info("Invalid formated file:" + item.FileName);
                            btnSend.Enabled = true;
                            return;
                        }
                        // Create the request and then assign the request a bulkFilingPacket
                        CivilFilingServiceReference.civilFilingRequest filingRequest =
                            new CivilFilingServiceReference.civilFilingRequest();
                        filingRequest.bulkFilingPacket = bfp;

                        //var filingRequest = TestCorp2Corp();  //Test Stub
                        //var filingRequest = TestCorp2Individual();  //Test Stub
                        //var filingRequest = TestIndividual2Corp();  //Test Stub
                        //var filingRequest = TestIndividual2Individual();  //Test Stub

                        string message = "Attempting to send the web request to:"
                            + proxy.Endpoint.Address.ToString();
                        richTextBox1.AppendText(Environment.NewLine + message);
                        responses.Add(message);
                        logger.Info(message);

                        CivilFilingServiceReference.civilFilingResponse filingReponse =
                            proxy.submitCivilFiling(filingRequest);

                        if (filingReponse.messages != null)
                        {
                            foreach (var msg in filingReponse.messages)
                            {
                                string filingMsg = "Code: " + msg.code + " Description: " + msg.description;
                                richTextBox1.AppendText(Environment.NewLine + filingMsg);
                                responses.Add(filingMsg);
                                logger.Warn(filingMsg);
                            }
                        }
                        if (filingReponse.efilingNumber != null)
                        {
                            //TODO: Color these blue
                            string eFilingNumberMsg = "Efiling Sequence Number :" +
                                filingReponse.efilingNumber.efilingCourtDiv.ToString() +
                                filingReponse.efilingNumber.efilingCourtYr.ToString() +
                                filingReponse.efilingNumber.efilingSeqNo.ToString();

                            richTextBox1.AppendText(Environment.NewLine + eFilingNumberMsg);
                            responses.Add(eFilingNumberMsg);
                            logger.Info(eFilingNumberMsg);
                        }
                        if (filingReponse.docketNumber != null)
                        {
                            string dockerNumberMsg = "Docker Number :" +
                                filingReponse.docketNumber.docketVenue
                                + "-" + filingReponse.docketNumber.docketTypeCode
                                + "-" + filingReponse.docketNumber.docketCourtYear
                                + "-" + filingReponse.docketNumber.docketSeqNum;

                            //TODO: Color these blue
                            richTextBox1.AppendText(Environment.NewLine + "Docket number: " + dockerNumberMsg);
                            responses.Add(dockerNumberMsg);
                            logger.Info(dockerNumberMsg);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                // TODO: Colors these red
                richTextBox1.AppendText(Environment.NewLine + ex.Message);
                responses.Add(ex.Message);
                logger.Error(ex.Message);

                if (ex.InnerException != null)
                {
                    richTextBox1.AppendText(Environment.NewLine + ex.InnerException.Message);
                    responses.Add(ex.InnerException.Message);
                    logger.Error(ex.InnerException.Message);

                }
            }
            // TODO: If we have any error codes and the transaction fails... we
            // Should let the user have different message so they can correct the
            // issue and try again... using Done.  Have a good day... is best when
            // the entire process is a success.
            richTextBox1.AppendText(Environment.NewLine + "Sumbmission complete.  If you received any errors please review and try the submission again.");
            logger.Info("End of Submission");

            // Write out the responses
            // TODO: rootDirectory is global so we need to tidy this up.
            saveResponseToFile(rootDirectory, responses);
            // Disable the btn until the request has finished or error's out
            btnSend.Enabled = true;
        }

        /// <summary>
        /// readXmlFileBFP parses the xml soap file and creates the bulk filing packet.
        /// If an attachment is avaiable the attachment will be created.
        /// </summary>
        /// <param name="filePathXml"></param>
        /// <returns></returns>
        private CivilFilingServiceReference.bulkFilingPacket readXmlFileBFP(string filePathXml)
        {
            CivilFilingServiceReference.bulkFilingPacket bfp = null;
            responses.Add("Reading file: " + Path.GetFileName(filePathXml));

            var xmldoc = new XmlDocument();
            xmldoc.Load(filePathXml);
            var names = new XmlNamespaceManager(xmldoc.NameTable);
            names.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            names.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            names.AddNamespace("tns2", "http://webservice.civilfiling.ecourts.ito.aoc.nj/");

            var node = xmldoc.SelectSingleNode("/soapenv:Envelope/soapenv:Body/tns2:submitCivilFiling/arg0/bulkFilingPacket", names);
            XmlSerializer ser = new XmlSerializer(typeof(CivilFilingServiceReference.bulkFilingPacket));

            // If the xml doc is not formatted with the correct namespaces the node will 
            // be returned null
            if (node == null)
            {
                var soapmsg = "/soapenv:Envelope/soapenv:Body/tns2:submitCivilFiling/arg0/bulkFilingPacket";
                var message = "File soap format is incorrect.  Review the namespaces to make sure they contain: " + Environment.NewLine +
                    soapmsg;
                richTextBox1.AppendText(Environment.NewLine + message);
                responses.Add("ERROR xml nodes should contain: " + message);
                logger.Error("ERROR xml nodes should contain" + message);
                return null;
            }
            try
            {
                bfp = (CivilFilingServiceReference.bulkFilingPacket)ser.Deserialize(new StringReader(node.OuterXml));
                string filePath = null;
                string fileName = bfp.attachmentList[0].documentName + bfp.attachmentList[0].extention;
                responses.Add("Creating Attachment file: " + fileName);
                
                // PDF file might not be in same directory so we should check to 
                // to see is we have the PDF in our list of files
                CourtCaseFiles pdfFile = files.Find(item => item.FileName.ToUpper() == fileName.ToUpper());
                if (pdfFile != null)
                {
                    filePath = pdfFile.FullFilePath;
                }
                else //If users does not attach any file assume the pdf is in same directory as XML file.
                {
                    filePath = Path.GetDirectoryName(filePathXml) + @"\" + fileName;
                }

                byte[] bytes = File.ReadAllBytes(filePath);
                bfp.attachmentList[0].bytes = bytes;

                // Add the Branch Id
                // Branch Id - need the 
                //CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute();
                //attr.name = "branchId";
                //attr.value = "0001";
                //CivilFilingServiceReference.attribute[] attrs = new CivilFilingServiceReference.attribute[1];
                //attrs[0] = attr;
                //bfp.attributes = attrs;
            }
            catch(System.Exception ex)
            {
                var message = "Error reading file : " + ex.Message;
                if (ex.InnerException != null)
                    message = Environment.NewLine + ex.InnerException.Message;
                richTextBox1.AppendText(Environment.NewLine + message);
                responses.Add("ERROR Attaching file: " + message);
                logger.Error(message);
                // Stop all processing
                return null;
            }
            return bfp;
        }

        /// <summary>
        /// saveResponseToFile writes out the log history to the same directory the xml
        /// file is located.
        /// </summary>
        /// <param name="filePathOfOrigin"></param>
        /// <param name="responses"></param>
        private void saveResponseToFile(string filePathOfOrigin, List<string> responses)
        {
            // TODO: Put name in config
            string filename = @"\responses_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";
            try
            {
                using (StreamWriter outputFile = new StreamWriter(filePathOfOrigin + filename))
                {
                    foreach (string response in responses)
                        outputFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + " | " + response);
                }
            }
            catch (IOException ex)
            {
                richTextBox1.AppendText(Environment.NewLine + ex.Message);
                logger.Error(ex.Message);
            }
            catch(System.Exception ex)
            {
                richTextBox1.AppendText(Environment.NewLine + ex.Message);
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// TestCorp2Corp - Generates a Sample Message for posting to the NJ Courts
        /// </summary>
        /// <returns></returns>
        private CivilFilingServiceReference.civilFilingRequest TestCorp2Corp()
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
        private CivilFilingServiceReference.civilFilingRequest TestCorp2Individual()
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
        private CivilFilingServiceReference.civilFilingRequest TestIndividual2Corp()
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
        private CivilFilingServiceReference.civilFilingRequest TestIndividual2Individual()
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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}