<%@ Page Title="Download Data" Language="vb" MasterPageFile="~/Site.Master"  AutoEventWireup="false" CodeBehind="Download.aspx.vb" Inherits="WebApplication2.Download" %>

<asp:Content ID="ContentHeader" ContentPlaceHolderID="HeaderContent" Runat="Server">
    <h2>Download Blocking Log </h2>
    
    <link href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" rel="stylesheet" />
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/@popperjs/core@2.5.4/dist/umd/popper.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
</asp:Content>
<asp:Content ID="Content_Download_Blocking_Log" ContentPlaceHolderID="MainContent" Runat="Server">
    
    <form id="form1">
        
        <div class="container mt-4">
            <div class="row mb-3">
                <%--
                <div class="col">
                    <label>Requester</label>
                    <asp:DropDownList ID="Requester" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">All</asp:ListItem>
                    </asp:DropDownList>
                </div> --%>
                <div class="col">
                    <label>SID</label>
                    <asp:TextBox ID="txtSID" runat="server" CssClass="form-control" placeholder="SID"></asp:TextBox>
                </div>
                <div class="col">
                    <label>Account Number</label>
                    <asp:TextBox ID="txtAccountNumber" runat="server" CssClass="form-control" placeholder="Account Number"></asp:TextBox>
                </div>
                <div class="col">
                    <label>Restriction Reason</label>
                    <asp:DropDownList ID="RestrictionReason" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">All</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col">
                    <label>Registered Date (From)</label>
                    <asp:TextBox ID="txtRegisteredDateFrom" runat="server" CssClass="form-control" type="date"></asp:TextBox>
                </div>
                <div class="col">
                    <label>Registered Date (To)</label>
                    <asp:TextBox ID="txtRegisteredDateTo" runat="server" CssClass="form-control" type="date"></asp:TextBox>
                </div>
            </div>
            <div class="row mb-3">
                <div class="col">
                    <label>Event Name</label>
                    <asp:TextBox ID="txtEventName" runat="server" CssClass="form-control" placeholder="Event Name"></asp:TextBox>
                </div>
                <div class="col">
                    <label>Ref. Number</label>
                    <asp:TextBox ID="txtRefNumber" runat="server" CssClass="form-control" placeholder="Ref. Number"></asp:TextBox>
                </div>
                <div class="col">
                    <label>Case Name</label>
                    <asp:TextBox ID="txtCaseName" runat="server" CssClass="form-control" placeholder="Case Name"></asp:TextBox>
                </div>
                <div class="col">
                    <label>Instrument</label>
                    <asp:TextBox ID="txtInstrument" runat="server" CssClass="form-control" placeholder="Instrument"></asp:TextBox>
                </div>
                
                <div class="col">
                    <label>Closing Price Date</label>
                    <asp:TextBox ID="txtClosingPriceDate" runat="server" CssClass="form-control" type="date"></asp:TextBox>
                </div>
            </div>
            <div class="row mb-3">
                <div class="col">
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="btn btn-light" OnClick="btnReset_Click" />
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-danger" OnClick="btnSearch_Click" />
                </div>
            </div>

            <div class="container mt-4">
                <!-- Tempat untuk Alert -->
                <asp:Panel ID="AlertPanel" runat="server" Visible="False" CssClass="alert alert-danger alert-dismissible fade show" role="alert">
                    <asp:Label ID="AlertMessage" runat="server" Text=""></asp:Label>
                    <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
                </asp:Panel>
                <!-- Form Anda di sini -->
            </div>


            
            <div class="row mb-3">
                <div class="col">

                    <asp:LinkButton ID="LinkButton1" runat="server" CssClass="btn btn-outline-secondary" OnClick="btnDownloadXLS_Click">
                        <svg xmlns="http://www.w3.org/2000/svg" width="25" height="25" fill="currentColor" class="bi bi-filetype-xls" viewBox="0 0 16 16">
                            <path fill-rule="evenodd" d="M14 4.5V14a2 2 0 0 1-2 2h-1v-1h1a1 1 0 0 0 1-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5zM6.472 15.29a1.2 1.2 0 0 1-.111-.449h.765a.58.58 0 0 0 .254.384q.106.073.25.114.143.041.319.041.246 0 .413-.07a.56.56 0 0 0 .255-.193.5.5 0 0 0 .085-.29.39.39 0 0 0-.153-.326q-.152-.12-.462-.193l-.619-.143a1.7 1.7 0 0 1-.539-.214 1 1 0 0 1-.351-.367 1.1 1.1 0 0 1-.123-.524q0-.366.19-.639.19-.272.527-.422.338-.15.777-.149.457 0 .78.152.324.153.5.41.18.255.2.566h-.75a.56.56 0 0 0-.12-.258.6.6 0 0 0-.247-.181.9.9 0 0 0-.369-.068q-.325 0-.513.152a.47.47 0 0 0-.184.384q0 .18.143.3a1 1 0 0 0 .405.175l.62.143q.326.075.566.211a1 1 0 0 1 .375.358q.135.222.135.56 0 .37-.188.656a1.2 1.2 0 0 1-.539.439q-.351.158-.858.158-.381 0-.665-.09a1.4 1.4 0 0 1-.478-.252 1.1 1.1 0 0 1-.29-.375m-2.945-3.358h-.893L1.81 13.37h-.036l-.832-1.438h-.93l1.227 1.983L0 15.931h.861l.853-1.415h.035l.85 1.415h.908L2.253 13.94zm2.727 3.325H4.557v-3.325h-.79v4h2.487z"/>
                        </svg>
                    </asp:LinkButton>

                    <asp:LinkButton ID="LinkButton2" runat="server" CssClass="btn btn-outline-secondary" OnClick="btnDownloadTXT_Click">
                        <svg xmlns="http://www.w3.org/2000/svg" width="25" height="25" fill="currentColor" class="bi bi-filetype-txt" viewBox="0 0 16 16">
                            <path fill-rule="evenodd" d="M14 4.5V14a2 2 0 0 1-2 2h-2v-1h2a1 1 0 0 0 1-1V4.5h-2A1.5 1.5 0 0 1 9.5 3V1H4a1 1 0 0 0-1 1v9H2V2a2 2 0 0 1 2-2h5.5zM1.928 15.849v-3.337h1.136v-.662H0v.662h1.134v3.337zm4.689-3.999h-.894L4.9 13.289h-.035l-.832-1.439h-.932l1.228 1.983-1.24 2.016h.862l.853-1.415h.035l.85 1.415h.907l-1.253-1.992zm1.93.662v3.337h-.794v-3.337H6.619v-.662h3.064v.662H8.546Z"/>
                        </svg>
                    </asp:LinkButton>

                </div>
            </div>
            <div class="table-responsive">
                <asp:GridView ID="GridView1" runat="server" CssClass="table table-striped table-bordered" AutoGenerateColumns="False" AllowPaging="True" PageSize="10" OnPageIndexChanging="GridView1_PageIndexChanging">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="No" />
                        <asp:BoundField DataField="SID" HeaderText="SID" />
                        <asp:BoundField DataField="HolderName" HeaderText="Holder Name" />
                        <asp:BoundField DataField="AccountName" HeaderText="Account Name" />
                        <asp:BoundField DataField="AccountNumber" HeaderText="Account Number" />
                        <asp:BoundField DataField="RestrictionReason" HeaderText="Restriction Reason" />
                        <asp:BoundField DataField="Reference" HeaderText="Reference" />
                        <asp:BoundField DataField="RestrictionStatus" HeaderText="Restriction Status" />
                        <asp:BoundField DataField="RestrictWholeAccount" HeaderText="Restrict Whole Account" />
                        <asp:BoundField DataField="RegisteredDate" HeaderText="Registered Date" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="ReleaseDate" HeaderText="Release Date" DataFormatString="{0:yyyy-MM-dd}" />
                        <asp:BoundField DataField="EventName" HeaderText="Event Name" />
                        <asp:BoundField DataField="CaseName" HeaderText="Case Name" />
                        <asp:BoundField DataField="CaseID" HeaderText="Case ID" />
                        <asp:BoundField DataField="Notes" HeaderText="Notes" />
                        <asp:BoundField DataField="Instrument" HeaderText="Instrument" />
                        <asp:BoundField DataField="AssetBlocked" HeaderText="Asset Blocked" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="MarketValue" HeaderText="Market Value" DataFormatString="{0:C}" />
                        <asp:BoundField DataField="ClosingPriceDate" HeaderText="Closing Price Date" DataFormatString="{0:yyyy-MM-dd}" />
                    </Columns>
                    <FooterStyle CssClass="bg-light" />
                </asp:GridView>
            </div>

        </div>
    </form>
    <script>
        $(document).ready(function () {
        });
    </script>
</asp:Content>

