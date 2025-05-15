Imports System.IO.Compression
Imports System.IO
Imports System.Text.Json

Public Class BackupPreviewForm
    Public Property ZipPath As String = ""
    Public Property SourceFolder As String = ""
    Public Property PreviewMode As String = "zip" ' "zip" or "folder"

    Private Function IsValidSchedule1Backup(zipPath As String) As Boolean
        Try
            Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
                ' Find all Game.json files inside SaveGame_ folders
                Dim validSaves As List(Of ZipArchiveEntry) = New List(Of ZipArchiveEntry)()

                For Each entry As ZipArchiveEntry In archive.Entries
                    If entry.FullName.EndsWith("Game.json", StringComparison.OrdinalIgnoreCase) AndAlso
                   entry.FullName.Contains("SaveGame_") Then
                        validSaves.Add(entry)
                    End If
                Next

                If validSaves.Count = 0 Then Return False

                For Each entry In validSaves
                    Using stream = entry.Open()
                        Using reader As New StreamReader(stream)
                            Dim json = reader.ReadToEnd()
                            Dim doc = JsonDocument.Parse(json)

                            If Not doc.RootElement.TryGetProperty("GameVersion", Nothing) OrElse
                           Not doc.RootElement.TryGetProperty("OrganisationName", Nothing) Then
                                Return False
                            End If
                        End Using
                    End Using
                Next

                Return True
            End Using
        Catch
            Return False
        End Try
    End Function

    Private Sub LoadPreviewFromZip(zipPath As String)
        Using archive As ZipArchive = ZipFile.OpenRead(zipPath)
            Dim saveEntries = archive.Entries.
    Where(Function(entry) entry.FullName.EndsWith("Game.json", StringComparison.OrdinalIgnoreCase) AndAlso entry.FullName.Contains("SaveGame_"))


            For Each entry In saveEntries
                          Using stream As Stream = entry.Open()
                              Using reader As New StreamReader(stream)
                                  Dim jsonText = reader.ReadToEnd()
                                  Try
                                      Dim doc = JsonDocument.Parse(jsonText)
                                      Dim version = doc.RootElement.GetProperty("GameVersion").GetString()
                                      Dim org = doc.RootElement.GetProperty("OrganisationName").GetString()
                                      Dim saveFolder = entry.FullName.Split("/"c).Reverse().Skip(1).FirstOrDefault()
                                      ListBoxContents.Items.Add($"{saveFolder} — Org: {org} | Version: {version}")
                                  Catch
                                      ListBoxContents.Items.Add("Could not read metadata from: " & entry.FullName)
                                  End Try
                              End Using
                          End Using
                      Next
    End Using
    End Sub

    Private Sub LoadPreviewFromFolder(basePath As String)
        ' Get version folders (e.g., "v1.3.2")
        Dim versionDirs = Directory.GetDirectories(basePath)

        If versionDirs.Length = 0 Then
            ListBoxContents.Items.Add("⚠ No version folders found in Saves directory.")
            Return
        End If

        Dim validSaveCount As Integer = 0

        For Each versionDir In versionDirs
            Dim saveDirs = Directory.GetDirectories(versionDir, "SaveGame_*", SearchOption.TopDirectoryOnly)

            For Each saveDir In saveDirs
                Dim jsonPath = Path.Combine(saveDir, "Game.json")

                If File.Exists(jsonPath) Then
                    Try
                        Dim json = File.ReadAllText(jsonPath)
                        Dim doc = JsonDocument.Parse(json)
                        Dim version = doc.RootElement.GetProperty("GameVersion").GetString()
                        Dim org = doc.RootElement.GetProperty("OrganisationName").GetString()
                        Dim saveName = Path.GetFileName(saveDir)
                        ListBoxContents.Items.Add($"{saveName} — Org: {org} | Version: {version}")
                        validSaveCount += 1
                    Catch ex As Exception
                        ListBoxContents.Items.Add($"❌ Error reading {jsonPath}: {ex.Message}")
                    End Try
                Else
                    ListBoxContents.Items.Add($"⚠ Missing Game.json in: {saveDir}")
                End If
            Next
        Next

        If validSaveCount = 0 Then
            ListBoxContents.Items.Add("⚠ No valid SaveGame folders found in any version folder.")
        Else
            ListBoxContents.Items.Add("-------------------------")
            ListBoxContents.Items.Add($"— {validSaveCount} save(s) found —")
        End If
    End Sub




    Private Sub BackupPreviewForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            If PreviewMode = "zip" Then
                If Not File.Exists(ZipPath) Then
                    MessageBox.Show("Zip file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Me.DialogResult = DialogResult.Cancel
                    Me.Close()
                    Return
                End If

                If Not IsValidSchedule1Backup(ZipPath) Then
                    MessageBox.Show("This backup is invalid. It must include at least one SaveGame with a valid Game.json.", "Invalid Backup", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Me.DialogResult = DialogResult.Cancel
                    Me.Close()
                    Return
                End If

                LoadPreviewFromZip(ZipPath)

            ElseIf PreviewMode = "folder" Then
                If Not Directory.Exists(SourceFolder) Then
                    MessageBox.Show("Save folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Me.DialogResult = DialogResult.Cancel
                    Me.Close()
                    Return
                End If

                LoadPreviewFromFolder(SourceFolder)
            End If

        Catch ex As Exception
            MessageBox.Show("Failed to load preview: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Me.DialogResult = DialogResult.Cancel
            Me.Close()
        End Try
    End Sub






    Private Sub btnContinue_Click(sender As Object, e As EventArgs) Handles btnContinue.Click
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class
