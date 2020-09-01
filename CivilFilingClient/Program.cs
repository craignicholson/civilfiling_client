using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CivilFilingClient
{
    static class Program
    {
        /// <summary>
        /// Initialize NLog logger
        /// </summary>
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            List<string> responses = new List<string>();

            string[] args = Environment.GetCommandLineArgs();
            // Required 4 parameters - which means five for winform exe
            //string username, string password, string endpoint, string xmlFilePath, List<string> responses)
            if (args.Length == 5)
            {
                //Console ... there is none... it all goes to bit bucket dev/null when calling via CLI this way
                Console.WriteLine("Beginning of CLI Process");
                _logger.Info("Beginning of CLI Process");
                foreach (var arg in args)
                {
                    _logger.Info("args[] : "  + arg);
                }
                //1st arg is trash, it is just the app name.
                var arg1 = args[0];
                var username = args[1];
                var password = args[2];
                var endpoint = args[3];
                var xmlfilepath = args[4];
                //var pdffilepath = args[5]; //Should pdf file path be here?  I think so... the more restrictive the more success
                FileSuitEngine suit = new FileSuitEngine(username, password, endpoint, xmlfilepath, responses);
                suit.FileSuitXml();
                foreach(var log in responses)
                {
                    Console.WriteLine(log);
                }
                _logger.Info("End of CLI Process");
                Console.WriteLine("End of CLI Process");
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}
