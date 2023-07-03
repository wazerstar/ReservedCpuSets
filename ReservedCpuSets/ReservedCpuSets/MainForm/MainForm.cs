using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ReservedCpuSets {
    public partial class MainForm : Form {

        public MainForm() {
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
                _ = cpuListBox.Items.Add($"CPU {i}");
            }

            // load current configuration into the program

            string bitmask = SharedFunctions.GetReservedCpuSets();

            int last_bit_index = bitmask.Length - 1;

            for (int i = 0; i < Environment.ProcessorCount; i++) {
                cpuListBox.SetItemChecked(i, bitmask[last_bit_index - i] == '1');
            }
        }

        private void Button1_Click(object sender, EventArgs e) {
            if (IsAllCPUsChecked()) {
                _ = MessageBox.Show("At least one CPU must be unreserved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int affinity = 0;

            foreach (int i in cpuListBox.CheckedIndices) {
                affinity |= 1 << i;
            }

            if (affinity == 0) {
                // all CPUs unreserved correspond to the registry key being deleted
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel", true)) {
                    try {
                        key.DeleteValue("ReservedCpuSets");
                    } catch (ArgumentException) {
                        // ignore error if the key does not exist
                    }
                }
            } else {
                byte[] bytes = BitConverter.GetBytes(affinity);
                byte[] padded_bytes = new byte[8];
                Array.Copy(bytes, 0, padded_bytes, 0, bytes.Length);

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel", true)) {
                    key.SetValue("ReservedCpuSets", padded_bytes, RegistryValueKind.Binary);
                }
            }

            Environment.Exit(0);
        }

        private void InvertSelection_Click(object sender, EventArgs e) {
            for (int i = 0; i < cpuListBox.Items.Count; i++) {
                cpuListBox.SetItemChecked(i, !cpuListBox.GetItemChecked(i));
            }
        }

        private void CheckAll_Click(object sender, EventArgs e) {
            CheckAllCPUs(true);
        }

        private void UncheckAll_Click(object sender, EventArgs e) {
            CheckAllCPUs(false);
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void AboutToolStripMenuItem_Click(object sender, EventArgs e) {
            AboutForm about = new AboutForm {
                StartPosition = FormStartPosition.Manual,
                Location = Location
            };
            about.Show();
        }
    }
}
