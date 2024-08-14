Imports System.Data.SqlClient
Imports MySql.Data.MySqlClient
Imports System.Web.Configuration

Public Class Contact
    Inherits Page

    Private connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("MyDbConnectionString").ConnectionString
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ' Initialize page, e.g., load dropdowns or data
            PopulateRecipientDropDown()
            PopulateMemberCodeDropDown()
        End If

        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                ' Jika koneksi berhasil, tampilkan pesan sukses
                Response.Write("Connection successful!")
            End Using
        Catch ex As Exception
            ' Jika koneksi gagal, tampilkan pesan error
            Response.Write("Connection failed: " & ex.Message)
        End Try
    End Sub

    Private Sub PopulateRecipientDropDown()
        Dim recipientList As New List(Of ListItem) From {
            New ListItem("OJK", "OJK"),
            New ListItem("KPK", "KPK"),
            New ListItem("DJP", "DJP"),
            New ListItem("Kepolisian", "Kepolisian"),
            New ListItem("Kejaksaan", "Kejaksaan"),
            New ListItem("Pengadilan Tinggi", "Pengadilan Tinggi"),
            New ListItem("PPATK", "PPATK"),
            New ListItem("IDX", "IDX"),
            New ListItem("KPEI", "KPEI"),
            New ListItem("Participant", "Participant"),
            New ListItem("Others", "Others")
        }

        Recipient.Items.AddRange(recipientList.ToArray())
    End Sub

    Private Sub PopulateMemberCodeDropDown()
        Dim memberCodeList As New List(Of ListItem) From {
        New ListItem("01", "01"),
        New ListItem("02", "02"),
        New ListItem("03", "03"),
        New ListItem("04", "04"),
        New ListItem("05", "05"),
        New ListItem("06", "06"),
        New ListItem("07", "07"),
        New ListItem("08", "08"),
        New ListItem("09", "09"),
        New ListItem("10", "10")
    }

        Member_Code.Items.AddRange(memberCodeList.ToArray())
    End Sub


    Protected Sub Btn_Reset_Click(sender As Object, e As EventArgs)
        ' Clear all the input fields
        Contact_ID.Value = String.Empty
        Recipient.SelectedIndex = 0
        Member_Code.SelectedIndex = 0
        Person_To_Salutation.SelectedIndex = 0
        Person_To_Name.Text = String.Empty
        Position.Text = String.Empty
        EMail.Text = String.Empty
        Company.Text = String.Empty
        Address_1.Text = String.Empty
        Address_2.Text = String.Empty
        Address_3.Text = String.Empty
    End Sub

    Protected Sub Btn_Save_Click(sender As Object, e As EventArgs)
        Using conn As New MySqlConnection(connectionString)
            conn.Open()

            Response.Write("Btn_Save_Click is called<br/>")

            ' Debug: Tampilkan semua kunci dan nilai di Request.Form
            For Each key As String In Request.Form.AllKeys
                Response.Write($"{key}: {Request.Form(key)}<br/>")
            Next

            ' Buat query SQL
            Dim query As String = "INSERT INTO LB_BLOK_CONTACT (Recipient, Member_Code, Salutation, Name, Position, EMail, Company, Address_1, Address_2, Address_3) " &
                              "VALUES (@Recipient, @Member_Code, @Salutation, @Name, @Position, @EMail, @Company, @Address_1, @Address_2, @Address_3)"

            ' Mengambil dan memproses data utama
            Dim salutation As String = Request.Form("ctl00$MainContent$Person_To_Salutation")
            Dim name As String = Request.Form("ctl00$MainContent$Person_To_Name")
            Dim email As String = Request.Form("ctl00$MainContent$EMail")
            Dim address1 As String = Request.Form("ctl00$MainContent$Address_1")

            Dim salutations() As String = If(String.IsNullOrEmpty(salutation), New String() {}, salutation.Split(","c))
            Dim names() As String = If(String.IsNullOrEmpty(name), New String() {}, name.Split(","c))
            Dim emails() As String = If(String.IsNullOrEmpty(email), New String() {}, email.Split(","c))
            Dim addresses1() As String = If(String.IsNullOrEmpty(address1), New String() {}, address1.Split(","c))

            ' Menyimpan entry pertama
            Using cmd As New MySqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@Recipient", Recipient.SelectedItem.Text)
                cmd.Parameters.AddWithValue("@Member_Code", Member_Code.SelectedItem.Text)
                cmd.Parameters.AddWithValue("@Salutation", If(salutations.Length > 0, salutations(0), String.Empty))
                cmd.Parameters.AddWithValue("@Name", If(names.Length > 0, names(0), String.Empty))
                cmd.Parameters.AddWithValue("@Position", Position.Text)
                cmd.Parameters.AddWithValue("@EMail", If(emails.Length > 0, emails(0), String.Empty))
                cmd.Parameters.AddWithValue("@Company", Company.Text)
                cmd.Parameters.AddWithValue("@Address_1", If(addresses1.Length > 0, addresses1(0), String.Empty))
                cmd.Parameters.AddWithValue("@Address_2", Address_2.Text)
                cmd.Parameters.AddWithValue("@Address_3", Address_3.Text)
                cmd.ExecuteNonQuery()
            End Using

            ' Menyimpan data tambahan
            Dim i As Integer = 2
            While True
                Dim salutationAdditional As String = Request.Form($"Person_To_Salutation_{i}")
                Dim nameAdditional As String = Request.Form($"Person_To_Name_{i}")
                Dim emailAdditional As String = Request.Form($"EMail_{i}")
                Dim address1Additional As String = Request.Form($"Address_1_{i}")

                If String.IsNullOrEmpty(nameAdditional) Then Exit While

                Using cmd As New MySqlCommand(query, conn)
                    cmd.Parameters.Clear()

                    cmd.Parameters.AddWithValue("@Recipient", Recipient.SelectedItem.Text)
                    cmd.Parameters.AddWithValue("@Member_Code", Member_Code.SelectedItem.Text)
                    cmd.Parameters.AddWithValue("@Salutation", If(Not String.IsNullOrEmpty(salutationAdditional), salutationAdditional, String.Empty))
                    cmd.Parameters.AddWithValue("@Name", If(Not String.IsNullOrEmpty(nameAdditional), nameAdditional, String.Empty))
                    cmd.Parameters.AddWithValue("@Position", Position.Text)
                    cmd.Parameters.AddWithValue("@EMail", If(Not String.IsNullOrEmpty(emailAdditional), emailAdditional, String.Empty))
                    cmd.Parameters.AddWithValue("@Company", Company.Text)
                    cmd.Parameters.AddWithValue("@Address_1", If(Not String.IsNullOrEmpty(address1Additional), address1Additional, String.Empty))
                    cmd.Parameters.AddWithValue("@Address_2", Address_2.Text)
                    cmd.Parameters.AddWithValue("@Address_3", Address_3.Text)

                    cmd.ExecuteNonQuery()
                End Using

                i += 1
            End While

            ' Menambahkan pesan sukses atau tindakan lain setelah penyimpanan
            ' Contoh: Response.Redirect("ConfirmationPage.aspx")
            ' Atau menambahkan notifikasi sukses di UI
        End Using
    End Sub









    Protected Sub Btn_Contact_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        ' Handle Row Commands for GridView, e.g., Edit or Delete
        If e.CommandName = "EditCommand" Then
            ' Load data into form for editing
        ElseIf e.CommandName = "DeleteCommand" Then
            ' Delete the selected contact
        End If
    End Sub


End Class
