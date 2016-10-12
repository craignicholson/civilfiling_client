using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilFilingClient
{
    public class Attachments
    {
        public string FileName { get; set; }
        public string FullFilePath { get; set; }
        public string FileExtension { get; set; }
        public string DirectoryName { get; set; }
        public bool IsSubmitted { get; set; }
        public Attachments(
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
}
