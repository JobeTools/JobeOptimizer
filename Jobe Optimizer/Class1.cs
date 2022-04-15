using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Windows.Forms;
using System.ServiceProcess;

namespace Jobe_Optimizer
{
    internal static class Utilities
    {
        internal static void RunCommand(string command)
        {
            using (Process p = new Process())
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/C " + command;

                try
                {
                    p.Start();
                    p.WaitForExit();
                    p.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hmm I seemed to have skunked it ");
                }
            }
        }
        internal static bool ServiceExists(string serviceName)
        {
            return ServiceController.GetServices().Any(serviceController => serviceController.ServiceName.Equals(serviceName));
        }
        internal static void StopService(string serviceName)
        {
 

            if (ServiceExists(serviceName))
            {
                ServiceController sc = new ServiceController(serviceName);
                if (sc.CanStop)
                {
                    sc.Stop();
                }
            }
        }
    }
}
