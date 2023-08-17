using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MN.ExistTravelOutlook
{
    public partial class TripCodeForm : Form
    {
        public TripCodeForm()
        {
            InitializeComponent();
        }

        public HashSet<string> AskForTripCodes()
        {
            var result = ShowDialog();

            switch (result)
            {
                case DialogResult.OK:
                    return tripCodesTextBox.Text.Split(' ').ToHashSet();
                case DialogResult.Ignore:
                    return new HashSet<string>();
                default:
                    return null;
            }
        }

        private void tripCodesTextBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = tripCodesTextBox.Text != "";
        }

        private void noTripCodeRequiredButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(tripCodesTextBox.Text, @"^E\d{4}( E\d{4})*$"))
            {
                Error.Show("Invalid trip codes format");
                return;
            }

            DialogResult = DialogResult.OK;
        }
    }
}
