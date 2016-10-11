using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilFilingClient
{
    public class CourtCaseFilesClean
    {
        public string XMLFileName { get; set; }
        public string XMLFullFilePath { get; set; }
        public string XMLFileExtension { get; set; }
        public string XMLDirectoryName { get; set; }
        public string PDFFileName { get; set; }
        public string PDFFullFilePath { get; set; }
        public string PDFFileExtension { get; set; }
        public string PDFDirectoryName { get; set; }
        public bool IsSubmitted { get; set; }
        public CourtCaseFilesClean(
            string xmlfileName,
            string xmlfullFilePath,
            string xmlfileExtension,
            string xmldirectoryName,
            string pdffileName,
            string pdffullFilePath,
            string pdffileExtension,
            string pdfdirectoryName,

            bool isSubmitted)
        {
            XMLFileName = xmlfileName;
            XMLFullFilePath = xmlfullFilePath;
            XMLFileExtension = xmlfileExtension;
            XMLDirectoryName = xmldirectoryName;

            PDFFileName = pdffileName;
            PDFFullFilePath = pdffullFilePath;
            PDFFileExtension = pdffileExtension;
            PDFDirectoryName = pdfdirectoryName;
            IsSubmitted = isSubmitted;
        }
    }
}

