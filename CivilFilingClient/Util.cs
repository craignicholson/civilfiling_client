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
            pdffilepath = string.Empty;
            if (File.Exists(xmlfilepath))
            {
                CivilFilingServiceReference.bulkFilingPacket bfp = null;
                responses.Add("ReadXmlfile.Deserialize file: " + Path.GetFileName(xmlfilepath));
                _logger.Info("ReadXmlfile.Deserialize file: " + Path.GetFileName(xmlfilepath));

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
                    responses.Add("Error xml nodes should contain: " + message);
                    _logger.Error("Error xml nodes should contain" + message);
                    return null;
                }
                try
                {
                    bfp = (CivilFilingServiceReference.bulkFilingPacket)ser.Deserialize(new StringReader(node.OuterXml));
                    string fileName = bfp.attachmentList[0].documentName + bfp.attachmentList[0].extention;
                    responses.Add("Creating Attachment file: " + fileName);
                    // Users of the eCourts system do not see a file extension on the document when they download the pdf.
                    // They have to right click and save as pdf, instead of just clicking to view.  Setting the document
                    // to the full name to see if this fixes the issue.  
                    // (See example file with this project to copy for your code)
                    // Example from the Documentation from eCourts is provided.
                    // Document Name does not have ethe extention.
                    // < attachmentList >
                    //      < contentType > application / pdf </ contentType >
                    //      < docType > pdf </ docType >
                    //      < documentCode > CMPL </ documentCode >
                    //      < documentDescription > Complaint </ documentDescription >
                    //      < documentName > Test </ documentName >
                    //      < extention >.pdf </ extention >
                    //</ attachmentList >

                    bfp.attachmentList[0].documentName = fileName;

                    // DEPRECATED
                    // PDF file might not be in same directory so we should check to 
                    // to see is we have the PDF in our list of files
                    // TODO: THIS CODE SEEMS BUSTED SINCE FILES IS ALWAYS EMPTY NOW
                    CourtCaseFiles pdfFile = files.Find(item => item.FileName.ToUpper() == fileName.ToUpper());
                    if (pdfFile != null)
                    {
                        pdffilepath = pdfFile.FullFilePath;
                        responses.Add("PDF Exists: " + pdfFile);
                        _logger.Info("PDF Exists: " + pdfFile);

                    }
                    else //If users does not attach any file assume the pdf is in same directory as XML file.
                    {
                        //TODO: Will a pdf always be a requirement?
                        responses.Add("Using the xml file directory and xml documentName for attachment : " + Path.GetDirectoryName(xmlfilepath) + @"\" + fileName);
                        _logger.Info("Using the xml file directoryand xml documentName for attachment : " + Path.GetDirectoryName(xmlfilepath) + @"\" + fileName);

                        pdffilepath = Path.GetDirectoryName(xmlfilepath) + @"\" + fileName;
                        // Checking the existance of the specified
                        if (File.Exists(pdffilepath))
                        {
                            var fileFoundmsg = "Specified file exists.";
                            responses.Add(fileFoundmsg);
                            _logger.Info(fileFoundmsg);

                        }
                        else
                        {
                            Console.WriteLine();
                            var fileNotFoundmsg = "Specified file does not exist in the current directory.";
                            responses.Add(fileNotFoundmsg);
                            _logger.Info(fileNotFoundmsg);
                        }

                    }
                    byte[] bytes = File.ReadAllBytes(pdffilepath);
                    bfp.attachmentList[0].bytes = bytes;
                }
                catch (System.Exception ex)
                {
                    var message = "Error reading file : " + ex.Message;
                    if (ex.InnerException != null)
                        message = Environment.NewLine + ex.InnerException.Message;

                    responses.Add("Creating Attachment file Error reading file: " + message);
                    _logger.Error("Creating Attachment file Error reading file: " + message);
                    // Stop all processing
                    return null;
                }
                return bfp;
            }
            else
            {
                _logger.Warn("Files does not exist. Check the file path for : " + xmlfilepath);
                return null;
            }
        }

        /// <summary>
        /// SaveResponseToFile (request from customer) writes the processing messages for a current file to the same directory the xml or csv
        /// file is located and moves all successfully parsed files to a main directory and then clears all the responses from the list.
        /// The log history file name will be the DocketNumber_reponses.txt or Failed_responses.txt located in the original directory with the files.
        /// The goal of this method is to provide one log file per processed file to make it easier for user to review 
        /// and issues or history for the file processed. Please note we also log everything using Nlog to the /log folder.
        /// One log file per day only saving a specific number of files per time frame. See Nlog.config maxArchiveFiles config.
        /// </summary>
        /// <param name="filePathOfOrigin"> The root folder where the file originated</param>
        /// <param name="responses">List of responses from the processing and submission to eCourts</param>
        /// <param name="docketNumber">On successful submission we will have a docket number on failure, Failed will be in this string</param>
        /// <param name="files">List of the CourtCaseFiles we processed</param>
        public static void SaveResponseToFile(List<string> responses, string docketNumber, string xmlFilePath, string pdfFilePath)
        {
            try
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
                    responses.Add("Successful Submission Check Log File : " + logFilename);
                    _logger.Info("Successful Submission Check Log File : " + logFilename);
                }
                else // File did not load 
                {
                    // Create Error Log File : Failed_TestCorp2CorpBAD.XML_201610111313522504
                    logFilename = @"\" + Path.GetFileName(xmlFilePath) + "_" + docketNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";
                    responses.Add("Failed Submission Check Log File : " + logFilename);
                    _logger.Info("Failed Submission Check Log File : " + logFilename);
                }
                try
                {
                    responses.Add("Writing log file : " + root_dir + logFilename);
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
                        responses.Add("Archive File : " + Path.GetFileName(xmlFilePath));
                        responses.Add("Archive File : " + Path.GetFileName(pdfFilePath));
                        responses.Add("Archive Path : " + root_dir + @"\" + newDirectory);
                        _logger.Info("Archive File : " + Path.GetFileName(xmlFilePath));
                        _logger.Info("Archive File : " + Path.GetFileName(pdfFilePath));
                        _logger.Info("Archive Path : " + root_dir + @"\" + newDirectory);
                        File.Move(xmlFilePath, root_dir + @"\" + newDirectory + @"\" + Path.GetFileName(xmlFilePath));
                        File.Move(pdfFilePath, root_dir + @"\" + newDirectory + @"\" + Path.GetFileName(pdfFilePath));
                    }
                }
                catch (IOException ex)
                {
                    responses.Add("SaveResponseToFile IOException writing log file : " + ex.Message);
                    _logger.Error(ex.Message);
                    throw;
                }
                catch (System.Exception ex)
                {
                    responses.Add("SaveResponseToFile Exception writing log file : " + ex.Message);
                    _logger.Error(ex.Message);
                    throw;
                }
            }
            catch (System.Exception ex)
            {
                responses.Add("Error for " + xmlFilePath + " : " + ex.Message);
                _logger.Error("Error for " + xmlFilePath + " : " + ex.Message);
            }
        }

        /// <summary>
        /// IsStringTooLongCheck verifies if string is greater than max length of chars
        /// and shortens when over max length.
        /// </summary>
        /// <returns>The string too long check.</returns>
        /// <param name="value">Value.</param>
        public static string SubStr(string value, int maxlength)
        {
            value = value.Trim();
            if (value.Length > maxlength)
            {
                var msg = string.Format("Truncated string: from {0} to {1} chars, Original text : {2}, Truncated text: {3}", value.Length, maxlength, value, value.Substring(0, maxlength));
                _logger.Info(msg);

                return value.Substring(0, maxlength).Trim();
            }
            else
                return value.Trim();
        }
    }
}