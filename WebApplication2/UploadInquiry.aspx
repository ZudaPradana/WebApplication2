<%@ Page Title="Upload Inquiry" Language="vb" MasterPageFile="~/Site.Master" AutoEventWireup="false" CodeBehind="UploadInquiry.aspx.vb" Inherits="WebApplication2.UploadInquiry" %>

<asp:Content ID="Content_Inquiry_Approval" ContentPlaceHolderID="MainContent" Runat="Server">
    <style type="text/css">
        .hiddencol
        {
            display: none;
        }
    </style>
	<link rel="stylesheet" href="https://code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
	<script src="https://code.jquery.com/jquery-1.8.2.js"></script>
	<script src="https://code.jquery.com/ui/1.8.24/jquery-ui.js"></script>

	<br />
	<h3><div style="font-size: 28px">Inquiry Hasil Upload</div></h3>

    <div class="body-right">
	<div class="body-formulir" style="padding:20px;">
		<div class="container mt-5" id="main">
            <br />
			<table>
				<tr>
                    <td><asp:TextBox ID="Search" runat="server" Width="200px" Height="24px"></asp:TextBox></td>
                    <td><asp:Button ID="Btn_Search" runat="server" Text="Search" class="button" OnClick="Btn_Search_Click"/></td>
                </tr>
            </table>
            <br />
            <div style="max-width: 1024px; overflow: auto;">
                <asp:GridView CssClass="table table-bordered" ID="GV_Upload_Inquiry" runat="server" AllowPaging="true" PageSize="10" AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" OnRowCommand="Btn_InquiryUpload_RowCommand">
                <AlternatingRowStyle BackColor="White" />
                    <FooterStyle BackColor="#990000" Font-Bold="True" />
                    <HeaderStyle BackColor="#e8e4da" Font-Bold="True" />
                    <PagerStyle BackColor="#e8e4da" ForeColor="#333333" HorizontalAlign="Center" />
                    <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                    <Columns>
                        <asp:BoundField DataField="UploadLogID" HeaderText="UploadLogID" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                        <asp:BoundField DataField="FileName" HeaderText="File Name" />
                        <asp:BoundField DataField="Success" HeaderText="Success" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="right" />
                        <asp:BoundField DataField="Failed" HeaderText="Failed" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="right" />
                        <asp:BoundField DataField="UploadStatus" HeaderText="Upload Status" />
                        <asp:BoundField DataField="UploadTime" HeaderText="Upload Time" />
                        <asp:BoundField DataField="Timestamp" HeaderText="Timestamp" />
                        <asp:TemplateField HeaderText="Result" ItemStyle-Width="120px" ItemStyle-HorizontalAlign="center">
                            <ItemTemplate>
                                <asp:Button ID="DownloadButton" runat="server" Text="Download" CommandName="DownloadCommand" CommandArgument='<%# Eval("UploadLogID") & "|" & Eval("Filename")%>' OnClientClick="return confirm('Are you sure you want to download this item?');" CssClass="button" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        </Columns>
                </asp:GridView>
            </div>
		</div>
	</div>
    </div>

</asp:Content>
