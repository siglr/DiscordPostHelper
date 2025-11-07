Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.IO
Imports System.Xml.Linq

Namespace Localization

    Public Enum SupportedLanguage
        English
        French
    End Enum

    Public NotInheritable Class LocalizationManager

        Private Shared ReadOnly _instance As New LocalizationManager()
        Private ReadOnly _translations As IReadOnlyDictionary(Of SupportedLanguage, IReadOnlyDictionary(Of String, String))
        Private ReadOnly _loggedWarnings As ConcurrentDictionary(Of String, Boolean)
        Private ReadOnly _cultures As IReadOnlyDictionary(Of SupportedLanguage, CultureInfo)

        Private Sub New()
            _loggedWarnings = New ConcurrentDictionary(Of String, Boolean)(StringComparer.OrdinalIgnoreCase)
            _translations = LoadTranslations()
            _cultures = New Dictionary(Of SupportedLanguage, CultureInfo) From {
                {SupportedLanguage.English, CultureInfo.GetCultureInfo("en-US")},
                {SupportedLanguage.French, CultureInfo.GetCultureInfo("fr-FR")}
            }
        End Sub

        Private Shared Function LoadTranslations() As IReadOnlyDictionary(Of SupportedLanguage, IReadOnlyDictionary(Of String, String))
            Dim assembly As Reflection.Assembly = GetType(LocalizationManager).Assembly
            Dim potentialResourceNames As String() = {
                $"{GetType(LocalizationManager).Namespace}.LocalizationResources.xml",
                $"{assembly.GetName().Name}.Localization.LocalizationResources.xml"
            }

            For Each resourceName As String In potentialResourceNames
                Dim resourceStream As Stream = assembly.GetManifestResourceStream(resourceName)
                If resourceStream IsNot Nothing Then
                    Using resourceStream
                        Return ParseTranslationStream(resourceStream)
                    End Using
                End If
            Next

            Dim fallbackPath As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Localization", "LocalizationResources.xml")
            If File.Exists(fallbackPath) Then
                Using fileStream As FileStream = File.OpenRead(fallbackPath)
                    Return ParseTranslationStream(fileStream)
                End Using
            End If

            Throw New FileNotFoundException("Unable to locate localization manifest 'LocalizationResources.xml'.")
        End Function

        Private Shared Function ParseTranslationStream(stream As Stream) As IReadOnlyDictionary(Of SupportedLanguage, IReadOnlyDictionary(Of String, String))
            Dim document As XDocument = XDocument.Load(stream)
            Dim root As XElement = document.Root

            If root Is Nothing OrElse Not String.Equals(root.Name.LocalName, "localization", StringComparison.OrdinalIgnoreCase) Then
                Throw New InvalidDataException("Localization manifest is missing the root 'localization' element.")
            End If

            Dim english As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim french As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each entry As XElement In root.Elements("entry")
                Dim keyAttribute As XAttribute = entry.Attribute("key")
                Dim key As String = keyAttribute?.Value

                If String.IsNullOrWhiteSpace(key) Then
                    Continue For
                End If

                Dim englishValue As String = entry.Element("english")?.Value
                Dim frenchValue As String = entry.Element("french")?.Value

                english(key) = If(englishValue, String.Empty)
                french(key) = If(frenchValue, String.Empty)
            Next

            Return New Dictionary(Of SupportedLanguage, IReadOnlyDictionary(Of String, String)) From {
                {SupportedLanguage.English, english},
                {SupportedLanguage.French, french}
            }
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
            Dim languageTranslations As IReadOnlyDictionary(Of String, String) = Nothing
            Dim value As String = Nothing

            If _translations.TryGetValue(language, languageTranslations) AndAlso languageTranslations.TryGetValue(key, value) Then
                Return value
            End If

            LogMissingKey(key, culture)

            If language <> SupportedLanguage.English Then
                Dim fallbackCulture As CultureInfo = GetCulture(SupportedLanguage.English)
                Dim fallbackTranslations As IReadOnlyDictionary(Of String, String) = Nothing
                Dim fallbackValue As String = Nothing

                If _translations.TryGetValue(SupportedLanguage.English, fallbackTranslations) AndAlso fallbackTranslations.TryGetValue(key, fallbackValue) Then
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
