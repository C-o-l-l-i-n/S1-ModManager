<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AddFeats
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(AddFeats))
        Label1 = New Label()
        BtnBackupSaves = New Button()
        BtnRestoreSaves = New Button()
        BtnEditMoney = New Button()
        BtnRenameSave = New Button()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Comic Sans MS", 22F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label1.Location = New Point(12, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(358, 52)
        Label1.TabIndex = 0
        Label1.Text = "Additional Features"
        ' 
        ' BtnBackupSaves
        ' 
        BtnBackupSaves.Location = New Point(21, 83)
        BtnBackupSaves.Name = "BtnBackupSaves"
        BtnBackupSaves.Size = New Size(170, 52)
        BtnBackupSaves.TabIndex = 1
        BtnBackupSaves.Text = "Backup Saves"
        BtnBackupSaves.UseVisualStyleBackColor = True
        ' 
        ' BtnRestoreSaves
        ' 
        BtnRestoreSaves.Location = New Point(197, 83)
        BtnRestoreSaves.Name = "BtnRestoreSaves"
        BtnRestoreSaves.Size = New Size(170, 52)
        BtnRestoreSaves.TabIndex = 2
        BtnRestoreSaves.Text = "Restore Saves"
        BtnRestoreSaves.UseVisualStyleBackColor = True
        ' 
        ' BtnEditMoney
        ' 
        BtnEditMoney.Location = New Point(21, 141)
        BtnEditMoney.Name = "BtnEditMoney"
        BtnEditMoney.Size = New Size(170, 52)
        BtnEditMoney.TabIndex = 3
        BtnEditMoney.Text = "Edit Bank Balance"
        BtnEditMoney.UseVisualStyleBackColor = True
        ' 
        ' BtnRenameSave
        ' 
        BtnRenameSave.Location = New Point(197, 141)
        BtnRenameSave.Name = "BtnRenameSave"
        BtnRenameSave.Size = New Size(170, 52)
        BtnRenameSave.TabIndex = 4
        BtnRenameSave.Text = "Rename Organization"
        BtnRenameSave.UseVisualStyleBackColor = True
        ' 
        ' AddFeats
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.ControlDarkDark
        ClientSize = New Size(401, 222)
        Controls.Add(BtnRenameSave)
        Controls.Add(BtnEditMoney)
        Controls.Add(BtnRestoreSaves)
        Controls.Add(BtnBackupSaves)
        Controls.Add(Label1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        MaximumSize = New Size(419, 269)
        MinimumSize = New Size(419, 269)
        Name = "AddFeats"
        Text = "Additional Features"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents BtnBackupSaves As Button
    Friend WithEvents BtnRestoreSaves As Button
    Friend WithEvents BtnEditMoney As Button
    Friend WithEvents BtnRenameSave As Button
End Class
