using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace ReservedCpuSets {
    public partial class Form1 : Form {

        readonly string kernel_key = "SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel";

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

            if (affinity == 0) {
                // all CPUs unreserved correspond to the registry key being deleted
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(kernel_key, true)) {
                    try {
                        key.DeleteValue("ReservedCpuSets");
                    } catch (System.ArgumentException) { 
                        // ignore error if the key does not exist
                    }
                }

                this.Close();
                return; // required to properly exit
            }

            byte[] bytes = BitConverter.GetBytes(affinity);
            byte[] padded_bytes = new byte[8];
            Array.Copy(bytes, 0, padded_bytes, 0, bytes.Length);

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(kernel_key, true)) {
                key.SetValue("ReservedCpuSets", padded_bytes, RegistryValueKind.Binary);
            }

            this.Close();
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
    }
}
