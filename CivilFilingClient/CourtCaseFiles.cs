namespace CivilFilingClient
{
    /// <summary>
    /// CourtCaseFiles will maintain a list of all files selected during submission process.
    /// We will process each of the XML files and search for the corresponding PDF files found inside
    /// of the soapenvelope. We will assume the XML directory will have
    /// the PDF file located in the same directory as the XML file.
    /// </summary>
    public class CourtCaseFiles
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
}
