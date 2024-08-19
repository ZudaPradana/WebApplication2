<%@ Page Title="Contact" Language="VB" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.vb" Inherits="WebApplication2.Contact" %>

<asp:Content ID="Content_Add_New_Contract" ContentPlaceHolderID="MainContent" Runat="Server">
    <style type="text/css">
        .hiddencol
        {
            display: none;
        }

            .person-to-group, .email-group, .address-group {
        display: flex;
        align-items: center;
        margin-bottom: 5px;
    }

    .addPersonToButton {
        margin-left: 10px;
        height: 27px;
        width: 30px;
        background-color: #007bff;
        color: white;
        border: none;
        cursor: pointer;
    }

    #personToContainer, #emailContainer, #addressContainer {
        position: relative;
            display: flex;
    flex-direction: column;
    }
    </style>
	<link rel="stylesheet" href="https://code.jquery.com/ui/1.8.24/themes/base/jquery-ui.css">
	<script src="https://code.jquery.com/jquery-1.8.2.js"></script>
	<script src="https://code.jquery.com/ui/1.8.24/jquery-ui.js"></script>

    <br />
	<h3><div style="font-size: 28px">Add New Contact</div></h3>


	<div class="body-right">	
        <div class="body-formulir" style="padding:20px;">
		    <div class="container mt-5" id="main">
			    <form method="post" id="contactForm" >
                    <asp:HiddenField ID="Contact_ID" runat="server" />
			        <table border="0" width="100%" cellpadding="3">
				        <tbody>
                            <tr>
                                <td width="150px"><asp:Label ID="Label_Recipient" runat="server" autocomplete="off">Recipient</asp:Label></td>
                                <td>
                                    <asp:DropDownList ID="Recipient" runat="server" Height="27px" Width="260px">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                            </tr>

                             <tr>
                                <td><asp:Label ID="Label_Member_Code" runat="server">Member Code</asp:Label></td>
                                <td>
                                    <asp:DropDownList ID="Member_Code" runat="server" Height="27px" Width="260px">
                                    </asp:DropDownList>
                                </td>
                                <td></td>
                             </tr>
                            <tr>
                                <td><asp:Label ID="Label_Person_To" runat="server" ClientIDMode="Static">Person To</asp:Label></td>
                                <td>
                                    <div id="personToContainer">
                                        <div class="person-to-group">
                                            <asp:DropDownList ID="Person_To_Salutation" ClientIDMode="Static" runat="server" Height="27px" Width="60px">
                                                <asp:ListItem Selected="True">Bapak</asp:ListItem>
                                                <asp:ListItem>Ibu</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:TextBox ID="Person_To_Name" runat="server" Width="195px" Height="24px"></asp:TextBox>
                                            <button type="button" class="addPersonToButton" onclick="addPersonToField()">+</button>
                                        </div>
                                    </div>
                                </td>
                                <td></td>
                            </tr>


                             <tr>
                                <td><asp:Label ID="Label_Position" runat="server">Position</asp:Label></td>
                                <td><asp:TextBox ID="Position" runat="server" Width="260px" Height="24px"></asp:TextBox></td>
                                <td></td>
                             </tr>
                            <tr>
                                <td><asp:Label ID="Label_EMail" runat="server" ClientIDMode="Static">E-Mail</asp:Label></td>
                                <td>
                                    <div id="emailContainer">
                                        <div class="email-group">
                                            <asp:TextBox ID="EMail" runat="server" Width="260px" Height="24px"></asp:TextBox>
                                        </div>
                                    </div>
                                </td>
                                <td></td>
                            </tr>
                                                         <tr>
                                <td><asp:Label ID="Label_Company" runat="server">Company</asp:Label></td>
                                <td><asp:TextBox ID="Company" runat="server" Width="260px" Height="24px"></asp:TextBox></td>
                                <td></td>
                             </tr>
                            <tr>
                                <td><asp:Label ID="Label_Address_1" runat="server" ClientIDMode="Static">Address 1</asp:Label></td>
                                <td>
                                    <div id="addressContainer">
                                        <div class="address-group">
                                        <asp:TextBox ID="Address_1" runat="server" Width="260px" Height="24px"></asp:TextBox>
                                            </div>
                                    </div>
                                </td>
                                <td></td>
                            </tr>
                             <tr>
                                <td><asp:Label ID="Label_Address_2" runat="server" ClientIDMode="Static">Address 2</asp:Label></td>
                                <td><asp:TextBox ID="Address_2" runat="server" Width="260px" Height="24px"></asp:TextBox></td>
                                <td></td>
                             </tr>
                             <tr>
                                <td><asp:Label ID="Label_Address_3" runat="server" ClientIDMode="Static">Address 3</asp:Label></td>
                                <td><asp:TextBox ID="Address_3" runat="server" Width="260px" Height="24px"></asp:TextBox></td>
                                <td></td>
                             </tr>
                             <tr>
                                <td></td>
                                <td>
								    <asp:Button ID="Btn_Reset" runat="server" Text="Reset" class="button" onClick="Btn_Reset_Click"/>
								    <asp:Button ID="Btn_Save" runat="server" Text="Save" class="button" OnClick="Btn_Save_Click"/>
                                </td>
                                <td></td>
                             </tr>
				    </tbody>
			    </table>
			    </form>
		    </div>
	    </div>

		<div style="max-width: 100%; overflow: auto;">
            <br/>
			<asp:GridView CssClass="table table-bordered" ID="GV_New_Contact" runat="server" AllowPaging="true" PageSize="10" AutoGenerateColumns="false" CellPadding="4" ForeColor="#333333" OnRowCommand="Btn_Contact_RowCommand">
				<AlternatingRowStyle BackColor="White" />
				<FooterStyle BackColor="#990000" Font-Bold="True" />
				<HeaderStyle BackColor="#e8e4da" Font-Bold="True" />
				<PagerStyle BackColor="#e8e4da" ForeColor="#333333" HorizontalAlign="Center" />
				<SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
                <Columns>
                    <asp:BoundField DataField="ContactID" HeaderText="ID" ItemStyle-CssClass="hiddencol" HeaderStyle-CssClass="hiddencol" />
                    <asp:BoundField DataField="Recipient" HeaderText="Recipient" />
                    <asp:BoundField DataField="Member" HeaderText="Member" />
                    <asp:BoundField DataField="Salutation" HeaderText="Salutation" />
                    <asp:BoundField DataField="Name" HeaderText="Name" />
                    <asp:BoundField DataField="Position" HeaderText="Position" />
                    <asp:BoundField DataField="EMail" HeaderText="E-Mail" />
                    <asp:BoundField DataField="Company" HeaderText="Company" />
                    <asp:BoundField DataField="Address_1" HeaderText="Address 1" />
                    <asp:BoundField DataField="Address_2" HeaderText="Address 2" />
                    <asp:BoundField DataField="Address_3" HeaderText="Address 3" />
                    <asp:TemplateField HeaderText="Action" ItemStyle-Width="200px" ItemStyle-HorizontalAlign="center">
                        <ItemTemplate>
                            <asp:Button ID="EditButton" runat="server" Text="Edit" CommandName="EditCommand" CommandArgument='<%# Eval("ContactID") %>' CssClass="button" />
                            <asp:Button ID="DeleteButton" runat="server" Text="Delete" CommandName="DeleteCommand" CommandArgument='<%# Eval("ContactID") & "|" & Eval("Salutation") & "|" & Eval("Name") %>' OnClientClick="return confirm('Are you sure you want to delete this item?');" CssClass="button" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
			</asp:GridView>
		</div>

    </div>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            // Menambahkan event listener untuk tombol Reset
            var resetButton = document.getElementById('<%= Btn_Reset.ClientID %>');
            if (resetButton) {
                resetButton.addEventListener('click', function (e) {
                    e.preventDefault(); // Menghindari aksi default dari tombol

                    document.getElementById('<%= Contact_ID.ClientID %>').value = '';
            document.getElementById('<%= Recipient.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= Member_Code.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= Person_To_Salutation.ClientID %>').selectedIndex = 0;
            document.getElementById('<%= Person_To_Name.ClientID %>').value = '';
            document.getElementById('<%= Position.ClientID %>').value = '';
            document.getElementById('<%= EMail.ClientID %>').value = '';
            document.getElementById('<%= Company.ClientID %>').value = '';
            document.getElementById('<%= Address_1.ClientID %>').value = '';
            document.getElementById('<%= Address_2.ClientID %>').value = '';
            document.getElementById('<%= Address_3.ClientID %>').value = '';
        });
            } else {
                console.error('Button with ID "Btn_Reset" not found.');
            }
        });

        function addPersonToField() {
            var personToContainer = document.getElementById('personToContainer');
            var emailContainer = document.getElementById('emailContainer');
            var addressContainer = document.getElementById('addressContainer');

            var personToGroup = personToContainer.querySelector('.person-to-group');
            var newPersonToGroup = personToGroup.cloneNode(true);

            var newIndex = personToContainer.childElementCount + 1;

            // Update the name attribute for name field
            newPersonToGroup.querySelectorAll('input[type="text"]').forEach((input) => {
                input.value = '';
                input.name = `Person_To_Name_${newIndex}`; // Set name attribute for name field
            });

            // Update the name for salutation select field
            var salutationSelect = newPersonToGroup.querySelector('select');
            if (salutationSelect) {
                salutationSelect.name = `Person_To_Salutation_${newIndex}`; // Set name attribute for salutation field
            }

            personToContainer.appendChild(newPersonToGroup);

            var newEmailField = emailContainer.querySelector('input[type="text"]').cloneNode(true);
            newEmailField.value = '';
            newEmailField.name = `EMail_${newIndex}`; // Set name attribute
            emailContainer.appendChild(newEmailField);

            var newAddressField = addressContainer.querySelector('input[type="text"]').cloneNode(true);
            newAddressField.value = '';
            newAddressField.name = `Address_1_${newIndex}`; // Set name attribute
            addressContainer.appendChild(newAddressField);

            // Log the entire container for verification
            console.log('personToContainer:', personToContainer.innerHTML);
            console.log('emailContainer:', emailContainer.innerHTML);
            console.log('addressContainer:', addressContainer.innerHTML);
        }





    </script>
    
    </asp:Content>
