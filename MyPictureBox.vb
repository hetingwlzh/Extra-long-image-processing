Imports System.ComponentModel.Design
Imports System.Windows.Forms.AxHost
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Status
Imports PdfSharp.Drawing

Public Class MyPictureBox
    Inherits PictureBox
    Public 原始图像 As Image ' = Image.FromFile(PicturePath)
    Public 当前图像 As Image ' = Image.FromFile(PicturePath)
    Public 分页线列表 As New List(Of Single)
    Public currentScale As Double = 1 '当前缩放比例
    Public 鼠标拖拽分割线序号 As Integer = -1
    Public 鼠标悬停分割线序号 As Integer = -1
    Public 鼠标拖拽起始删除线序号 As Integer = -1
    Public 鼠标拖拽结尾删除线序号 As Integer = -1
    Public 鼠标拖拽删除区域序号 As Integer = -1
    Public 鼠标选取的删除区域序号 As Integer = -1
    Public 鼠标拖拽的区域x As Single
    Public 鼠标拖拽的区域y As Single

    Public 像素 As Integer = 2
    Public 鼠标点击位置 As Point
    Public 当前预览页面 As Integer

    Public 删除区域列表 As New List(Of MyPoint)

    Public 是否在所有删除区域外 As Boolean = True

    Public 修边模式 As Boolean = False
    Public 左边距 As Integer = 50
    Public 右边距 As Integer = 50
    Public 上边距 As Integer = 100
    Public 下边距 As Integer = 50

    Public 当前操作边距序号 As Integer = -1

    Public 新添加删除区域默认高度 As Integer = 30

    Class MyPoint
        Public x As Single
        Public y As Single
        Public index As Integer
        Sub New(_x As Single, _y As Single, Optional _index As Integer = Nothing)
            x = _x
            y = _y
            index = _index
        End Sub
        Sub New(other As MyPoint)
            x = other.x
            y = other.y
            index = other.index
        End Sub
    End Class
    Class MyPointComparer
        Implements IComparer(Of MyPoint)

        Public Function Compare(ByVal MyPoint1 As MyPoint, ByVal MyPoint2 As MyPoint) As Integer Implements IComparer(Of MyPoint).Compare
            If MyPoint1.x < MyPoint2.x Then
                Return -1
            ElseIf MyPoint1.x > MyPoint2.x Then
                Return 1
            Else
                Return 0
            End If
        End Function
    End Class

    Public Sub SetImage(图像 As Image)
        原始图像 = 图像
        当前图像 = 图像
        分页线列表.Clear()
        currentScale = 1
        鼠标拖拽分割线序号 = -1
        鼠标悬停分割线序号 = -1
        当前预览页面 = -1
        删除区域列表.Clear()
    End Sub



    Public Function 开始修边() As Boolean


        Dim showImage As Bitmap = New Bitmap(当前图像)
        Dim rect As New Rectangle(左边距, 上边距, Width / currentScale - 左边距 - 右边距, Height / currentScale - 上边距 - 下边距) '指定截取区域

        SetImage(showImage.Clone(rect, showImage.PixelFormat))


        左边距 = 15
        右边距 = 15
        上边距 = 15
        下边距 = 15



        Invalidate()


    End Function
    Public Function 添加删除区域(开始百分比 As Single， 结束百分比 As Single)
        Dim 区域 As New MyPoint(开始百分比, 结束百分比)
        删除区域列表.Add(区域)
        合并删除区域()
        Invalidate()
        Return 删除区域列表.Count
    End Function
    Public Function 在当前位置添加删除区域(Optional delWidth As Integer = 0)
        If delWidth = 0 Then
            delWidth = 新添加删除区域默认高度
        End If
        Dim start, over As Single
        start = 鼠标点击位置.Y / Height
        over = Math.Min(1, 鼠标点击位置.Y / Height + delWidth / Height)
        添加删除区域(start, over)
        Return 删除区域列表.Count
    End Function

    Public Sub 合并删除区域()
        If 删除区域列表.Count <= 1 Then
            Return
        End If

        删除区域列表.Sort(New MyPointComparer())

        Dim i As Integer = 0
        While i < 删除区域列表.Count - 1
            If 删除区域列表(i).y >= 删除区域列表(i + 1).x Then
                ' 如果当前区间的y值大于或等于下一个区间的x值，那么这两个区间重叠或包含，需要合并
                删除区域列表(i).y = Math.Max(删除区域列表(i).y, 删除区域列表(i + 1).y)
                ' 删除下一个区间
                删除区域列表.RemoveAt(i + 1)
            Else
                ' 如果当前区间的y值小于下一个区间的x值，那么这两个区间不重叠，不需要合并
                i += 1
            End If
        End While
    End Sub
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

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        鼠标点击位置 = e.Location

        If 修边模式 Then


            If Math.Abs(e.X - 左边距 * currentScale) < 像素 Then
                当前操作边距序号 = 1
                Return
            End If


            If Math.Abs(e.X - (Width - 右边距 * currentScale)) < 像素 Then
                当前操作边距序号 = 2
                Return
            End If



            If Math.Abs(e.Y - 上边距 * currentScale) < 像素 Then
                当前操作边距序号 = 3
                Return
            End If

            If Math.Abs(e.Y - (Height - 下边距 * currentScale)) < 像素 Then
                当前操作边距序号 = 4
                Return
            End If













        Else
            ' 检查鼠标是否在画线位置并且按下
            For i As Integer = 0 To 分页线列表.Count - 1
                If Math.Abs(e.Y - Height * 分页线列表(i)) < 像素 Then
                    鼠标拖拽分割线序号 = i
                    鼠标悬停分割线序号 = i

                    Return
                End If
            Next

            鼠标拖拽起始删除线序号 = -1
            鼠标拖拽结尾删除线序号 = -1
            鼠标选取的删除区域序号 = -1
            鼠标拖拽删除区域序号 = -1
            For i As Integer = 0 To 删除区域列表.Count - 1
                If Math.Abs(e.Y - Height * 删除区域列表(i).x) < 像素 Then
                    鼠标拖拽起始删除线序号 = i
                    鼠标选取的删除区域序号 = i
                    Return
                End If
                If Math.Abs(e.Y - Height * 删除区域列表(i).y) < 像素 Then
                    鼠标拖拽结尾删除线序号 = i
                    鼠标选取的删除区域序号 = i
                    Return
                End If


                If e.Y > Height * 删除区域列表(i).x And e.Y < Height * 删除区域列表(i).y Then
                    鼠标拖拽删除区域序号 = i
                    鼠标选取的删除区域序号 = i
                    鼠标拖拽的区域x = 删除区域列表(i).x
                    鼠标拖拽的区域y = 删除区域列表(i).y
                    Return
                End If
            Next


        End If


    End Sub

    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)

        鼠标拖拽分割线序号 = -1
        鼠标拖拽起始删除线序号 = -1
        鼠标拖拽结尾删除线序号 = -1
        鼠标拖拽删除区域序号 = -1
        当前操作边距序号 = -1
        分页线列表.Sort()
    End Sub

    Public Function 添加分页线(分页线位置 As Single)
        分页线列表.Add(分页线位置)
        分页线列表.Sort()
        Invalidate()
        Return 分页线列表.Count
    End Function
    Public Function 删除当前分页线()
        If 鼠标悬停分割线序号 >= 0 Then
            分页线列表.RemoveAt(鼠标悬停分割线序号)
            鼠标悬停分割线序号 = -1
            分页线列表.Sort()
            Invalidate()
        End If
        Return 分页线列表.Count
    End Function
    Public Sub 删除所有分页线()
        分页线列表.Clear()
        鼠标悬停分割线序号 = -1
        当前预览页面 = -1
        Invalidate()
    End Sub
    Public Sub 删除当前删除区域()
        删除区域列表.RemoveAt(鼠标选取的删除区域序号)
        Invalidate()
    End Sub

    Public Sub 设置当前删除区域默认高度(Optional 默认高度 As Integer = 0)
        If 默认高度 = 0 Then
            新添加删除区域默认高度 = (删除区域列表(鼠标选取的删除区域序号).y - 删除区域列表(鼠标选取的删除区域序号).x) * Height
        Else
            新添加删除区域默认高度 = 默认高度
        End If

    End Sub




    Public Sub 清除空删除区域列表()
        删除区域列表.Clear()
        鼠标选取的删除区域序号 = -1
        鼠标拖拽删除区域序号 = -1
        鼠标拖拽起始删除线序号 = -1
        鼠标拖拽结尾删除线序号 = -1
        Invalidate()
    End Sub
    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        MyBase.OnMouseMove(e)


        If 修边模式 Then


            If Math.Abs(e.X - 左边距 * currentScale) < 像素 Or Math.Abs(e.X - (Width - 右边距 * currentScale)) < 像素 Then
                Me.Cursor = Cursors.SizeWE
                Me.Invalidate()
                Return
            End If


            If Math.Abs(e.Y - 上边距 * currentScale) < 像素 Or Math.Abs(e.Y - (Height - 下边距 * currentScale)) < 像素 Then
                Me.Cursor = Cursors.SizeNS
                Me.Invalidate()
                Return
            End If


            If e.Button = MouseButtons.Left Then
                If 当前操作边距序号 = 1 Then
                    左边距 = e.X / currentScale
                    If 左边距 < 0 Then
                        左边距 = 0
                    ElseIf 左边距 >= (当前图像.Width - 右边距) Then
                        左边距 = (当前图像.Width - 右边距) - 2
                    End If
                ElseIf 当前操作边距序号 = 2 Then
                    右边距 = (Width - e.X) / currentScale
                    If 右边距 < 0 Then
                        右边距 = 0
                    ElseIf 右边距 >= (当前图像.Width - 左边距) Then
                        右边距 = (当前图像.Width - 左边距) - 2
                    End If
                ElseIf 当前操作边距序号 = 3 Then
                    上边距 = e.Y / currentScale
                    If 上边距 < 0 Then
                        上边距 = 0
                    ElseIf 上边距 >= (当前图像.Height - 下边距) Then
                        上边距 = (当前图像.Height - 下边距) - 2
                    End If

                ElseIf 当前操作边距序号 = 4 Then
                    下边距 = (Height - e.Y) / currentScale
                    If 下边距 < 0 Then
                        下边距 = 0
                    ElseIf 下边距 >= (当前图像.Height - 上边距) Then
                        下边距 = (当前图像.Height - 上边距) - 2
                    End If
                End If


                Me.Invalidate()

            Else
                Me.Cursor = Cursors.Default
            End If
        Else


            ' 检查鼠标是否在画线位置
            For i As Integer = 0 To 分页线列表.Count - 1
                If Math.Abs(e.Y - Height * 分页线列表(i)) < 像素 Then
                    Me.Cursor = Cursors.Cross
                    '上次鼠标是双箭头 = True
                    '鼠标拖拽分割线序号 = i

                    '鼠标悬停分割线序号 = i
                    Me.Invalidate()
                    Return
                    'ElseIf 上次鼠标是双箭头 = True Then
                    '    Me.Cursor = Cursors.Default
                    '    上次鼠标是双箭头 = False

                End If
            Next


            ' 检查鼠标是否在画线位置
            是否在所有删除区域外 = True
            For i As Integer = 0 To 删除区域列表.Count - 1

                If Math.Abs(e.Y - Height * 删除区域列表(i).x) < 像素 Then
                    Me.Cursor = Cursors.SizeNS
                    '上次鼠标是十字形 = True
                    '鼠标拖拽分割线序号 = i

                    '鼠标悬停分割线序号 = i
                    'Me.Invalidate()
                    Return
                    'ElseIf 上次鼠标是十字形 = True Then

                    '    Me.Cursor = Cursors.Default
                    '    上次鼠标是双箭头 = False


                End If

                If Math.Abs(e.Y - Height * 删除区域列表(i).y) < 像素 Then
                    Me.Cursor = Cursors.SizeNS
                    '上次鼠标是十字形 = True
                    '鼠标拖拽分割线序号 = i

                    '鼠标悬停分割线序号 = i
                    'Me.Invalidate()
                    Return
                    'ElseIf 上次鼠标是十字形 = True Then

                    '    Me.Cursor = Cursors.Default
                    '    上次鼠标是双箭头 = False
                End If

                If e.Y > Height * 删除区域列表(i).x And e.Y < Height * 删除区域列表(i).y Then
                    Me.Cursor = Cursors.Hand
                    是否在所有删除区域外 = False
                    '上次鼠标是手形 = True
                    'Return


                    'ElseIf 上次鼠标是手形 = True Then

                    '    Me.Cursor = Cursors.Default
                    '    上次鼠标是手形 = False


                End If


            Next


            If 是否在所有删除区域外 = True Then
                Me.Cursor = Cursors.Default
            End If






            ' 如果鼠标在拖拽状态，并且有一个有效的拖拽索引
            If e.Button = MouseButtons.Left Then
                If 鼠标拖拽分割线序号 <> -1 Then
                    分页线列表(鼠标拖拽分割线序号) = e.Y / Height

                    If 分页线列表(鼠标拖拽分割线序号) < 0 Then
                        分页线列表(鼠标拖拽分割线序号) = 0
                    ElseIf 分页线列表(鼠标拖拽分割线序号) > (Height - 4) / Height Then
                        分页线列表(鼠标拖拽分割线序号) = (Height - 4) / Height

                    End If

                    Me.Invalidate()
                ElseIf 鼠标拖拽起始删除线序号 <> -1 Then
                    删除区域列表(鼠标拖拽起始删除线序号).x = e.Y / Height
                    If (删除区域列表(鼠标拖拽起始删除线序号).y - 删除区域列表(鼠标拖拽起始删除线序号).x) * Height <= 2 Then
                        删除区域列表(鼠标拖拽起始删除线序号).x = 删除区域列表(鼠标拖拽起始删除线序号).y - 2 / Height
                    End If

                    If 删除区域列表(鼠标拖拽起始删除线序号).x < 0 Then
                        删除区域列表(鼠标拖拽起始删除线序号).x = 0
                    ElseIf 删除区域列表(鼠标拖拽起始删除线序号).x > (Height - 2) / Height Then
                        删除区域列表(鼠标拖拽起始删除线序号).x = (Height - 2) / Height

                    End If
                    Me.Invalidate()

                ElseIf 鼠标拖拽结尾删除线序号 <> -1 Then
                    删除区域列表(鼠标拖拽结尾删除线序号).y = e.Y / Height
                    If (删除区域列表(鼠标拖拽结尾删除线序号).y - 删除区域列表(鼠标拖拽结尾删除线序号).x) * Height <= 2 Then
                        删除区域列表(鼠标拖拽结尾删除线序号).y = 删除区域列表(鼠标拖拽结尾删除线序号).x + 2 / Height
                    End If

                    If 删除区域列表(鼠标拖拽结尾删除线序号).y < 2 / Height Then
                        删除区域列表(鼠标拖拽结尾删除线序号).y = 2 / Height
                    ElseIf 删除区域列表(鼠标拖拽结尾删除线序号).y > 1 Then
                        删除区域列表(鼠标拖拽结尾删除线序号).y = 1

                    End If
                    Me.Invalidate()

                ElseIf 鼠标拖拽删除区域序号 <> -1 Then
                    删除区域列表(鼠标拖拽删除区域序号).x = 鼠标拖拽的区域x + (e.Y / Height - 鼠标点击位置.Y / Height)
                    删除区域列表(鼠标拖拽删除区域序号).y = 鼠标拖拽的区域y + (e.Y / Height - 鼠标点击位置.Y / Height)

                    If 删除区域列表(鼠标拖拽删除区域序号).x < 0 / Height Then
                        删除区域列表(鼠标拖拽删除区域序号).x = 0 / Height
                    ElseIf 删除区域列表(鼠标拖拽删除区域序号).x > (Height - 5) / Height Then
                        删除区域列表(鼠标拖拽删除区域序号).x = (Height - 5) / Height

                    End If

                    If 删除区域列表(鼠标拖拽删除区域序号).y < 5 / Height Then
                        删除区域列表(鼠标拖拽删除区域序号).y = 5 / Height
                    ElseIf 删除区域列表(鼠标拖拽删除区域序号).y > 1 Then
                        删除区域列表(鼠标拖拽删除区域序号).y = 1

                    End If


                    Me.Invalidate()


                Else
                End If
                Form1.拖动操作记录 = True
            ElseIf Me.Cursor <> Cursors.Default And Me.Cursor <> Cursors.Hand Then
                Me.Cursor = Cursors.Default


            End If



        End If









        Dim p As New Point(e.X, e.Y)
        Form1.刷新信息(p)

    End Sub



    Public Sub 图片缩放(比例 As Single, Optional 是否绝对缩放 As Boolean = False)


        Dim top0 As Integer = Me.Top
        If 当前图像 IsNot Nothing Then
            If 是否绝对缩放 = False Then
                If 当前图像.Width * currentScale * 比例 > 10 Then
                    currentScale *= 比例
                    Invalidate()
                    'Top = 0
                    Left = 0
                    Width = 当前图像.Width * currentScale
                    Height = 当前图像.Height * currentScale

                End If
            Else
                If 当前图像.Width * 比例 > 10 Then
                    currentScale = 比例
                    Invalidate()
                    'Top = 0
                    Left = 0
                    Width = 当前图像.Width * currentScale
                    Height = 当前图像.Height * currentScale
                End If
            End If

            If Top < -(Height - Form1.Panel1.Height) Then
                Top = -(Height - Form1.Panel1.Height)
            End If

        End If
        'Me.Top *= currentScale



    End Sub





    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        'MyBase.OnPaint(e)
        If 当前图像 IsNot Nothing Then
            ' 计算缩放后的尺寸
            Dim scaledWidth As Integer = CInt(当前图像.Width * currentScale)
            Dim scaledHeight As Integer = CInt(当前图像.Height * currentScale)
            Dim n As Integer = 0
            ' 绘制缩放后的图片
            Dim g As Graphics = e.Graphics

            g.DrawImage(当前图像, 0, 0, scaledWidth, scaledHeight)




            If 修边模式 Then

                Dim pen1 As New Pen(Color.SkyBlue, 1) ' 创建一个红色的画笔，线宽为2

                Dim rangeBrush As New SolidBrush(Color.FromArgb(150, Color.FromArgb(30, 30, 100)))

                e.Graphics.DrawLine(pen1, CType(左边距 * currentScale, Single), 0, CType(左边距 * currentScale, Single), Height)
                e.Graphics.DrawLine(pen1, CType(Width - 右边距 * currentScale, Single), 0, CType(Width - 右边距 * currentScale, Single), Height)
                e.Graphics.DrawLine(pen1, 0, CType(上边距 * currentScale, Single), Width, CType(上边距 * currentScale, Single))
                e.Graphics.DrawLine(pen1, 0, CType(Height - 下边距 * currentScale, Single), Width, CType(Height - 下边距 * currentScale, Single))


                e.Graphics.FillRectangle(rangeBrush, New Rectangle(0, 0, 左边距 * currentScale, Height))
                e.Graphics.FillRectangle(rangeBrush, New Rectangle(Width - 右边距 * currentScale, 0, 右边距 * currentScale, Height))
                e.Graphics.FillRectangle(rangeBrush, New Rectangle(0, 0, Width, 上边距 * currentScale))
                e.Graphics.FillRectangle(rangeBrush, New Rectangle(0, Height - 下边距 * currentScale, Width, 下边距))


                pen1.Dispose()














            Else
                Dim drawFont As New Font("微软雅黑", Math.Max(10, CInt(Width / 10)), FontStyle.Bold Or FontStyle.Italic)
                Dim drawBrush As New SolidBrush(Color.FromArgb(40, Color.Green))

                Dim textBrush As New SolidBrush(Color.FromArgb(100, Color.Red))

                If 分页线列表 IsNot Nothing Then
                    Dim pen1 As New Pen(Color.Red, 0.5) ' 创建一个红色的画笔，线宽为2
                    Dim pen2 As New Pen(Color.Green, 0.5) ' 创建一个红色的画笔，线宽为2
                    Dim pen3 As New Pen(Color.DeepSkyBlue, 15) ' 创建一个红色的画笔，线宽为2
                    For Each position In 分页线列表
                        ' 在每个分割位置绘制一条线
                        If n = 鼠标悬停分割线序号 Then
                            e.Graphics.DrawLine(pen2, 0, Height * position, Me.Width, Height * position)
                        Else
                            e.Graphics.DrawLine(pen1, 0, Height * position, Me.Width, Height * position)
                        End If
                        Dim yy As Single
                        If 当前预览页面 = n Then
                            If n < 分页线列表.Count - 1 Then
                                yy = Height * 分页线列表(n + 1)
                            Else
                                yy = Height
                            End If
                            'e.Graphics.DrawLine(pen3, 0, Height * position, 0, yy)
                            e.Graphics.FillRectangle(drawBrush, New Rectangle(0, Height * position, Me.Width, yy - Height * position))
                        End If
                        n += 1
                        g.DrawString("P" & n, drawFont, textBrush, 3, Height * position)
                    Next
                    pen1.Dispose()
                    pen2.Dispose()
                    pen3.Dispose()
                End If

                If 删除区域列表 IsNot Nothing Then
                    Dim pen1 As New Pen(Color.Red, 0.5) ' 创建一个红色的画笔，线宽为2
                    Dim pen2 As New Pen(Color.Green, 0.5) ' 创建一个红色的画笔，线宽为2
                    Dim pen3 As New Pen(Color.DeepSkyBlue, 1) ' 创建一个红色的画笔，线宽为2
                    For Each Range In 删除区域列表
                        'e.Graphics.DrawLine(pen3, 0, Height * Range.x, Me.Width, Height * Range.x)
                        'e.Graphics.DrawLine(pen3, 0, Height * Range.y, Me.Width, Height * Range.y)
                        ' 创建一个半透明的红色Brush
                        Dim semiTransparentRedBrush As New SolidBrush(Color.FromArgb(100, Color.Red))
                        ' 绘制一个半透明的红色矩形
                        e.Graphics.FillRectangle(semiTransparentRedBrush, New Rectangle(0, Height * Range.x, Me.Width, Height * Range.y - Height * Range.x))
                    Next
                    pen1.Dispose()
                    pen2.Dispose()
                    pen3.Dispose()
                End If
            End If
        End If


    End Sub



    Public Function 删除拼接图片() As Image
        If 删除区域列表.Count < 1 Then
            Exit Function
        End If
        ' 首先对删除区域列表进行排序和合并
        删除区域列表.Sort(New MyPointComparer())
        合并删除区域()


        转换分割线位置()


        ' 创建一个新的Bitmap，宽度和原图一样，高度是原图的高度减去要删除的部分
        Dim NewImage As New Bitmap(当前图像.Width, CInt(当前图像.Height * (1 - GetTotalDeletePercentage(删除区域列表))))
        Using g As Graphics = Graphics.FromImage(NewImage)
            Dim currentHeight As Single = 0
            Dim previousY As Single = 0
            For Each point As MyPoint In 删除区域列表
                ' 截取从previousY到point.x的部分
                Dim height As Single = point.x - previousY
                g.DrawImage(当前图像, New RectangleF(0, currentHeight, 当前图像.Width, height * 当前图像.Height), New RectangleF(0, previousY * 当前图像.Height, 当前图像.Width, height * 当前图像.Height), GraphicsUnit.Pixel)
                currentHeight += height * 当前图像.Height
                previousY = point.y
            Next
            ' 截取从最后一个删除区域到图片底部的部分
            g.DrawImage(当前图像, New RectangleF(0, currentHeight, 当前图像.Width, (1 - previousY) * 当前图像.Height), New RectangleF(0, previousY * 当前图像.Height, 当前图像.Width, (1 - previousY) * 当前图像.Height), GraphicsUnit.Pixel)
        End Using
        ' 保存新的图片
        Me.当前图像 = NewImage
        删除区域列表.Clear()
        图片缩放(currentScale, True)
        Return Me.当前图像
        'NewImage.Save("NewImage.jpg", Imaging.ImageFormat.Jpeg)
    End Function


    Private Sub 转换分割线位置()

        Dim 当前图片宽度, 当前图片高度, 删除区域高度, 删除后图片高度 As Integer
        当前图片宽度 = 当前图像.Width
        当前图片高度 = 当前图像.Height

        Dim 删除区域列表2 As New List(Of MyPoint)
        For Each range In 删除区域列表
            删除区域列表2.Add(New MyPoint(range))
        Next


        For j = 0 To 删除区域列表2.Count - 1
            删除区域高度 = (删除区域列表2(j).y - 删除区域列表2(j).x) * 当前图像.Height
            删除后图片高度 = 当前图片高度 - 删除区域高度
            For i = 0 To 分页线列表.Count - 1

                If 分页线列表(i) < 删除区域列表2(j).x Then
                    分页线列表(i) = 分页线列表(i) * 当前图片高度 / 删除后图片高度
                ElseIf 分页线列表(i) >= 删除区域列表2(j).x And 分页线列表(i) <= 删除区域列表2(j).y Then
                    分页线列表(i) = 删除区域列表2(j).x * 当前图像.Height / 删除后图片高度
                Else
                    分页线列表(i) = (分页线列表(i) * 当前图片高度 - 删除区域高度) / 删除后图片高度
                End If



            Next
            For k = j + 1 To 删除区域列表2.Count - 1
                删除区域列表2(k).x = (删除区域列表2(k).x * 当前图片高度 - 删除区域高度) / 删除后图片高度
                删除区域列表2(k).y = (删除区域列表2(k).y * 当前图片高度 - 删除区域高度) / 删除后图片高度
            Next

            当前图片高度 = 删除后图片高度
        Next
    End Sub

    Public Function GetTotalDeletePercentage(删除区域列表 As List(Of MyPoint)) As Single
        Dim total As Single = 0
        For Each point As MyPoint In 删除区域列表
            total += point.y - point.x
        Next
        Return total
    End Function














End Class
