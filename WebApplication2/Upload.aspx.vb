Imports System.IO
Imports Microsoft.VisualBasic.Logging
Imports MySql.Data.MySqlClient

Public Class Upload
    Inherits Page
    Private connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("MyDbConnectionString").ConnectionString
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

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

    ' Event handler untuk tombol Submit
    Protected Sub Btn_Submit_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Btn_Submit.Click
        Try
            ' Pastikan kontrol tidak bernilai Nothing dan ada file yang diupload
            If UploadFile IsNot Nothing AndAlso UploadFile.HasFile Then
                ' Tentukan path folder dan nama file
                Dim folderPath As String = Server.MapPath("~/Uploads/")
                Dim fileName As String = UploadFile.FileName
                Dim filePath As String = System.IO.Path.Combine(folderPath, fileName)

                ' Periksa apakah folder ada; jika tidak, buat folder
                If Not System.IO.Directory.Exists(folderPath) Then
                    System.IO.Directory.CreateDirectory(folderPath)
                End If

                ' Simpan file ke folder yang ditentukan
                UploadFile.SaveAs(filePath)
                Console.WriteLine("File uploaded successfully: " & filePath)

                ' Panggil metode untuk memproses file
                ProcessFile(filePath, fileName)

                ' Tampilkan pesan sukses jika file diproses tanpa error
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showSuccessToast", "toastr.success('File berhasil diunggah dan diproses.');", True)
            Else
                ' Jika tidak ada file yang diupload, tampilkan pesan error
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showErrorToast", "toastr.error('Tidak ada file yang diunggah. Harap pilih file untuk diunggah.');", True)
            End If
        Catch ex As Exception
            ' Tampilkan pesan error jika ada pengecualian
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showErrorToast", $"toastr.error('Error: {ex.Message}');", True)
        End Try
    End Sub


    ' Metode untuk memproses file yang diupload
    Private Sub ProcessFile(ByVal filePath As String, ByVal fileName As String)
        Try
            ' Baca semua baris dari file
            Dim lines() As String = System.IO.File.ReadAllLines(filePath)

            ' Cek apakah file kosong
            If lines.Length = 0 Then
                Throw New Exception("File kosong. Tidak ada data untuk diproses.")
            End If

            Dim sID As String = GetUniqueID(0)
            Dim iSuccessCount As Integer = 0
            Dim iFailCount As Integer = 0
            Dim iEmpty As Integer = 0
            Dim iNoofData As Integer = lines.Length()

            ' Proses setiap baris setelah header
            For i As Integer = 1 To lines.Length
                Dim line As String = lines(i - 1).Trim()
                If (Not String.IsNullOrWhiteSpace(line)) Then
                    ' Pisahkan baris berdasarkan karakter separator '|'
                    Dim arrLine() As String = line.Split("|"c)

                    Dim blockingID As String = GetUniqueID(1)

                    ' Proses data
                    Dim sMessageError As String = String.Empty
                    Dim result = ProcessBlockingData(arrLine, blockingID, sMessageError)

                    If Not String.IsNullOrEmpty(result.Item2) Then
                        GetSaveUploadDetailLog(sID, result.Item2, i, arrLine(0).Trim().ToUpper(), line, result.Item1, sMessageError)
                    Else
                        GetSaveUploadDetailLog(sID, blockingID, i, arrLine(0).Trim().ToUpper(), line, result.Item1, sMessageError)
                    End If

                    If (result.Item1) Then
                        iSuccessCount += 1
                    Else
                        iFailCount += 1
                    End If
                Else
                    iEmpty += 1
                End If
            Next
            iNoofData = (iNoofData - iEmpty)

            GetSaveUploadLog(sID, fileName, iNoofData, iSuccessCount, iFailCount)
        Catch ex As Exception
            Throw New Exception("Error membaca file: " & ex.Message)
        End Try
    End Sub


    ' Fungsi untuk mendapatkan Unique ID
    Protected Function GetUniqueID(ByVal in_iAddNUmber As Integer) As String
        ' Get Unique ID
        Dim oCurrentTime As DateTime = DateTime.UtcNow
        Dim sCurrentTime As String = oCurrentTime.ToString
        Dim sParsedDateTime As DateTime = DateTime.Parse(sCurrentTime)
        Dim dUnixTime As Double = (sParsedDateTime - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds + in_iAddNUmber
        Dim sUTC As String = dUnixTime.ToString
        Dim sPrefix As String = ""
        Dim oRandom As New Random()
        For i As Integer = 1 To 5
            sPrefix &= ChrW(oRandom.Next(65, 90))
        Next
        Return (sPrefix & sUTC)
    End Function

    Protected Sub GetSaveUploadLog(ByVal in_sID As String, ByVal in_sFileName As String, ByVal in_iDataRow As Integer,
                                ByVal in_iDataSuccess As Integer, ByVal in_iDataFail As Integer)
        Dim sUpdatedBy As String = ""
        Dim sStatus As String = "SUCCESS"

        ' determine status
        If (in_iDataFail > 0) Then
            sStatus = "PARTIAL"
            If (in_iDataRow = in_iDataFail) Then sStatus = "FAIL"
        End If

        Using con As New MySqlConnection(connectionString)
            con.Open()
            Dim sSqlInsertUploadLog As String = "INSERT INTO LB_BLOK_UPLOADLOG(UPLOADLOG_ID, FILENAME, CATEGORY, DATA_ROW, DATA_SUCCESS, DATA_FAIL, UPLOAD_TIME, UPLOAD_STATUS, UPDATED_BY, LAST_UPDATE) " &
                                                            "VALUES(@UPLOADLOG_ID, @FILENAME, 'BLOCKING', @DATA_ROW, @DATA_SUCCESS, @DATA_FAIL, CURRENT_TIMESTAMP, @UPLOAD_STATUS, @UPDATED_BY, CURRENT_TIMESTAMP)"
            ' Insert into LB_BLOK_UPLOADLOG
            Using cmd As New MySqlCommand(sSqlInsertUploadLog, con)
                cmd.Parameters.AddWithValue("@UPLOADLOG_ID", in_sID)
                cmd.Parameters.AddWithValue("@FILENAME", in_sFileName)
                cmd.Parameters.AddWithValue("@DATA_ROW", in_iDataRow)
                cmd.Parameters.AddWithValue("@DATA_SUCCESS", in_iDataSuccess)
                cmd.Parameters.AddWithValue("@DATA_FAIL", in_iDataFail)
                cmd.Parameters.AddWithValue("@UPLOAD_STATUS", sStatus)
                cmd.Parameters.AddWithValue("@UPDATED_BY", sUpdatedBy)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Protected Sub GetSaveUploadDetailLog(ByVal in_sID As String, ByVal in_sSubID As String, ByVal in_iRow As Integer,
                                            ByVal in_sCommand As String, ByVal in_sContentLine As String, ByVal in_bStatus As Boolean,
                                            ByRef out_sMessageError As String)

        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()
                Dim sSqlInsertUploadDetailLog As String = "INSERT INTO LB_BLOK_UPLOAD_DETAILLOG(UPLOADLOG_ID, UPLOADLOG_SUB_ID, SEQ_NO, COMMAND, RECIPIENT, DATA, STATUS, NOTES, LAST_UPDATE) " &
                                                                "VALUES(@UPLOADLOG_ID, @UPLOADLOG_SUB_ID, @SEQ_NO, @COMMAND, '-', @DATA, @STATUS, @NOTES, CURRENT_TIMESTAMP)"
                ' Insert into LB_BLOK_UPLOAD_DETAILLOG
                Using cmd As New MySqlCommand(sSqlInsertUploadDetailLog, con)
                    cmd.Parameters.AddWithValue("@UPLOADLOG_ID", in_sID)
                    cmd.Parameters.AddWithValue("@UPLOADLOG_SUB_ID", in_sSubID)
                    cmd.Parameters.AddWithValue("@SEQ_NO", in_iRow)
                    cmd.Parameters.AddWithValue("@COMMAND", in_sCommand)
                    cmd.Parameters.AddWithValue("@DATA", in_sContentLine)
                    cmd.Parameters.AddWithValue("@STATUS", IIf(in_bStatus, "SUCCESS", "FAIL"))
                    cmd.Parameters.AddWithValue("@NOTES", out_sMessageError)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        Catch ex As MySqlException
            out_sMessageError = "Terjadi kesalahan saat memproses data: " & ex.Message
        End Try
    End Sub

    ' Fungsi untuk memproses data blocking
    Protected Function ProcessBlockingData(ByVal in_arFileContentLine() As String, ByVal sSubID As String, ByRef out_sMessageError As String) As Tuple(Of Boolean, String)
        Dim bSuccess As Boolean = True
        Dim sAction As String = in_arFileContentLine(0).Trim().ToUpper()
        Dim sCaseID As String = in_arFileContentLine(1).Trim()
        Dim sSID As String = in_arFileContentLine(2).Trim()
        Dim sAccountNumber As String = in_arFileContentLine(5).Trim()
        Dim sHolderName As String = in_arFileContentLine(3).Trim()
        Dim sAccountName As String = in_arFileContentLine(4).Trim()
        Dim sRestrictionReason As String = in_arFileContentLine(6).Trim()
        Dim sReference As String = in_arFileContentLine(7).Trim()
        Dim sRestrictWholeAccount As String = in_arFileContentLine(8).Trim()
        Dim sRegisteredDate As String = in_arFileContentLine(9).Trim()
        Dim sReleaseDate As String = in_arFileContentLine(10).Trim()
        Dim sRestrictionStatus As String = in_arFileContentLine(11).Trim()
        Dim sInstrument As String = in_arFileContentLine(12).Trim()
        Dim sAssetBlocked As String = in_arFileContentLine(13).Trim()
        Dim sNotes As String = in_arFileContentLine(14).Trim()

        Dim dtRegisteredDate As DateTime
        Dim dtReleaseDate As DateTime

        Dim currentBlockingId As String



        Try
            Using con As New MySqlConnection(connectionString)
                con.Open()

                ' Check if data already exists
                Dim sSQLCheckExisting As String = "SELECT BLOCKING_ID FROM LB_BLOK_BLOCKING_DATA " &
                                   "WHERE CASE_ID = @CaseID AND SID = @SID AND ACCOUNT_NO = @AccountNo"
                Dim dataExists As Boolean = False

                Using cmdCheck As New MySqlCommand(sSQLCheckExisting, con)
                    cmdCheck.Parameters.AddWithValue("@CaseID", sCaseID)
                    cmdCheck.Parameters.AddWithValue("@SID", sSID)
                    cmdCheck.Parameters.AddWithValue("@AccountNo", sAccountNumber)

                    Using reader As MySqlDataReader = cmdCheck.ExecuteReader()
                        If reader.Read() Then
                            ' Data exists
                            dataExists = True
                            ' Retrieve BLOCKING_ID
                            currentBlockingId = If(reader.IsDBNull(reader.GetOrdinal("BLOCKING_ID")), String.Empty, reader.GetString(reader.GetOrdinal("BLOCKING_ID")))
                        End If
                    End Using
                End Using

                If DateTime.TryParseExact(sRegisteredDate, "dd/MM/yyyy", Nothing, Globalization.DateTimeStyles.None, dtRegisteredDate) AndAlso
    DateTime.TryParseExact(sReleaseDate, "dd/MM/yyyy", Nothing, Globalization.DateTimeStyles.None, dtReleaseDate) Then
                    If sAction = "CREATE" Then
                        If Not dataExists Then
                            ' Insert untuk aksi CREATE
                            Dim sSQLInsertBlocking As String = "INSERT INTO LB_BLOK_BLOCKING (BLOCKING_ID, CASE_ID, RESTRICTION_REASON, BLOCKING_ACTION, BLOCKING_STATUS, BLOCKING_APPROVAL, BLOCKING_CREATE_DATE, UPDATED_BY, LAST_UPDATE) " &
                                                        "VALUES(@BlockingID, @CaseID, @RestrictionReason, 'CREATE', 'WAITING', 'WAITING', CURDATE(), '', CURRENT_TIMESTAMP)"
                            Dim sSQLInsertBlockingData As String = "INSERT INTO LB_BLOK_BLOCKING_DATA (BLOCKING_ID, CASE_ID, SID, ACCOUNT_NO, HOLDER_NAME, ACCOUNT_NAME, RESTRICTION_REASON, REFERENCE, RESTRICT_WHOLE_ACCOUNT, REGISTERED_DATE, RELEASE_DATE, RESTRICTION_STATUS, NOTES, LAST_UPDATE) " &
                                                            "VALUES(@BlockingID, @CaseID, @SID, @AccountNo, @HolderName, @AccountName, @RestrictionReason, @Reference, @RestrictWholeAccount, @RegisteredDate, @ReleaseDate, @RestrictionStatus, @Notes, CURRENT_TIMESTAMP)"
                            Dim sSQLInsertBlockingInstrument As String = "INSERT INTO LB_BLOK_BLOCKING_DATA_INSTRUMENT (BLOCKING_ID, CASE_ID, SID, ACCOUNT_NO, INSTRUMENT, ASSET_BLOCKED, LAST_UPDATE) " &
                                                                    "VALUES(@BlockingID, @CaseID, @SID, @AccountNo, @Instrument, @AssetBlocked, CURRENT_TIMESTAMP)"

                            ' Insert into LB_BLOK_BLOCKING
                            Using cmd As New MySqlCommand(sSQLInsertBlocking, con)
                                cmd.Parameters.AddWithValue("@BlockingID", sSubID)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.Parameters.AddWithValue("@RestrictionReason", sRestrictionReason)
                                cmd.ExecuteNonQuery()
                            End Using

                            ' Insert into LB_BLOK_BLOCKING_DATA
                            Using cmd As New MySqlCommand(sSQLInsertBlockingData, con)
                                cmd.Parameters.AddWithValue("@BlockingID", sSubID)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.Parameters.AddWithValue("@SID", sSID)
                                cmd.Parameters.AddWithValue("@AccountNo", sAccountNumber)
                                cmd.Parameters.AddWithValue("@HolderName", sHolderName)
                                cmd.Parameters.AddWithValue("@AccountName", sAccountName)
                                cmd.Parameters.AddWithValue("@RestrictionReason", sRestrictionReason)
                                cmd.Parameters.AddWithValue("@Reference", sReference)
                                cmd.Parameters.AddWithValue("@RestrictWholeAccount", sRestrictWholeAccount)
                                cmd.Parameters.AddWithValue("@RegisteredDate", dtRegisteredDate)
                                cmd.Parameters.AddWithValue("@ReleaseDate", dtReleaseDate)
                                cmd.Parameters.AddWithValue("@RestrictionStatus", sRestrictionStatus)
                                cmd.Parameters.AddWithValue("@Notes", sNotes)
                                cmd.ExecuteNonQuery()
                            End Using

                            ' Insert into LB_BLOK_BLOCKING_DATA_INSTRUMENT
                            Using cmd As New MySqlCommand(sSQLInsertBlockingInstrument, con)
                                cmd.Parameters.AddWithValue("@BlockingID", sSubID)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.Parameters.AddWithValue("@SID", sSID)
                                cmd.Parameters.AddWithValue("@AccountNo", sAccountNumber)
                                cmd.Parameters.AddWithValue("@Instrument", sInstrument)
                                cmd.Parameters.AddWithValue("@AssetBlocked", sAssetBlocked)
                                cmd.ExecuteNonQuery()
                            End Using
                        Else
                            out_sMessageError = "Data dengan CaseID, SID, dan AccountNo yang sama sudah ada."
                            bSuccess = False
                        End If

                    ElseIf sAction = "MODIFY" Then
                        If dataExists Then
                            ' Update data yang sudah ada
                            Dim sSQLUpdateBlockingData As String = "UPDATE LB_BLOK_BLOCKING_DATA SET HOLDER_NAME = @HolderName, ACCOUNT_NAME = @AccountName, RESTRICTION_REASON = @RestrictionReason, REFERENCE = @Reference, RESTRICT_WHOLE_ACCOUNT = @RestrictWholeAccount, REGISTERED_DATE = @RegisteredDate, RELEASE_DATE = @ReleaseDate, RESTRICTION_STATUS = @RestrictionStatus, NOTES = @Notes, LAST_UPDATE = CURRENT_TIMESTAMP WHERE CASE_ID = @CaseID AND SID = @SID AND ACCOUNT_NO = @AccountNo"

                            Using cmd As New MySqlCommand(sSQLUpdateBlockingData, con)
                                cmd.Parameters.AddWithValue("@HolderName", sHolderName)
                                cmd.Parameters.AddWithValue("@AccountName", sAccountName)
                                cmd.Parameters.AddWithValue("@RestrictionReason", sRestrictionReason)
                                cmd.Parameters.AddWithValue("@Reference", sReference)
                                cmd.Parameters.AddWithValue("@RestrictWholeAccount", sRestrictWholeAccount)
                                cmd.Parameters.AddWithValue("@RegisteredDate", dtRegisteredDate)
                                cmd.Parameters.AddWithValue("@ReleaseDate", dtReleaseDate)
                                cmd.Parameters.AddWithValue("@RestrictionStatus", sRestrictionStatus)
                                cmd.Parameters.AddWithValue("@Notes", sNotes)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.Parameters.AddWithValue("@SID", sSID)
                                cmd.Parameters.AddWithValue("@AccountNo", sAccountNumber)
                                cmd.ExecuteNonQuery()
                            End Using

                            ' Update LB_BLOK_BLOCKING_DATA_INSTRUMENT if necessary
                            Dim sSQLUpdateBlockingInstrument As String = "UPDATE LB_BLOK_BLOCKING_DATA_INSTRUMENT SET INSTRUMENT = @Instrument, ASSET_BLOCKED = @AssetBlocked, LAST_UPDATE = CURRENT_TIMESTAMP WHERE CASE_ID = @CaseID AND SID = @SID AND ACCOUNT_NO = @AccountNo"

                            Using cmd As New MySqlCommand(sSQLUpdateBlockingInstrument, con)
                                cmd.Parameters.AddWithValue("@Instrument", sInstrument)
                                cmd.Parameters.AddWithValue("@AssetBlocked", sAssetBlocked)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.Parameters.AddWithValue("@SID", sSID)
                                cmd.Parameters.AddWithValue("@AccountNo", sAccountNumber)
                                cmd.Parameters.AddWithValue("@Notes", sNotes)
                                cmd.ExecuteNonQuery()
                            End Using
                        Else
                            out_sMessageError = "Data yang akan diubah tidak ditemukan."
                            bSuccess = False
                        End If

                    ElseIf sAction = "DELETE" Then
                        If dataExists Then
                            ' Hapus data dari ketiga tabel
                            Dim sSQLDeleteBlockingData As String = "DELETE FROM LB_BLOK_BLOCKING_DATA WHERE CASE_ID = @CaseID AND SID = @SID AND ACCOUNT_NO = @AccountNo"
                            Dim sSQLDeleteBlockingInstrument As String = "DELETE FROM LB_BLOK_BLOCKING_DATA_INSTRUMENT WHERE CASE_ID = @CaseID AND SID = @SID AND ACCOUNT_NO = @AccountNo"
                            Dim sSQLDeleteBlocking As String = "DELETE FROM LB_BLOK_BLOCKING WHERE CASE_ID = @CaseID"

                            Using cmd As New MySqlCommand(sSQLDeleteBlockingData, con)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.Parameters.AddWithValue("@SID", sSID)
                                cmd.Parameters.AddWithValue("@AccountNo", sAccountNumber)
                                cmd.ExecuteNonQuery()
                            End Using

                            Using cmd As New MySqlCommand(sSQLDeleteBlockingInstrument, con)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.Parameters.AddWithValue("@SID", sSID)
                                cmd.Parameters.AddWithValue("@AccountNo", sAccountNumber)
                                cmd.ExecuteNonQuery()
                            End Using

                            Using cmd As New MySqlCommand(sSQLDeleteBlocking, con)
                                cmd.Parameters.AddWithValue("@CaseID", sCaseID)
                                cmd.ExecuteNonQuery()
                            End Using
                        Else
                            out_sMessageError = "Data yang akan dihapus tidak ditemukan."
                            bSuccess = False
                        End If
                    Else
                        out_sMessageError = "Aksi tidak valid."
                        bSuccess = False
                    End If
                Else
                    out_sMessageError = "Format tanggal tidak valid. Harap gunakan format 'dd/MM/yyyy'."
                    bSuccess = False
                End If
            End Using
        Catch ex As MySqlException
            out_sMessageError = "Terjadi kesalahan saat memproses data: " & ex.Message
            bSuccess = False
        End Try

        Return Tuple.Create(bSuccess, currentBlockingId)
    End Function


    ' Reset button
    Protected Sub Btn_Reset_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Button Reset Handler
        ' Parameter : {Object} Objects
        ' Return    : -
        GetResetFields()
    End Sub

    Protected Sub GetResetFields()
        ' Reset Fields
        ' Parameter : -
        ' Return    : -
        UploadFile.Dispose()
        UploadFile.PostedFile.InputStream.Dispose()
    End Sub

End Class