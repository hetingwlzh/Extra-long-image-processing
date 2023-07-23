Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
Imports AForge.Imaging
Imports AForge.Imaging.Filters
Imports System.Drawing
Imports AForge
Imports Accord.Imaging.Filters

Module Module1

    Public Class ConvolutionMatrix
        Public TopLeft As Double = 0
        Public TopMid As Double = 0
        Public TopRight As Double = 0
        Public MidLeft As Double = 0
        Public Pixel As Double = 1
        Public MidRight As Double = 0
        Public BottomLeft As Double = 0
        Public BottomMid As Double = 0
        Public BottomRight As Double = 0

        Public Sub ApplySharpen(ByVal val As Double)
            TopLeft = 0
            TopMid = -1
            TopRight = 0
            MidLeft = -1
            Pixel = val + 4
            MidRight = -1
            BottomLeft = 0
            BottomMid = -1
            BottomRight = 0
        End Sub

        Public Function Convolve(ByVal sourceImage As Bitmap) As Bitmap
            Dim sourceData As BitmapData = sourceImage.LockBits(New Rectangle(0, 0, sourceImage.Width, sourceImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb)

            Dim pixelBuffer As Byte() = New Byte(sourceData.Stride * sourceData.Height - 1) {}
            Marshal.Copy(sourceData.Scan0, pixelBuffer, 0, pixelBuffer.Length)

            sourceImage.UnlockBits(sourceData)

            Dim resultImage As New Bitmap(sourceImage.Width, sourceImage.Height)

            Dim resultData As BitmapData = resultImage.LockBits(New Rectangle(0, 0, resultImage.Width, resultImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb)

            Dim resultBuffer As Byte() = New Byte(resultData.Stride * resultData.Height - 1) {}

            Dim pixelColor As Color = Color.Empty

            Dim blue As Double = 0.0R
            Dim green As Double = 0.0R
            Dim red As Double = 0.0R

            Dim filterWidth As Integer = 3
            Dim filterHeight As Integer = 3

            Dim imageWidth As Integer = sourceImage.Width
            Dim imageHeight As Integer = sourceImage.Height

            Dim pixelIndex As Integer = 0

            For offsetY As Integer = 0 To imageHeight - 1
                For offsetX As Integer = 0 To imageWidth - 1
                    blue = 0.0R
                    green = 0.0R
                    red = 0.0R

                    pixelIndex = offsetY * sourceData.Stride + offsetX * 4

                    ' kernel calculations
                    For filterY As Integer = 0 To filterHeight - 1
                        For filterX As Integer = 0 To filterWidth - 1
                            Dim calcOffset As Integer = pixelIndex + (filterX - 1) * 4 + (filterY - 1) * sourceData.Stride

                            If calcOffset >= 0 AndAlso calcOffset < pixelBuffer.Length Then
                                blue += CType(pixelBuffer(calcOffset), Double) * Me(filterX, filterY)
                                green += CType(pixelBuffer(calcOffset + 1), Double) * Me(filterX, filterY)
                                red += CType(pixelBuffer(calcOffset + 2), Double) * Me(filterX, filterY)
                            End If
                        Next
                    Next

                    blue *= Pixel
                    green *= Pixel
                    red *= Pixel

                    blue += Me.Pixel * Me.TopLeft + Me.Pixel * Me.TopMid + Me.Pixel * Me.TopRight + Me.Pixel * Me.MidLeft + Me.Pixel * Me.MidRight + Me.Pixel * Me.BottomLeft + Me.Pixel * Me.BottomMid + Me.Pixel * Me.BottomRight

                    green += Me.Pixel * Me.TopLeft + Me.Pixel * Me.TopMid + Me.Pixel * Me.TopRight + Me.Pixel * Me.MidLeft + Me.Pixel * Me.MidRight + Me.Pixel * Me.BottomLeft + Me.Pixel * Me.BottomMid + Me.Pixel * Me.BottomRight

                    red += Me.Pixel * Me.TopLeft + Me.Pixel * Me.TopMid + Me.Pixel * Me.TopRight + Me.Pixel * Me.MidLeft + Me.Pixel * Me.MidRight + Me.Pixel * Me.BottomLeft + Me.Pixel * Me.BottomMid + Me.Pixel * Me.BottomRight

                    If blue > Byte.MaxValue Then
                        blue = Byte.MaxValue

                    ElseIf blue < Byte.MinValue Then
                        blue = Byte.MinValue
                    End If

                    If green > Byte.MaxValue Then
                        green = Byte.MaxValue
                    ElseIf green < Byte.MinValue Then
                        green = Byte.MinValue
                    End If

                    If red > Byte.MaxValue Then
                        red = Byte.MaxValue
                    ElseIf red < Byte.MinValue Then
                        red = Byte.MinValue
                    End If


                    resultBuffer(pixelIndex) = CType(blue, Byte)
                    resultBuffer(pixelIndex + 1) = CType(green, Byte)
                    resultBuffer(pixelIndex + 2) = CType(red, Byte)
                    resultBuffer(pixelIndex + 3) = CType(pixelBuffer(pixelIndex + 3), Byte)
                Next
            Next

            Marshal.Copy(resultBuffer, 0, resultData.Scan0, resultBuffer.Length)

            resultImage.UnlockBits(resultData)

            Return resultImage
        End Function

        Default Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As Double
            Get
                Select Case y
                    Case 0
                        Select Case x
                            Case 0
                                Return TopLeft

                            Case 1
                                Return TopMid

                            Case 2
                                Return TopRight

                            Case Else
                                Return Pixel
                        End Select

                    Case 1
                        Select Case x
                            Case 0
                                Return MidLeft

                            Case 1
                                Return Pixel

                            Case 2
                                Return MidRight

                            Case Else
                                Return Pixel
                        End Select

                    Case 2
                        Select Case x
                            Case 0
                                Return BottomLeft

                            Case 1
                                Return BottomMid

                            Case 2
                                Return BottomRight

                            Case Else
                                Return Pixel
                        End Select

                    Case Else
                        Return Pixel
                End Select
            End Get
        End Property

    End Class




    '    当调用 AdjustImage 函数时，您需要传递以下四个参数

    'imagePath：要调整的图像的文件路径，必须是一个字符串类型。
    'brightness：亮度调整的值，必须是一个 Double 类型的数字。该值越大，图像就越亮；该值越小，图像就越暗。默认值为 1.0，表示不进行亮度调整。
    'contrast：对比度调整的值，必须是一个 Double 类型的数字。该值越大，图像的对比度就越高；该值越小，图像的对比度就越低。默认值为 1.0，表示不进行对比度调整。
    'sharpen：锐化度调整的值，必须是一个 Double 类型的数字。该值越大，图像就越锐利；该值越小，图像就越模糊。默认值为 0.0，表示不进行锐化度调整。
    '请注意，亮度、对比度和锐化度的调整值都应该在 0.0 到 2.0 的范围内，其中 1.0 表示不进行调整。如果超出此范围，则可能会导致图像过度处理或失真。
    Public Function AdjustImage(originalImage As System.Drawing.Image, brightness As Double, contrast As Double, sharpen As Double) As System.Drawing.Image
        'Dim originalImage As Image = Image.FromFile(imagePath)
        Dim adjustedImage As New Bitmap(originalImage.Width, originalImage.Height)

        ' Create a Graphics object to draw the adjusted image
        Dim g As Graphics = Graphics.FromImage(adjustedImage)

        ' Create an ImageAttributes object to adjust brightness and contrast
        Dim attributes As New ImageAttributes()
        Dim matrix As New ColorMatrix()

        ' Adjust brightness
        matrix.Matrix00 = brightness
        matrix.Matrix11 = brightness
        matrix.Matrix22 = brightness

        ' Adjust contrast
        matrix.Matrix33 = contrast
        matrix.Matrix44 = 1

        attributes.SetColorMatrix(matrix)

        ' Draw the adjusted image with the specified brightness and contrast
        g.DrawImage(originalImage, New Rectangle(0, 0, originalImage.Width, originalImage.Height), 0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, attributes)

        ' Apply sharpening filter
        If sharpen <> 0 Then
            Dim sharpenFilter As New ConvolutionMatrix()
            sharpenFilter.ApplySharpen(sharpen)
            adjustedImage = sharpenFilter.Convolve(adjustedImage)
        End If

        Return adjustedImage
    End Function



    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    'Public Function SharpenImage(inputImage As Bitmap) As Bitmap
    '    ' 创建Unsharp Mask滤波器
    '    Dim filter As New UnsharpMask()

    '    ' 设置滤波器参数
    '    filter.Radius = 5
    '    filter.Amount = 1.5
    '    filter.Threshold = 0

    '    ' 应用滤波器
    '    Dim outputImage As Bitmap = filter.Apply(inputImage)

    '    Return outputImage
    'End Function


    Public Function SharpenImage(inputImage As Bitmap,
                                 锐度 As Double,
                                 输入黑白级别最小值 As Integer,
                                 输入黑白级别最大值 As Integer,
                                 输出黑白级别最小值 As Integer,
                                 输出黑白级别最大值 As Integer，
                                 Optional 是否黑白色 As Boolean = False) As Bitmap

        If inputImage IsNot Nothing Then


            '' 创建GaussianSharpen滤波器
            'Dim filter As New AForge.Imaging.Filters.GaussianSharpen()

            '' 设置滤波器参数
            'filter.Sigma = 锐度

            '' 应用滤波器
            'Dim outputImage As Bitmap = filter.Apply(inputImage)






            '' 创建一个锐化滤波器
            'Dim sharpen As Integer(,) = New Integer(2, 2) {{-1, -1, -1}, {-1, 锐度, -1}, {-1, -1, -1}}

            '' 创建一个卷积滤波器
            'Dim convolution As New Accord.Imaging.Filters.Convolution(sharpen)

            '' 锐化图像

            'Dim outputImage As Bitmap = convolution.Apply(inputImage)

            Dim outputImage As Bitmap = inputImage
            If 是否黑白色 = True Then
                Dim filter4 As New Accord.Imaging.Filters.Grayscale(0.2125, 0.7154, 0.0721) ' 将图像转换为黑白色
                outputImage = filter4.Apply(outputImage)
            End If



            outputImage = AdjustImage(outputImage, 锐度, 1, 1)










            Dim filter2 As New AForge.Imaging.Filters.LevelsLinear()

            filter2.Input = New IntRange(输入黑白级别最小值, 输入黑白级别最大值)  ' 设置输入黑白级别
            filter2.Output = New IntRange(输出黑白级别最小值, 输出黑白级别最大值)  ' 设置输出黑白级别

            outputImage = filter2.Apply(outputImage)




            'Dim filter3 As New Accord.Imaging.Filters.Median() '去除噪点
            'outputImage = filter3.Apply(outputImage)







            Return outputImage
        End If

    End Function



    Public Class paper
        Public Name As String
        Public Width As Integer
        Public Height As Integer
        Sub New(_name As String, _width As Integer, _height As Integer)
            Name = _name
            Width = _width
            Height = _height
        End Sub
        Public Function 获取纵横比() As Single
            Return Height / Width
        End Function
        Public Function 获取横纵比() As Single
            Return Width / Height
        End Function
    End Class

End Module
