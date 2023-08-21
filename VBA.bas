Option Explicit

Private WithEvents officeInboxItems As Items

Private Sub Application_Startup()
    Dim mapi As NameSpace
    Set mapi = Application.GetNamespace("MAPI")
    Dim officeInboxFolder As Folder
    Set officeInboxFolder = mapi.Folders("Office Exist Luxury Travel")
    Set officeInboxFolder = officeInboxFolder.Folders("Inbox")
    Set officeInboxItems = officeInboxFolder.Items
End Sub

Private Sub officeInboxItems_ItemAdd(ByVal item As Object)
    If Not TypeOf item Is MailItem Then
        Exit Sub
    End If
    
    On Error GoTo ErrorHandler
   
    Dim mailItem2 As MailItem
    Set mailItem2 = item
    
    Dim inboxFolder As Folder
    Set inboxFolder = mailItem2.Parent
    
    If inboxFolder.Name <> "Inbox" Then
        Exit Sub
    End If
    
    Dim tripCodes As Collection
    Set tripCodes = ExtractTripCodes(mailItem2.Subject)
    Dim i As Integer
    Dim tripCode As String
    
    For i = 1 To mailItem2.Recipients.Count
        Dim tripCodes2 As Collection
        Set tripCodes2 = ExtractTripCodes(GetEmailAddress(mailItem2.Recipients(i)))
        Dim j As Integer
        For j = 1 To tripCodes2.Count
            tripCode = tripCodes2(j)
            On Error Resume Next
            tripCodes.Add tripCode, tripCode
            On Error GoTo ErrorHandler
        Next
    Next
    
    If tripCodes.Count = 0 Then
        Exit Sub
    End If
    
    For i = 1 To tripCodes.Count
        Dim folder2 As Folder
        Set folder2 = Nothing
        tripCode = tripCodes(i)
        On Error Resume Next
        Set folder2 = inboxFolder.Folders.item(tripCode)
        On Error GoTo ErrorHandler
        If folder2 Is Nothing Then
            Set folder2 = inboxFolder.Folders.Add(tripCode)
        End If
        
        If i > 1 Then
            Set mailItem2 = mailItem2.Copy
        End If
        
        Set mailItem2 = mailItem2.Move(folder2)
    Next
    Exit Sub
ErrorHandler:
    MsgBox "Could not process new email. Error: " + Err.Description, vbCritical
End Sub

Private Sub Application_ItemSend(ByVal item As Object, Cancel As Boolean)
    If Not TypeOf item Is MailItem Then
        Exit Sub
    End If
    
    Cancel = True
    On Error GoTo ErrorHandler
    
    Dim mailItem2 As MailItem
    Set mailItem2 = item
    Dim tripCodes As Collection
    Set tripCodes = ExtractTripCodes(mailItem2.Subject)
    Dim i As Integer
    Dim tripCode As String
    
    If tripCodes.Count = 0 Then
        Dim str As String
        str = InputBox("Your email subject does not have a trip code. Enter space-separated codes in E1234 format. Type 'NO' if no trip code required", "Trip code")
        If UCase(str) = "NO" Then
            Cancel = False
            Exit Sub
        End If
        
        Dim parts() As String
        parts = Split(str, " ")
        
        If UBound(parts, 1) = -1 Then
            MsgBox "The email was not sent. The trip code is not provided", vbExclamation
            Exit Sub
        End If
        
        Dim subjectPrefix As String
        subjectPrefix = ""
        
        For i = 0 To UBound(parts, 1)
            tripCode = parts(i)
            If ExtractTripCodes(tripCode).Count = 0 Then
                MsgBox "The email was not sent. The trip code '" + tripCode + "' is in the wrong format", vbExclamation
                Exit Sub
            End If
            
            tripCodes.Add (tripCode)
            subjectPrefix = subjectPrefix + "[" + tripCode + "] "
        Next

mailItem2.Subject = subjectPrefix + mailItem2.Subject
    End If
    
    Dim senderEmailAddress As String
    senderEmailAddress = GetEmailAddress(Application.Session.CurrentUser)
    Dim senderHost As String
    senderHost = Split(senderEmailAddress, "@")(1)
    
    Dim recipient2 As Recipient
    
    Dim recipientsToRemoveIndices As New Collection
    
    For i = 1 To mailItem2.Recipients.Count
        Set recipient2 = mailItem2.Recipients(i)
        Dim recipientAddress As String
        recipientAddress = GetEmailAddress(recipient2)
        If InStr(recipientAddress, senderHost) <> 0 And InStr(recipientAddress, "+") = 0 Then
            recipientsToRemoveIndices.Add (recipient2.Index)
        End If
    Next
    
    Dim index2 As Integer
      
    For i = 1 To tripCodes.Count
        tripCode = tripCodes(i)
        Set recipient2 = mailItem2.ReplyRecipients.Add(AddTripCode(senderEmailAddress, tripCode))
        recipient2.Resolve
        
        Dim j As Integer
        
        For j = 1 To recipientsToRemoveIndices.Count
            index2 = recipientsToRemoveIndices(j)
            Set recipient2 = mailItem2.Recipients(index2)
            Dim recipient3 As Recipient
            Set recipient3 = mailItem2.Recipients.Add(AddTripCode(GetEmailAddress(recipient2), tripCode))
            recipient3.Type = recipient2.Type
            recipient3.Resolve
        Next
    Next
    
    For i = recipientsToRemoveIndices.Count To 1 Step -1
        index2 = recipientsToRemoveIndices(i)
        mailItem2.Recipients.Remove (index2)
    Next
   
    Cancel = False
    Exit Sub
    
ErrorHandler:
    MsgBox "The email was not sent. Error: " + Err.Description, vbCritical
End Sub

Private Function AddTripCode(ByVal email As String, ByVal tripCode As String) As String
    AddTripCode = Replace(email, "@", "+" + tripCode + "@")
End Function

Private Function ExtractTripCodes(ByVal str As String) As Collection
    Set ExtractTripCodes = New Collection
    Dim regex As RegExp
    Set regex = New RegExp
    regex.Pattern = "\bE\d{4}\b"
    regex.Global = True
    Dim matches As MatchCollection
    Set matches = regex.Execute(str)
    Dim i As Integer
    For i = 0 To matches.Count - 1
        Dim tripCode As String
        tripCode = matches(i).Value
        On Error Resume Next
        ExtractTripCodes.Add tripCode, tripCode
        On Error GoTo 0
    Next
End Function

Private Function GetEmailAddress(ByVal recipient2 As Recipient) As String
    If recipient2.AddressEntry.Type = "EX" Then
        Dim exchangeUser2 As ExchangeUser
        Set exchangeUser2 = recipient2.AddressEntry.GetExchangeUser()
        
        If Not exchangeUser2 Is Nothing Then
            GetEmailAddress = exchangeUser2.PrimarySmtpAddress
        Else
            On Error Resume Next
            GetEmailAddress = recipient2.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x39FE001F")
            Exit Function
        End If
    Else
        GetEmailAddress = recipient2.Address
    End If
End Function