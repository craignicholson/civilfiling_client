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
        /// Initiatlize NLog logger
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
            // Use Case, One xml file and multiple pdfs? I think this is how it should be.
            // xml file can have many plantiff and defendants?
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                // Read the files
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
            richTextBox1.AppendText(Environment.NewLine + "Send button clicked");
            responses.Add("Send button clicked");
            logger.Info("Send button clicked");

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

                        // Create the request and then assign the request a bulkFilingPacket
                        //CivilFilingServiceReference.civilFilingRequest filingRequest =
                        //    new CivilFilingServiceReference.civilFilingRequest();
                        //filingRequest.bulkFilingPacket = bfp;

                        var filingRequest = SampleMessage();  //Test Stub

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
                            richTextBox1.AppendText(Environment.NewLine + "Docket number:");
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
            responses.Add("Reading file: " + Path.GetFileName(filePathXml));

            var xmldoc = new XmlDocument();
            xmldoc.Load(filePathXml);
            var names = new XmlNamespaceManager(xmldoc.NameTable);
            names.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");
            names.AddNamespace("soapenv", "http://schemas.xmlsoap.org/soap/envelope/");
            names.AddNamespace("tns2", "http://webservice.civilfiling.ecourts.ito.aoc.nj/");

            var node = xmldoc.SelectSingleNode("/soapenv:Envelope/soapenv:Body/tns2:submitCivilFiling/arg0/bulkFilingPacket", names);
            XmlSerializer ser = new XmlSerializer(typeof(CivilFilingServiceReference.bulkFilingPacket));
            var bfp = (CivilFilingServiceReference.bulkFilingPacket)ser.Deserialize(new StringReader(node.OuterXml));

            try
            {
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
            }
            catch(System.Exception ex)
            {
                richTextBox1.AppendText(Environment.NewLine + ex.Message);
                responses.Add("ERROR Attaching file: " + ex.Message);
                logger.Error(ex.Message);
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
                        outputFile.WriteLine(response);
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
        /// SampleMessage - Generates a Sample Message for posting to the NJ Courts
        /// </summary>
        /// <returns></returns>
        private CivilFilingServiceReference.civilFilingRequest SampleMessage()
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
            string filePath = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\CivilFilingClient\TestFiles\some.pdf";
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
            plantiff.corporationName = "Test Creditor";
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
            defendant.corporationName = "Trump Corp";
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
            packet.attorneyId = "888888005";
            packet.attorneyFirmId = "F88888003";

            packet.civilCase = caseData;
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            packet.defendantList = defendants;
            packet.documentRedactionInd = "Y";
            packet.fee = fee;

            // Branch Id - need the 
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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}