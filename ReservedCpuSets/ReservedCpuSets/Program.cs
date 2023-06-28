using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReservedCpuSets
{
    internal static class Program
    {
        static int GetWindowsBuildNumber() {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion")) {
                return int.Parse(key.GetValue("CurrentBuildNumber") as string);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // 10240 is Windows 10 version 1507
            if (GetWindowsBuildNumber() < 10240) {
                MessageBox.Show("ReservedCpuSets supports Windows 10 and above only", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            // TODO: parse args properly

            string[] args = Environment.GetCommandLineArgs();

            if (args.Length > 1 && args[1] == "--load-cpusets") {
                Form1.LoadCpuSet();
                Environment.Exit(0);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
