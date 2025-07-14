Public Class PreferredUnits
    Private _altitude As AltitudeUnits
    Private _distance As DistanceUnits
    Private _speed As SpeedUnits
    Private _GateDiameter As GateDiameterUnits
    Private _windSpeed As WindSpeedUnits
    Private _barometric As BarometricUnits
    Private _temperature As TemperatureUnits
    Private _GateMeasurement As GateMeasurementChoices

    Public Enum DistanceUnits As Integer
        Metric = 0
        Imperial = 1
        Both = 2
    End Enum

    Public Enum SpeedUnits As Integer
        Metric = 0
        Imperial = 1
        Knots = 2
    End Enum

    Public Enum BarometricUnits As Integer
        hPa = 0
        inHg = 1
        Both = 2
    End Enum

    Public Enum TemperatureUnits As Integer
        Celsius = 0
        Fahrenheit = 1
        Both = 2
    End Enum

    Public Enum WindSpeedUnits As Integer
        MeterPerSecond = 0
        Knots = 1
        Both = 2
    End Enum

    Public Enum AltitudeUnits As Integer
        Metric = 0
        Imperial = 1
        Both = 2
    End Enum

    Public Enum GateDiameterUnits As Integer
        Metric = 0
        Imperial = 1
        Both = 2
    End Enum

    Public Enum GateMeasurementChoices As Integer
        Diameter = 0
        Radius = 1
    End Enum

    Public Sub New()

        'Load values from registry
        _distance = SupportingFeatures.ReadRegistryKey("DistanceUnit", DistanceUnits.Both)

        Dim defaultUnitForSpeed As SpeedUnits
        Select Case _distance
            Case DistanceUnits.Imperial
                defaultUnitForSpeed = SpeedUnits.Imperial
            Case DistanceUnits.Metric
                defaultUnitForSpeed = SpeedUnits.Metric
            Case Else
                defaultUnitForSpeed = SpeedUnits.Metric
        End Select
        _speed = SupportingFeatures.ReadRegistryKey("SpeedUnit", defaultUnitForSpeed)
        _barometric = SupportingFeatures.ReadRegistryKey("BarometricUnit", BarometricUnits.Both)
        _temperature = SupportingFeatures.ReadRegistryKey("TemperatureUnit", TemperatureUnits.Both)
        _windSpeed = SupportingFeatures.ReadRegistryKey("WindSpeedUnit", WindSpeedUnits.Both)
        _altitude = SupportingFeatures.ReadRegistryKey("AltitudeUnit", AltitudeUnits.Both)
        _GateDiameter = SupportingFeatures.ReadRegistryKey("GateDiameterUnit", GateDiameterUnits.Both)
        _GateMeasurement = SupportingFeatures.ReadRegistryKey("GateMeasurement", GateMeasurementChoices.Diameter)

    End Sub

    Public ReadOnly Property GateLabel As String
        Get
            Select Case _GateMeasurement
                Case GateMeasurementChoices.Diameter
                    Return "Gate diameter"
                Case GateMeasurementChoices.Radius
                    Return "Gate radius"
                Case Else
                    Return String.Empty
            End Select
        End Get
    End Property

    Public Property Distance As DistanceUnits
        Get
            Return _distance
        End Get
        Set(value As DistanceUnits)
            _distance = value
            SupportingFeatures.WriteRegistryKey("DistanceUnit", _distance)
        End Set
    End Property

    Public Property Speed As SpeedUnits
        Get
            Return _speed
        End Get
        Set(value As SpeedUnits)
            _speed = value
            SupportingFeatures.WriteRegistryKey("SpeedUnit", _distance)
        End Set
    End Property

    Public Property Barometric As BarometricUnits
        Get
            Return _barometric
        End Get
        Set(value As BarometricUnits)
            _barometric = value
            SupportingFeatures.WriteRegistryKey("BarometricUnit", _barometric)
        End Set
    End Property

    Public Property Temperature As TemperatureUnits
        Get
            Return _temperature
        End Get
        Set(value As TemperatureUnits)
            _temperature = value
            SupportingFeatures.WriteRegistryKey("TemperatureUnit", _temperature)
        End Set
    End Property

    Public Property WindSpeed As WindSpeedUnits
        Get
            Return _windSpeed
        End Get
        Set(value As WindSpeedUnits)
            _windSpeed = value
            SupportingFeatures.WriteRegistryKey("WindSpeedUnit", _windSpeed)
        End Set
    End Property

    Public Property Altitude As AltitudeUnits
        Get
            Return _altitude
        End Get
        Set(value As AltitudeUnits)
            _altitude = value
            SupportingFeatures.WriteRegistryKey("AltitudeUnit", _altitude)
        End Set
    End Property

    Public Property GateDiameter As GateDiameterUnits
        Get
            Return _GateDiameter
        End Get
        Set(value As GateDiameterUnits)
            _GateDiameter = value
            SupportingFeatures.WriteRegistryKey("GateDiameterUnit", _GateDiameter)
        End Set
    End Property

    Public Property GateMeasurement As GateMeasurementChoices
        Get
            Return _GateMeasurement
        End Get
        Set(value As GateMeasurementChoices)
            _GateMeasurement = value
            SupportingFeatures.WriteRegistryKey("GateMeasurement", _GateMeasurement)
        End Set
    End Property

End Class
