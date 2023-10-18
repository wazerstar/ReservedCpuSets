using System.Diagnostics;
using System.Windows.Forms;

namespace ReservedCpuSets {
    public partial class AboutForm : Form {
        public AboutForm() {
            InitializeComponent();
        }

        private void GitHubLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            _ = Process.Start("https://github.com/amitxv");
        }
    }
}
