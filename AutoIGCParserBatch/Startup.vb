' Startup.vb
Imports System.Windows.Forms

Module Startup
    <STAThread()>
    Public Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New frmDiscord())
    End Sub
End Module
