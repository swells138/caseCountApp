using System.Windows.Forms;

namespace JiraTicketStats
{
    public partial class CaseStats : Form
    {
        // persisted second CSV path
        private string _secondCsvPath;

        // helper/legacy small textbox (mapped to designer control at runtime)
        private TextBox textBox1;

        public CaseStats()
        {
            InitializeComponent();
        }
    }
}
