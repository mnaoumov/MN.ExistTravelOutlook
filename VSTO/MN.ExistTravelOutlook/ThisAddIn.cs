using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Office.Interop.Outlook;
using Exception = System.Exception;

namespace MN.ExistTravelOutlook
{
    public partial class ThisAddIn
    {
        private Items _officeInboxItems;
        private Folders _officeInboxFolders;
        private const string OfficeMailboxName = "Office Exist Luxury Travel";
        private const string InboxName = "Inbox";
        private const string SmtpSchemaName = "http://schemas.microsoft.com/mapi/proptag/0x39FE001F";

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            var mapi = Application.GetNamespace("MAPI");
            var folder = TryGetFolder(mapi, OfficeMailboxName);
            if (folder == null)
            {
                Error.Show($"{OfficeMailboxName} mailbox is not found");
                return;
            }

            var officeInboxFolder = TryGetFolder(folder, InboxName);

            if (officeInboxFolder == null)
            {
                Error.Show($"Inbox in {OfficeMailboxName} mailbox is not found");
                return;
            }

            _officeInboxItems = officeInboxFolder.Items;
            _officeInboxFolders = officeInboxFolder.Folders;
            _officeInboxItems.ItemAdd += HandleOfficeInboxItemAdded;
            Application.ItemSend += HandleItemSent;
        }

        private static MAPIFolder TryGetFolder(MAPIFolder folder, string folderName)
        {
            return TryGetFolder(folder.Folders, folderName);
        }

        private static MAPIFolder TryGetFolder(_Folders folders, string folderName)
        {
            for (var i = 1; i <= folders.Count; i++)
            {
                var folder = folders[i];
                if (folder.Name == folderName)
                {
                    return folder;
                }
            }

            return null;
        }

        private static MAPIFolder TryGetFolder(_NameSpace nameSpace, string folderName) =>
            TryGetFolder(nameSpace.Folders, folderName);

        private void HandleItemSent(object item, ref bool cancel)
        {
            try
            {
                if (!(item is MailItem mailItem))
                {
                    return;
                }

                var tripCodes = ExtractTripCodes(mailItem.Subject);

                if (tripCodes.Count == 0)
                {
                    tripCodes = new TripCodeForm().AskForTripCodes();
                    if (tripCodes == null)
                    {
                        Error.Show("The email was not sent. The trip code is not provided");
                        cancel = true;
                        return;
                    }

                    if (tripCodes.Count == 0)
                    {
                        return;
                    }

                    var subjectPrefix = string.Concat(tripCodes.Select(tripCode => $"[{tripCode}] "));
                    mailItem.Subject = subjectPrefix + mailItem.Subject;
                }

                var senderEmailAddress = GetEmailAddress(Application.Session.CurrentUser);
                var senderHost = senderEmailAddress.Split('@')[1];

                var recipientsToRemoveIndices = (
                    from Recipient recipient in mailItem.Recipients
                    let recipientAddress = GetEmailAddress(recipient)
                    where recipientAddress.Contains(senderHost) && !recipientAddress.Contains('+')
                    select recipient.Index).ToList();

                foreach (var tripCode in tripCodes)
                {
                    var recipient = mailItem.ReplyRecipients.Add(AddTripCode(senderEmailAddress, tripCode));
                    recipient.Resolve();

                    foreach (var index in recipientsToRemoveIndices)
                    {
                        recipient = mailItem.Recipients[index];
                        var recipient2 = mailItem.Recipients.Add(AddTripCode(GetEmailAddress(recipient), tripCode));
                        recipient2.Type = recipient.Type;
                        recipient2.Resolve();
                    }
                }

                foreach (var index in recipientsToRemoveIndices.AsEnumerable().Reverse())
                {
                    mailItem.Recipients.Remove(index);
                }
            }
            catch (Exception e)
            {
                Error.Show($"Error sending email\r\n{e}");
            }
        }

        private static string AddTripCode(string email, string tripCode) => email.Replace("@", "+" + tripCode + "@");

        private void HandleOfficeInboxItemAdded(object item)
        {
            try
            {
                if (!(item is MailItem mailItem))
                {
                    return;
                }

                var inboxFolder = (Folder) mailItem.Parent;
                if (inboxFolder.Name != InboxName)
                {
                    return;
                }

                var tripCodes = ExtractTripCodes(mailItem.Subject);

                foreach (Recipient recipient in mailItem.Recipients)
                {
                    tripCodes.UnionWith(ExtractTripCodes(GetEmailAddress(recipient)));
                }

                if (tripCodes.Count == 0)
                {
                    return;
                }

                var isFirst = true;

                foreach (var folder in tripCodes.Select(GetOrCreateInboxSubfolder))
                {
                    if (!isFirst)
                    {
                        mailItem = mailItem.Copy();
                    }

                    mailItem = mailItem.Move(folder);
                    isFirst = false;
                }

            }
            catch (Exception e)
            {
                Error.Show($"Error handling new email\r\n{e}");
            }
        }

        private MAPIFolder GetOrCreateInboxSubfolder(string subfolderName)
        {
            return TryGetFolder(_officeInboxFolders, subfolderName) ?? _officeInboxFolders.Add(subfolderName);
        }

        private static string GetEmailAddress(Recipient recipient)
        {
            if (recipient.AddressEntry.Type != "EX")
            {
                return recipient.Address;
            }

            var exchangeUser = recipient.AddressEntry.GetExchangeUser();

            if (exchangeUser != null)
            {
                return exchangeUser.PrimarySmtpAddress;
            }

            try
            {
                return recipient.PropertyAccessor.GetProperty(SmtpSchemaName);
            }
            catch
            {
                return "";
            }
        }

        private static HashSet<string> ExtractTripCodes(string text) =>
            Regex.Matches(text, @"\b(E\d{4})(\b|_)")
            .Cast<Match>()
            .Select(match => match.Groups[1].Value)
            .ToHashSet();

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
