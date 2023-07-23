Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button

Public Class PrintPictureBox

    Inherits PictureBox
    Public MyImage As Bitmap
    Public 左边距 As Integer = 5
    Public 右边距 As Integer = 5
    Public 上边距 As Integer = 5
    Public 下边距 As Integer = 5
    Public 是否为页面布局模式 As Boolean = True

    Private isDragging As Boolean = False
    Private dragStartPoint As Point
    Private dragState As Integer = 0  ' 0表示鼠标没有处于拖拽状态，1，2，3，4分别表示鼠标处于拖拽左右上下页边距的状态
    Public 是否保持纵横比 As Boolean = True

    Public 所在容器的宽度 As Integer = 100
    Public 所在容器的高度 As Integer = 100
    Private 鼠标悬停分割线序号 As Integer = 0
    Public 纸张名字 As String = "A4"
    Public 纸张宽度 As Integer = 210
    Public 纸张高度 As Integer = 297
    'Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
    '    MyBase.OnMouseMove(e)

    '    ' 检查鼠标是否在画线位置
    '    For i As Integer = 0 To splitPositions.Count - 1
    '        If Math.Abs(e.Y - Height * splitPositions(i)) < 3 Then
    '            Me.Cursor = Cursors.SizeNS
    '            Return
    '        End If
    '    Next

    '    Me.Cursor = Cursors.Default
    'End Sub

    'Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
    '    MyBase.OnMouseDown(e)

    '    ' 检查鼠标是否在画线位置并且按下
    '    For i As Integer = 0 To splitPositions.Count - 1
    '        If Math.Abs(e.Y - Height * splitPositions(i)) < 像素 Then
    '            draggingIndex = i
    '            像素 = 1
    '            Return
    '        End If
    '    Next
    'End Sub

    'Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
    '    MyBase.OnMouseUp(e)

    '    draggingIndex = -1
    '    像素 = 3
    'End Sub

    'Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
    '    MyBase.OnMouseMove(e)

    '    ' 检查鼠标是否在画线位置
    '    For i As Integer = 0 To splitPositions.Count - 1
    '        If Math.Abs(e.Y - Height * splitPositions(i)) < 像素 Then
    '            Me.Cursor = Cursors.SizeNS
    '            draggingIndex = i


    '            Me.Invalidate()
    '            Return

    '        End If
    '    Next




    '    ' 如果鼠标在拖拽状态，并且有一个有效的拖拽索引
    '    If e.Button = MouseButtons.Left And draggingIndex <> -1 Then
    '        splitPositions(draggingIndex) = e.Y / Height
    '        Me.Invalidate()

    '    Else
    '        Me.Cursor = Cursors.Default
    '    End If
    '    Dim p As New Point(e.X, e.Y)
    '    Form1.刷新信息(p)

    'End Sub

    Public Function CreatImg(原始图片 As Bitmap, 鼠标点击位置 As Point) As Bitmap
        If 是否为页面布局模式 Then
            MyImage = 原始图片

        Else


            '确保截取区域不超出原始图片区域
            Dim rectX As Integer = Math.Max(0, Math.Min(鼠标点击位置.X - Width / 2, 原始图片.Width - Width))
            Dim rectY As Integer = Math.Max(0, Math.Min(鼠标点击位置.Y - Height / 2, 原始图片.Height - Height))
            Dim rect As New Rectangle(rectX, rectY, Width, Height) '指定截取区域
            MyImage = 原始图片.Clone(rect, 原始图片.PixelFormat)
        End If

        Return MyImage
    End Function
    Public Sub 重置页边距()
        If 左边距 < 0 Then
            左边距 = 5
        End If
        If 右边距 < 0 Then
            右边距 = 5
        End If
        If 上边距 < 0 Then
            上边距 = 5
        End If
        If 下边距 < 0 Then
            下边距 = 5
        End If



    End Sub
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)

        If 是否为页面布局模式 Then
            dragStartPoint = e.Location
            If Math.Abs(e.X - 左边距) < 10 Then
                dragState = 1
            ElseIf Math.Abs(e.X - (Width - 右边距)) < 10 Then
                dragState = 2
            ElseIf Math.Abs(e.Y - 上边距) < 10 Then
                dragState = 3
            ElseIf Math.Abs(e.Y - (Height - 下边距)) < 10 Then
                dragState = 4
            Else
                dragState = 0
            End If
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)

        dragState = 0
    End Sub


    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)

        If 是否为页面布局模式 Then

            If Math.Abs(e.X - 左边距) < 10 Then
                Cursor = Cursors.SizeWE
                鼠标悬停分割线序号 = 1
                Invalidate()
            ElseIf Math.Abs(e.X - (Width - 右边距)) < 10 Then
                Cursor = Cursors.SizeWE
                鼠标悬停分割线序号 = 2
                Invalidate()
            ElseIf Math.Abs(e.Y - 上边距) < 10 Then
                Cursor = Cursors.SizeNS
                鼠标悬停分割线序号 = 3
                Invalidate()
            ElseIf Math.Abs(e.Y - (Height - 下边距)) < 10 Then
                Cursor = Cursors.SizeNS
                鼠标悬停分割线序号 = 4
                Invalidate()
            Else

                Cursor = Cursors.Default
                鼠标悬停分割线序号 = 0
            End If



            'If Math.Abs(e.X - 左边距) < 5 OrElse Math.Abs(e.X - (Width - 右边距)) < 5 Then
            '    Cursor = Cursors.SizeWE
            'ElseIf Math.Abs(e.Y - 上边距) < 5 OrElse Math.Abs(e.Y - (Height - 下边距)) < 5 Then
            '    Cursor = Cursors.SizeNS
            'Else
            '    Cursor = Cursors.Default
            'End If
        End If

        If dragState <> 0 Then
            Select Case dragState
                Case 1
                    左边距 += e.X - dragStartPoint.X

                Case 2
                    右边距 -= e.X - dragStartPoint.X
                Case 3
                    上边距 += e.Y - dragStartPoint.Y
                Case 4
                    下边距 -= e.Y - dragStartPoint.Y
            End Select

            dragStartPoint = e.Location
            Invalidate()  ' 重绘控件
        End If
    End Sub

    Public Sub 设置页边距(pageWidth As Integer， pageHight As Integer, left As Integer, right As Integer, top As Integer, bottle As Integer)
        左边距 = Width * left / pageWidth
        右边距 = Width * right / pageWidth
        上边距 = Height * top / pageWidth
        下边距 = Height * bottle / pageWidth

    End Sub


    Public Sub 设置纸张(PaperName As String, PaperWidth As Integer, PaperHeight As Integer)
        纸张名字 = PaperName
        纸张宽度 = PaperWidth
        纸张高度 = PaperHeight
        Me.Dock = DockStyle.None
        Dim 纸张高宽比 As Single = 纸张高度 / 纸张宽度  ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        If 纸张高宽比 > 所在容器的高度 / 所在容器的宽度 Then
            Height = 所在容器的高度
            Width = Height / 纸张高宽比
        Else
            Width = 所在容器的宽度
            Height = Width * 纸张高宽比
        End If
    End Sub

    Public Function 获取左边距(pageWidth As Integer， pageHight As Integer) As Integer
        Return pageWidth * 左边距 / Width

    End Function


    Public Function 获取右边距(pageWidth As Integer， pageHight As Integer) As Integer
        Return pageWidth * 右边距 / Width

    End Function

    Public Function 获取上边距(pageWidth As Integer， pageHight As Integer) As Integer
        Return pageHight * 上边距 / Height

    End Function

    Public Function 获取下边距(pageWidth As Integer， pageHight As Integer) As Integer
        Return pageHight * 下边距 / Height

    End Function

    Protected Overrides Sub OnPaint(e As PaintEventArgs)


        'MyBase.OnPaint(e)
        If MyImage IsNot Nothing Then

            If 是否为页面布局模式 Then

                Me.Dock = DockStyle.None
                Dim 纸张高宽比 As Single = 纸张高度 / 纸张宽度  ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
                If 纸张高宽比 > 所在容器的高度 / 所在容器的宽度 Then
                    Height = 所在容器的高度
                    Width = Height / 纸张高宽比
                Else
                    Width = 所在容器的宽度
                    Height = Width * 纸张高宽比
                End If


                Dim 页边距内宽, 页边距内高, 绘制宽度, 绘制高度 As Integer
                Dim 纵横比 As Single

                If 是否保持纵横比 Then '保持纵横比

                    页边距内宽 = Width - 左边距 - 右边距
                    页边距内高 = Height - 上边距 - 下边距
                    纵横比 = MyImage.Height / MyImage.Width
                    绘制宽度 = 页边距内宽
                    绘制高度 = CInt(绘制宽度 * 纵横比)
                    If 绘制高度 > 页边距内高 Then
                        绘制高度 = 页边距内高
                        绘制宽度 = CInt(绘制高度 / 纵横比)
                    End If
                Else '拉伸填充


                    绘制宽度 = Width - 左边距 - 右边距
                    绘制高度 = Height - 上边距 - 下边距
                End If

                Dim g As Graphics = e.Graphics

                'g.DrawImage(MyImage, 左边距, 上边距, 绘制宽度, 绘制高度)

                g.DrawImage(MyImage, CType((Width - 绘制宽度 + 左边距 - 右边距) / 2, Single), 上边距, 绘制宽度, 绘制高度)




                Dim pen1 As New Pen(Color.Red)
                Dim pen2 As New Pen(Color.Green)
                Dim pen As Pen
                If 鼠标悬停分割线序号 = 1 Then
                    pen = pen2
                Else
                    pen = pen1
                End If
                e.Graphics.DrawLine(pen, 左边距, 0, 左边距, Height)
                If 鼠标悬停分割线序号 = 2 Then
                    pen = pen2
                Else
                    pen = pen1
                End If
                e.Graphics.DrawLine(pen, Width - 右边距, 0, Width - 右边距, Height)
                If 鼠标悬停分割线序号 = 3 Then
                    pen = pen2
                Else
                    pen = pen1
                End If
                e.Graphics.DrawLine(pen, 0, 上边距, Width, 上边距)
                If 鼠标悬停分割线序号 = 4 Then
                    pen = pen2
                Else
                    pen = pen1
                End If
                e.Graphics.DrawLine(pen, 0, Height - 下边距, Width, Height - 下边距)
            Else
                Me.Dock = DockStyle.Fill
                Dim g As Graphics = e.Graphics

                g.DrawImage(MyImage, 0, 0, MyImage.Width, MyImage.Height)



                Dim pen1 As New Pen(Color.Red, 0.1)
                pen1.DashStyle = Drawing2D.DashStyle.Dash
                e.Graphics.DrawLine(pen1, 0, CInt(Height / 2), Width, CInt(Height / 2))
                e.Graphics.DrawLine(pen1, CInt(Width / 2), 0, CInt(Width / 2), Height)
                pen1.Dispose()
            End If
        Else
            Me.Dock = DockStyle.Fill
        End If


        'If 是否为页面布局模式 = True Then

        'Else

        'End If
















        ''MyBase.OnPaint(e)
        'If Form1.originalBitmap IsNot Nothing Then
        '    ' 计算缩放后的尺寸
        '    Dim scaledWidth As Integer = CInt(Form1.originalBitmap.Width * Form1.currentScale)
        '    Dim scaledHeight As Integer = CInt(Form1.originalBitmap.Height * Form1.currentScale)
        '    Dim n As Integer = 0
        '    ' 绘制缩放后的图片
        '    Dim g As Graphics = e.Graphics

        '    g.DrawImage(Form1.originalBitmap, 0, 0, scaledWidth, scaledHeight)






        '    If splitPositions IsNot Nothing Then
        '        Dim pen1 As New Pen(Color.Red, 0.5) ' 创建一个红色的画笔，线宽为2
        '        Dim pen2 As New Pen(Color.Green, 0.5) ' 创建一个红色的画笔，线宽为2
        '        For Each position In splitPositions
        '            ' 在每个分割位置绘制一条线
        '            If n = draggingIndex Then

        '                e.Graphics.DrawLine(pen2, 0, Height * position, Me.Width, Height * position)
        '            Else
        '                e.Graphics.DrawLine(pen1, 0, Height * position, Me.Width, Height * position)
        '            End If
        '            n += 1
        '        Next

        '        pen1.Dispose()
        '        pen2.Dispose()
        '    End If
        'End If


    End Sub
End Class
