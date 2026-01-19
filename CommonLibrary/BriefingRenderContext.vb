Imports System

<Flags>
Public Enum InstalledSimFlags
    None = 0
    MSFS2020 = 1
    MSFS2024 = 2
End Enum

Public Enum BriefingHostMode
    DesignerPreview
    EndUser
End Enum

Public Enum PresetNameDisplayMode
    Friendly
    Exact
End Enum

Public Class BriefingRenderContext

    Public Sub New()
        HostMode = BriefingHostMode.DesignerPreview
        InstalledSims = InstalledSimFlags.MSFS2020 Or InstalledSimFlags.MSFS2024
        PresetNameDisplayMode = PresetNameDisplayMode.Friendly
    End Sub

    Public Property HostMode As BriefingHostMode

    Public Property InstalledSims As InstalledSimFlags

    Public Property PresetNameDisplayMode As PresetNameDisplayMode

End Class
