using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CivilFilingClient
{
    public partial class frmGetCivilFilingStatus : Form
    {
        public frmGetCivilFilingStatus()
        {
            InitializeComponent();
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            var address = new EndpointAddress("https://dptng.njcourts.gov:2045/civilFilingWS_t");
            // Create a new client with the endpoint we want to send the request
            var client = new CivilFilingServiceReference.CivilFilingWSClient("CivilFilingWSPort", address);
            // Create the response outside of the using since instantiation inside of using limits the scope of the variable
            
            
            using (new OperationContextScope(client.InnerChannel))
            {
                OperationContext.Current.OutgoingMessageHeaders.Add(new SecurityHeader("feinsuch", "888888029", "P@ssword"));
                string message = "Attempting to send the web request to:" + client.Endpoint.Address.ToString();
                //Responses.Add(message);
                //_logger.Warn(message);
                richTextBox1.AppendText(message);
                try
                {
                    CivilFilingServiceReference.civilFilingRequest filingRequest = new CivilFilingServiceReference.civilFilingRequest();
                    var efile = new CivilFilingServiceReference.efilingNumber();
                    //SCP2016312599
                    efile.efilingCourtDiv = "SCP";
                    efile.efilingCourtYr = "16";
                    efile.efilingSeqNo = 312599;
                    richTextBox1.AppendText(Environment.NewLine + "before");
                    CivilFilingServiceReference.bulkFilingPacket bfp = new CivilFilingServiceReference.bulkFilingPacket();
                    bfp.efilingNumber = efile;
                    filingRequest.bulkFilingPacket = bfp;
                    bfp.efilingNumber = efile;
                    
                    // Web Service Can timeout.  We should still write a log file of Automation tracking.
                    richTextBox1.AppendText(Environment.NewLine + "getCivilFilingStatus...");
                    var response = client.getCivilFilingStatus(filingRequest);

                    richTextBox1.AppendText(Environment.NewLine + "Did it work? queueFilingProcessed: " + response.queueFilingProcessed);
                    //richTextBox1.AppendText("Did it work? docketNumber: " + response.docketNumber);
                    //richTextBox1.AppendText("Did it work? efilingNumber: " + response.efilingNumber);
                    //foreach(var msg in response.messages)
                    //{
                    //    richTextBox1.AppendText("Did it work? code: " + msg.code);
                    //    richTextBox1.AppendText("Did it work? description: " + msg.code);
                    //}
                   
                    
                }
                catch (System.Exception ex)
                {
                    // TODO: Colors these red
                    richTextBox1.AppendText(Environment.NewLine + "Error: " + ex.Message);
                    //Responses.Add("eCourts " + Endpoint + " error :" + message);
                    //_logger.Error("eCourts " + Endpoint + " error :" + message);

                    if (ex.InnerException != null)
                    {
                        richTextBox1.AppendText(Environment.NewLine + "Error: " + ex.Message);
                       // Responses.Add("eCourts " + Endpoint + " error :" + ex.InnerException);
                       // _logger.Error("eCourts " + Endpoint + " error :" + ex.InnerException);
                    }
                    //Util.SaveResponseToFile(Responses, "Failed", XmlFilePath, pdfFilepath);
                    // throw the error back to the UI so they know the timeout occured
                    // note this will be logged 2x because we are throwing it up the stack
                    //throw new System.ArgumentException("eCourts " + Endpoint + " error :", ex.Message); ;
                }
            }

        }
    }
}
