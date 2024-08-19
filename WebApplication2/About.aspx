<%@ Page Title="About" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.vb" Inherits="WebApplication2.About" %>

<asp:Content ID="Content_Upload_File" ContentPlaceHolderID="MainContent" Runat="Server">
	<link rel="stylesheet" href="https://code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
	<link href="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.css" rel="stylesheet"/>
	<script src="https://code.jquery.com/jquery-1.8.2.js"></script>
	<script src="https://code.jquery.com/ui/1.8.24/jquery-ui.js"></script>

	<br />
	<h3><div style="font-size: 28px">Upload Blocking Data</div></h3>

	<div class="body-right">
		<asp:Label ID="Label_Output" runat="server" />
		<div class="body-formulir" style="padding:20px;">
			<div class="container mt-5" id="main">
				<form method="post">
					<table border="0" width="100%" cellpadding="3">
						<tbody>
                            <tr>
                                <td width="150px"><asp:Label ID="Label_Upload_File" runat="server" autocomplete="off">Blocking Data File</asp:Label></td>
                                <td>
                                    <asp:FileUpload ID="UploadFile" runat="server" accept=".txt" />
                                </td>
                            </tr>
                            <tr>
                                <td width="150px"></td>
                                <td>
                                    <asp:Button ID="Btn_Reset" runat="server" Text="Reset" class="button" OnClick="Btn_Reset_Click" />
                                    <asp:Button ID="Btn_Submit" runat="server" Text="Submit" class="button" OnClick="Btn_Submit_Click" />
                                </td>
                            </tr>
						</tbody>
					</table>
				</form>
			</div>
		</div>
	</div>

	<script src="https://cdnjs.cloudflare.com/ajax/libs/toastr.js/latest/toastr.min.js"></script>
</asp:Content>