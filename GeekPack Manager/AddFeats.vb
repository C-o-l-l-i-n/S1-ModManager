Imports System.IO
Imports System.IO.Compression
Imports System.Text.Json


Public Class AddFeats
    Private Sub AddFeats_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim path As String = Form1.GamePath

    End Sub
    Private Sub BtnEditMoney_Click(sender As Object, e As EventArgs) Handles BtnEditMoney.Click
        Dim savesPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "..\LocalLow\TVGS\Schedule I\Saves"
    )
        savesPath = Path.GetFullPath(savesPath)

        Dim versionDirs = Directory.GetDirectories(savesPath)
        Dim saveFolders As New List(Of String)

        ' Gather SaveGame paths
        For Each versionDir In versionDirs
            Dim candidates = Directory.GetDirectories(versionDir, "SaveGame_*", SearchOption.TopDirectoryOnly)
            saveFolders.AddRange(candidates)
        Next

        If saveFolders.Count = 0 Then
            MessageBox.Show("No SaveGame folders found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Prompt user to pick a save
        Dim selectForm As New Form With {
        .Text = "Select Save to Edit",
        .Width = 400,
        .Height = 300,
        .StartPosition = FormStartPosition.CenterParent
    }

        Dim listBox As New ListBox With {.Dock = DockStyle.Fill}
        Dim saveDisplayNames As New List(Of String)

        For Each savePath In saveFolders
            Dim gameJsonPath = Path.Combine(savePath, "Game.json")
            Dim displayName As String = Path.GetFileName(savePath)

            If File.Exists(gameJsonPath) Then
                Try
                    Dim json = File.ReadAllText(gameJsonPath)
                    Dim saveJsonDoc = JsonDocument.Parse(json)
                    Dim name = saveJsonDoc.RootElement.GetProperty("SaveName").GetString()
                    displayName = name & $" ({Path.GetFileName(savePath)})"
                Catch
                    Try
                        Dim json = File.ReadAllText(gameJsonPath)
                        Dim saveJsonDoc = JsonDocument.Parse(json)

                        If saveJsonDoc.RootElement.TryGetProperty("OrganisationName", Nothing) Then
                            Dim orgName = saveJsonDoc.RootElement.GetProperty("OrganisationName").GetString()
                            displayName = orgName & $" ({Path.GetFileName(savePath)})"
                        Else
                            MessageBox.Show("OrganisationName property not found in: " & gameJsonPath)
                        End If
                    Catch ex As Exception
                        MessageBox.Show($"Error reading Game.json in {savePath}:{vbCrLf}{ex.Message}")
                    End Try

                End Try
            End If

            saveDisplayNames.Add(displayName)
        Next


        listBox.Items.AddRange(saveDisplayNames.ToArray())
        selectForm.Controls.Add(listBox)

        Dim okButton As New Button With {.Text = "OK", .Dock = DockStyle.Bottom}
        selectForm.Controls.Add(okButton)

        AddHandler okButton.Click, Sub() selectForm.DialogResult = DialogResult.OK
        If selectForm.ShowDialog() <> DialogResult.OK OrElse listBox.SelectedItem Is Nothing Then Return

        Dim selectedSave = saveFolders(listBox.SelectedIndex)
        Dim moneyPath = Path.Combine(selectedSave, "Money.json")

        If Not File.Exists(moneyPath) Then
            MessageBox.Show("Money.json not found in selected save.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Load current balance
        Dim moneyJson = File.ReadAllText(moneyPath)
        Dim doc = JsonDocument.Parse(moneyJson)
        Dim oldBalance = doc.RootElement.GetProperty("OnlineBalance").GetDecimal()

        ' Prompt for new amount
        Dim newAmountStr = InputBox($"Current Balance: {oldBalance}" & vbCrLf & "Enter new balance amount:", "Edit Money")
        If Not Decimal.TryParse(newAmountStr, Nothing) Then
            MessageBox.Show("Invalid amount.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If
        Dim newAmount = Decimal.Parse(newAmountStr)

        ' Update JSON
        Dim updatedJson = New Dictionary(Of String, Object) From {
        {"OnlineBalance", newAmount},
        {"Networth", newAmount}
    }

        ' Preserve other fields if any
        For Each prop In doc.RootElement.EnumerateObject()
            If Not updatedJson.ContainsKey(prop.Name) Then
                updatedJson(prop.Name) = JsonSerializer.Deserialize(Of Object)(prop.Value.GetRawText())
            End If
        Next

        ' Write updated Money.json
        Dim options As New JsonSerializerOptions With {.WriteIndented = True}
        File.WriteAllText(moneyPath, JsonSerializer.Serialize(updatedJson, options))

        MessageBox.Show("Money updated successfully!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub BtnRenameSave_Click(sender As Object, e As EventArgs) Handles BtnRenameSave.Click
        Dim savesPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "..\LocalLow\TVGS\Schedule I\Saves"
    )
        savesPath = Path.GetFullPath(savesPath)

        Dim versionDirs = Directory.GetDirectories(savesPath)
        Dim saveFolders As New List(Of String)

        For Each versionDir In versionDirs
            Dim candidates = Directory.GetDirectories(versionDir, "SaveGame_*", SearchOption.TopDirectoryOnly)
            saveFolders.AddRange(candidates)
        Next

        If saveFolders.Count = 0 Then
            MessageBox.Show("No saves found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Create list of display names using OrganisationName
        Dim saveDisplayNames As New List(Of String)
        For Each savePath In saveFolders
            Dim gameJsonPath = Path.Combine(savePath, "Game.json")
            Dim displayName As String = Path.GetFileName(savePath)

            If File.Exists(gameJsonPath) Then
                Try
                    Dim json = File.ReadAllText(gameJsonPath)
                    Dim saveJsonDoc = JsonDocument.Parse(json)
                    Dim orgName = saveJsonDoc.RootElement.GetProperty("OrganisationName").GetString()
                    displayName = orgName & $" ({Path.GetFileName(savePath)})"
                Catch
                    ' fallback to folder name
                End Try
            End If

            saveDisplayNames.Add(displayName)
        Next

        ' Selection dialog
        Dim selectForm As New Form With {
        .Text = "Select Save to Rename",
        .Width = 400,
        .Height = 300,
        .StartPosition = FormStartPosition.CenterParent
    }

        Dim listBox As New ListBox With {.Dock = DockStyle.Fill}
        listBox.Items.AddRange(saveDisplayNames.ToArray())
        selectForm.Controls.Add(listBox)

        Dim okButton As New Button With {.Text = "OK", .Dock = DockStyle.Bottom}
        selectForm.Controls.Add(okButton)

        AddHandler okButton.Click, Sub() selectForm.DialogResult = DialogResult.OK
        If selectForm.ShowDialog() <> DialogResult.OK OrElse listBox.SelectedItem Is Nothing Then Return

        Dim selectedPath = saveFolders(listBox.SelectedIndex)
        Dim gameJsonPathFinal = Path.Combine(selectedPath, "Game.json")

        If Not File.Exists(gameJsonPathFinal) Then
            MessageBox.Show("Game.json not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        ' Get new name
        Dim newName = InputBox("Enter new Organization name:", "Rename Save")
        If String.IsNullOrWhiteSpace(newName) Then
            MessageBox.Show("Name cannot be blank.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Modify and write updated JSON
        Try
            Dim oldJson = File.ReadAllText(gameJsonPathFinal)
            Dim parsed = JsonDocument.Parse(oldJson)
            Dim updated As New Dictionary(Of String, Object)

            ' Copy all properties, overwrite OrganisationName
            For Each prop In parsed.RootElement.EnumerateObject()
                If prop.Name = "OrganisationName" Then
                    updated(prop.Name) = newName
                Else
                    updated(prop.Name) = JsonSerializer.Deserialize(Of Object)(prop.Value.GetRawText())
                End If
            Next

            Dim options As New JsonSerializerOptions With {.WriteIndented = True}
            File.WriteAllText(gameJsonPathFinal, JsonSerializer.Serialize(updated, options))

            MessageBox.Show("Save renamed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Me.Refresh()

        Catch ex As Exception
            MessageBox.Show("Rename failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private ReadOnly Property SavesPath As String
        Get
            Dim localLowPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "..\LocalLow\TVGS\Schedule I\Saves"
        )
            Return Path.GetFullPath(localLowPath)
        End Get
    End Property

    Private Sub BtnBackupSaves_Click(sender As Object, e As EventArgs) Handles BtnBackupSaves.Click
        If Not Directory.Exists(SavesPath) Then
            MessageBox.Show("No saves found to back up.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim previewForm As New BackupPreviewForm()
        previewForm.PreviewMode = "folder"
        previewForm.SourceFolder = SavesPath

        If previewForm.ShowDialog() <> DialogResult.OK Then
            MessageBox.Show("Backup cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Using saveDialog As New SaveFileDialog()
            saveDialog.Title = "Save Backup As"
            saveDialog.Filter = "ZIP Archive|*.zip"
            saveDialog.FileName = $"Schedule1_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip"

            If saveDialog.ShowDialog() = DialogResult.OK Then
                Try
                    If File.Exists(saveDialog.FileName) Then File.Delete(saveDialog.FileName)
                    ZipFile.CreateFromDirectory(SavesPath, saveDialog.FileName, CompressionLevel.Optimal, False)
                    MessageBox.Show("Backup created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Failed to create backup: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub



    Private Sub BtnRestoreSaves_Click(sender As Object, e As EventArgs) Handles BtnRestoreSaves.Click
        Using openDialog As New OpenFileDialog()
            openDialog.Title = "Select Backup ZIP File"
            openDialog.Filter = "ZIP Archive|*.zip"

            If openDialog.ShowDialog() = DialogResult.OK Then
                ' Show preview form
                Dim previewForm As New BackupPreviewForm()
                previewForm.ZipPath = openDialog.FileName

                If previewForm.ShowDialog() <> DialogResult.OK Then
                    MessageBox.Show("Restore cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                Dim choice = MessageBox.Show(
                    "Do you want to remove all current saves before restoring?" & vbCrLf & vbCrLf &
                    "WARNING: If any folders already exist in your saves, they will be overwritten.",
                    "Restore Options",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning
                )

                If choice = DialogResult.Cancel Then Return

                Try
                    If choice = DialogResult.Yes AndAlso Directory.Exists(SavesPath) Then
                        Directory.Delete(SavesPath, True)
                    End If

                    Directory.CreateDirectory(SavesPath)

                    Using archive As ZipArchive = ZipFile.OpenRead(openDialog.FileName)
                        For Each entry In archive.Entries
                            Dim destinationPath = Path.Combine(SavesPath, entry.FullName)

                            If String.IsNullOrWhiteSpace(entry.Name) Then
                                Directory.CreateDirectory(destinationPath)
                            Else
                                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath))
                                entry.ExtractToFile(destinationPath, True)
                            End If
                        Next
                    End Using

                    MessageBox.Show("Backup restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show("Restore failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End Using
    End Sub


End Class