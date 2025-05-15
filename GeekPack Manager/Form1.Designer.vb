<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Label1 = New Label()
        Label2 = New Label()
        Button1 = New Button()
        Button2 = New Button()
        LabelVersion = New Label()
        Button3 = New Button()
        Label3 = New Label()
        SuspendLayout()
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Font = New Font("Comic Sans MS", 20F)
        Label1.Location = New Point(12, 9)
        Label1.Name = "Label1"
        Label1.Size = New Size(323, 47)
        Label1.TabIndex = 0
        Label1.Text = "Geek Pack Manager"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Font = New Font("Comic Sans MS", 15F)
        Label2.Location = New Point(206, 308)
        Label2.Name = "Label2"
        Label2.Size = New Size(197, 34)
        Label2.TabIndex = 1
        Label2.Text = "collinframe.com"
        ' 
        ' Button1
        ' 
        Button1.Location = New Point(31, 90)
        Button1.Name = "Button1"
        Button1.Size = New Size(342, 51)
        Button1.TabIndex = 2
        Button1.Text = "Install/Update Geek Pack and Dependencies"
        Button1.UseVisualStyleBackColor = True
        ' 
        ' Button2
        ' 
        Button2.Location = New Point(31, 158)
        Button2.Name = "Button2"
        Button2.Size = New Size(342, 51)
        Button2.TabIndex = 3
        Button2.Text = "Uninstall Geek Pack and Dependencies"
        Button2.UseVisualStyleBackColor = True
        ' 
        ' LabelVersion
        ' 
        LabelVersion.AutoSize = True
        LabelVersion.Font = New Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        LabelVersion.Location = New Point(12, 264)
        LabelVersion.Name = "LabelVersion"
        LabelVersion.Size = New Size(181, 28)
        LabelVersion.TabIndex = 4
        LabelVersion.Text = "App Version: 0.0.0"
        ' 
        ' Button3
        ' 
        Button3.Location = New Point(12, 308)
        Button3.Name = "Button3"
        Button3.Size = New Size(167, 29)
        Button3.TabIndex = 5
        Button3.Text = "Additional Features"
        Button3.UseVisualStyleBackColor = True
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Font = New Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label3.Location = New Point(12, 236)
        Label3.Name = "Label3"
        Label3.Size = New Size(327, 28)
        Label3.TabIndex = 6
        Label3.Text = "Installed Geek Pack Version: 0.0.0"
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(8F, 20F)
        AutoScaleMode = AutoScaleMode.Font
        BackColor = SystemColors.ControlDarkDark
        ClientSize = New Size(413, 351)
        Controls.Add(Label3)
        Controls.Add(Button3)
        Controls.Add(LabelVersion)
        Controls.Add(Button2)
        Controls.Add(Button1)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Icon = CType(resources.GetObject("$this.Icon"), Icon)
        Name = "Form1"
        Text = "Geek Pack Manager 1.1"
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents LabelVersion As Label
    Friend WithEvents Button3 As Button
    Friend WithEvents Label3 As Label

End Class
