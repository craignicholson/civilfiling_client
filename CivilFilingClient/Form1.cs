using NLog;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security;
using System.Windows.Forms;

/// <summary>
/// Authors: Craig Nicholson
/// CivilFilingClient sends data to the New Jeresy Courts - eCourt Filing System
/// All rights reserved.
/// </summary>
namespace CivilFilingClient
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Initialize NLog logger
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// files will contain the full filePath and fileName for the files
        /// we need to load.  This will be one xml file and one pdf file per
        /// submission.
        /// </summary>
        List<CourtCaseFiles> _files = new List<CourtCaseFiles>();

        /// <summary>
        /// Legacy clean this up later...
        /// </summary>
        string _rootDirectory = string.Empty;

        /// <summary>
        /// responses will contain all of the messages returned from the web service
        /// and any errors.  These messages will be written to a file for the users.
        /// </summary>
        List<string> _responses = new List<string>();

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

        public Form1()
        {
            InitializeComponent();
            InitializeOpenFileDialog();
            InitializeConfigurations();
        }

        private void InitializeConfigurations()
        {
            /// Production Endpoint, Username and Password
            _productionEndPoint = ConfigurationManager.AppSettings["productionEndPoint"].ToString();
            if (string.IsNullOrEmpty(_productionEndPoint))
            {
                _logger.Error("'productionEndPoint' is not configured in Config file");
                richTextBox1.AppendText(Environment.NewLine + "'productionEndPoint' is not configured in Config file");
            }
            _productionUsername = ConfigurationManager.AppSettings["productionUsername"].ToString();
            if (string.IsNullOrEmpty(_productionUsername))
            {
                _logger.Error("'productionUsername' is not configured in Config file");
                richTextBox1.AppendText(Environment.NewLine + "'productionUsername' is not configured in Config file");
            }
            _productionPwd = ConfigurationManager.AppSettings["productionPwd"].ToString();
            if (string.IsNullOrEmpty(_productionPwd))
            {
                _logger.Error("'productionPwd' is not configured in Config file");
                richTextBox1.AppendText(Environment.NewLine + "'productionPwd' is not configured in Config file");
            }

            /// Test Endpoint Username and password
            _testEndPoint = ConfigurationManager.AppSettings["testEndPoint"].ToString();
            if (string.IsNullOrEmpty(_testEndPoint))
            {
                _logger.Error("'testEndPoint' is not configured in Config file");
                richTextBox1.AppendText(Environment.NewLine + "'testEndPoint' is not configured in Config file");
            }

            _testUsername = ConfigurationManager.AppSettings["testUsername"].ToString();
            if (string.IsNullOrEmpty(_testUsername))
            {
                _logger.Error("'testUsername' is not configured in Config file");
                richTextBox1.AppendText(Environment.NewLine + "'testUsername' is not configured in Config file");
            }

            _testPwd = ConfigurationManager.AppSettings["testPwd"].ToString();
            if (string.IsNullOrEmpty(_testPwd))
            {
                _logger.Error("'testPwd' is not configured in Config file");
                richTextBox1.AppendText(Environment.NewLine + "'testPwd' is not configured in Config file");
            }

            // Mode is set in the app.config and correct values are Test and Production
            _mode = ConfigurationManager.AppSettings["mode"].ToString();
            if (string.IsNullOrEmpty(_mode))
            {
                _logger.Error("'mode' is not configured in Config file");
                richTextBox1.AppendText(Environment.NewLine + "'mode' is not configured in Config file");
            }

            lblMode.Text = _mode;
            if (_mode == "Test")
            {
                _CurrentEndPoint = _testEndPoint;
                _CurrentUsername = _testUsername;
                _CurrentPwd = _testPwd;
                testToolStripMenuItem_Click(null, null);
            }
            else if(_mode == "Production")
            {
                _CurrentEndPoint = _productionEndPoint;
                _CurrentUsername = _productionUsername;
                _CurrentPwd = _productionPwd;
                productionToolStripMenuItem_Click(null, null);
            }
            else
            {
                lblMode.Text = "Mode should set to 'Test' or 'Production";
            }
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
            // xml file can have many plantiff and defendants
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                // Read all the files
                foreach (String file in openFileDialog1.FileNames)
                {
                    // Load the files into the List for processing.
                    try
                    {
                        //richTextBox1.AppendText(Environment.NewLine + Path.GetFullPath(file));
                        richTextBox1.AppendText(Environment.NewLine + Path.GetFileName(file));
                        _rootDirectory = Path.GetDirectoryName(file);
                        _files.Add(new CourtCaseFiles(Path.GetFileName(file),
                                                       file,
                                                       Path.GetExtension(file).ToString(),
                                                       Path.GetDirectoryName(file),
                                                       false));
                        _responses.Add("Attachment: " + file);
                        _logger.Info("Attachment: " + file);
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
                        MessageBox.Show("Cannot load the file: " + file.Substring(file.LastIndexOf('\\'))
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
            openFileDialog1.FileName = string.Empty;
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
            // Create the EndPoint, username and password, and UsernameTokenId
            try
            {
                foreach (var item in _files.ToArray())
                {
                    if (item.FileExtension.ToUpper() == ".XML")
                    {
                        richTextBox1.AppendText(Environment.NewLine + "Process started...");
                        _responses.Add("Attempting to send request");
                        _logger.Info("Attempting to send request");
                        FileSuitEngine suit = new FileSuitEngine(_CurrentUsername, _CurrentPwd, _CurrentEndPoint, item.FullFilePath, _responses);
                        item.IsSubmitted = suit.FileSuit();
                    }
                    //Write responses to RichTextBox... I know this is a hack... not real time
                    foreach(var log in _responses)
                    {
                        richTextBox1.AppendText(Environment.NewLine + log);
                    }
                    //clean up
                    _files.Remove(item);
                    _responses.Clear();
                }
            }
            catch (System.Exception ex)
            {
                // TODO: Colors these red
                richTextBox1.AppendText(Environment.NewLine + ex.Message);
                _logger.Error(ex.Message);

                if (ex.InnerException != null)
                {
                    richTextBox1.AppendText(Environment.NewLine + ex.InnerException.Message);
                    _logger.Error(ex.InnerException.Message);
                }
            }
            richTextBox1.AppendText(Environment.NewLine + "Processes has completed.  Please review logs for results.");
            btnSend.Enabled = true;
        }

        /// <summary>
        /// clearIsSubmitted Items clears out the response log and removes
        /// all files which were submitted.  A submitted item can be
        /// a failed or successful submission.
        /// </summary>
        private void clearIsSubmittedItems()
        {
            _responses.Clear();
            foreach (CourtCaseFiles item in _files)
            {
                if (item.IsSubmitted)
                    _files.Remove(item);
            }
        }

        // productionToolStripMenuItem_Click - sets the mode to production
        // which means we will use the productionEndPoint
        private void productionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            productionToolStripMenuItem.Checked = true;
            testToolStripMenuItem.Checked = false;
            lblMode.Text = "Production";
            lblMode.ForeColor = System.Drawing.Color.Red;

            _CurrentEndPoint = _productionEndPoint;
            _CurrentUsername = _productionUsername;
            _CurrentPwd = _productionPwd;

        }

        // testToolStripMenuItem_Click - sets the mode to production
        // which means we will use the testEndPoint
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testToolStripMenuItem.Checked = true;
            productionToolStripMenuItem.Checked = false;
            lblMode.Text = "Test";
            lblMode.ForeColor = System.Drawing.Color.Green;

            _CurrentEndPoint = _testEndPoint;
            _CurrentUsername = _testUsername;
            _CurrentPwd = _testPwd;
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            //Do nothing...
        }
    }
}