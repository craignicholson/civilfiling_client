using NLog;
using System;
using System.Windows.Forms;

namespace CivilFilingClient
{
    static class Program
    {
        /// <summary>
        /// Initialize NLog logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                foreach (string arg in args)
                {
                    logger.Info("args: " + arg);
                }
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
