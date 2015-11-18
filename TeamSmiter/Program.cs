using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamSmiter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Idle += new EventHandler(Application_Idle);
            Application.Run(new MainViewer());
        }

        private static void Application_Idle(object sender, EventArgs e)
        {
            while (NativeModule.AppStillIdle)
            {
                if (MainViewer.Instance.IsServer)
                {
                    MainViewer.Instance.ServerHandleData();
                }
            }
        }
    }
}
