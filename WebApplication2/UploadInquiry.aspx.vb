Imports System.IO
Imports Microsoft.VisualBasic.Logging
Imports MySql.Data.MySqlClient

Public Class UploadInquiry
    Inherits Page
    Private connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("MyDbConnectionString").ConnectionString
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindGridData()
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

    Protected Sub Btn_Search_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Button Search Handler
        ' Parameter : {Object} Objects
        ' Return    : -
        BindGridData()
    End Sub

    Private Sub BindGridData()
        ' Get Data From Database And Store To GridView
        ' Parameter : -
        ' Return    : -
        Using con As New MySqlConnection(connectionString)
            con.Open()
            Dim sSearch As String = Search.Text
            Dim sSqlQueryUploadLog As String = "SELECT UPLOADLOG_ID AS 'UploadLogID', FILENAME AS 'FileName', DATA_SUCCESS AS 'Success'," &
            " DATA_FAIL AS 'Failed', 'FINISHED' AS 'UpLoadStatus'," &
            " DATE_FORMAT(UPLOAD_TIME, '%d-%b-%Y %H:%i') AS 'UploadTime', DATE_FORMAT(LAST_UPDATE, '%d-%b-%Y %H:%i') AS 'TimeStamp'" &
            " FROM LB_BLOK_UPLOADLOG WHERE (1=1)"

            If (Not String.IsNullOrWhiteSpace(sSearch)) Then
                sSqlQueryUploadLog &= " AND (FILENAME LIKE '%" & sSearch & "%')"
            End If

            sSqlQueryUploadLog &= " ORDER BY DATE_FORMAT(LAST_UPDATE, '%Y%m%d %H%i') DESC"

            Using cmd As New MySqlCommand(sSqlQueryUploadLog, con)
                Dim dt As New DataTable()
                dt.Columns.AddRange(New DataColumn() {
                New DataColumn("UploadLogID"),
                New DataColumn("FileName"),
                New DataColumn("Success"),
                New DataColumn("Failed"),
                New DataColumn("UploadStatus"),
                New DataColumn("UploadTime"),
                New DataColumn("Timestamp")
                })

                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    dt.Load(reader)
                End Using

                GV_Upload_Inquiry.DataSource = dt
                GV_Upload_Inquiry.DataBind()
            End Using
        End Using
    End Sub

    Protected Sub Btn_InquiryUpload_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        ' Button On Grid View Handler
        ' Parameter : -
        ' Return    : -

        ' button download click handler
        If (e.CommandName = "DownloadCommand") Then
            Dim arrID() As String = e.CommandArgument.ToString().Split("|")
            Dim sID As String = arrID(0)
            Dim sFileName As String = arrID(1)
            Dim sTextLine As String = String.Empty
            Dim sFullPathName = "D:\\inetpub\\wwwroot\\IntraNewDesign\\Intranew\\Module\\LogbookPemblokiran\\InquiryHasilUpload\\DownloadFile\\"
            Dim bSuccess As Boolean = True
            Dim sMessageResult As String = String.Empty

            ' get main log
            Dim sSQLQuery As String = "SELECT UPLOADLOG_ID, DATE_FORMAT(UPLOAD_TIME, '%d-%m-%Y %H:%i') AS UPLOAD_TIME, DATA_ROW, DATA_SUCCESS," &
                                      " DATA_FAIL, DATE_FORMAT(LAST_UPDATE, '%d-%m-%Y %H:%i') AS LAST_UPDATE," &
                                      " DATE_FORMAT(UPLOAD_TIME, '%Y%m%d') AS UPLOADDATE" &
                                      " FROM LB_BLOK_UPLOADLOG" &
                                      " WHERE UPLOADLOG_ID = @UploadLogID"

            Dim sFileNameDownload As String = String.Empty

            Try
                Dim oDataTable As New DataTable()
                Using con As New MySqlConnection(connectionString)
                    con.Open()
                    Using cmd As New MySqlCommand(sSQLQuery, con)
                        cmd.Parameters.AddWithValue("@UploadLogID", sID)
                        Using reader As MySqlDataReader = cmd.ExecuteReader()
                            oDataTable.Load(reader)
                        End Using
                    End Using
                End Using

                If (oDataTable.Rows.Count > 0) Then
                    Dim oDataRow As DataRow = oDataTable.Rows(0)
                    sTextLine = "Upload time: " & oDataRow("UPLOAD_TIME").ToString() & vbCrLf
                    sTextLine &= "Success: " & oDataRow("DATA_SUCCESS").ToString() & vbCrLf
                    sTextLine &= "Failed: " & oDataRow("DATA_FAIL").ToString() & vbCrLf
                    sTextLine &= "Total Data: " & oDataRow("DATA_ROW").ToString() & vbCrLf
                    sTextLine &= "Finish time: " & oDataRow("LAST_UPDATE").ToString() & vbCrLf & vbCrLf

                    sFileNameDownload = "(Result)_" & sFileName & "_" & oDataRow("UPLOADDATE").ToString() & "_.txt"
                Else
                    sMessageResult = "gagal retrieve main data"
                    bSuccess = False
                End If
            Catch oEx As Exception
                sMessageResult = "fatal error retrieve main data"
                bSuccess = False
            End Try

            ' get sub log
            If (bSuccess) Then
                ' create sql script
                sSQLQuery = "SELECT UPLOADLOG_ID, UPLOADLOG_SUB_ID, DATA, NOTES" &
                            " FROM LB_BLOK_UPLOAD_DETAILLOG" &
                            " WHERE UPLOADLOG_ID = @UploadLogID" &
                            " ORDER BY SEQ_NO ASC"

                Try
                    Dim oDataTable As New DataTable()
                    Using con As New MySqlConnection(connectionString)
                        con.Open()
                        Using cmd As New MySqlCommand(sSQLQuery, con)
                            cmd.Parameters.AddWithValue("@UploadLogID", sID)
                            Using reader As MySqlDataReader = cmd.ExecuteReader()
                                oDataTable.Load(reader)
                            End Using
                        End Using
                    End Using

                    ' fill field form
                    If (oDataTable.Rows.Count > 0) Then
                        sTextLine &= "ACTION|Case ID|SID|Holder Name|Account Name|Account Number|Restriction Reason|Reference|Restrict Whole Account|Registered Date|Release Date|Restriction Status|Instrument|Asset Blocked|Notes|" & vbCrLf
                        For Each oDataRow As DataRow In oDataTable.Rows
                            sTextLine &= oDataRow("DATA").ToString() & "|" & oDataRow("NOTES").ToString() & vbCrLf
                        Next
                    Else
                        sMessageResult = "gagal retrieve sub data"
                        bSuccess = False
                    End If
                Catch oEx As Exception
                    sMessageResult = "fatal error retrieve sub data"
                    bSuccess = False
                End Try
            End If

            ' save to file
            Dim sFullPathFileName As String = sFullPathName & sFileNameDownload
            If (bSuccess) Then

                ' check folder add if not exists
                If (Not Directory.Exists(sFullPathName)) Then
                    Directory.CreateDirectory(sFullPathName)
                End If

                Try
                    Using oSW As New System.IO.StreamWriter(sFullPathFileName, False) ' overwrite
                        oSW.Write(sTextLine)
                    End Using
                    sMessageResult = "berhasil menyimpan data ke file [" & sFileNameDownload & "]"
                    bSuccess = True
                Catch oExp As IOException
                    sMessageResult = "fatal error menyimpan data ke file [" & sFileNameDownload & "]: " & oExp.Message
                    bSuccess = False
                End Try
            End If

            ' save file to client
            If (bSuccess) Then
                Dim oTargetFile As New System.IO.FileInfo(sFullPathFileName)
                Response.Clear()
                Response.Buffer = True
                Response.AddHeader("Content-Disposition", "attachment; filename=" & oTargetFile.Name)
                Response.AddHeader("Content-Length", oTargetFile.Length.ToString())
                Response.ContentType = "application/octet-stream"
                Response.WriteFile(oTargetFile.FullName)
                Response.End()
            End If

            ' result
            Response.Write("<script language=""javascript"">alert('File [" & sFileName & "] " & sMessageResult & ".');</script>")
        End If
    End Sub


End Class