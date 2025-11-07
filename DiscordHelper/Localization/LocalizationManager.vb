Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.Resources

Namespace Localization

    Public Enum SupportedLanguage
        English
        French
    End Enum

    Public NotInheritable Class LocalizationManager

        Private Shared ReadOnly _instance As New LocalizationManager()
        Private ReadOnly _resourceManager As ResourceManager
        Private ReadOnly _loggedWarnings As ConcurrentDictionary(Of String, Boolean)
        Private ReadOnly _cultures As IReadOnlyDictionary(Of SupportedLanguage, CultureInfo)

        Private Sub New()
            _loggedWarnings = New ConcurrentDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            _resourceManager = CreateResourceManager()
            _cultures = New Dictionary(Of SupportedLanguage, CultureInfo) From {
                {SupportedLanguage.English, CultureInfo.GetCultureInfo("en-US")},
                {SupportedLanguage.French, CultureInfo.GetCultureInfo("fr-FR")}
            }
        End Sub

        Private Shared Function CreateResourceManager() As ResourceManager
            Dim assembly As Reflection.Assembly = GetType(LocalizationManager).Assembly
            Dim potentialBaseNames As String() = {
                $"{GetType(LocalizationManager).Namespace}.LocalizationResources",
                $"{assembly.GetName().Name}.Localization.LocalizationResources"
            }

            For Each baseName As String In potentialBaseNames
                Try
                    Dim candidate As New ResourceManager(baseName, assembly)

                    ' Attempt to materialize the neutral resource set to ensure the manifest name is valid.
                    Dim resourceSet As ResourceSet = candidate.GetResourceSet(CultureInfo.InvariantCulture, createIfNotExists:=True, tryParents:=False)
                    If resourceSet IsNot Nothing Then
                        Return candidate
                    End If
                Catch ex As MissingManifestResourceException
                    ' Ignore and try the next base name.
                End Try
            Next

            Throw New MissingManifestResourceException("Unable to locate embedded localization resources.")
        End Function

        Public Shared ReadOnly Property Instance As LocalizationManager
            Get
                Return _instance
            End Get
        End Property

        Public Function Format(key As String, language As SupportedLanguage, ParamArray args() As Object) As String
            Dim template As String = GetStringInternal(key, language)
            Dim culture As CultureInfo = GetCulture(language)
            Dim formatArgs As Object() = args
            If formatArgs Is Nothing Then
                formatArgs = New Object() {}
            End If
            Return String.Format(culture, template, formatArgs)
        End Function

        Public Function FormatPlural(key As String, language As SupportedLanguage, count As Integer, ParamArray args() As Object) As String
            Dim formKey As String
            If count = 0 Then
                formKey = $"{key}.zero"
            ElseIf count = 1 Then
                formKey = $"{key}.one"
            Else
                formKey = $"{key}.many"
            End If

            Dim template As String = GetStringInternal(formKey, language)
            Dim finalArgs As Object()

            If args Is Nothing OrElse args.Length = 0 Then
                finalArgs = New Object() {count}
            Else
                finalArgs = New Object(args.Length) {}
                finalArgs(0) = count
                Array.Copy(args, 0, finalArgs, 1, args.Length)
            End If

            Return String.Format(GetCulture(language), template, finalArgs)
        End Function

        Public Function GetCulture(language As SupportedLanguage) As CultureInfo
            Return _cultures(language)
        End Function

        Private Function GetStringInternal(key As String, language As SupportedLanguage) As String
            Dim culture As CultureInfo = GetCulture(language)
            Dim value As String = _resourceManager.GetString(key, culture)

            If value IsNot Nothing Then
                Return value
            End If

            LogMissingKey(key, culture)

            If language <> SupportedLanguage.English Then
                Dim fallbackCulture As CultureInfo = GetCulture(SupportedLanguage.English)
                Dim fallbackValue As String = _resourceManager.GetString(key, fallbackCulture)
                If fallbackValue IsNot Nothing Then
                    Return fallbackValue
                End If
                LogMissingKey(key, fallbackCulture)
            End If

            Return key
        End Function

        Private Sub LogMissingKey(key As String, culture As CultureInfo)
            Dim logKey As String = $"{culture.Name}:{key}"
            If _loggedWarnings.TryAdd(logKey, True) Then
                Debug.WriteLine($"[Localization] Missing translation for key '{key}' in culture '{culture.Name}'.")
            End If
        End Sub

    End Class

End Namespace
