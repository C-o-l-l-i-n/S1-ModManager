Imports System.Net
Imports System.IO
Imports System.IO.Compression
Imports System.Drawing.Text
Imports Microsoft.Win32
Imports System.Text.RegularExpressions

Public Class Form1
    ' Application update configuration
    Private Const CurrentVersion As String = "1.1.3"
    Private Const VersionCheckUrl As String = "https://cloud.collinframe.com/s/NdAHppZC7Y4FWBz/download"
    Private Const UpdateUrlConfig As String = "https://cloud.collinframe.com/s/bqb6qSKMf7CoC4C/download"

    ' Modpack update configuration
    Private Const ModpackVersionCheckUrl As String = "https://cloud.collinframe.com/s/Z7WzHgLZdwNRXJc/download"
    Private Const PackUrlConfig As String = "https://cloud.collinframe.com/s/Tz6yqgHENf4Efgf/download"
    Private Const ModpackVersionFileName As String = "mpv.txt"
    Private ModpackVersion As String = "0.0.0"

    ' Game path management
    Public Shared GamePath As String = Nothing
    Private WithEvents updateClient As New WebClient()

    ' Form load event handler
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize game path and check for updates
        GamePath = FindSchedule1Path()
        If String.IsNullOrEmpty(GamePath) OrElse Not Directory.Exists(GamePath) Then
            GamePath = AskUserForGamePath()
        End If

        If String.IsNullOrEmpty(GamePath) Then
            MessageBox.Show("Unable to locate Schedule I directory. Some features may not work.",
                          "Game Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Else
            WriteToLog("Schedule I path set to: " & GamePath)
        End If

        LoadLocalModpackVersion()
        LabelVersion.Text = $"App Version: {CurrentVersion}"
        CheckForUpdates()
    End Sub

    ' Loads the local modpack version from file
    Private Sub LoadLocalModpackVersion()
        Try
            Dim versionPath As String = Path.Combine(GamePath, "Mods", "GPM", ModpackVersionFileName)
            If File.Exists(versionPath) Then
                ModpackVersion = File.ReadAllText(versionPath).Trim()
            Else
                ModpackVersion = "0.0.0" ' Default version indicates pack is not installed
                WriteToLog("Modpack version file not found in expected path, assuming not installed.")
            End If
        Catch ex As Exception
            WriteToLog("Failed to load modpack version: " & ex.Message)
            ModpackVersion = "0.0.0"
        End Try
        UpdateGPVLabel(ModpackVersion)
    End Sub

    ' Updates the Geek Pack Version label
    Private Sub UpdateGPVLabel(verNum As String)
        Label3.Text = "Installed Geek Pack Version: " + verNum
    End Sub

    ' Saves the modpack version to file
    Private Sub SaveLocalModpackVersion(version As String)
        Try
            Dim versionPath As String = Path.Combine(GamePath, "Mods", "GPM", ModpackVersionFileName)
            Directory.CreateDirectory(Path.GetDirectoryName(versionPath))
            File.WriteAllText(versionPath, version)
        Catch ex As Exception
            WriteToLog("Failed to save modpack version: " & ex.Message)
        End Try
    End Sub

    ' Retrieves the modpack download URL from server
    Private Async Function GetPackUrlFromServer() As Task(Of String)
        Try
            Using client As New WebClient()
                Dim url As String = Await client.DownloadStringTaskAsync(PackUrlConfig)
                Return url.Trim()
            End Using
        Catch ex As Exception
            WriteToLog($"Failed to fetch Pack URL: {ex.Message}")
            Return Nothing
        End Try
    End Function

    ' Checks for modpack updates
    Private Async Sub CheckModpackVersion()
        Try
            Await Task.Delay(1000) ' 1 second delay to allow UI to initialize

            Using client As New WebClient()
                Dim latestVersion As String = Await client.DownloadStringTaskAsync(ModpackVersionCheckUrl)

                If VersionNewer(latestVersion, ModpackVersion) Then
                    Dim result As DialogResult = MessageBox.Show(
                        $"A new modpack version is available (v{latestVersion}). Would you like to install it now?",
                        "Modpack Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)

                    If result = DialogResult.Yes Then
                        Await InstallModpack(latestVersion)
                    End If
                End If
            End Using
        Catch ex As Exception
            WriteToLog($"Modpack update check failed: {ex.Message}")
        End Try
    End Sub

    ' Installs the modpack
    Private Async Function InstallModpack(Optional newVersion As String = Nothing) As Task
        Button1_Click(Nothing, Nothing)
        If Not String.IsNullOrEmpty(newVersion) Then SaveLocalModpackVersion(newVersion)
    End Function

    ' Writes messages to log file (currently disabled)
    Private Sub WriteToLog(message As String)
        Try
            ' Logging disabled but kept for potential future use
            'Dim logPath As String = Path.Combine(Application.StartupPath, "update.log")
            'File.AppendAllText(logPath, $"{DateTime.Now}: {message}{Environment.NewLine}")
        Catch
            ' Silently fail if logging fails
        End Try
    End Sub

    ' Finds the Schedule I game path by checking Steam installation
    Private Function FindSchedule1Path() As String
        Try
            ' Check Steam registry for installation path
            Dim steamPath As String = Registry.GetValue("HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", "").ToString()
            If String.IsNullOrWhiteSpace(steamPath) Then Return Nothing

            ' Check library folders file
            Dim libraryFile As String = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf")
            If Not File.Exists(libraryFile) Then Return Nothing

            ' Search all library folders for the game
            Dim content As String = File.ReadAllText(libraryFile)
            Dim matches = Regex.Matches(content, """path""\s+""([^""]+)""")

            For Each match As Match In matches
                Dim libPath As String = match.Groups(1).Value.Replace("\\", "\")
                Dim potentialGamePath = Path.Combine(libPath, "steamapps", "common", "Schedule I")
                If Directory.Exists(potentialGamePath) Then
                    Return potentialGamePath
                End If
            Next

            ' Fallback to default Steam path
            Dim defaultPath As String = Path.Combine(steamPath, "steamapps", "common", "Schedule I")
            If Directory.Exists(defaultPath) Then Return defaultPath

            Return Nothing
        Catch ex As Exception
            WriteToLog($"Error locating Schedule I path: {ex.Message}")
            Return Nothing
        End Try
    End Function

    ' Prompts user to manually locate the game path
    Private Function AskUserForGamePath() As String
        Using dialog As New FolderBrowserDialog()
            dialog.Description = "Please locate the Schedule I installation directory"
            If dialog.ShowDialog() = DialogResult.OK Then
                ' Validate this is actually the game directory
                If File.Exists(Path.Combine(dialog.SelectedPath, "version.dll")) OrElse
                   Directory.Exists(Path.Combine(dialog.SelectedPath, "MelonLoader")) Then
                    Return dialog.SelectedPath
                End If
            End If
        End Using
        Return Nothing
    End Function

    ' Gets the update URL from server configuration
    Private Async Function GetUpdateUrlFromServer() As Task(Of String)
        Try
            Using client As New WebClient()
                Dim url As String = Await client.DownloadStringTaskAsync(UpdateUrlConfig)
                Return url.Trim() ' Remove any whitespace or newlines
            End Using
        Catch ex As Exception
            WriteToLog($"Failed to fetch Update URL: {ex.Message}")
            Return Nothing
        End Try
    End Function

    ' Checks for application updates
    Private Async Sub CheckForUpdates()
        Try
            Using client As New WebClient()
                Dim latestVersion As String = Await client.DownloadStringTaskAsync(VersionCheckUrl)

                If VersionNewer(latestVersion, CurrentVersion) Then
                    Dim result As DialogResult = MessageBox.Show(
                        $"A new version is available (v{latestVersion}). Would you like to update now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question)

                    If result = DialogResult.Yes Then
                        Await DownloadAndApplyUpdate()
                    End If
                End If
            End Using
        Catch ex As Exception
            WriteToLog($"Update check failed: {ex.Message}")
        End Try
        CheckModpackVersion()
    End Sub

    ' Downloads and applies application updates
    Private Async Function DownloadAndApplyUpdate() As Task
        Try
            ' Create update progress form
            Dim updateForm As New Form() With {
                .Text = "Updating Application",
                .Width = 400,
                .Height = 150,
                .FormBorderStyle = FormBorderStyle.FixedDialog,
                .StartPosition = FormStartPosition.CenterScreen,
                .ControlBox = False
            }

            Dim progressLabel As New Label() With {.Text = "Downloading update...", .Width = 360, .Top = 20, .Left = 20}
            Dim progressBar As New ProgressBar() With {.Width = 360, .Height = 30, .Top = 50, .Left = 20}

            updateForm.Controls.Add(progressLabel)
            updateForm.Controls.Add(progressBar)
            updateForm.Show()
            Application.DoEvents()

            ' Set up file paths for update process
            Dim updateExePath As String = Path.Combine(Path.GetTempPath(), "S1-GeekPack-Manager_Update.exe")
            Dim currentExe As String = Application.ExecutablePath
            Dim currentDir As String = Path.GetDirectoryName(currentExe)
            Dim batchFile As String = Path.Combine(Path.GetTempPath(), "update.bat")
            Dim logFile As String = Path.Combine(Path.GetTempPath(), "update_debug.log")

            ' Configure download progress handler
            AddHandler updateClient.DownloadProgressChanged, Sub(sender, e)
                                                                 progressBar.Value = e.ProgressPercentage
                                                                 progressLabel.Text = $"Downloading update: {e.ProgressPercentage}%"
                                                                 Application.DoEvents()
                                                             End Sub

            WriteToLog("Starting update download...")
            Dim dynamicUpdateUrl As String = Await GetUpdateUrlFromServer()
            If String.IsNullOrEmpty(dynamicUpdateUrl) Then
                WriteToLog("Update URL is empty or failed to fetch.")
                MessageBox.Show("Failed to fetch update. Please try again later.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                updateForm.Close()
                Return
            End If

            ' Download the update
            WriteToLog("Downloading update from: " & dynamicUpdateUrl)
            Await updateClient.DownloadFileTaskAsync(New Uri(dynamicUpdateUrl), updateExePath)

            ' Verify download was successful
            If Not File.Exists(updateExePath) OrElse New FileInfo(updateExePath).Length = 0 Then
                WriteToLog("Update download failed - file missing or empty")
                MessageBox.Show("Update download failed. Please try again.", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                updateForm.Close()
                Return
            End If

            ' Prepare update batch script
            progressLabel.Text = "Preparing update script..."
            Application.DoEvents()

            Dim batchContent As String =
$"@echo off
echo Starting update batch... > ""{logFile}""
:waitloop
tasklist /FI ""IMAGENAME eq {Path.GetFileName(currentExe)}"" | find /I ""{Path.GetFileName(currentExe)}"" >nul
if %ERRORLEVEL%==0 (
    timeout /t 1 >nul
    goto waitloop
)

echo Replacing EXE... >> ""{logFile}""
copy /Y ""{updateExePath}"" ""{currentExe}"" >> ""{logFile}"" 2>&1

if exist ""{currentExe}"" (
    echo Starting new version... >> ""{logFile}""
    start """" ""{currentExe}""
) else (
    echo ERROR: File copy failed >> ""{logFile}""
)

timeout /t 2 >nul
del ""{updateExePath}""
del ""%~f0""
"

            ' Write and execute the batch script
            File.WriteAllText(batchFile, batchContent)
            WriteToLog("Batch file written: " & batchFile)

            progressLabel.Text = "Applying update..."
            Application.DoEvents()
            WriteToLog("Launching update batch...")

            Process.Start(New ProcessStartInfo(batchFile) With {.CreateNoWindow = True, .UseShellExecute = False})

            updateForm.Close()
            Application.Exit()

        Catch ex As Exception
            WriteToLog($"Update failed: {ex.Message}")
            MessageBox.Show($"Failed to apply update: {ex.Message}", "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            updateClient.Dispose()
        End Try
    End Function

    ' Compares two version strings to determine if version1 is newer than version2
    Private Function VersionNewer(version1 As String, version2 As String) As Boolean
        Dim v1Parts() As String = version1.Split("."c)
        Dim v2Parts() As String = version2.Split("."c)

        For i As Integer = 0 To Math.Min(v1Parts.Length, v2Parts.Length) - 1
            Dim v1 As Integer = Integer.Parse(v1Parts(i))
            Dim v2 As Integer = Integer.Parse(v2Parts(i))

            If v1 > v2 Then Return True
            If v1 < v2 Then Return False
        Next

        Return v1Parts.Length > v2Parts.Length
    End Function

    ' Install/Update button click handler
    Private Async Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Const melonLoaderUrl As String = "https://github.com/LavaGang/MelonLoader/releases/latest/download/MelonLoader.x64.zip"
        Dim modFilesUrl As String = Await GetPackUrlFromServer()

        If String.IsNullOrEmpty(modFilesUrl) Then
            MessageBox.Show("Failed to fetch mod pack URL.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Button1.Text = "Try Again"
            Button1.Enabled = True
            Return
        End If

        WriteToLog("Modpack download URL: " & modFilesUrl)

        Dim tempZipPath As String = Path.Combine(Path.GetTempPath(), "MelonLoader.zip")
        Dim tempModsZipPath As String = Path.Combine(Path.GetTempPath(), "mod_files.zip")

        ' Confirm installation with user
        Dim confirmResult As DialogResult = MessageBox.Show(
            "WARNING: Ensure game is closed before continuing." & vbCrLf & vbCrLf &
            "This will:" & vbCrLf &
            "1. Remove existing installation (if applicable)" & vbCrLf &
            "2. Install/update MelonLoader" & vbCrLf &
            "3. Remove all mods/plugins" & vbCrLf &
            "4. Install latest mod files",
            "Important - Close Game First",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)

        If confirmResult <> DialogResult.Yes Then
            Button1.Text = "Install/Update Cancelled"
            Return
        End If

        Try
            Button1.Enabled = False
            Button1.Text = "Preparing..."
            WriteToLog("Starting MelonLoader installation...")

            ' Validate game directory exists
            If Not Directory.Exists(GamePath) Then
                WriteToLog($"Game directory not found at: {GamePath}")
                MessageBox.Show($"Game directory not found at: {GamePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Button1.Text = "Try Again"
                Return
            End If

            ' Files and folders to clean up before installation
            Dim filesToDelete As String() = {
                Path.Combine(GamePath, "version.dll"),
                Path.Combine(GamePath, "dobby.dll")
            }

            Dim foldersToClean As String() = {
                Path.Combine(GamePath, "MelonLoader"),
                Path.Combine(GamePath, "Mods"),
                Path.Combine(GamePath, "Plugins"),
                Path.Combine(GamePath, "UserData")
            }

            ' Clean up old files
            Button1.Text = "Removing old files..."
            WriteToLog("Cleaning up old files...")
            For Each filePath In filesToDelete
                If File.Exists(filePath) Then
                    File.Delete(filePath)
                End If
            Next

            For Each folderPath In foldersToClean
                If Directory.Exists(folderPath) Then
                    Directory.Delete(folderPath, True)
                End If
            Next

            ' Download MelonLoader
            Button1.Text = "Downloading MelonLoader..."
            WriteToLog("Downloading MelonLoader...")
            Using client As New WebClient()
                AddHandler client.DownloadProgressChanged, Sub(s, progressArgs)
                                                               Button1.Text = $"Downloading MelonLoader... {progressArgs.ProgressPercentage}%"
                                                           End Sub
                Await client.DownloadFileTaskAsync(New Uri(melonLoaderUrl), tempZipPath)
            End Using

            ' Install MelonLoader
            Button1.Text = "Installing MelonLoader..."
            WriteToLog("Extracting MelonLoader...")
            Using archive As ZipArchive = ZipFile.OpenRead(tempZipPath)
                For Each entry In archive.Entries
                    If entry.Name = "version.dll" OrElse entry.Name = "dobby.dll" Then
                        Dim destPath As String = Path.Combine(GamePath, entry.Name)
                        entry.ExtractToFile(destPath, True)
                    ElseIf entry.FullName.StartsWith("MelonLoader/") Then
                        Dim destPath As String = Path.Combine(GamePath, entry.FullName.Replace("/", "\"))
                        If Not String.IsNullOrEmpty(entry.Name) Then
                            Directory.CreateDirectory(Path.GetDirectoryName(destPath))
                            entry.ExtractToFile(destPath, True)
                        End If
                    End If
                Next
            End Using
            File.Delete(tempZipPath)

            ' Download mod files
            Button1.Text = "Downloading mod files..."
            WriteToLog("Downloading mod files...")
            Using client As New WebClient()
                AddHandler client.DownloadProgressChanged, Sub(s, progressArgs)
                                                               Button1.Text = $"Downloading mods... {progressArgs.ProgressPercentage}%"
                                                           End Sub
                Await client.DownloadFileTaskAsync(New Uri(modFilesUrl), tempModsZipPath)
            End Using

            ' Install mod files
            Button1.Text = "Installing mod files..."
            WriteToLog("Extracting mod files...")
            ZipFile.ExtractToDirectory(tempModsZipPath, GamePath)
            File.Delete(tempModsZipPath)

            ' Finalize installation
            Button1.Text = "Install/Update Geek Pack and Dependencies"
            WriteToLog("Installation completed successfully")

            ' Update version display
            Dim versionPath As String = Path.Combine(GamePath, "Mods", "GPM", "mpv.txt")
            If File.Exists(versionPath) Then
                Dim newVersion As String = File.ReadAllText(versionPath).Trim()
                WriteToLog("Detected updated modpack version: " & newVersion)
                UpdateGPVLabel(newVersion)
            Else
                WriteToLog("mpv.txt not found after install")
            End If

            MessageBox.Show("Installation completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            WriteToLog($"Installation error: {ex.Message}")
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Button1.Text = "Try Again"
        Finally
            Button1.Enabled = True
        End Try
    End Sub

    ' Uninstall button click handler
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        ' Confirm uninstallation with user
        Dim confirmResult As DialogResult = MessageBox.Show(
            "WARNING: This will completely remove:" & vbCrLf &
            "- version.dll" & vbCrLf &
            "- MelonLoader folder" & vbCrLf &
            "- All mods/plugins" & vbCrLf &
            "- UserData folder",
            "Confirm Uninstall",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning)

        If confirmResult <> DialogResult.Yes Then
            Button2.Text = "Cancelled Uninstall"
            Return
        End If

        Try
            Button2.Enabled = False
            Button2.Text = "Uninstalling..."
            WriteToLog("Starting uninstallation...")

            ' Validate game directory exists
            If Not Directory.Exists(GamePath) Then
                WriteToLog("Game directory not found during uninstall")
                MessageBox.Show("Game directory not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Remove core files
            Dim versionDllPath As String = Path.Combine(GamePath, "version.dll")
            If File.Exists(versionDllPath) Then
                File.Delete(versionDllPath)
            End If

            ' Remove all related folders
            Dim foldersToDelete As String() = {
                Path.Combine(GamePath, "MelonLoader"),
                Path.Combine(GamePath, "Mods"),
                Path.Combine(GamePath, "Plugins"),
                Path.Combine(GamePath, "UserData")
            }

            For Each folderPath In foldersToDelete
                If Directory.Exists(folderPath) Then
                    Directory.Delete(folderPath, True)
                End If
            Next

            ' Finalize uninstallation
            Button2.Text = "Uninstall Geek Pack and Dependencies"
            UpdateGPVLabel("0.0.0")
            WriteToLog("Uninstallation completed successfully")
            MessageBox.Show("Uninstallation completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            WriteToLog($"Uninstall error: {ex.Message}")
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Button2.Text = "Try Again"
        Finally
            Button2.Enabled = True
        End Try
    End Sub

    ' Easter egg - changes label font and color on click
    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click
        Dim installedFonts As New InstalledFontCollection()
        Dim allFonts As FontFamily() = installedFonts.Families

        ' Pick a random font that supports regular style
        Dim randomFont As FontFamily = allFonts _
            .Where(Function(f) f.IsStyleAvailable(FontStyle.Regular)) _
            .OrderBy(Function(f) Guid.NewGuid()) _
            .FirstOrDefault()

        If randomFont IsNot Nothing Then
            ' Apply random font
            Label1.Font = New Font(randomFont, 20, FontStyle.Regular)
        End If

        ' Generate a random color
        Dim rnd As New Random()
        Dim randomColor As Color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256))
        Label1.ForeColor = randomColor
    End Sub

    ' Opens website when label is clicked
    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click
        Process.Start(New ProcessStartInfo With {
            .FileName = "https://collinframe.com/",
            .UseShellExecute = True
        })
    End Sub

    ' Shows additional features form
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        AddFeats.Show()
    End Sub
End Class