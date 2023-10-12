using CommandLine;

using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ReservedCpuSets {
    public class Options {
        [Option("timeout")]
        public int Timeout { get; set; }

        [Option("load-cpusets")]
        public bool LoadCpuSets { get; set; }
    }

    internal static class Program {
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport("kernel32.dll")]
        private static extern bool FreeLibrary(IntPtr hLibModule);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        private delegate int SetSystemCpuSetDelegate(int mask);

        private static int LoadCpuSet() {
            string bitmask = SharedFunctions.GetReservedCpuSets();

            // bitmask must be inverted because the logic is flipped
            string inverted_system_bitmask = "";

            for (int i = 0; i < bitmask.Length; i++) {
                inverted_system_bitmask += bitmask[i] == '0' ? 1 : 0;
            }

            int system_affinity = Convert.ToInt32(inverted_system_bitmask, 2);

            IntPtr module_handle = LoadLibrary("ReservedCpuSets.dll");

            if (module_handle == IntPtr.Zero) {
                _ = MessageBox.Show("Failed to apply changes. Could not load ReservedCpuSets.dll", "ReservedCpuSets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            IntPtr func_ptr = GetProcAddress(module_handle, "SetSystemCpuSet");

            if (func_ptr == IntPtr.Zero) {
                _ = MessageBox.Show("Failed to apply changes. GetProcAddress Failed", "ReservedCpuSets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

#pragma warning disable IDE1006 // Naming Styles
            SetSystemCpuSetDelegate SetSystemCpuSet = Marshal.GetDelegateForFunctionPointer<SetSystemCpuSetDelegate>(func_ptr);
#pragma warning restore IDE1006 // Naming Styles

            // all CPUs = 0 rather than all bits set to 1
            if (SetSystemCpuSet(Convert.ToInt32(bitmask) == 0 ? 0 : system_affinity) != 0) {
                _ = MessageBox.Show("Failed to apply changes. Could not apply system-wide CPU set", "ReservedCpuSets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 1;
            }

            _ = FreeLibrary(module_handle);

            return 0;
        }

        [STAThread]
        private static void Main() {
            // 10240 is Windows 10 version 1507
            if (SharedFunctions.GetWindowsBuildNumber() < 10240) {
                _ = MessageBox.Show("ReservedCpuSets supports Windows 10 and above only", "ReservedCpuSets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            _ = Parser.Default.ParseArguments<Options>(Environment.GetCommandLineArgs()).WithParsed(o => {
                if (o.LoadCpuSets) {
                    Thread.Sleep(o.Timeout * 1000);
                    Environment.Exit(LoadCpuSet());
                }
            });


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
