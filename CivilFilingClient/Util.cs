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
        /// Each xml file's soap message should contain a pdf
        /// If an attachment is avaiable the attachment will be created.
        /// responses will be passed by reference on the heap.
        /// </summary>
        /// <param name="filePathXml"></param>
        /// <returns></returns>
        public static CivilFilingServiceReference.bulkFilingPacket ReadXmlfile(string filePathXml, List<string> responses, List<CourtCaseFiles> files)
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
                responses.Add("ERROR xml nodes should contain: " + message);
                _logger.Error("ERROR xml nodes should contain" + message);
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
        public static void SaveResponseToFile(string filePathOfOrigin, List<string> responses, string docketNumber, List<CourtCaseFiles> files)
        {
            //TODO: This really just needs to be one file per reponse, a user can select more than one xml and pdf file each run... 
            string newDirectory = DateTime.Now.ToString("yyyyMMdd");
            Directory.CreateDirectory(filePathOfOrigin + @"\" + newDirectory);
            string filename = @"\" + docketNumber + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".txt";
            try
            {
                using (StreamWriter outputFile = new StreamWriter(filePathOfOrigin + filename))
                {
                    foreach (string response in responses)
                        outputFile.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff") + " | " + response);
                }
                // Move the files we processed into the Archive folder (yyyyMMdd)
                // when the process is successful
                // Otherwise leave the files in the current folder.
                if (docketNumber != "Failed")
                {
                    foreach (var item in files)
                    {
                        File.Move(item.DirectoryName + @"\" + item.FileName,
                            filePathOfOrigin + @"\" + newDirectory + @"\" + item.FileName);
                    }
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