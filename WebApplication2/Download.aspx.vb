Imports MySql.Data.MySqlClient
Imports Org.BouncyCastle.Asn1.Ocsp

Public Class Download
    Inherits Page
    Private connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("MyDbConnectionString").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                If Not IsPostBack Then
                    BindGrid()
                    'GetPrepareRequester()
                    GetPrepareRestriction()
                End If
            End Using
        Catch ex As Exception
            ' Jika koneksi gagal, tampilkan pesan error
            Response.Write("Connection failed: " & ex.Message)
        End Try
    End Sub


    'Protected Sub GetPrepareRequester()
    '    ' Get Fill Requester
    '    ' Parameter : -
    '    ' Return    : -
    '    Dim sSelect As String = "-- Select --"
    '    Requester.Items.Clear()
    '    Requester.Items.Add(sSelect)
    '    Requester.Items.Add("Authority - Kejaksaan")
    '    Requester.Items.Add("Authority - Kepolisian")
    '    Requester.Items.Add("Authority - KPK")
    '    Requester.Items.Add("Authority - Pengadilan Tinggi")
    '    Requester.Items.Add("Authority - PPATK")
    '    Requester.Items.Add("Authority - DJP")
    '    Requester.Items.Add("OJK")
    '    Requester.Items.Add("KSEI")
    '    Requester.Items.Add("IDX")
    '    Requester.Items.Add("KPEI")
    '    Requester.SelectedValue = sSelect
    'End Sub

    Protected Sub GetPrepareRestriction()
        ' Get Fill Restriction
        ' Parameter : -
        ' Return    : -
        RestrictionReason.Items.Add("Blocked by Authority – Kejaksaan")
        RestrictionReason.Items.Add("Blocked By Authority - Kepolisian")
        RestrictionReason.Items.Add("Blocked By Authority - KPK")
        RestrictionReason.Items.Add("Blocked By Authority - Pengadilan Tinggi")
        RestrictionReason.Items.Add("Blocked By Authority - PPATK")
        RestrictionReason.Items.Add("Blocked By Authority - DJP")
        RestrictionReason.Items.Add("Blocked By OJK")
        RestrictionReason.Items.Add("Blocked By KSEI")
        RestrictionReason.Items.Add("Blocked By IDX")
        RestrictionReason.Items.Add("Blocked By KPEI")
        RestrictionReason.SelectedValue = 0
    End Sub

    ' Fungsi untuk membangun query dan parameter berdasarkan filter
    Private Function GetFilteredCommand() As MySqlCommand
        Dim query As String = "SELECT ID, SID, HolderName, AccountName, AccountNumber, RestrictionReason, Reference, RestrictionStatus, RestrictWholeAccount, RegisteredDate, ReleaseDate, EventName, CaseName, CaseID, Notes, Instrument, AssetBlocked, MarketValue, ClosingPriceDate FROM lb_blok_blocking_report WHERE 1=1"
        Dim cmd As New MySqlCommand()

        ' Apply filters based on user input
        'If Not String.IsNullOrEmpty(Requester.SelectedValue) Then
        '    query &= " AND Requester = @Requester"
        '    cmd.Parameters.AddWithValue("@Requester", Requester.SelectedValue)
        'End If

        If Not String.IsNullOrEmpty(txtSID.Text) Then
            query &= " AND SID LIKE @SID"
            cmd.Parameters.AddWithValue("@SID", "%" & txtSID.Text.Trim() & "%")
        End If

        If Not String.IsNullOrEmpty(txtAccountNumber.Text) Then
            query &= " AND AccountNumber LIKE @AccountNumber"
            cmd.Parameters.AddWithValue("@AccountNumber", "%" & txtAccountNumber.Text.Trim() & "%")
        End If

        If Not String.IsNullOrEmpty(RestrictionReason.SelectedValue) Then
            query &= " AND LOWER(RestrictionReason) LIKE LOWER(@RestrictionReason)"
            cmd.Parameters.AddWithValue("@RestrictionReason", "%" & RestrictionReason.SelectedValue.Trim() & "%")
        End If


        If Not String.IsNullOrEmpty(txtRefNumber.Text) Then
            query &= " AND Reference LIKE @RefNumber"
            cmd.Parameters.AddWithValue("@RefNumber", "%" & txtRefNumber.Text.Trim() & "%")
        End If

        ' Registered Date range filter
        If Not String.IsNullOrEmpty(txtRegisteredDateFrom.Text) Then
            query &= " AND RegisteredDate >= @RegisteredDateFrom"
            cmd.Parameters.AddWithValue("@RegisteredDateFrom", txtRegisteredDateFrom.Text)
        End If

        If Not String.IsNullOrEmpty(txtRegisteredDateTo.Text) Then
            query &= " AND RegisteredDate <= @RegisteredDateTo"
            cmd.Parameters.AddWithValue("@RegisteredDateTo", txtRegisteredDateTo.Text)
        End If

        If Not String.IsNullOrEmpty(txtEventName.Text) Then
            query &= " AND EventName LIKE @EventName"
            cmd.Parameters.AddWithValue("@EventName", "%" & txtEventName.Text.Trim() & "%")
        End If

        If Not String.IsNullOrEmpty(txtCaseName.Text) Then
            query &= " AND CaseName LIKE @CaseName"
            cmd.Parameters.AddWithValue("@CaseName", "%" & txtCaseName.Text.Trim() & "%")
        End If

        If Not String.IsNullOrEmpty(txtInstrument.Text) Then
            query &= " AND Instrument LIKE @Instrument"
            cmd.Parameters.AddWithValue("@Instrument", "%" & txtInstrument.Text.Trim() & "%")
        End If

        ' Closing Price Date filter
        If Not String.IsNullOrEmpty(txtClosingPriceDate.Text) Then
            query &= " AND ClosingPriceDate = @ClosingPriceDate"
            cmd.Parameters.AddWithValue("@ClosingPriceDate", txtClosingPriceDate.Text)
        End If

        cmd.CommandText = query
        Return cmd
    End Function

    Private Sub BindGrid()
        Using con As New MySqlConnection(connectionString)
            Dim cmd As MySqlCommand = GetFilteredCommand()
            cmd.Connection = con

            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            GridView1.DataSource = dt
            GridView1.DataBind()
        End Using
    End Sub

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        AlertPanel.Visible = False

        ' Validasi bahwa setidaknya satu kolom harus diisi sebelum melakukan pencarian
        If String.IsNullOrEmpty(txtSID.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtAccountNumber.Text.Trim()) AndAlso
            String.IsNullOrEmpty(RestrictionReason.SelectedValue) AndAlso
            String.IsNullOrEmpty(txtRegisteredDateFrom.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtRegisteredDateTo.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtRefNumber.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtEventName.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtCaseName.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtInstrument.Text.Trim()) Then
            ShowAlert("Please fill in at least one field other than 'Closing Price Date' before searching.")
            Exit Sub
        End If

        ' Validasi bahwa jika "Registered Date From" diisi, "Registered Date From" tidak boleh lebih lama dari "Registered Date From"

        If String.IsNullOrEmpty(txtRegisteredDateFrom.Text) AndAlso Not String.IsNullOrEmpty(txtRegisteredDateTo.Text) Then
            ShowAlert("Fill the Registration date (from) first.")
            Exit Sub
        End If

        ' Validasi bahwa jika "Registered Date From" diisi, "Closing Price Date" tidak boleh lebih lama dari "Registered Date From"
        If Not String.IsNullOrEmpty(txtRegisteredDateFrom.Text) AndAlso Not String.IsNullOrEmpty(txtClosingPriceDate.Text) Then
            Dim registeredDateFrom As DateTime = DateTime.Parse(txtRegisteredDateFrom.Text)
            Dim closingPriceDate As DateTime = DateTime.Parse(txtClosingPriceDate.Text)

            If closingPriceDate < registeredDateFrom Then
                ShowAlert("Closing Price Date cannot be earlier than Registered Date From.")
                Exit Sub
            End If
        End If

        BindGrid()
    End Sub

    ' Fungsi untuk menampilkan alert
    Private Sub ShowAlert(message As String)
        AlertMessage.Text = message
        AlertPanel.Visible = True
    End Sub

    Protected Sub btnReset_Click(ByVal sender As Object, ByVal e As EventArgs)
        'Requester.SelectedValue = ""
        txtSID.Text = ""
        txtAccountNumber.Text = ""
        RestrictionReason.SelectedIndex = 0
        txtRegisteredDateFrom.Text = ""
        txtRegisteredDateTo.Text = ""
        txtClosingPriceDate.Text = ""
        txtRefNumber.Text = ""
        txtEventName.Text = ""
        txtCaseName.Text = ""
        txtInstrument.Text = ""

        AlertPanel.Visible = False

        BindGrid()
    End Sub

    Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        GridView1.PageIndex = e.NewPageIndex
        BindGrid()
    End Sub

    Protected Sub btnDownloadCSV_Click(ByVal sender As Object, ByVal e As EventArgs)
        ExportToFile("csv")
    End Sub
    Protected Sub btnDownloadXLS_Click(ByVal sender As Object, ByVal e As EventArgs)
        ExportToFile("xls")
    End Sub

    Protected Sub btnDownloadTXT_Click(ByVal sender As Object, ByVal e As EventArgs)
        ExportToFile("txt")
    End Sub

    Private Sub ExportToFile(fileType As String)
        Dim sb As New StringBuilder()
        Using con As New MySqlConnection(connectionString)
            Dim cmd As MySqlCommand = GetFilteredCommand()
            cmd.Connection = con
            con.Open()

            Dim reader As MySqlDataReader = cmd.ExecuteReader()
            Dim columns As String() = {"No", "SID", "HolderName", "AccountName", "AccountNumber", "RestrictionReason", "Reference", "RestrictionStatus", "RestrictWholeAccount", "RegisteredDate", "ReleaseDate", "EventName", "CaseName", "CaseID", "Notes", "Instrument", "AssetBlocked", "MarketValue", "ClosingPriceDate"}

            If fileType = "xls" Then
                ' Membuat header tabel untuk Excel
                sb.AppendLine("<table border='1'>")
                sb.AppendLine("<tr>")
                For Each column As String In columns
                    sb.AppendLine("<th>" & column & "</th>")
                Next
                sb.AppendLine("</tr>")

                ' Menambahkan baris data
                Dim index As Integer = 1
                While reader.Read()
                    sb.AppendLine("<tr>")
                    sb.AppendLine("<td>" & index & "</td>")

                    For i As Integer = 0 To reader.FieldCount - 1
                        If Not i = 0 Then
                            sb.AppendLine("<td>" & reader(i).ToString() & "</td>")
                        End If

                    Next
                    sb.AppendLine("</tr>")
                    index = index + 1
                End While
                sb.AppendLine("</table>")
            Else
                ' Menangani ekspor untuk csv atau txt
                sb.AppendLine(String.Join(If(fileType = "csv", ",", "|"), columns))

                Dim index As Integer = 1
                While reader.Read()
                    Dim row As New List(Of String)
                    row.Add(index)

                    For i As Integer = 0 To reader.FieldCount - 1
                        If Not i = 0 Then
                            row.Add(reader(i).ToString())
                        End If
                    Next
                    sb.AppendLine(String.Join(If(fileType = "csv", ",", "|"), row))
                    index = index + 1

                End While
            End If
        End Using

        ' Mengirim file ke client
        Dim fileName As String = "Download_Data." & fileType
        Response.Clear()
        If fileType = "csv" Then
            Response.ContentType = "text/csv"
        ElseIf fileType = "xls" Then
            Response.ContentType = "application/vnd.ms-excel"
        Else
            Response.ContentType = "text/plain"
        End If
        Response.AddHeader("content-disposition", "attachment;filename=" & fileName)
        Response.Write(sb.ToString())
        Response.End()
    End Sub


End Class
