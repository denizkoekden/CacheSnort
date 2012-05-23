' ***********************************
'Cache Snort by Deniz Kökden 2011
'in 12h geschrieben und immernoch verbuggt!
'Funktioniert aber gut wenn man sich auskennt ;)
'TODO: 
'   -Player //vllt.
'   -schönere GUI // vllt.
'   -Code bereinigung // WICHTIG!
' ***********************************
' Quellen:
'Klasse für INI: http://dotnet-snippets.de/dns/klasse-fuer-verwendung-von-ini-dateien-SID938.aspx
'Cache ermitteln: http://www.vb-paradise.de/allgemeines/sourcecode-austausch/46349-firefox-und-google-chrome-cache-pfad-ermitteln/



Imports System.IO
Imports System.Net

Public Class Form1

    Public lnk As String
    Dim TRT As String
    Dim oldfile As String = ""

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        CacheSuchen()
        Label3.Text = ""


        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "Images") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "Images")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "MP3") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "MP3")

        End If

        If File.Exists("cache_snort_config.ini") Then

            LoadConfig()
        Else
            WriteDefaultConfig()
            LoadConfig()
        End If


    End Sub


    Private Sub Button3_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        FolderBrowserDialog1.ShowDialog()
        ComboBox1.Text = FolderBrowserDialog1.SelectedPath
        ListBox3.Items.Add(ComboBox1.Text)
    End Sub

    Private Sub Button9_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button9.Click
        ListBox3.Items.Clear()
    End Sub

    Private Sub Button10_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button10.Click
        Try
            ListBox3.Items.RemoveAt(ListBox3.SelectedIndex.ToString)
        Catch
            My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
        End Try

    End Sub

    Private Sub Button4_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        CacheSuchen()

    End Sub

    Private Sub Button2_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim file As String, i As Integer



        ListBox1.Items.Clear()
        ListBox2.Items.Clear()

        For i = 0 To ListBox3.Items.Count - 1


            Dim files() As String = Directory.GetFiles(ListBox3.Items.Item(i))

            For Each file In files
                ListBox1.Items.Add(file)

            Next
        Next i

        Try
            ProgressBar1.Maximum = ListBox1.Items.Count - 1
            ProgressBar1.Minimum = 0
            Label3.Text = "Snorte" & " " & ListBox1.Items.Count.ToString & "Dateien..."
            Label3.Refresh()


        Catch

            MsgBox("Cache ist leer!")


        End Try


        Button6_Click_1(Nothing, New System.EventArgs)

    End Sub

    Private Sub Button6_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click

        On Error Resume Next

        Dim i As Integer, datei As String, fs As FileStream, br As BinaryReader
        Dim id3tag As String, id3Tag2 As String
        Dim flvcount As Integer
        Dim imagecount As Integer
        Dim mp3count As Integer
        Dim gifcount As Integer
        Dim jpegcount As Integer
        Dim pngcount As Integer

        flvcount = 0
        imagecount = 0
        mp3count = 0
        gifcount = 0
        jpegcount = 0
        pngcount = 0





        For i = 0 To ListBox1.Items.Count - 1
            ListBox1.SelectedIndex = i

            Dim reader As StreamReader = New StreamReader(ListBox1.Text)

            TextBox1.Text = reader.ReadLine
            reader.Close()

            ProgressBar1.Value = i

            'Erste If abfrage "Ist FLV?
            If InStr(TextBox1.Text, "FLV", CompareMethod.Text) <> 0 Then
                'Zweite If abfrage "FLV aktiv?
                If CheckBox2.Checked = True Then



                    File.Copy(ListBox1.Text, ComboBox2.Text & "\" & "FLV" & "\" & "CacheSnort_" & CStr(i) & "_" & Format(Now, "yyyyMMdd_HHmmss") & ".flv")
                    flvcount = flvcount + 1
                    File.Delete(ListBox1.Text)
                    'Zweite If abfrage schließen (FLV ist aktiv)
                Else : End If

                'Wenn Datei = kein FLV dann
            Else


                datei = ListBox1.Text
                id3tag = ""
                id3Tag2 = ""


                Dim mp3file As New FileInfo(datei)
                Dim tagpostion As Integer = (mp3file.Length - 128)

                'Die ausgelesen ID3-Tags
                Dim Song As String
                Dim Artist As String
                Dim Album As String

                'Die bereinigten ID3-Tags
                Dim Song_Clean As String
                Dim Album_Clean As String
                Dim Artist_Clean As String


                fs = New FileStream(datei, FileMode.Open, FileAccess.Read)
                br = New BinaryReader(fs)

                fs.Position = tagpostion
                id3tag = br.ReadChars(3)
                'dritte If abfrage ist ein MP3 mit ID3-Tag v1 oder v2?
                If id3tag <> "ID3" Then
                    fs.Position = 0
                    id3Tag2 = br.ReadChars(3)
                    'dritte If abfrage abschließen
                End If

                'vierte If abfrage ist MP3 aktiviert?

                If id3tag = "ID3" Or id3Tag2 = "ID3" Then
                    'Ja ist es Treffer!
                    If CheckBox1.Checked = True Then
                        mp3count = mp3count + 1

                        fs.Position = tagpostion + 3
                        Song = br.ReadChars(30)
                        Song = Song.Trim(vbNullChar)
                        Song_Clean = SafeFileName(Song)

                        fs.Position = tagpostion + 33
                        Artist = br.ReadChars(30)
                        Artist = Artist.Trim(vbNullChar)
                        Artist_Clean = SafeFileName(Artist)

                        fs.Position = tagpostion + 63
                        Album = br.ReadChars(30)
                        Album = Album.Trim(vbNullChar)
                        Album_Clean = SafeFileName(Album)

                        fs.Close()

                        File.Copy(ListBox1.Text, ComboBox2.Text & "\" & "MP3" & "\" & Artist_Clean & "-" & Song_Clean & ".mp3")
                        File.Delete(ListBox1.Text)
                    End If
                Else 'Wenn datei auch kein MP3 dann





                    Dim GIFreader As StreamReader = New StreamReader(ListBox1.Text)



                    TextBox1.Text = GIFreader.ReadLine
                    GIFreader.Close()

                    ProgressBar1.Value = i
                    If CheckBox3.Checked = True Then
                        If InStr(TextBox1.Text, "GIF", CompareMethod.Text) <> 0 Then 'sechste If abfrage ist es denn eine GIF Datei?


                            imagecount = imagecount + 1
                            gifcount = gifcount + 1
                            File.Copy(ListBox1.Text, ComboBox2.Text & "\" & "Images" & "\" & "CacheSnort_" & CStr(i) & "_" & Format(Now, "yyyyMMdd_HHmmss") & ".gif")

                        Else

                            Dim JFIFreader As StreamReader = New StreamReader(ListBox1.Text)

                            TextBox1.Text = JFIFreader.ReadLine
                            JFIFreader.Close()

                            ProgressBar1.Value = i

                            If InStr(TextBox1.Text, "JFIF", CompareMethod.Text) <> 0 Then 'suche auch nach JPEG 

                                imagecount = imagecount + 1
                                jpegcount = jpegcount + 1
                                File.Copy(ListBox1.Text, ComboBox2.Text & "\" & "Images" & "\" & "CacheSnort_" & CStr(i) & "_" & Format(Now, "yyyyMMdd_HHmmss") & ".jpg")
                            Else
                                Dim PNGreader As StreamReader = New StreamReader(ListBox1.Text)

                                TextBox1.Text = PNGreader.ReadLine
                                PNGreader.Close()

                                ProgressBar1.Value = i

                                If InStr(TextBox1.Text, "PNG", CompareMethod.Text) <> 0 Then 'suche auch nach PNG

                                    imagecount = imagecount + 1
                                    pngcount = pngcount + 1
                                    File.Copy(ListBox1.Text, ComboBox2.Text & "\" & "Images" & "\" & "CacheSnort_" & CStr(i) & "_" & Format(Now, "yyyyMMdd_HHmmss") & ".png")


                                Else : End If
                            End If
                        End If
                    End If
                End If
            End If




        Next i

        ProgressBar1.Value = 0
        ListBox2.Items.Clear()
        ListBox2.Items.Add("Folgende Daten wurden gefunden:")
        ListBox2.Items.Add(flvcount.ToString & " " & "Flash-Dateien gefunden!")
        ListBox2.Items.Add(mp3count.ToString & " " & "MP3-Dateien gefunden!")
        ListBox2.Items.Add(imagecount.ToString & " " & "Bilder gefunden!")
        ListBox2.Items.Add(jpegcount.ToString & " " & "davon sind jpg Dateien")
        ListBox2.Items.Add(gifcount.ToString & " " & "davon sind gif Dateien")
        ListBox2.Items.Add(pngcount.ToString & " " & "davon sind png Dateien")
        Label3.Text = "Scan abgeschlossen!"




    End Sub

    Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click
        FolderBrowserDialog2.ShowDialog()
        ComboBox2.Text = FolderBrowserDialog2.SelectedPath
    End Sub

    Private Sub CacheSuchen()

        Dim LocalApplicationData As String = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
        Dim FirefoxPfad As String = LocalApplicationData & "\Mozilla\Firefox\Profiles\"

        Dim FirefoxCache As String = ""
        Dim ChromeCache As String = LocalApplicationData & "\Google\Chrome\User Data\Default\Cache"


        If IO.Directory.Exists(FirefoxPfad) = True Then
            Dim profiles() As String = IO.Directory.GetDirectories(FirefoxPfad)
            Dim profilelist As New ListBox
            For Each i As String In profiles
                Dim profiledir As String = IO.Path.GetFileName(i)
                profilelist.Items.Add(profiledir)
                For j = 0 To profilelist.Items.Count - 1
                    FirefoxCache = FirefoxPfad & profilelist.Items.Item(j) & "\Cache"
                Next
            Next i
            If Not ListBox3.Items.Contains(FirefoxCache) Then
                ListBox3.Items.Add(FirefoxCache)
            End If
        Else : End If

        If IO.Directory.Exists(ChromeCache) = True Then
            If Not ListBox3.Items.Contains(ChromeCache) Then
                ListBox3.Items.Add(ChromeCache)
            End If
        Else : End If


    End Sub


    Private Sub SaveConfig()
        Dim INI As New INIDatei, i As Integer

        INI.Pfad = My.Application.Info.DirectoryPath & "\" & "cache_snort_config.ini"

        For i = 0 To ListBox3.Items.Count - 1


            INI.WertSchreiben("Caches", CStr(i), ListBox3.Items(i))
        Next i
        INI.WertSchreiben("Ausgabeort", "Location", ComboBox2.Text)
        INI.WertSchreiben("Caches", "max", ListBox3.Items.Count - 1)

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "Images") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "Images")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "MP3") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "MP3")

        End If



    End Sub

    Private Sub WriteDefaultConfig()
        Dim INI As New INIDatei, i As Integer

        INI.Pfad = My.Application.Info.DirectoryPath & "\" & "cache_snort_config.ini"

        For i = 0 To ListBox3.Items.Count - 1


            INI.WertSchreiben("Caches", CStr(i), ListBox3.Items(i))
        Next i
        INI.WertSchreiben("Ausgabeort", "Location", "C:\CacheSnort")
        INI.WertSchreiben("Caches", "max", ListBox3.Items.Count - 1)

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "Images") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "Images")

        End If

        If System.IO.Directory.Exists(ComboBox2.Text & "\" & "MP3") = False Then

            System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "MP3")

        End If

    End Sub


    Private Sub LoadConfig()
        Dim INI As New INIDatei, max As Integer

        INI.Pfad = My.Application.Info.DirectoryPath & "\" & "cache_snort_config.ini"
        Try
            ComboBox2.Text = INI.WertLesen("Ausgabeort", "Location")
            max = CInt(INI.WertLesen("Caches", "max"))
            ListBox3.Items.Clear()

            For i = 0 To max

                ListBox3.Items.Add(INI.WertLesen("Caches", CStr(i)))
            Next

            If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV") = False Then

                System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV")

            End If

            If System.IO.Directory.Exists(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube") = False Then

                System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "FLV" & "\" & "YouTube")

            End If

            If System.IO.Directory.Exists(ComboBox2.Text & "\" & "Images") = False Then

                System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "Images")

            End If

            If System.IO.Directory.Exists(ComboBox2.Text & "\" & "MP3") = False Then

                System.IO.Directory.CreateDirectory(ComboBox2.Text & "\" & "MP3")

            End If

        Catch
            File.Delete(INI.Pfad)
            WriteDefaultConfig()
            MsgBox("Die Standart Konfiguration wurde wiederhergestellt!", , "Konfigurationsdatei beschädigt!")

        End Try



    End Sub


    Public Function SafeFileName(ByVal nom As String) As String


        Dim safe As String = nom.Trim

        'Leerzeichen ersetzen
        safe = safe.Replace(" ", "-")

        'doppelte Leerzeichen ersetzen
        If safe.IndexOf("--") > 1 Then
            While (safe.IndexOf("--") > -1)
                safe = safe.Replace("--", "-")
            End While
        End If

        'Ungültige Buchstaben aussotieren
        safe = System.Text.RegularExpressions.Regex.Replace(safe, "[^A-Za-z0-9\\-_]", "")

        'Länge kürzen
        If safe.Length > 50 Then
            safe = safe.Substring(0, 49)
        End If

        'Start und Ende einer Datei sichern
        Dim replace As Char() = {"-", "."}
        safe = safe.TrimStart(replace)
        safe = safe.TrimEnd(replace)

        Return safe

    End Function

    Private Sub TabControl1_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabControl1.Click
        If TabControl1.SelectedIndex = 0 Then
            Me.Text = ("CacheSnort - SnortModus")
            SaveConfig()
        Else
            Me.Text = ("CacheSnort - Einstellungen")
        End If
    End Sub

    Private Sub Button11_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button11.Click
        SaveConfig()
    End Sub

    Private Sub Button8_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button8.Click
        If ValidDirPath(ComboBox1.Text) = True Then
            ListBox3.Items.Add(ComboBox1.Text)
        Else
        End If

    End Sub

    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

        Dim i As Integer

        For i = 0 To ListBox3.Items.Count - 1


            Dim files() As String = Directory.GetFiles(ListBox3.Items.Item(i))
            Dim file As String




            For Each file In files
                Try
                    ListBox1.Items.Remove(file)

                    My.Computer.FileSystem.DeleteFile(file,
                        Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs,
                        Microsoft.VisualBasic.FileIO.RecycleOption.DeletePermanently)

                Catch ex As Exception


                    MsgBox("Datei noch in Benutzung, Browser offen?")


                End Try

            Next
        Next i

           


    End Sub

    Private Sub Button1_Click_1(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        ListBox1.Items.Clear()
        ListBox2.Items.Clear()
    End Sub



    Private Sub Button12_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button12.Click
        Application.Exit()
    End Sub

    Public Function ValidDirPath(ByVal Path As String) As Boolean
        Dim ASCValue As Integer = Asc(UCase(Mid(Path, 1, 1)))
        If InStr(Path, "\\") = 0 And InStr(Path, "/") = 0 And InStr(Path, "*") = 0 And InStr(Path, "?") = 0 And InStr(Path, """") = 0 And InStr(Path, "<") = 0 And InStr(Path, ">") = 0 And InStr(Path, "|") = 0 Then
            If ASCValue >= 65 And ASCValue <= 90 Then
                If Mid(Path, 2, 2) = ":\" Then
                    Return True
                Else
                    Return False
                End If
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    End Class