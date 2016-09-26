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
using System.Xml.Serialization;

namespace CivilFilingClient
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initiatlize the NLog logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// filePaths will contain the full filePath and fileName for the files
        /// we need to load.  This will be one xml file and one pdf file per
        /// submission.
        /// </summary>
        List<string> filePaths = new List<string>();

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
                        richTextBox1.AppendText(Environment.NewLine + Path.GetDirectoryName(file));
                        logger.Info(file);
                        filePaths.Add(file);
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
            richTextBox1.AppendText("Send btn clicked");

            CivilFilingServiceReference.@case caseData = new CivilFilingServiceReference.@case();
            caseData.caseAction="028";
            caseData.courtSection = "SCP";
            caseData.defendantCaption = "test Def caption";
            caseData.demandAmount = new decimal(1000);
            caseData.plaintiffCaption ="test plantiff caption";
            caseData.juryDemand = "N";
            caseData.lawFirmCaseId = "100";
            caseData.otherCourtActions = "N";
            caseData.serviceMethod = "03";
            caseData.venue = "ATL";
            caseData.venueOfIncident = "ATL";

            CivilFilingServiceReference.attachment att = new CivilFilingServiceReference.attachment();
            string filePath = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\CivilFilingClient\some.pdf";
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
            fee.accountNumber = "12345";
            fee.attorneyClientRefNumber = "12345";
            fee.attorneyFee = new decimal(1000);
            fee.feeExempt = "N";
            fee.paymentType = "CG";

            CivilFilingServiceReference.address pAddress = new CivilFilingServiceReference.address();
            pAddress.addressLine1 = "25, market street";
            pAddress.addressLine2 = "Trenton";
            pAddress.stateCode = "NJ";
            pAddress.zipCode = "12345";

            CivilFilingServiceReference.party plantiff = new CivilFilingServiceReference.party();
            plantiff.adaAccommodationInd = "N";
            plantiff.address = pAddress;
            plantiff.corporationName = "ABC Corp";
            plantiff.corporationType = "CO";
            plantiff.interpreterInd = "N";
            plantiff.partyAffiliation = "ADM";
            plantiff.partyDescription = "BUS";
            plantiff.interpreterInd = "N";
            plantiff.language = "SPA";
            plantiff.accommodationType = "a";

            CivilFilingServiceReference.party defendant = new CivilFilingServiceReference.party();
            defendant.adaAccommodationInd = "N";
            defendant.address = pAddress;
            defendant.corporationName = "XYZ Corp";
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
            packet.attorneyId = "288551973";
            packet.attorneyFirmId = "9735384700";

            packet.civilCase = caseData;
            int numberOfDefendants = 1;
            CivilFilingServiceReference.party[] defendants = new CivilFilingServiceReference.party[numberOfDefendants];
            defendants[0] = defendant;
            packet.defendantList = defendants;
            packet.documentRedactionInd = "Y";
            packet.fee = fee;

            // Lots of object have attributes...
            CivilFilingServiceReference.attribute attr = new CivilFilingServiceReference.attribute();
            attr.name = "name";
            attr.value = "TestName";

            int numberOfPlantiffs = 1;
            CivilFilingServiceReference.party[] plantiffs = new CivilFilingServiceReference.party[numberOfPlantiffs];
            plantiffs[0] = plantiff;
            packet.plaintiffList = plantiffs;
            
            CivilFilingServiceReference.civilFilingRequest filingRequest = 
                new CivilFilingServiceReference.civilFilingRequest();

            filingRequest.bulkFilingPacket= packet;
            
            // Create the proxy
            // Set the correct endpoint... this is working differnt than expected... old soap stuff...
            // Add the security in the header
            // Send the Request
            // Wait for the reponse
            // parse out the responses
            CivilFilingServiceReference.CivilFilingWSClient proxy = 
                new CivilFilingServiceReference.CivilFilingWSClient();
            
            richTextBox1.AppendText(Environment.NewLine + proxy.Endpoint.Address.ToString());
            
            try
            {
                string message = Environment.NewLine + "Attempting to send the web request";
                richTextBox1.AppendText(message);
                responses.Add(message);
                CivilFilingServiceReference.civilFilingResponse filingReponse = 
                    proxy.submitCivilFiling(filingRequest);

                foreach (var msg in filingReponse.messages)
                {
                    string filingMsg = Environment.NewLine + "Code: " + msg.code + " Description: " + msg.description;
                    responses.Add(filingMsg);
                    richTextBox1.AppendText(filingMsg);
                }
                if (filingReponse.efilingNumber != null)
                {
                    //TODO: Color these blue
                    string eFilingNumberMsg = "eFiling seq number:" +
                        filingReponse.efilingNumber.efilingCourtDiv.ToString() +
                        filingReponse.efilingNumber.efilingCourtYr.ToString() +
                        filingReponse.efilingNumber.efilingSeqNo.ToString();

                    richTextBox1.AppendText(Environment.NewLine + eFilingNumberMsg);
                    responses.Add(eFilingNumberMsg);
                }
                if (filingReponse.docketNumber != null)
                {
                    string dockerNumberMsg = "Docker number:" +
                        filingReponse.docketNumber.docketVenue
                        + "-" + filingReponse.docketNumber.docketTypeCode
                        + "-" + filingReponse.docketNumber.docketCourtYear
                        + "-" + filingReponse.docketNumber.docketSeqNum;

                    //TODO: Color these blue
                    richTextBox1.AppendText(Environment.NewLine + "Docket number:");
                    responses.Add(dockerNumberMsg);
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
            richTextBox1.AppendText(Environment.NewLine + "Done.  Have a good day.");
            
            // Disable the btn until the request has finished or error's out
            btnSend.Enabled = true;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private CivilFilingServiceReference.civilFilingRequest readXMLFile()
        {
            CivilFilingServiceReference.civilFilingRequest filingRequest = 
                new CivilFilingServiceReference.civilFilingRequest();

            string filePathXml = @"C:\Users\Craig Nicholson\Documents\Visual Studio 2015\Projects\CivilFilingClient\Message1.xml";
            // define the object we are going to populate
            CivilFilingServiceReference.ECourtsCivilServiceException caseData;
            try
            {
                var serializer = new XmlSerializer(typeof(CivilFilingServiceReference.ECourtsCivilServiceException));
                using(FileStream fileStrem = new FileStream(filePathXml, FileMode.Open))
                {
                    caseData = (CivilFilingServiceReference.ECourtsCivilServiceException)serializer.Deserialize(fileStrem);
                }
            }
            catch(System.Exception ex)
            {
                richTextBox1.AppendText(Environment.NewLine + ex.Message);
                logger.Error(ex.Message);
            }
            return filingRequest;
        }

        private void saveResponseToFile(string filePathOfOrigin, string[] responses)
        {
            // TODO: Timestamp the file name
            // TODO: Put name in config
            using (StreamWriter outputFile = new StreamWriter(filePathOfOrigin + @"\Responses.txt"))
            {
                foreach (string response in responses)
                    outputFile.WriteLine(response);
            }       
        }
    }
}