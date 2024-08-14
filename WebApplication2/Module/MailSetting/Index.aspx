<%@ Page Language="VB" AutoEventWireup="true" CodeBehind="Index.aspx.vb" Inherits="WebApplication2.Module.MailSetting.Index" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="HeaderContent" Runat="Server">
    <ul class="breadcrumb no-list clearfix">
        <li><a href="../Index.aspx"><strong>Logbook Pemblokiran</strong></a></li>
        <li><a href="Index.aspx"><strong>Mail Setting</strong></a></li>
    </ul>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" Runat="Server">
    <style type="text/css">
        .auto-style1 {
            width: 199px;
        }
        .auto-style2 {
            width: 23px;
        }
    </style>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/jquery-1.8.2.js"></script>
    <script src="https://code.jquery.com/ui/1.8.24/jquery-ui.js"></script>

    <div class="body-right">
        <div class="body-formulir" style="padding:20px;">
            <div class="container mt-5" id="main">
                <h3>Mail Setting</h3><br />
                <ul>
                    <li><a href="SignatureSetup.aspx"><strong>Signature Setup</strong></a></li>
                    <li><a href="AddNewContact.aspx"><strong>Add New Contact</strong></a></li>
                    <li><a href="UploadFile.aspx"><strong>Upload File</strong></a></li>
                    <li><a href="MailLog.aspx"><strong>Mail Log</strong></a></li>
                    <li><a href="MailTemplate.aspx"><strong>Mail Template</strong></a></li>
                </ul>			
            </div>
        </div>
    </div>
</asp:Content>
