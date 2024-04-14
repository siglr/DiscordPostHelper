Imports System.Windows.Media.Media3D

Namespace WindLayers3DDisplay

    Public Class WindLayers3DControl

        Private _windLayers As New Dictionary(Of Integer, RotateTransform3D)
        Private _originalAngles As New Dictionary(Of Integer, Integer)

        ' The method to call when you want to update the camera angle
        Public Sub SetCameraPositionAndDirection(angle As Double, height As Double, tilt As Double, distance As Double, fov As Double)
            ' Adjust the angle to align with conventional compass directions:
            ' Subtracting 90 degrees (to make 0 degrees point North) and converting to radians
            Dim adjustedAngle As Double = (angle - 90) * Math.PI / 180

            ' Calculate the new X and Y positions of the camera based on the adjusted angle
            Dim xPosition As Double = Math.Cos(adjustedAngle) * distance '0.9 ' Assuming a radius of 0.9 for simplicity
            Dim yPosition As Double = Math.Sin(adjustedAngle) * distance '0.9

            ' Assuming the MainViewport is correctly linked with the x:Name in XAML
            Dim perspectiveCamera As PerspectiveCamera = TryCast(MainViewport.Camera, PerspectiveCamera)
            If perspectiveCamera IsNot Nothing Then
                perspectiveCamera.FieldOfView = fov
                ' Set the camera's position with the Z position (height) remaining constant at 0.9
                perspectiveCamera.Position = New Point3D(xPosition, yPosition, height) '0.7

                ' The LookDirection is the inverse of the position on the X and Y axes, 
                ' with Z being the fixed vertical tilt at -0.55
                perspectiveCamera.LookDirection = New Vector3D(-xPosition, -yPosition, tilt) '-0.55
            End If
        End Sub

        ' Method to create a simple circular disk geometry
        Private Function CreateDiskGeometry() As MeshGeometry3D
            Dim disk As New MeshGeometry3D()

            ' Define the center point and a point on the circumference
            Dim center As New Point3D(0, 0, 0)
            Dim radius As Double = 0.25 ' Set the radius for your disk

            ' Create a circle by defining points around the center
            Const pointCount As Integer = 30 ' The number of points to create the circle
            Dim angleIncrement As Double = 2 * Math.PI / pointCount

            ' Add the center point
            disk.Positions.Add(center)

            For i As Integer = 0 To pointCount - 1
                Dim angle As Double = i * angleIncrement
                Dim x As Double = center.X + radius * Math.Cos(angle)
                Dim y As Double = center.Y + radius * Math.Sin(angle)
                disk.Positions.Add(New Point3D(x, y, center.Z)) ' All points have the same Z to stay flat
            Next

            ' Create the triangle indices to form the circle
            For i As Integer = 1 To pointCount - 1
                disk.TriangleIndices.Add(0)
                disk.TriangleIndices.Add(i)
                disk.TriangleIndices.Add(i + 1)
            Next
            ' Close the circle by connecting the last point to the first
            disk.TriangleIndices.Add(0)
            disk.TriangleIndices.Add(pointCount)
            disk.TriangleIndices.Add(1)

            ' Create the triangle indices to form the circle for the front face
            For i As Integer = 1 To pointCount - 1
                disk.TriangleIndices.Add(0)
                disk.TriangleIndices.Add(i)
                disk.TriangleIndices.Add(i + 1)
            Next
            ' Close the circle by connecting the last point to the first for the front face
            disk.TriangleIndices.Add(0)
            disk.TriangleIndices.Add(pointCount)
            disk.TriangleIndices.Add(1)

            ' Create the triangle indices for the back face
            For i As Integer = 1 To pointCount - 1
                disk.TriangleIndices.Add(0)
                disk.TriangleIndices.Add(i + 1)
                disk.TriangleIndices.Add(i)
            Next
            ' Close the circle by connecting the last point to the first for the back face
            disk.TriangleIndices.Add(0)
            disk.TriangleIndices.Add(1)
            disk.TriangleIndices.Add(pointCount)

            Return disk
        End Function

        ' Method to create a simple 2D arrow geometry
        Private Function CreateArrowGeometry(windSpeed As Byte, maxWindSpeed As Byte) As MeshGeometry3D
            Dim arrow As New MeshGeometry3D()

            ' Define the max and min widths for the arrow head based on the wind speed
            Dim minWidth As Double = 0.002 ' Min width for the arrow shaft
            Dim maxWidth As Double = 0.12  ' Max width for the arrow head
            Dim extrusionDepth As Double = 0.002 ' Extrusion depth along the Z-axis

            ' Interpolate the width based on the wind speed
            Dim headWidth As Double = minWidth + (maxWidth - minWidth) * (windSpeed / maxWindSpeed)

            DefineArrowHead(arrow, extrusionDepth, headWidth)

            DefineArrowShaft(arrow, minWidth, extrusionDepth)

            Return arrow

        End Function

        Private Sub DefineArrowShaft(arrow As MeshGeometry3D, minWidth As Double, extrusionDepth As Double)
            ' Define the points for the arrow shaft (a thin rectangle) and extrude
            Dim shaftLength As Double = 0.2   ' The length of the arrow shaft
            Dim shaftWidth As Double = minWidth ' The width of the arrow shaft

            ' Original shaft points
            arrow.Positions.Add(New Point3D(-shaftWidth, 0, 0)) ' Shaft bottom left
            arrow.Positions.Add(New Point3D(shaftWidth, 0, 0))  ' Shaft bottom right
            arrow.Positions.Add(New Point3D(-shaftWidth, -shaftLength, 0)) ' Shaft top left
            arrow.Positions.Add(New Point3D(shaftWidth, -shaftLength, 0))  ' Shaft top right

            ' Extruded shaft points
            arrow.Positions.Add(New Point3D(-shaftWidth, 0, extrusionDepth)) ' Shaft bottom left extruded
            arrow.Positions.Add(New Point3D(shaftWidth, 0, extrusionDepth))  ' Shaft bottom right extruded
            arrow.Positions.Add(New Point3D(-shaftWidth, -shaftLength, extrusionDepth)) ' Shaft top left extruded
            arrow.Positions.Add(New Point3D(shaftWidth, -shaftLength, extrusionDepth))  ' Shaft top right extruded

            ' Define the triangles for the shaft extruded top face
            AddSideTriangle(arrow, 10, 12, 11)
            AddSideTriangle(arrow, 11, 12, 13)

            ' Sides for the shaft
            AddSideTriangle(arrow, 6, 10, 7)  ' Front face triangle 1
            AddSideTriangle(arrow, 7, 10, 11) ' Front face triangle 2

            AddSideTriangle(arrow, 7, 11, 9)  ' Right face triangle 1
            AddSideTriangle(arrow, 9, 11, 13) ' Right face triangle 2

            AddSideTriangle(arrow, 9, 13, 8)  ' Back face triangle 1
            AddSideTriangle(arrow, 8, 13, 12) ' Back face triangle 2

            AddSideTriangle(arrow, 8, 12, 6)  ' Left face triangle 1
            AddSideTriangle(arrow, 6, 12, 10) ' Left face triangle 2

            ' Define the tip of the shaft (external square tip) based on extrusion depth
            Dim tipStart As Double = -shaftLength
            arrow.Positions.Add(New Point3D(-shaftWidth, tipStart, 0))
            arrow.Positions.Add(New Point3D(shaftWidth, tipStart, 0))
            arrow.Positions.Add(New Point3D(-shaftWidth, tipStart, extrusionDepth))
            arrow.Positions.Add(New Point3D(shaftWidth, tipStart, extrusionDepth))

            ' Tip faces
            Dim lastPos As Integer = arrow.Positions.Count - 1
            AddSideTriangle(arrow, lastPos - 3, lastPos - 1, lastPos - 2) ' Bottom tip face
            AddSideTriangle(arrow, lastPos - 1, lastPos, lastPos - 2)     ' Top tip face

        End Sub

        Private Sub DefineArrowHead(arrow As MeshGeometry3D, extrusionDepth As Double, headWidth As Double)
            ' Define the points for the arrow head
            arrow.Positions.Add(New Point3D(-headWidth, 0, 0)) ' Left point
            arrow.Positions.Add(New Point3D(headWidth, 0, 0))  ' Right point
            arrow.Positions.Add(New Point3D(0, 0.22, 0))       ' Top point

            ' Extruded points for the head
            arrow.Positions.Add(New Point3D(-headWidth, 0, extrusionDepth))
            arrow.Positions.Add(New Point3D(headWidth, 0, extrusionDepth))
            arrow.Positions.Add(New Point3D(0, 0.22, extrusionDepth))

            ' Define the triangles for the arrow head
            AddSideTriangle(arrow, 0, 1, 2)

            ' Extruded head top face
            AddSideTriangle(arrow, 3, 5, 4)

            ' Sides for the head
            AddSideTriangle(arrow, 0, 3, 1)
            AddSideTriangle(arrow, 1, 3, 4)
            AddSideTriangle(arrow, 1, 4, 2)
            AddSideTriangle(arrow, 2, 4, 5)
            AddSideTriangle(arrow, 2, 5, 0)
            AddSideTriangle(arrow, 0, 5, 3)

        End Sub

        Private Sub AddSideTriangle(arrow As MeshGeometry3D, p1 As Integer, p2 As Integer, p3 As Integer)
            arrow.TriangleIndices.Add(p1)
            arrow.TriangleIndices.Add(p2)
            arrow.TriangleIndices.Add(p3)
        End Sub

        ' Function to create a cylinder geometry
        Private Function CreateCylinderGeometry(baseRadius As Double, topRadius As Double, height As Double, sides As Integer) As MeshGeometry3D
            Dim mesh As New MeshGeometry3D()

            ' Add the center point for the top and bottom caps
            mesh.Positions.Add(New Point3D(0, 0, 0)) ' Bottom center
            mesh.Positions.Add(New Point3D(0, 0, height)) ' Top center

            ' Create the bottom circle
            For i As Integer = 0 To sides - 1
                Dim angle As Double = i * 2 * Math.PI / sides
                mesh.Positions.Add(New Point3D(baseRadius * Math.Cos(angle), baseRadius * Math.Sin(angle), 0))
            Next

            ' Create the top circle
            For i As Integer = 0 To sides - 1
                Dim angle As Double = i * 2 * Math.PI / sides
                mesh.Positions.Add(New Point3D(topRadius * Math.Cos(angle), topRadius * Math.Sin(angle), height))
            Next

            ' Create the triangles for the sides
            For i As Integer = 0 To sides - 1
                Dim nextIndex As Integer = (i + 1) Mod sides
                Dim currentBaseIndex As Integer = i + 2 ' Offset by 2 for the center points
                Dim nextBaseIndex As Integer = nextIndex + 2
                Dim currentTopIndex As Integer = i + sides + 2
                Dim nextTopIndex As Integer = nextIndex + sides + 2

                ' Triangle 1
                mesh.TriangleIndices.Add(currentBaseIndex)
                mesh.TriangleIndices.Add(nextBaseIndex)
                mesh.TriangleIndices.Add(currentTopIndex)

                ' Triangle 2
                mesh.TriangleIndices.Add(currentTopIndex)
                mesh.TriangleIndices.Add(nextBaseIndex)
                mesh.TriangleIndices.Add(nextTopIndex)
            Next

            ' Create the triangles for the bottom cap
            For i As Integer = 0 To sides - 1
                Dim nextIndex As Integer = If(i + 1 >= sides, 2, i + 3)
                mesh.TriangleIndices.Add(0) ' Center of the bottom cap
                mesh.TriangleIndices.Add(i + 2)
                mesh.TriangleIndices.Add(nextIndex)
            Next

            ' Create the triangles for the top cap
            For i As Integer = 0 To sides - 1
                Dim nextIndex As Integer = If(i + 1 >= sides, 2, i + 3) ' Get the next index in the circle or loop back to the start
                ' The order of these vertices should be reversed if the face is not visible.
                mesh.TriangleIndices.Add(1) ' Center of the top cap
                mesh.TriangleIndices.Add(i + sides + 2) ' Current point on the top circle
                mesh.TriangleIndices.Add(nextIndex) ' Next point on the top circle
            Next
            Return mesh
        End Function

        ' Normalize an altitude value between a minimum and maximum altitude range,
        ' mapping it to a Z value between the specified minimumZ and maximumZ values.
        Private Function NormalizeAltitudeToZ(altitude As Double, minAltitude As Double, maxAltitude As Double, minimumZ As Double, maximumZ As Double) As Double
            Return minimumZ + (altitude - minAltitude) / (maxAltitude - minAltitude) * (maximumZ - minimumZ)
        End Function

        ' Function to add a central cylinder to the viewport
        Public Sub AddCentralCylinder(minAltitude As Double, maxAltitude As Double)
            ' Define the dimensions of the cylinder
            Dim baseRadius As Double = 0.01 ' Thin radius for the cylinder
            Dim topRadius As Double = baseRadius ' Top radius is the same for a straight cylinder
            Dim height As Double = NormalizeAltitudeToZ(maxAltitude, minAltitude, maxAltitude, 0.00001, 0.8)
            Dim sides As Integer = 20 ' Number of sides to approximate a circle

            ' Create the cylinder geometry
            Dim cylinderGeometry As MeshGeometry3D = CreateCylinderGeometry(baseRadius, topRadius, height, sides)

            ' Create a material for the cylinder
            Dim cylinderMaterial As Material = New DiffuseMaterial(New SolidColorBrush(Color.FromArgb(225, 255, 153, 51)))

            ' Create the geometry model and apply the material
            Dim cylinder As New GeometryModel3D(cylinderGeometry, cylinderMaterial)

            ' Center the cylinder in X and Y
            Dim cylinderTransform As New TranslateTransform3D(0, 0, 0)

            ' Apply the transform
            cylinder.Transform = cylinderTransform

            ' Create the ModelVisual3D to host the cylinder model
            Dim cylinderModel As New ModelVisual3D()
            cylinderModel.Content = cylinder

            ' Add the cylinder to the viewport
            MainViewport.Children.Add(cylinderModel)
        End Sub

        ' Method to add a wind layer at a specific altitude with a wind direction arrow
        Public Sub AddWindLayer(altitude As Double, windDirection As Double, windSpeed As Byte, minAltitude As Double, maxAltitude As Double)
            ' Calculate the Z position based on the altitude
            Dim zPos As Double = NormalizeAltitudeToZ(altitude, minAltitude, maxAltitude, 0.00001, 0.8)

            ' Create the disk geometry for the altitude layer
            Dim diskGeometry As MeshGeometry3D = CreateDiskGeometry()
            Dim diskMaterial As Material = New DiffuseMaterial(New SolidColorBrush(Color.FromArgb(85, 173, 216, 230))) ' Semi-transparent blue
            Dim disk As New GeometryModel3D(diskGeometry, diskMaterial)

            ' Create the arrow geometry for the wind direction
            Dim arrowGeometry As MeshGeometry3D = CreateArrowGeometry(windSpeed, 26)
            Dim arrowMaterial As Material = New DiffuseMaterial(New SolidColorBrush(Color.FromArgb(184, 0, 145, 255)))
            Dim arrow As New GeometryModel3D(arrowGeometry, arrowMaterial)

            ' Set the altitude position of the disk
            Dim diskAltitudeTransform As New TranslateTransform3D(0, 0, zPos)
            disk.Transform = diskAltitudeTransform

            ' Create a rotation transform to rotate the arrow to the wind direction.
            ' This assumes 0 degrees is North, and rotates clockwise from there.
            Dim rotateTransform As New RotateTransform3D(New AxisAngleRotation3D(New Vector3D(0, 0, 1), -windDirection))

            _windLayers.Add(altitude, rotateTransform)
            _originalAngles.Add(altitude, windDirection)

            ' Apply the rotation and altitude transforms to the arrow.
            Dim arrowTransformGroup As New Transform3DGroup()
            arrowTransformGroup.Children.Add(New TranslateTransform3D(0, 0, zPos + 0.0001)) ' Slightly above the disk to avoid Z-fighting
            arrowTransformGroup.Children.Add(rotateTransform) ' Apply rotation

            arrow.Transform = arrowTransformGroup

            ' Create ModelVisual3D instances to host the geometries
            Dim diskModel As New ModelVisual3D()
            diskModel.Content = disk

            Dim arrowModel As New ModelVisual3D()
            arrowModel.Content = arrow

            ' Add the ModelVisual3D instances to the viewport
            MainViewport.Children.Add(diskModel)
            MainViewport.Children.Add(arrowModel)
        End Sub

        Public Sub SetAllLayersDirection(angleToShift As Integer)

            ' Check if the wind layer exists for the given altitude
            For Each layer As RotateTransform3D In _windLayers.Values
                layer.Rotation = New AxisAngleRotation3D(New Vector3D(0, 0, 1), -angleToShift)
            Next

        End Sub

        Public Sub ShiftAllLayersDirection(angleToShift As Integer)
            For Each key As Integer In _windLayers.Keys
                ' Get the original angle
                Dim originalAngle As Double = _originalAngles(key)

                ' Calculate the new angle, adjusting for the initial negative angle setup
                ' Ensure it's within 0-359 by adding 360 before modulo operation to handle negative shifts correctly
                Dim newAngle As Double = ((originalAngle - angleToShift) + 360) Mod 360

                ' Update the rotation with the new angle, keeping the negative sign for consistency
                Dim rotateTransform As RotateTransform3D = _windLayers(key)
                Dim axisAngleRotation As AxisAngleRotation3D = CType(rotateTransform.Rotation, AxisAngleRotation3D)
                axisAngleRotation.Angle = -newAngle ' Apply negative to align with initial setup
            Next
        End Sub

        Public Sub ResetViewport()
            If MainViewport.Children.Count > 2 Then
                For i As Integer = (MainViewport.Children.Count) - 1 To 2 Step -1
                    MainViewport.Children.RemoveAt(i)
                Next
                _windLayers.Clear()
                _originalAngles.Clear()
            End If
        End Sub
    End Class

End Namespace
