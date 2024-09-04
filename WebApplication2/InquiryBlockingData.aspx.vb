Imports System.Security.Cryptography
Imports MySql.Data.MySqlClient

Public Class InquiryBlockingData
    Inherits Page
    Private connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("MyDbConnectionString").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Using connection As New MySqlConnection(connectionString)
                connection.Open()
                If Not IsPostBack Then
                    BindGrid()
                    GetPrepareRequester()
                End If
            End Using
        Catch ex As Exception
            ' Jika koneksi gagal, tampilkan pesan error
            Response.Write("Connection failed: " & ex.Message)
        End Try
    End Sub


    Protected Sub GetPrepareRequester()
        ' Get Fill Requester
        ' Parameter : -
        ' Return    : -
        'Requester.Items.Clear()
        Requester.Items.Add("Authority - Kejaksaan")
        Requester.Items.Add("Authority - Kepolisian")
        Requester.Items.Add("Authority - KPK")
        Requester.Items.Add("Authority - Pengadilan Tinggi")
        Requester.Items.Add("Authority - PPATK")
        Requester.Items.Add("Authority - DJP")
        Requester.Items.Add("OJK")
        Requester.Items.Add("KSEI")
        Requester.Items.Add("IDX")
        Requester.Items.Add("KPEI")
        Requester.SelectedValue = 0

    End Sub

    ' Fungsi untuk membangun query dan parameter berdasarkan filter
    Private Function GetFilteredCommand() As String
        Dim query As String = "SELECT EventName, Requester, c.CaseId, CaseName, RefNumber, Date FROM lb_blok_inquiry_blocking_data c join lb_blok_inquiry_blocking_data_details cd using(CaseID) WHERE 1=1"

        'Apply filters based on user input
        If Not String.IsNullOrEmpty(Requester.SelectedValue) Then
            query &= " AND Requester LIKE '%" & Requester.SelectedValue.Trim() & "%'"
        End If
        If Not String.IsNullOrEmpty(TxtEventName.Text) Then
            query &= " AND EventName LIKE '%" & TxtEventName.Text.Trim() & "%'"
        End If

        If Not String.IsNullOrEmpty(txtCaseName.Text) Then
            query &= " AND CaseName LIKE '%" & txtCaseName.Text.Trim() & "%'"
        End If

        If Not String.IsNullOrEmpty(txtCaseId.Text) Then
            query &= " AND CaseID LIKE '%" & txtCaseId.Text.Trim() & "%'"
        End If

        If Not String.IsNullOrEmpty(txtRefNumber.Text) Then
            query &= " AND RefNumber LIKE '%" & txtRefNumber.Text.Trim() & "%'"
        End If

        ' Registered Date range filter
        If Not String.IsNullOrEmpty(txtDateFrom.Text) Then
            query &= " AND Date >= '" & txtDateFrom.Text & "'"
        End If

        If Not String.IsNullOrEmpty(txtDateTo.Text) Then
            query &= " AND Date <= '" & txtDateTo.Text & "'"
        End If

        'Closing Price Date filter
        If Not String.IsNullOrEmpty(txtClosingPriceDate.Text) Then
            query &= " AND cd.ClosingPriceDate = '" & txtClosingPriceDate.Text & "'"
        End If

        Return query

    End Function

    Private Sub BindGrid()
        Using con As New MySqlConnection(connectionString)
            Dim cmd As New MySqlCommand()
            cmd.CommandText = GetFilteredCommand()
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
        If String.IsNullOrEmpty(txtCaseId.Text.Trim()) AndAlso
            String.IsNullOrEmpty(Requester.SelectedValue.Trim()) AndAlso
            String.IsNullOrEmpty(txtDateFrom.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtDateTo.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtRefNumber.Text.Trim()) AndAlso
            String.IsNullOrEmpty(TxtEventName.Text.Trim()) AndAlso
            String.IsNullOrEmpty(txtCaseName.Text.Trim()) Then
            ShowAlert("Please fill in at least one field other than 'Closing Price Date' before searching.")
            Exit Sub
        End If

        ' Validasi bahwa jika "Registered Date From" diisi, "Registered Date From" tidak boleh lebih lama dari "Registered Date From"
        If Not String.IsNullOrEmpty(txtDateFrom.Text) AndAlso Not String.IsNullOrEmpty(txtDateTo.Text) Then
            Dim registeredDateFrom As DateTime = DateTime.Parse(txtDateFrom.Text)
            Dim registeredDateTo As DateTime = DateTime.Parse(txtDateTo.Text)
            If registeredDateTo < registeredDateFrom Then
                ShowAlert("Registered Date (to) cannot be earlier than Registerd Date From")
                Exit Sub
            End If
        End If

        ' Validasi bahwa jika "Registered Date From" diisi, "Closing Price Date" tidak boleh lebih lama dari "Registered Date From"
        If Not String.IsNullOrEmpty(txtDateFrom.Text) AndAlso Not String.IsNullOrEmpty(txtClosingPriceDate.Text) Then
            Dim registeredDateFrom As DateTime = DateTime.Parse(txtDateFrom.Text)
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
        Requester.SelectedIndex = 0
        txtCaseId.Text = ""
        TxtEventName.Text = ""
        txtDateFrom.Text = ""
        txtDateTo.Text = ""
        txtClosingPriceDate.Text = ""
        txtRefNumber.Text = ""
        txtCaseName.Text = ""

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
            Dim cmd As New MySqlCommand()
            cmd.CommandText = GetFilteredCommand()
            cmd.Connection = con
            con.Open()

            Dim reader As MySqlDataReader = cmd.ExecuteReader()
            Dim columns As String() = {"No", "EventName", "Requester", "CaseID", "CaseName", "RefNumber", "Date"}

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
                        sb.AppendLine("<td>" & reader(i).ToString() & "</td>")
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
        Dim fileName As String = "Inquiry_BlockingData." & fileType
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

    Protected Sub GridView1_RowCommand(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand
        If e.CommandName = "ViewDetails" Then
            Dim caseId As String = e.CommandArgument.ToString()
            Response.Redirect("DetailInquiryBlockingData.aspx?CaseID=" & caseId)
        End If
    End Sub


End Class