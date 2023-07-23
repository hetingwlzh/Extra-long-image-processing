
Imports PdfSharp.Pdf
Imports PdfSharp.Drawing
Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Button
Imports PdfSharp.Charting
Imports System.Drawing.Imaging
Imports System.Web
Imports System.Windows.Forms.VisualStyles.VisualStyleElement

Public Class Form1
    Public PicturePath As String
    'Public splitPositions As New List(Of Single) '分割线列表

    Public originalBitmap As Image ' = Image.FromFile(PicturePath)

    Public 原始预处理图像 As Image
    Public tempImage As Image
    Public PicturWidth As Integer '= originalBitmap.Width
    Public PicturHeight As Integer '= originalBitmap.Height
    Public 临时文件路径 As String = Application.StartupPath & "\temp\"
    Public 左边距 As Integer = 0
    Public 右边距 As Integer = 0
    Public 上边距 As Integer = 0
    Public 下边距 As Integer = 0

    Public 纸张名字 As String = "A4"
    Public 纸张宽度 As Integer = 210
    Public 纸张高度 As Integer = 297

    Public 当前生成的PDF文件路径 As String = 临时文件路径


    Public 拖动操作记录 As Boolean = False
    ' 当前缩放比例
    'Public currentScale As Double = 1
    ' 原始图片


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.WindowState = FormWindowState.Maximized
        SplitContainer1.SplitterWidth = 10
        Label11.Text = "锐度" & TrackBar1.Value
        Label22.Text = "输入小" & TrackBar2.Value
        Label33.Text = "输入大" & TrackBar3.Value
        Label44.Text = "灰度小" & TrackBar4.Value
        Label55.Text = "灰度大" & TrackBar5.Value
        SplitContainer1.SplitterDistance = SplitContainer1.Width * 0.5
        刷新图片()
        Me.KeyPreview = True
        ComboBox1.SelectedIndex = 0

    End Sub

    Private Sub 刷新图片()
        If PicturePath <> "" Then
            MyPictureBox1.Load(PicturePath)
            MyPictureBox1.分页线列表.Clear()
            originalBitmap = Image.FromFile(PicturePath)
            PicturWidth = originalBitmap.Width
            PicturHeight = originalBitmap.Height


            MyPictureBox1.图片缩放(Panel1.Width / PicturWidth)

            刷新信息()
        End If




    End Sub

    Public Sub 刷新信息(Optional point1 As Drawing.Point = Nothing)

        If PicturePath <> "" Then
            Dim fileInfo As New FileInfo(PicturePath)

            Dim resolution As String = originalBitmap.HorizontalResolution & "x" & originalBitmap.VerticalResolution
            Dim sizeInBytes As Long = fileInfo.Length
            Dim size As String = fileInfo.Length / 1024 & " KB"
            Dim fileName As String = fileInfo.Name
            Dim width As Integer = originalBitmap.Width
            Dim height As Integer = originalBitmap.Height
            Dim sizeInPixels As String = width & " × " & height
            If sizeInBytes < 1024 * 1024 Then
                size = Math.Round(sizeInBytes / 1024, 2) & " KB"
            Else
                size = Math.Round(sizeInBytes / (1024 * 1024), 2) & " MB"
            End If
            Dim p As String = point1.ToString
            Label1.Text = "位置: " & p & Environment.NewLine &
                        "尺寸: " & sizeInPixels & Environment.NewLine &
                        "大小: " & size & Environment.NewLine &
                        "文件名: " & fileName & Environment.NewLine &
                        "缩放系数: " & Math.Round(MyPictureBox1.currentScale， 4）


        End If

    End Sub



    Private Function CutPicture(x As Integer, y As Integer, Width As Integer, Height As Integer) As Bitmap
        Dim originalImage As Bitmap = New Bitmap(PicturePath)


        Dim rect As New Rectangle(x, y, Width, Height) '指定截取区域
        '确保截取区域不超出原始图片区域
        Dim rectX As Integer = Math.Min(x, originalImage.Width - Width)
        Dim rectY As Integer = Math.Min(y, originalImage.Height - Height)
        Dim croppedImage As Bitmap = originalImage.Clone(rect, originalImage.PixelFormat)

        Return croppedImage
    End Function

    Private Function CutPicture(px As Double, py As Double, Width As Integer, Height As Integer) As Bitmap
        Dim originalImage As Bitmap = New Bitmap(PicturePath)

        '确保截取区域不超出原始图片区域
        Dim rectX As Integer = Math.Min(originalImage.Width * px, originalImage.Width - Width)
        Dim rectY As Integer = Math.Min(originalImage.Height * py, originalImage.Height - Height)

        Dim rect As New Rectangle(rectX, rectY, Width, Height) '指定截取区域
        Dim croppedImage As Bitmap = originalImage.Clone(rect, originalImage.PixelFormat)

        Return croppedImage
    End Function






    'Public Sub SaveImagesToPdf(picturePath As String, height As Integer, pdfPath As String)
    '    Dim originalImage As Bitmap = New Bitmap(picturePath)
    '    Dim pdf = New PdfDocument()
    '    splitPositions.Clear()
    '    For i As Integer = 0 To originalImage.Height - 1 Step height
    '        Dim page = pdf.AddPage()
    '        page.Height = height
    '        page.Width = originalImage.Width

    '        Dim gfx = XGraphics.FromPdfPage(page)
    '        Dim rect As New Rectangle(0, i, originalImage.Width, Math.Min(height, originalImage.Height - i))
    '        splitPositions.Add(i / originalImage.Height)

    '        Dim croppedImage As Bitmap = originalImage.Clone(rect, originalImage.PixelFormat)
    '        ' 将Bitmap对象保存为文件
    '        Dim tempFilePath As String = 临时文件路径 & "\temp" & i & ".bmp"
    '        croppedImage.Save(tempFilePath)
    '        ' 使用XImage.FromFile方法来创建XImage对象
    '        Dim xImage0 As XImage = XImage.FromFile(tempFilePath)

    '        gfx.DrawImage(xImage0, 0, 0, croppedImage.Width, croppedImage.Height)
    '        xImage0.Dispose()
    '        System.IO.File.Delete(tempFilePath)
    '    Next
    '    MyPictureBox1.分页线列表 = splitPositions
    '    MyPictureBox1.Invalidate()
    '    pdf.Save(pdfPath)
    'End Sub


    Public Sub CutImagesToPdf(picturePath As String, pdfPath As String)
        Dim 待分割图像 As Bitmap = MyPictureBox1.当前图像
        Dim pdf = New PdfDocument()

        MyPictureBox1.分页线列表.Sort()
        MyPictureBox1.分页线列表 = MyPictureBox1.分页线列表.Distinct().ToList()
        For i As Integer = 0 To MyPictureBox1.分页线列表.Count - 1
            Dim Position, nextPosition As Integer
            Position = MyPictureBox1.分页线列表(i) * 待分割图像.Height
            If i + 1 <= MyPictureBox1.分页线列表.Count - 1 Then
                nextPosition = MyPictureBox1.分页线列表(i + 1) * 待分割图像.Height

            Else
                nextPosition = 待分割图像.Height

            End If


            Dim page = pdf.AddPage()
            'page.Height = NumericUpDown1.Value  'nextPosition - Position
            'page.Width = 待分割图像.Width



            page.Width = XUnit.FromMillimeter(纸张宽度)
            page.Height = XUnit.FromMillimeter(纸张高度)














            Dim gfx = XGraphics.FromPdfPage(page)
            Dim rect As New Rectangle(0, Position, 待分割图像.Width, nextPosition - Position)


            Dim croppedImage As Bitmap = 待分割图像.Clone(rect, 待分割图像.PixelFormat)


            ' 将Bitmap对象保存为文件
            Dim tempFilePath As String = 临时文件路径 & "\tempp" & i & ".bmp"
            croppedImage.Save(tempFilePath)
            ' 使用XImage.FromFile方法来创建XImage对象
            Dim xImage0 As XImage = XImage.FromFile(tempFilePath)
            Dim 页边距内宽, 页边距内高, 绘制宽度, 绘制高度 As Integer
            Dim 纵横比 As Single


            左边距 = PrintPictureBox1.获取左边距(page.Width, page.Height)
            右边距 = PrintPictureBox1.获取右边距(page.Width, page.Height)
            上边距 = PrintPictureBox1.获取上边距(page.Width, page.Height)
            下边距 = PrintPictureBox1.获取下边距(page.Width, page.Height)




            '左边距 = 0
            '右边距 = 0
            '上边距 = 0
            '下边距 = 0
            Dim PageWidth, PageHeight As Integer
            PageWidth = page.Width
            PageHeight = page.Height
            If RadioButton1.Checked = True Then '保持纵横比

                页边距内宽 = PageWidth - 左边距 - 右边距
                页边距内高 = PageHeight - 上边距 - 下边距
                纵横比 = croppedImage.Height / croppedImage.Width
                绘制宽度 = 页边距内宽
                绘制高度 = CInt(绘制宽度 * 纵横比)
                If 绘制高度 > 页边距内高 Then
                    绘制高度 = 页边距内高
                    绘制宽度 = CInt(绘制高度 / 纵横比)
                End If
            ElseIf RadioButton2.Checked = True Then '拉伸填充
                绘制宽度 = PageWidth - 左边距 - 右边距
                绘制高度 = PageHeight - 上边距 - 下边距
            End If

            'gfx.DrawImage(xImage0, 左边距, 上边距, 绘制宽度, 绘制高度)
            gfx.DrawImage(xImage0, (PageWidth - 绘制宽度 + 左边距 - 右边距) / 2, 上边距, 绘制宽度, 绘制高度)

            xImage0.Dispose()
            System.IO.File.Delete(tempFilePath)






        Next


        MyPictureBox1.Invalidate()
        pdf.Save(pdfPath)
    End Sub




    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click




        自动分页()

        预览图象(New Drawing.Point(1, 1))


        'SaveImagesToPdf(PicturePath, NumericUpDown1.Value, 临时文件路径 & "\等高分割.pdf")
    End Sub
    Public Function 自动分页(Optional 页数 As Integer = 0)
        Dim 页面总宽, 页面总高 As XUnit
        页面总宽 = XUnit.FromMillimeter(纸张宽度)
        页面总高 = XUnit.FromMillimeter(纸张高度)
        左边距 = PrintPictureBox1.获取左边距(页面总宽, 页面总高)
        右边距 = PrintPictureBox1.获取右边距(页面总宽, 页面总高)
        上边距 = PrintPictureBox1.获取上边距(页面总宽, 页面总高)
        下边距 = PrintPictureBox1.获取下边距(页面总宽, 页面总高)
        MyPictureBox1.分页线列表.Clear()

        Dim 页面填充区宽高比 As Single = (CInt(页面总宽) - 左边距 - 右边距) / (CInt(页面总高) - 上边距 - 下边距)
        Dim 截取高度 As Integer = MyPictureBox1.当前图像.Width / 页面填充区宽高比
        If 页数 = 0 Then '自动分页

            For i As Integer = 0 To MyPictureBox1.当前图像.Height - 1 Step 截取高度
                MyPictureBox1.分页线列表.Add(i / MyPictureBox1.当前图像.Height)
            Next
            PrintPictureBox1.是否为页面布局模式 = True
            PrintPictureBox1.Invalidate()
            MyPictureBox1.Invalidate()
        Else

        End If

    End Function


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If MyPictureBox1.分页线列表.Count > 0 Then
            当前生成的PDF文件路径 = Path.Combine(临时文件路径, "自定义分割" & Now.ToString("yyyy-MM-dd HH：mm：ss") & ".pdf")
            CutImagesToPdf(PicturePath, 当前生成的PDF文件路径)

            Process.Start(当前生成的PDF文件路径)
        Else
            MsgBox("你还没有插入任何分页线。")
            Exit Sub
        End If

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        打开()
    End Sub

    Private Sub 打开()

        Dim openFileDialog As New OpenFileDialog()
        openFileDialog.Filter = "Image Files (*.jpg, *.png, *.bmp，*.jpeg)|*.jpg;*.png;*.bmp;*.jpeg"

        If openFileDialog.ShowDialog() = DialogResult.OK Then
            PicturePath = openFileDialog.FileName
            originalBitmap = Image.FromFile(PicturePath)
            MyPictureBox1.SetImage(originalBitmap)
            MyPictureBox1.当前预览页面 = -1

            PicturWidth = originalBitmap.Width
            PicturHeight = originalBitmap.Height



            Button6.Enabled = True
            Button7.Enabled = True
            ComboBox1.Enabled = True
            NumericUpDown1.Enabled = True
            Button3.Enabled = True
            Button4.Enabled = True
            Button1.Enabled = True
            MyPictureBox1.Enabled = True
            Panel1.Enabled = True





            刷新图片()
            MyPictureBox1.图片缩放(Panel1.Width / PicturWidth - 0.02, True)
            刷新信息()
        End If

    End Sub



    Private Sub MyPictureBox1_MouseClick(sender As Object, e As MouseEventArgs) Handles MyPictureBox1.MouseClick
        If 拖动操作记录 = True Then
            拖动操作记录 = False
            If MyPictureBox1.当前预览页面 >= 0 Then
                Dim p As New Drawing.Point(1, MyPictureBox1.Height * MyPictureBox1.分页线列表(MyPictureBox1.当前预览页面) + 1)
                预览图象(p)
            End If


        Else
            预览图象(e.Location)
        End If

    End Sub

    Public Sub 预览图象(position As Drawing.Point)


        Dim x, y As Double
        x = MyPictureBox1.当前图像.Width * (position.X) / MyPictureBox1.Width
        y = MyPictureBox1.当前图像.Height * (position.Y) / MyPictureBox1.Height
        ''Console.WriteLine("X: " + position.X.ToString() + ", Y: " + position.Y.ToString())


        'x = (position.X) / MyPictureBox1.Width - PictureBox2.Width / 2 / PicturWidth
        'y = (position.Y) / MyPictureBox1.Height - PictureBox2.Height / 2 / PicturHeight
        'If x < 0 Then
        '    x = 0
        'End If
        'If y < 0 Then
        '    y = 0
        'End If
        'PictureBox2.Image = CutPicture(x, y, PictureBox2.Width, PictureBox2.Height)
        'PrintPictureBox1.Image = CutPicture(x, y, PictureBox2.Width, PictureBox2.Height)
        If MyPictureBox1.分页线列表.Count = 0 Then
            PrintPictureBox1.是否为页面布局模式 = False
        End If
        If PrintPictureBox1.是否为页面布局模式 Then
            MyPictureBox1.分页线列表.Sort()
            Dim target As Single
            Dim start As Single
            Dim over As Single
            target = (position.Y) / MyPictureBox1.Height
            If MyPictureBox1.分页线列表.Count = 0 Then
                start = -1
                over = -1

            ElseIf target >= MyPictureBox1.分页线列表(MyPictureBox1.分页线列表.Count - 1) Then
                start = MyPictureBox1.分页线列表(MyPictureBox1.分页线列表.Count - 1)
                over = -1
                MyPictureBox1.当前预览页面 = MyPictureBox1.分页线列表.Count - 1

            Else
                For i As Integer = 0 To MyPictureBox1.分页线列表.Count - 2
                    If MyPictureBox1.分页线列表(i) < target AndAlso target <= MyPictureBox1.分页线列表(i + 1) Then
                        start = MyPictureBox1.分页线列表(i)
                        over = MyPictureBox1.分页线列表(i + 1)
                        MyPictureBox1.当前预览页面 = i

                        Exit For
                    End If
                Next
            End If
            MyPictureBox1.Invalidate()
            Dim showImage As Bitmap = New Bitmap(MyPictureBox1.当前图像)

            If start >= 0 And over = -1 Then
                over = 1
            End If

            If start >= 0 And over > 0 Then
                '确保截取区域不超出原始图片区域
                Dim startY As Integer = CInt(showImage.Height * start)
                Dim overY As Integer = CInt(showImage.Height * over)

                Dim rect As New Rectangle(0, startY, showImage.Width, overY - startY) '指定截取区域
                Dim croppedImage As Bitmap = showImage.Clone(rect, showImage.PixelFormat)

                原始预处理图像 = PrintPictureBox1.CreatImg(croppedImage, New Drawing.Point(x, y))
            End If


        Else

            原始预处理图像 = PrintPictureBox1.CreatImg(MyPictureBox1.当前图像, New Drawing.Point(x, y))
        End If


        PrintPictureBox1.Invalidate()
    End Sub



    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click

        适合宽度显示()
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click

        适合高度显示()
    End Sub

    Public Sub 适合宽度显示(Optional 修正宽度 As Integer = 20)

        MyPictureBox1.图片缩放((Panel1.Width - 修正宽度) / MyPictureBox1.当前图像.Width, True)
        刷新信息()
    End Sub
    Public Sub 适合高度显示(Optional 修正高度 As Integer = 20)

        MyPictureBox1.图片缩放((Panel1.Height - 修正高度) / MyPictureBox1.当前图像.Height, True)
        刷新信息()
    End Sub
    'Private Sub 图片缩放(比例 As Single, Optional 是否绝对缩放 As Boolean = False)

    '    If 是否绝对缩放 = False Then
    '        If PicturWidth * currentScale * 比例 > 10 Then
    '            currentScale *= 比例
    '            MyPictureBox1.Invalidate()
    '            MyPictureBox1.Top = 0
    '            MyPictureBox1.Left = 0
    '            MyPictureBox1.Width = PicturWidth * currentScale
    '            MyPictureBox1.Height = PicturHeight * currentScale

    '        End If
    '    Else
    '        If PicturWidth * 比例 > 10 Then
    '            currentScale = 比例
    '            MyPictureBox1.Invalidate()
    '            MyPictureBox1.Top = 0
    '            MyPictureBox1.Left = 0
    '            MyPictureBox1.Width = PicturWidth * currentScale
    '            MyPictureBox1.Height = PicturHeight * currentScale

    '        End If
    '    End If

    '    刷新信息()
    'End Sub

    Private Sub Panel1_DoubleClick(sender As Object, e As EventArgs) Handles Panel1.DoubleClick
        适合宽度显示()
    End Sub

    Private Sub Panel1_MouseWheel(sender As Object, e As MouseEventArgs) Handles Panel1.MouseWheel


        If Control.ModifierKeys = Keys.Control Then
            ' 按住Ctrl时缩放
            'Dim scrollPosition As Integer = Panel1.VerticalScroll.Value / Panel1.VerticalScroll.Maximum

            If e.Delta > 0 Then
                MyPictureBox1.图片缩放(1.03)

            Else
                MyPictureBox1.图片缩放(0.97)

            End If

            'Panel1.VerticalScroll.Value = Panel1.VerticalScroll.Maximum * scrollPosition

            ' 将滚动条设置回原来的位置

            MyPictureBox1.Invalidate()
            Exit Sub
        End If

        If MyPictureBox1.Top >= 3 Then
            MyPictureBox1.Top = 3
        ElseIf MyPictureBox1.Top <= -(MyPictureBox1.Height - Panel1.Height) Then
            MyPictureBox1.Top = -(MyPictureBox1.Height - Panel1.Height)
        Else


            MyPictureBox1.Top = MyPictureBox1.Top + e.Delta

        End If


        刷新信息()


    End Sub

    Private Sub Panel1_SizeChanged(sender As Object, e As EventArgs) Handles Panel1.SizeChanged
        MyPictureBox1.Width = Panel1.Width
        MyPictureBox1.图片缩放(1)
        刷新信息()
    End Sub

    'Private Sub PictureBox2_Paint(sender As Object, e As PaintEventArgs) Handles PictureBox2.Paint



    '    Dim pen1 As New Pen(Color.Red, 0.1)
    '    pen1.DashStyle = Drawing2D.DashStyle.Dash
    '    e.Graphics.DrawLine(pen1, 0, CInt(PictureBox2.Height / 2), PictureBox2.Width, CInt(PictureBox2.Height / 2))
    '    e.Graphics.DrawLine(pen1, CInt(PictureBox2.Width / 2), 0, CInt(PictureBox2.Width / 2), PictureBox2.Height)
    '    pen1.Dispose()


    'End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If 当前生成的PDF文件路径 = 临时文件路径 Or (Not File.Exists(当前生成的PDF文件路径)) Then
            Process.Start("explorer.exe", 当前生成的PDF文件路径)
        Else
            Process.Start("explorer.exe", "/select," & 当前生成的PDF文件路径)
        End If


    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        设置纸张(ComboBox1.SelectedItem.ToString())
    End Sub

    Public Sub 设置纸张(PaperName As String)

        Dim PaperWidth As Integer
        Dim PaoerHeight As Integer

        If PaperName = "A4纸" Then
            ' 执行A4纸任务的代码
            NumericUpDown1.Value = 1800
            PaperWidth = 210
            PaoerHeight = 297

        ElseIf PaperName = "B5纸" Then
            ' 执行B5纸任务的代码
            NumericUpDown1.Value = 1500
            PaperWidth = 176
            PaoerHeight = 250

        ElseIf PaperName = "16开" Then
            ' 执行16开任务的代码
            NumericUpDown1.Value = 1600
            PaperWidth = 185
            PaoerHeight = 260

        ElseIf PaperName = "演草纸" Then
            ' 执行演草纸任务的代码
            NumericUpDown1.Value = 1400
            PaperWidth = 185
            PaoerHeight = 260
        ElseIf PaperName = "自定义" Then
            ' 执行自定义任务的代码
            NumericUpDown1.Value = 2000
            PaperWidth = 210
            PaoerHeight = 210
        End If
        纸张名字 = PaperName
        纸张宽度 = PaperWidth
        纸张高度 = PaoerHeight


        PrintPictureBox1.设置纸张(PaperName, PaperWidth, PaoerHeight)


    End Sub


    Private Sub MyPictureBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles MyPictureBox1.MouseDoubleClick
        适合高度显示()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        PrintPictureBox1.是否为页面布局模式 = False
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            Button2.Enabled = True
            Button8.Enabled = True
            SplitContainer1.Panel2Collapsed = False
        Else
            Button2.Enabled = False
            Button8.Enabled = False
            SplitContainer1.Panel2Collapsed = True
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        PrintPictureBox1.是否为页面布局模式 = True
    End Sub

    Private Sub PrintPictureBox1_SizeChanged(sender As Object, e As EventArgs) Handles PrintPictureBox1.SizeChanged
        Me.Invalidate()
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll, TrackBar2.Scroll, TrackBar5.Scroll, TrackBar4.Scroll, TrackBar3.Scroll

        'MyPictureBox1.MyImage = AdjustImage(originalBitmap, (TrackBar1.Value + 10) / 10, (TrackBar2.Value + 10) / 10, (NumericUpDown2.Value) / 10)
        预览图片处理()
    End Sub
    Public Sub 预览图片处理()

        PrintPictureBox1.MyImage = SharpenImage(原始预处理图像,
                    (TrackBar1.Value / 20) * 1,
                    TrackBar2.Value / 20 * 255,
                    TrackBar3.Value / 20 * 255,
                    TrackBar4.Value / 20 * 255,
                    TrackBar5.Value / 20 * 255,
                    CheckBox2.Checked)


        Label11.Text = "锐度" & TrackBar1.Value
        Label22.Text = "输入小" & TrackBar2.Value
        Label33.Text = "输入大" & TrackBar3.Value
        Label44.Text = "灰度小" & TrackBar4.Value
        Label55.Text = "灰度大" & TrackBar5.Value

        PrintPictureBox1.Invalidate()
    End Sub
    Private Sub Button9_MouseDown(sender As Object, e As MouseEventArgs) Handles Button9.MouseDown
        tempImage = MyPictureBox1.当前图像
        MyPictureBox1.当前图像 = MyPictureBox1.原始图像
        MyPictureBox1.Invalidate()
    End Sub

    Private Sub Button9_MouseUp(sender As Object, e As MouseEventArgs) Handles Button9.MouseUp
        MyPictureBox1.当前图像 = tempImage
        MyPictureBox1.Invalidate()
    End Sub

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        'MyPictureBox1.MyImage = AdjustImage(originalBitmap, (TrackBar1.Value + 10) / 10, (TrackBar2.Value + 10) / 10, (NumericUpDown2.Value) / 10)
        If MyPictureBox1.当前图像 IsNot Nothing Then
            MyPictureBox1.当前图像 = SharpenImage(MyPictureBox1.当前图像,
                  (TrackBar1.Value / 20) * 1,
                        TrackBar2.Value / 20 * 255,
                        TrackBar3.Value / 20 * 255,
                        TrackBar4.Value / 20 * 255,
                        TrackBar5.Value / 20 * 255,
                        CheckBox2.Checked)
            MyPictureBox1.Invalidate()

            MsgBox("已处理完毕！")
        Else
            MsgBox("你还没有打开图片")
        End If



    End Sub

    Private Sub 添加分页线ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 添加分页线ToolStripMenuItem.Click
        MyPictureBox1.添加分页线(MyPictureBox1.鼠标点击位置.Y / MyPictureBox1.Height)

    End Sub

    Private Sub MyPictureBox1_MouseDown(sender As Object, e As MouseEventArgs) Handles MyPictureBox1.MouseDown


    End Sub

    Private Sub 删除分页线ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 删除分页线ToolStripMenuItem.Click
        MyPictureBox1.删除当前分页线()
    End Sub

    Private Sub 清除所有分页线ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 清除所有分页线ToolStripMenuItem.Click
        If MsgBox("确定要清除所有分页线吗？", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            MyPictureBox1.删除所有分页线()
        End If

    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        If RadioButton1.Checked Then
            PrintPictureBox1.是否保持纵横比 = True
            PrintPictureBox1.Invalidate()
        End If


    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        If RadioButton2.Checked Then
            PrintPictureBox1.是否保持纵横比 = False
            PrintPictureBox1.Invalidate()
        End If
    End Sub

    Private Sub SplitContainer1_Panel2_SizeChanged(sender As Object, e As EventArgs) Handles SplitContainer1.Panel2.SizeChanged
        PrintPictureBox1.所在容器的宽度 = SplitContainer1.Panel2.Width
        PrintPictureBox1.所在容器的高度 = SplitContainer1.Panel2.Height
    End Sub



    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        about.Show()
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        MyPictureBox1.SetImage(originalBitmap)
        MyPictureBox1.Invalidate()

    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        预览图片处理()
    End Sub

    Private Sub 添加删除区ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 添加删除区ToolStripMenuItem.Click
        MyPictureBox1.在当前位置添加删除区域()
    End Sub

    Private Sub 执行区域删除ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 执行区域删除ToolStripMenuItem.Click
        ' 保存滚动条的位置


        'Dim scrollPosition As Integer = Panel1.VerticalScroll.Value
        Dim top As Integer = MyPictureBox1.Top


        MyPictureBox1.删除拼接图片()

        ' 保存滚动条的位置



        ' 将滚动条设置回原来的位置
        'Panel1.VerticalScroll.Value = Panel1.VerticalScroll.Maximum
        MyPictureBox1.Top = top

    End Sub

    Private Sub ContextMenuStrip1_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip1.Opening
        If MyPictureBox1.鼠标选取的删除区域序号 >= 0 Then
            设为默认ToolStripMenuItem.Visible = True
        Else
            设为默认ToolStripMenuItem.Visible = False
        End If

        If MyPictureBox1.鼠标拖拽删除区域序号 >= 0 Then
            删除删除区ToolStripMenuItem.Visible = True
        Else
            删除删除区ToolStripMenuItem.Visible = False
        End If

        If MyPictureBox1.鼠标拖拽分割线序号 >= 0 Then
            删除分页线ToolStripMenuItem.Visible = True
        Else
            删除分页线ToolStripMenuItem.Visible = False
        End If


        If MyPictureBox1.删除区域列表.Count > 0 Then
            执行区域删除ToolStripMenuItem.Visible = True
            清除所有删除区ToolStripMenuItem.Visible = True
        Else
            执行区域删除ToolStripMenuItem.Visible = False
            清除所有删除区ToolStripMenuItem.Visible = False
        End If


        If MyPictureBox1.分页线列表.Count > 0 Then
            清除所有分页线ToolStripMenuItem.Visible = True
        Else
            清除所有分页线ToolStripMenuItem.Visible = False
        End If
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        Dim top As Integer = MyPictureBox1.Top


        MyPictureBox1.删除拼接图片()

        MyPictureBox1.Top = top
    End Sub

    Private Sub 删除删除区ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 删除删除区ToolStripMenuItem.Click
        MyPictureBox1.删除当前删除区域()
    End Sub

    Private Sub 清除所有删除区ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 清除所有删除区ToolStripMenuItem.Click
        If MsgBox("确定要 清除所有删除区 吗？", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            MyPictureBox1.清除空删除区域列表()
        End If



    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        预览图片处理()
    End Sub

    Private Sub PrintPictureBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles PrintPictureBox1.MouseDoubleClick
        PrintPictureBox1.是否为页面布局模式 = Not PrintPictureBox1.是否为页面布局模式
        PrintPictureBox1.Invalidate()
    End Sub

    Private Sub MyPictureBox1_PreviewKeyDown(sender As Object, e As PreviewKeyDownEventArgs) Handles MyPictureBox1.PreviewKeyDown

    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles MyBase.KeyDown

        If e.KeyCode = Keys.Q Then
            MyPictureBox1.添加分页线(MyPictureBox1.鼠标点击位置.Y / MyPictureBox1.Height)

        ElseIf e.KeyCode = Keys.W Then
            MyPictureBox1.在当前位置添加删除区域()
        ElseIf e.KeyCode = Keys.E Then
            Dim top As Integer = MyPictureBox1.Top
            MyPictureBox1.删除拼接图片()
            MyPictureBox1.Top = top
        End If
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        If MyPictureBox1.当前图像 IsNot Nothing Then
            If MyPictureBox1.修边模式 = True Then
                Button15.Text = "修边模式"
                Button15.BackColor = SystemColors.ActiveBorder



                MyPictureBox1.开始修边()

                适合宽度显示()


            Else
                Button15.BackColor = Color.Orange
                Button15.Text = "执行修边"
            End If

            MyPictureBox1.修边模式 = Not MyPictureBox1.修边模式
            MyPictureBox1.Invalidate()
        End If



    End Sub

    Private Sub 刷新ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 刷新ToolStripMenuItem.Click
        PrintPictureBox1.Invalidate()
    End Sub

    Private Sub 重置页边距ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 重置页边距ToolStripMenuItem.Click
        PrintPictureBox1.重置页边距()
        PrintPictureBox1.Invalidate()
    End Sub

    Private Sub 设为默认ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 设为默认ToolStripMenuItem.Click
        MyPictureBox1.设置当前删除区域默认高度()
    End Sub

    Private Sub ContextMenuStrip2_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ContextMenuStrip2.Opening

    End Sub
End Class
