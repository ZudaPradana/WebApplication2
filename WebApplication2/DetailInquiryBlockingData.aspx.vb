Imports System.Data.SqlClient
Imports System.Diagnostics.Eventing
Imports MySql.Data.MySqlClient

Public Class DetailInquiryBlockingData
    Inherits System.Web.UI.Page
    Private connectionString As String = System.Configuration.ConfigurationManager.ConnectionStrings("MyDbConnectionString").ConnectionString

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim caseID As String = Request.QueryString("CaseID")
            If Not String.IsNullOrEmpty(caseID) Then
                BindCaseDetails(caseID)
            End If
        End If
    End Sub

    Private Sub BindCaseDetails(caseID As String)

        Using con As New MySqlConnection(connectionString)
            Dim cmd As New MySqlCommand()
            cmd.CommandText = "SELECT * FROM CaseDetails WHERE CaseID = " & caseID
            cmd.Connection = con

            Dim da As New MySqlDataAdapter(cmd)
            Dim dt As New DataTable()
            da.Fill(dt)
            DetailsView1.DataSource = dt
            DetailsView1.DataBind()
        End Using
    End Sub
End Class