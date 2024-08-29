<%@ Page Title="Detail Inquiry Blocking Data" MasterPageFile="~/Site.Master" Language="vb" AutoEventWireup="false" CodeBehind="DetailInquiryBlockingData.aspx.vb" Inherits="WebApplication2.DetailInquiryBlockingData" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="HeaderContent" Runat="Server">
    <h2>Download Blocking Log </h2>
    
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>

<asp:Content ID="Content_Download_Blocking_Log" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <div class="container mt-5">
        <h2>Case Details</h2>
        <asp:DetailsView ID="DetailsView1" runat="server" AutoGenerateRows="False" CssClass="table table-bordered">
            <Fields>
                <asp:BoundField DataField="SID" HeaderText="SID" />
                <asp:BoundField DataField="HolderName" HeaderText="Holder Name" />
                <asp:BoundField DataField="AccountName" HeaderText="Account Name" />
                <asp:BoundField DataField="AccountNumber" HeaderText="Account Number" />
                <asp:BoundField DataField="RestrictionReason" HeaderText="Restriction Reason" />
                <asp:BoundField DataField="Reference" HeaderText="Reference" />
                <asp:BoundField DataField="RestrictionStatus" HeaderText="Restriction Status" />
                <asp:CheckBoxField DataField="RestrictWholeAccount" HeaderText="Restrict Whole Account" />
                <asp:BoundField DataField="RegisteredDate" HeaderText="Registered Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="ReleaseDate" HeaderText="Release Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="Instrument" HeaderText="Instrument" />
                <asp:BoundField DataField="AssetBlocked" HeaderText="Asset Blocked" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="ClosingPrice" HeaderText="Closing Price" DataFormatString="{0:N2}" />
                <asp:BoundField DataField="ClosingPriceDate" HeaderText="Closing Price Date" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="MarketValue" HeaderText="Market Value" DataFormatString="{0:N2}" />
            </Fields>
        </asp:DetailsView>
    </div>

</asp:Content>
