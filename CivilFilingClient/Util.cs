using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace CivilFilingClient
{
    static class Util
    {
        /// <summary>
        /// Initialize NLog logger
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// ReadXmlfile parses one xml soap file at a time and creates the bulk filing packet.
        /// Each xml file's soap message should contain a pdf.
        /// If an attachment is avaiable the attachment will be created.
        /// responses will be passed by reference on the heap.
        /// </summary>
        /// <param name="xmlfilepath">The full file path for the file</param>
        /// <param name="responses">List of responses logged and passed in by ref</param>
        /// <param name="files">List of the file user attached so we can look up full path to the pdf</param>
        /// <param name="pdffilepath">out parameter used to pass back the pdfFilepath to the caller of this method</param>
        /// <returns></returns>
        public static CivilFilingServiceReference.bulkFilingPacket ReadXmlfile(string xmlfilepath, List<string> responses, List<CourtCaseFiles> files, out string pdffilepath)
        {
            CivilFilingServiceReference.bulkFilingPacket bfp = null;
            pdffilepath = string.Empty;
            responses.Add("Reading file: " + Path.GetFileName(xmlfilepath));

            var xmldoc = new XmlDocument();
            xmldoc.Load(xmlfilepath);
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
                var message = "File soap format is incorrect.  Review the namespaces to make sure they contain: " + Environment.NewLine + soapmsg;
                responses.Add("ERROR xml nodes should contain: " + message);
                _logger.Error("ERROR xml nodes should contain" + message);
                return null;
            }
            try
            {
                bfp = (CivilFilingServiceReference.bulkFilingPacket)ser.Deserialize(new StringReader(node.OuterXml));
                string fileName = bfp.attachmentList[0].documentName + bfp.attachmentList[0].extention;
                responses.Add("Creating Attachment file: " + fileName);

                // PDF file might not be in same directory so we should check to 
                // to see is we have the PDF in our list of files
                CourtCaseFiles pdfFile = files.Find(item => item.FileName.ToUpper() == fileName.ToUpper());
                if (pdfFile != null)
                {
                    pdffilepath = pdfFile.FullFilePath;
                }
                else //If users does not attach any file assume the pdf is in same directory as XML file.
                {
                    //TODO: Will a pdf always be a requirement?
                    pdffilepath = Path.GetDirectoryName(xmlfilepath) + @"\" + fileName;
                }
                byte[] bytes = File.ReadAllBytes(pdffilepath);
                bfp.attachmentList[0].bytes = bytes;
            }
            catch (System.Exception ex)
            {
                var message = "Error reading file : " + ex.Message;
                if (ex.InnerException != null)
                    message = Environment.NewLine + ex.InnerException.Message;

                responses.Add("ERROR Attaching file: " + message);
                _logger.Error(message);
                // Stop all processing
                return null;
            }
            return bfp;
        }

        /// <summary>
        /// This is best used by the CLI Interface... but unused right now... 
        /// </summary>
        /// <param name="xmlfilepath"></param>
        /// <param name="responses"></param>
        /// <param name="pdffilepath"></param>
        /// <returns></returns>
        public static CivilFilingServiceReference.bulkFilingPacket ReadXmlfile(string xmlfilepath, List<string> responses, string pdffilepath)
        {
            if (File.Exists(xmlfilepath) && File.Exists(pdffilepath))
            {
                CivilFilingServiceReference.bulkFilingPacket bfp = null;
                pdffilepath = string.Empty;
                responses.Add("Reading file: " + Path.GetFileName(xmlfilepath));
                responses.Add("Attaching file: " + Path.GetFileName(pdffilepath));

                var xmldoc = new XmlDocument();
                xmldoc.Load(xmlfilepath);
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
                    var message = "File soap format is incorrect.  Review the namespaces to make sure they contain: " + Environment.NewLine + soapmsg;
                    responses.Add("ERROR xml nodes should contain: " + message);
                    _logger.Error("ERROR xml nodes should contain" + message);
                    return null;
                }
                try
                {
                    bfp = (CivilFilingServiceReference.bulkFilingPacket)ser.Deserialize(new StringReader(node.OuterXml));
                    string fileName = bfp.attachmentList[0].documentName + bfp.attachmentList[0].extention;
                    responses.Add("Creating Attachment file: " + fileName);
                    byte[] bytes = File.ReadAllBytes(pdffilepath);
                    bfp.attachmentList[0].bytes = bytes;
                }
                catch (System.Exception ex)
                {
                    var message = "Error reading file : " + ex.Message;
                    if (ex.InnerException != null)
                        message = " | " + ex.InnerException.Message;

                    responses.Add("Error reading file: " + message);
                    _logger.Error(message);
                    // Stop all processing
                    return null;
                }
                return bfp;
            } // file path was wrong
            else
            {
                _logger.Warn("Check the file path for : " + xmlfilepath);
                _logger.Warn("Check the file path for : " + pdffilepath);
                return null;
            }
        }

        /// <summary>
        /// SaveResponseToFile writes out the log history to the same directory the xml
        /// file is located, moves all success parsed files to a main directory YYYYMMDD
        /// and then clears all the responses from the list.
        /// The log history file name will be the DocketNumber_reponses.txt or Failed_responses.txt
        /// located in the original directory with the files.
        /// </summary>
        /// <param name="filePathOfOrigin"> The root folder where the file originated</param>
        /// <param name="responses">List of responses from the processing and submission to eCourts</param>
        /// <param name="docketNumber">On successful submission we will have a docket number on failure, Failed will be in this string</param>
        /// <param name="files">List of the CourtCaseFiles we processed</param>
        public static void SaveResponseToFile(List<string> responses, string docketNumber, string xmlFilePath, string pdfFilePath)
        {
            // Root Folder with xml file, is where we add the folder to archive the files for successful transactions
            string root_dir = Path.GetDirectoryName(xmlFilePath);
            // logFilename will have the \ slash on the file so we can skip leaving it on the root_dir
            string logFilename = string.Empty;
            string newDirectory = string.Empty;

            // If we had success create the Archive Folder and assign the logFileName as the docketNumber
            // example : ATL-DC-005347-16_201610061244395579
            if (docketNumber != "Failed")
            {
                newDirectory = DateTime.Now.ToString("yyyyMMdd");
                Directory.CreateDirectory(root_dir + @"\" + newDirectory);
                logFilename = @"\" + docketNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";
            }
            else //File did not load 
            {
                // Create Error Log File : Failed_TestCorp2CorpBAD.XML_201610111313522504
                logFilename = @"\" + Path.GetFileName(xmlFilePath) + "_" + docketNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";
            }

            try
            {
                // All log files are in the root of the origin file dir
                using (StreamWriter outputFile = new StreamWriter(root_dir + logFilename))
                {
                    foreach (string response in responses)
                        outputFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + " | " + response);
                }
                // When the process is successful
                // Move the files we processed into the Archive folder (yyyyMMdd)
                // Otherwise leave the files in the current folder.
                if (docketNumber != "Failed")
                {
                    File.Move(xmlFilePath, root_dir + @"\" + newDirectory + @"\" + Path.GetFileName(xmlFilePath));
                    File.Move(pdfFilePath, root_dir + @"\" + newDirectory + @"\" + Path.GetFileName(pdfFilePath));
                }
            }
            catch (IOException ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message);
                throw;
            }
        }
    }
}