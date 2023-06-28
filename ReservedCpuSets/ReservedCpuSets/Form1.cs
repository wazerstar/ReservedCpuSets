using Microsoft.Win32;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ReservedCpuSets {
    public partial class Form1 : Form {

        [DllImport("ReservedCpuSets.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetSystemCpuSet(int mask);

        readonly static string kernel_key = "SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel";

        public static void LoadCpuSet() {
            string bitmask = GetReservedCpuSets();
            int affinity = Convert.ToInt32(bitmask);
            int system_affinity;

            if (affinity == 0) {
                system_affinity = 0; // reset to default

                // all CPUs unreserved correspond to the registry key being deleted
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(kernel_key, true)) {
                    try {
                        key.DeleteValue("ReservedCpuSets");
                    } catch (ArgumentException) {
                        // ignore error if the key does not exist
                    }
                }
            } else {
                // bitmask must be inverted because the logic is flipped
                string inverted_system_bitmask = "";

                for (int i = 0; i < bitmask.Length; i++) {
                    inverted_system_bitmask += bitmask[i] == '0' ? 1 : 0;
                }

                system_affinity = Convert.ToInt32(inverted_system_bitmask, 2);
            }

            if (SetSystemCpuSet(system_affinity) != 0) {
                MessageBox.Show("Failed to apply system-wide CPU set", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(1);
            }
        }

        private static string GetReservedCpuSets() {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(kernel_key)) {
                try {
                    byte[] current_config = key.GetValue("ReservedCpuSets") as byte[];
                    Array.Reverse(current_config); // big to little endian
                    // convert to binary
                    string bitmask = string.Join("", current_config.Select(b => Convert.ToString(b, 2)));

                    // sterilize string and ensure it is length of core count
                    return bitmask.TrimStart('0').PadLeft(Environment.ProcessorCount, '0');
                } catch (System.ArgumentException) {
                    return "";
                }
            }
        }

        public Form1() {
            InitializeComponent();
        }

        private bool IsAllCPUsChecked() {
            for (int i = 0; i < cpuListBox.Items.Count; i++) {
                if (!cpuListBox.GetItemChecked(i)) {
                    return false;
                }
            }

            return true;
        }

        private void CheckAllCPUs(bool isChecked) {
            for (int i = 0; i < cpuListBox.Items.Count; i++) {
                cpuListBox.SetItemChecked(i, isChecked);
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
            for (int i = 0; i < Environment.ProcessorCount; i++) {
                cpuListBox.Items.Add($"CPU {i}");
            }

            // load current configuration into the program

            string bitmask = GetReservedCpuSets();

            if (bitmask != "") {
                int last_bit_index = bitmask.Length - 1;

                for (int i = 0; i < Environment.ProcessorCount; i++) {
                    cpuListBox.SetItemChecked(i, bitmask[last_bit_index - i] == '1');
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            if (IsAllCPUsChecked()) {
                MessageBox.Show("At least one CPU must be unreserved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int affinity = 0;

            foreach (int i in cpuListBox.CheckedIndices) {
                affinity |= 1 << i;
            }

            byte[] bytes = BitConverter.GetBytes(affinity);
            byte[] padded_bytes = new byte[8];
            Array.Copy(bytes, 0, padded_bytes, 0, bytes.Length);

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(kernel_key, true)) {
                key.SetValue("ReservedCpuSets", padded_bytes, RegistryValueKind.Binary);
            }

            LoadCpuSet();
            Environment.Exit(0);
        }

        private void invertSelection_Click(object sender, EventArgs e) {
            for (int i = 0; i < cpuListBox.Items.Count; i++) {
                cpuListBox.SetItemChecked(i, !cpuListBox.GetItemChecked(i));
            }
        }

        private void checkAll_Click(object sender, EventArgs e) {
            CheckAllCPUs(true);
        }

        private void uncheckAll_Click(object sender, EventArgs e) {
            CheckAllCPUs(false);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            About about = new About();
            about.StartPosition = FormStartPosition.Manual;
            about.Location = this.Location;
            about.Show();
        }
    }
}
