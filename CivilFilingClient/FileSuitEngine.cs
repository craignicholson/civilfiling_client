using NLog;
using System.Collections.Generic;
using System.IO;
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
            Xmlfilepath = xmlFilePath;
            Responses = responses;
        }

        private string Username { get; set; }
        private string Password { get; set; }
        private string Endpoint { get; set; }
        private string Xmlfilepath { get; set; }
        private string Pdffilepath { get; set; }
        private List<string> Responses { get; set; }

        public bool FileSuit()
        {
            bool IsSuccess = false;
            // 6 chars with leading zeros when response is a success
            // set to Error when not a success
            string strDocketNumber = string.Empty;
            List<CourtCaseFiles> files = new List<CourtCaseFiles>();

            string fileMsg = "Reading File:" + Xmlfilepath;
            Responses.Add(fileMsg);
            _logger.Info(fileMsg);
            

            var bfp = Util.ReadXmlfile(Xmlfilepath, Responses, files);
            if (bfp == null)
            {
                Responses.Add("Invalid formatted file:" + Path.GetFileName(Xmlfilepath));
                _logger.Info("Invalid formatted file:" + Path.GetFileName(Xmlfilepath));
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
                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("feinsuch-craig", Username, Password));
                string message = "Attempting to send the web request to:" + client.Endpoint.Address.ToString();
                Responses.Add(message);
                _logger.Warn(message);
                filingReponse = client.submitCivilFiling(filingRequest);
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

            //When no docket number is received the transaction has failed
            if (strDocketNumber.Length == 0)
            {
                strDocketNumber = "Failed_" + Path.GetFileName(Xmlfilepath).ToString();
                string failedMesssage = "Failed. Please review the eCourts | Code above.";
                Responses.Add(failedMesssage);
                _logger.Info(failedMesssage);
            }
            else
            {
                IsSuccess = true;
            }
            // Save the data out to a file, in this reality it should be taking one xml and one pdf file only
            Util.SaveResponseToFile(Path.GetDirectoryName(Xmlfilepath).ToString(), Responses, strDocketNumber, files);

            return IsSuccess;
        }
    }
}