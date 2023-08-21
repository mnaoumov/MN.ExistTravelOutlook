using System.Windows.Forms;

namespace MN.ExistTravelOutlook
{
    public static class Error
    {
        public static void Show(string text) => MessageBox.Show(text, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}