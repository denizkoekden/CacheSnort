Imports System.Text
Imports System.ComponentModel
Imports System.Runtime.InteropServices

Public Class FlashPlayer
    Inherits UserControl

    Private m_FlashHost As FlashHost
    Private m_AutoPlay As Boolean = True
    Private m_ColorFrom As Color = Color.Gray
    Private m_ColorTo As Color = Color.Gray
    Private m_ShowBorder As Boolean = False
    Private m_IsPlayerInit As Boolean = False

    ''' <summary>
    ''' Controller-Host für den FlashPlayer zur Darstellung
    ''' in einem UserControl.
    ''' </summary>
    Private Class FlashHost
        Inherits AxHost

        Public Sub New(ByVal sCLSID As String)
            MyBase.New(sCLSID)
        End Sub

        Public ReadOnly Property Player() As Object
            Get
                Return Me.GetOcx
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Initialisiert das UserControl
    ''' </summary>
    Public Sub New()
        Try
            ' Versionsunabhängiges LateBinding des FlashPlayers
            Dim tFlashPlayer As Type = Type.GetTypeFromProgID("ShockwaveFlash.ShockwaveFlash")
            If tFlashPlayer IsNot Nothing Then
                m_FlashHost = New FlashHost(tFlashPlayer.GUID.ToString)
                DirectCast(m_FlashHost, ISupportInitialize).BeginInit()
                SuspendLayout()
                m_FlashHost.Visible = True
                m_FlashHost.Dock = DockStyle.Fill
                Controls.Add(m_FlashHost)
                DirectCast(m_FlashHost, ISupportInitialize).EndInit()
                ResumeLayout(False)
                PerformLayout()
                m_IsPlayerInit = True
            Else
                MessageBox.Show("Der FlashPlayer konnte nicht initialisiert werden.", _
                   "Überprüfung FlashPlayer", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Initialisierung FlashPlayer", _
               MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Legt fest, ob das Video sofort abgespielt werden soll.
    ''' </summary>
    <DefaultValue(GetType(System.Boolean), "True")> _
    Public Property AutoPlay() As Boolean
        Get
            Return m_AutoPlay
        End Get
        Set(ByVal value As Boolean)
            m_AutoPlay = value
        End Set
    End Property

    ''' <summary>
    ''' Legt die erste Verlaufsfarbe für Border und Navigator fest.
    ''' </summary>
    <DefaultValue(GetType(System.Drawing.Color), "Gray")> _
    Public Property ColorFrom() As Color
        Get
            Return m_ColorFrom
        End Get
        Set(ByVal value As Color)
            m_ColorFrom = value
        End Set
    End Property

    ''' <summary>
    ''' Legt die zweite Verlaufsfarbe für Border und Navigator fest.
    ''' </summary>
    <DefaultValue(GetType(System.Drawing.Color), "Gray")> _
    Public Property ColorTo() As Color
        Get
            Return m_ColorTo
        End Get
        Set(ByVal value As Color)
            m_ColorTo = value
        End Set
    End Property

    ''' <summary>
    ''' Zeigt an, ob ein FlashPlayer initialisiert werden konnte.
    ''' </summary>
    <Browsable(False)> _
    Public ReadOnly Property IsPlayerInit() As Boolean
        Get
            Return m_IsPlayerInit
        End Get
    End Property

    ''' <summary>
    ''' Legt fest, ob der Player einen Rahmen angezeigen soll.
    ''' </summary>
    <DefaultValue(GetType(System.Boolean), "False")> _
    Public Property ShowBorder() As Boolean
        Get
            Return m_ShowBorder
        End Get
        Set(ByVal value As Boolean)
            m_ShowBorder = value
        End Set
    End Property

    ''' <summary>
    ''' Spielt das angegebene Video ab.
    ''' </summary>
    ''' <param name="URL">Die URL des Videos.</param>
    Public Sub PlayMovie(ByVal URL As String)
        If Not String.IsNullOrEmpty(URL) Then
            Dim sb As New StringBuilder
            sb.Append(URL)
            sb.AppendFormat("&autoplay={0}", IIf(m_AutoPlay, "1", "0"))
            sb.AppendFormat("&color1=0x{0}{1}{2}", String.Format("{0:X2}", m_ColorFrom.R), _
               String.Format("{0:X2}", m_ColorFrom.G), String.Format("{0:X2}", m_ColorFrom.B))
            sb.AppendFormat("&color2=0x{0}{1}{2}", String.Format("{0:X2}", m_ColorTo.R), _
               String.Format("{0:X2}", m_ColorTo.G), String.Format("{0:X2}", m_ColorTo.B))
            sb.AppendFormat("&border={0}", IIf(m_ShowBorder, "1", "0"))

            m_FlashHost.Player.Movie = sb.ToString
        End If
    End Sub
End Class
