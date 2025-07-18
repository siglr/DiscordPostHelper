Imports System
Imports System.Windows.Forms

Module Program
    <STAThread()>
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New ListenerContext())
    End Sub
End Module