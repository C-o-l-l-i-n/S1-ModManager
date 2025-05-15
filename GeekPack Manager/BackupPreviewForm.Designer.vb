<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class BackupPreviewForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BackupPreviewForm))
        ListBoxContents = New ListBox()
        btnContinue = New Button()
        btnCancel = New Button()
        SuspendLayout()
        ' 
        ' ListBoxContents
        ' 
        ListBoxContents.FormattingEnabled = True
        ListBoxContents.Location = New Point(53, 26)
        ListBoxContents.Name = "ListBoxContents"
        ListBoxContents.Size = New Size(546, 104)
        ListBoxContents.TabIndex = 0
        ' 
        ' btnContinue
        ' 
        btnContinue.Location = New Point(53, 146)
        btnContinue.Name = "btnContinue"
        btnContinue.Size = New Size(137, 70)
        btnContinue.TabIndex = 1
        btnContinue.Text = "Continue"
        btnContinue.UseVisualStyleBackColor = True
        ' 
        ' btnCancel
        ' 
        btnCancel.Location = New Point(462, 146)
        btnCancel.Name = "btnCancel"
        btnCancel.Size = New Size(137, 70)
        btnCancel.TabIndex = 2
        btnCancel.Text = "Cancel"
        btnCancel.UseVisualStyleBackColor = True
        ' 
        ' BackupPreviewForm
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.ControlDarkDark
        ClientSize = New Size(656, 249)
        Controls.Add(btnCancel)
        Controls.Add(btnContinue)
        Controls.Add(ListBoxContents)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximumSize = New Size(674, 296)
        MinimumSize = New Size(674, 296)
        Name = "BackupPreviewForm"
        Text = "Preview"
        ResumeLayout(False)
    End Sub

    Friend WithEvents ListBoxContents As ListBox
    Friend WithEvents btnContinue As Button
    Friend WithEvents btnCancel As Button
End Class
