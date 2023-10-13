using Microsoft.Win32;

using System;
using System.Reflection;
using System.Windows.Forms;

namespace ReservedCpuSets {
    public partial class MainForm : Form {

        public MainForm() {
            InitializeComponent();
        }

        private bool IsAddedToStartup() {
            using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run")) {
                return key.GetValue("ReservedCpuSets") != null;
            }
        }

        private void AddToStartup(bool is_enabled) {
            var entry_assembly = Assembly.GetEntryAssembly();

            using (var key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true)) {
                if (is_enabled) {
                    key.SetValue("ReservedCpuSets", $"\"{entry_assembly.Location}\" --load-cpusets --timeout 10");
                } else {
                    try {
                        key.DeleteValue("ReservedCpuSets");
                    } catch (ArgumentException) {
                        // ignore error if the key does not exist
                    }
                }
            }
        }

        private bool IsAllCPUsChecked() {
            for (var i = 0; i < cpuListBox.Items.Count; i++) {
                if (!cpuListBox.GetItemChecked(i)) {
                    return false;
                }
            }

            return true;
        }

        private void CheckAllCPUs(bool isChecked) {
            for (var i = 0; i < cpuListBox.Items.Count; i++) {
                cpuListBox.SetItemChecked(i, isChecked);
            }
        }

        private void MainForm_Load(object sender, EventArgs e) {
            for (var i = 0; i < Environment.ProcessorCount; i++) {
                _ = cpuListBox.Items.Add($"CPU {i}");
            }

            // load current configuration into the program

            var bitmask = SharedFunctions.GetReservedCpuSets();

            var last_bit_index = bitmask.Length - 1;

            for (var i = 0; i < Environment.ProcessorCount; i++) {
                cpuListBox.SetItemChecked(i, bitmask[last_bit_index - i] == '1');
            }

            if (SharedFunctions.GetWindowsBuildNumber() > 19044) {
                addToStartup.Enabled = false;
                addToStartup.ToolTipText = "Configuration does not need to be applied\non a per-boot basis on 21H2+";
            }

            // check if program is set to run at startup
            addToStartup.Checked = IsAddedToStartup();

            // 19044 is Windows 10 version 21H2
            if (!IsAddedToStartup() && SharedFunctions.GetWindowsBuildNumber() < 19044) {
                _ = MessageBox.Show("On 21H1 and below, the configuration must be applied on a per-boot basis.\nPlace the program somewhere safe and enable \"Add To Startup\" in the File menu", "ReservedCpuSets", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e) {
            if (IsAllCPUsChecked()) {
                _ = MessageBox.Show("At least one CPU must be unreserved", "ReservedCpuSets", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var affinity = 0;

            foreach (int i in cpuListBox.CheckedIndices) {
                affinity |= 1 << i;
            }

            if (affinity == 0) {
                // all CPUs unreserved correspond to the registry key being deleted
                using (var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel", true)) {
                    try {
                        key.DeleteValue("ReservedCpuSets");
                    } catch (ArgumentException) {
                        // ignore error if the key does not exist
                    }
                }
            } else {
                var bytes = BitConverter.GetBytes(affinity);
                var padded_bytes = new byte[8];
                Array.Copy(bytes, 0, padded_bytes, 0, bytes.Length);

                using (var key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel", true)) {
                    key.SetValue("ReservedCpuSets", padded_bytes, RegistryValueKind.Binary);
                }
            }

            Environment.Exit(0);
        }

        private void InvertSelection_Click(object sender, EventArgs e) {
            for (var i = 0; i < cpuListBox.Items.Count; i++) {
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
            var about = new AboutForm {
                StartPosition = FormStartPosition.Manual,
                Location = Location
            };
            about.Show();
        }

        private void AddToStartup_Click(object sender, EventArgs e) {
            AddToStartup(addToStartup.Checked);
        }
    }
}
