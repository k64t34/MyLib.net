Imports System
Imports System.Text

Public Class Inifile

#Region "Notes"
'
' Class:       Inifile.vb
' Author:      Ivan Lutrov <ivan@lutrov.com>
' Version:     1.3
' Description: Lightweight infile handling class
' Written:     September 2006.
' Notes:       
'
' THIS LIBRARY IS FREE SOFTWARE; YOU CAN REDISTRIBUTE IT AND/OR
' MODIFY IT UNDER THE TERMS OF THE GNU LESSER GENERAL PUBLIC LICENSE
' AS PUBLISHED BY THE FREE SOFTWARE FOUNDATION; EITHER VERSION 2.1 OF
' THE LICENSE, OR (AT YOUR OPTION) ANY LATER VERSION.
'
' THIS LIBRARY IS DISTRIBUTED IN THE HOPE THAT IT WILL BE USEFUL, BUT
' WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF
' MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE. SEE THE GNU
' LESSER GENERAL PUBLIC LICENSE FOR MORE DETAILS.
'
' YOU SHOULD HAVE RECEIVED A COPY OF THE GNU LESSER GENERAL PUBLIC
' LICENSE ALONG WITH THIS LIBRARY; IF NOT, WRITE TO:
' THE FREE SOFTWARE FOUNDATION, INC,
' 59 TEMPLE PLACE, SUITE 330, BOSTON, MA 02111-1307 USA
'
' 
#End Region

#Region "API declarations"

   Private Declare Ansi Function GetPrivateProfileString Lib "kernel32.dll" Alias "GetPrivateProfileStringA" (ByVal AppName As String, ByVal KeyName As String, ByVal DefVal As String, ByVal RetVal As StringBuilder, ByVal Size As Integer, ByVal FileName As String) As Integer
   Private Declare Ansi Function WritePrivateProfileString Lib "kernel32.dll" Alias "WritePrivateProfileStringA" (ByVal AppName As String, ByVal KeyName As String, ByVal Value As String, ByVal FileName As String) As Integer
   Private Declare Ansi Function GetPrivateProfileInt Lib "kernel32.dll" Alias "GetPrivateProfileIntA" (ByVal AppName As String, ByVal KeyName As String, ByVal DefVal As Integer, ByVal FileName As String) As Integer
   Private Declare Ansi Function FlushPrivateProfileString Lib "kernel32.dll" Alias "WritePrivateProfileStringA" (ByVal AppName As Integer, ByVal KeyName As Integer, ByVal Value As Integer, ByVal FileName As String) As Integer

#End Region

#Region "Constructor"

   Public Sub New(Optional ByVal FileName As String = "")
      If Len(FileName) > 0 Then
         MyFileName = FileName
      Else
      	'MyFileName = Replace(My.Application.ExecutablePath(), ".exe", ".ini")      	
      	MyFileName = Replace(CurDir(), ".exe", ".ini")      	
      	
      	
      End If
   End Sub

#End Region

#Region "Private properties"

   Private MyFileName As String

   ReadOnly Property FileName() As String
      Get
         Return MyFileName
      End Get
   End Property

#End Region

#Region "Public methods"

   Public Function LoadString(ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As String = "") As String
      Try
         Dim Result As New StringBuilder(256), Length As Integer
         Length = Me.GetPrivateProfileString(Section, Key, DefaultValue, Result, Result.Capacity, MyFileName)
         If Length > 0 Then
            Return Left(Result.ToString, Length)
         End If
      Catch Ex As Exception
         Me.ErrorHandler(Ex, "Inifile.LoadString")
      End Try
   End Function

   Public Function LoadInteger(ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As Integer = 0) As Integer
      Try
         Return Me.GetPrivateProfileInt(Section, Key, DefaultValue, MyFileName)
      Catch Ex As Exception
         Me.ErrorHandler(Ex, "Inifile.LoadInteger")
      End Try
   End Function

   Public Function LoadBoolean(ByVal Section As String, ByVal Key As String, Optional ByVal DefaultValue As Boolean = False) As Boolean
      Try
         Return IIf(Me.GetPrivateProfileInt(Section, Key, CInt(DefaultValue), MyFileName) = 1, True, False)
      Catch Ex As Exception
         Me.ErrorHandler(Ex, "Inifile.LoadBoolean")
      End Try
   End Function

   Public Sub SaveString(ByVal Section As String, ByVal Key As String, ByVal Value As String)
      Try
         With Me
            .WritePrivateProfileString(Section, Key, Value, MyFileName)
            .FlushPrivateProfileString(0, 0, 0, MyFileName)
         End With
      Catch Ex As Exception
         Me.ErrorHandler(Ex, "Inifile.SaveString")
      End Try
   End Sub

   Public Sub SaveInteger(ByVal Section As String, ByVal Key As String, ByVal Value As Integer)
      Try
         With Me
            .WritePrivateProfileString(Section, Key, CStr(Value), MyFileName)
            .FlushPrivateProfileString(0, 0, 0, MyFileName)
         End With
      Catch Ex As Exception
         Me.ErrorHandler(Ex, "Inifile.SaveInteger")
      End Try
   End Sub

   Public Sub SaveBoolean(ByVal Section As String, ByVal Key As String, ByVal Value As Boolean)
      Try
         With Me
            .WritePrivateProfileString(Section, Key, CStr(Value), MyFileName)
            .FlushPrivateProfileString(0, 0, 0, MyFileName)
         End With
      Catch Ex As Exception
         Me.ErrorHandler(Ex, "Inifile.SaveBoolean")
      End Try
   End Sub

   Private Sub ErrorHandler(ByVal Ex As Exception, ByVal Where As String)
      'Console.WriteLine("An error has occured in " + Where + "():" + vbCrLf + vbCrLf + Ex.ToString)
   End Sub

#End Region

End Class