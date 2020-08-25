using NLog;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Windows.Forms;

namespace CivilFilingClient
{
    public partial class frmSearchNotices : Form
    {
        /// <summary>
        /// Initialize NLog logger
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        // Configs
        string _CurrentEndPoint;
        string _CurrentUsername;
        string _CurrentPwd;

        string _productionEndPoint;
        string _productionUsername;
        string _productionPwd;

        string _testEndPoint;
        string _testUsername;
        string _testPwd;

        /// <summary>
        /// _mode holds 'Test' or 'Production' and assigns correct configs to the current configs
        /// </summary>
        string _mode;

        public frmSearchNotices()
        {
            InitializeComponent();
            InitializeConfigurations();
        }

        private void InitializeConfigurations()
        {
            /// Production Endpoint, Username and Password
            _productionEndPoint = ConfigurationManager.AppSettings["productionEndPoint"].ToString();
            if (string.IsNullOrEmpty(_productionEndPoint))
            {
                _logger.Error("'productionEndPoint' is not configured in Config file");
                rtbSearchNoticies.AppendText(Environment.NewLine + "'productionEndPoint' is not configured in Config file");
            }
            _productionUsername = ConfigurationManager.AppSettings["productionUsername"].ToString();
            if (string.IsNullOrEmpty(_productionUsername))
            {
                _logger.Error("'productionUsername' is not configured in Config file");
                rtbSearchNoticies.AppendText(Environment.NewLine + "'productionUsername' is not configured in Config file");
            }
            _productionPwd = ConfigurationManager.AppSettings["productionPwd"].ToString();
            if (string.IsNullOrEmpty(_productionPwd))
            {
                _logger.Error("'productionPwd' is not configured in Config file");
                rtbSearchNoticies.AppendText(Environment.NewLine + "'productionPwd' is not configured in Config file");
            }

            /// Test Endpoint Username and password
            _testEndPoint = ConfigurationManager.AppSettings["testEndPoint"].ToString();
            if (string.IsNullOrEmpty(_testEndPoint))
            {
                _logger.Error("'testEndPoint' is not configured in Config file");
                rtbSearchNoticies.AppendText(Environment.NewLine + "'testEndPoint' is not configured in Config file");
            }

            _testUsername = ConfigurationManager.AppSettings["testUsername"].ToString();
            if (string.IsNullOrEmpty(_testUsername))
            {
                _logger.Error("'testUsername' is not configured in Config file");
                rtbSearchNoticies.AppendText(Environment.NewLine + "'testUsername' is not configured in Config file");
            }

            _testPwd = ConfigurationManager.AppSettings["testPwd"].ToString();
            if (string.IsNullOrEmpty(_testPwd))
            {
                _logger.Error("'testPwd' is not configured in Config file");
                rtbSearchNoticies.AppendText(Environment.NewLine + "'testPwd' is not configured in Config file");
            }

            // Mode is set in the app.config and correct values are Test and Production
            _mode = ConfigurationManager.AppSettings["mode"].ToString();
            if (string.IsNullOrEmpty(_mode))
            {
                _logger.Error("'mode' is not configured in Config file");
                rtbSearchNoticies.AppendText(Environment.NewLine + "'mode' is not configured in Config file");
            }

            if (_mode == "Test")
            {
                _CurrentEndPoint = _testEndPoint;
                _CurrentUsername = _testUsername;
                _CurrentPwd = _testPwd;
            }
            else if (_mode == "Production")
            {
                _CurrentEndPoint = _productionEndPoint;
                _CurrentUsername = _productionUsername;
                _CurrentPwd = _productionPwd;
            }
            else
            {
                rtbSearchNoticies.AppendText("Mode should set to 'Test' or 'Production");
            }

            // Testing Block
            rtbSearchNoticies.AppendText(_CurrentEndPoint);
        }
        private void btnSearchNotices_Click(object sender, EventArgs e)
        {
            string dt = dtSearchNotices.Value.ToString("yyyyMMdd");
            rtbSearchNoticies.AppendText(dt);

            //var noticesRequest = new CivilFilingServiceReference2_0_V2.noticeRequest
            //{
            //    firmId = "F88888003",
            //    noticeDate = dt
            //};

            var noticesRequest = new CivilFilingServiceReferenceV2.noticeRequest
            {
                firmId = "F88888003",
                noticeDate = dt
            };

            var address = new EndpointAddress(_CurrentEndPoint);
            var client = new CivilFilingServiceReferenceV2.CivilFilingWSClient("CivilFilingWSPort", address);

            // Create the response outside of the using since instantiation inside of using limits the scope of the variable
            CivilFilingServiceReferenceV2.noticeResponse noticeReponse = null;
            using (new OperationContextScope(client.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("feinsuch", _CurrentUsername, _CurrentPwd));
                string message = "Attempting to send the web request to:" + client.Endpoint.Address.ToString();
                rtbSearchNoticies.AppendText(message);
                _logger.Warn(message);
                try
                {
                    // Web Service Can timeout.  We should still write a log file of Automation tracking.
                    noticeReponse = client.searchNotices(noticesRequest);
                }
                catch (System.Exception ex)
                {
                    // TODO: Colors these red
                    rtbSearchNoticies.AppendText("eCourts " + _CurrentEndPoint + " error :" + message);
                    _logger.Error("eCourts " + _CurrentEndPoint + " error :" + message);

                    if (ex.InnerException != null)
                    {
                        rtbSearchNoticies.AppendText("eCourts " + _CurrentEndPoint + " error :" + ex.InnerException);
                        _logger.Error("eCourts " + _CurrentEndPoint + " error :" + ex.InnerException);
                    }
                    //Util.SaveResponseToFile(Responses, "Failed", XmlFilePath, pdfFilepath);
                    // throw the error back to the UI so they know the timeout occured
                    // note this will be logged 2x because we are throwing it up the stack
                    throw new System.ArgumentException("eCourts " + _CurrentEndPoint + " error :", ex.Message); ;
                }
            }
            // Review the request results / messages
            if (noticeReponse.messages != null)
            {
                foreach (var msg in noticeReponse.messages)
                {
                    string filingMsg = "eCourts | Code: " + msg.code + " Description: " + msg.description;
                    rtbSearchNoticies.AppendText(filingMsg);
                    _logger.Warn(filingMsg);
                }
            }
            if (noticeReponse.notices != null && noticeReponse.notices.Length > 0)
            {
                foreach (var notice in noticeReponse.notices)
                {
                    rtbSearchNoticies.AppendText(notice.firmId);
                    rtbSearchNoticies.AppendText(notice.noticeDate);
                    rtbSearchNoticies.AppendText(notice.noticeDesc);
                    rtbSearchNoticies.AppendText(notice.noticeType);
                    rtbSearchNoticies.AppendText(notice.docketNumber.docketSeqNum.ToString());
                    rtbSearchNoticies.AppendText(notice.docketNumber.docketCourtYear.ToString());
                    rtbSearchNoticies.AppendText(notice.docketNumber.docketTypeCode.ToString());
                    rtbSearchNoticies.AppendText(notice.docketNumber.docketVenue.ToString());
                }
            }
            _logger.Info("End of Submission");

        }
    }
}
