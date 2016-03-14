'
' Сделано в SharpDevelop.
' Пользователь: skorik
' Дата: 21.05.2013
' Время: 11:22
' 
' Для изменения этого шаблона используйте Сервис | Настройка | Кодирование | Правка стандартных заголовков.
'
Imports System.Runtime.InteropServices
Public Class ConsoleTextColor
  Private hConsoleHandle As IntPtr
      Private ConsoleOutputLocation As COORD
      Private ConsoleInfo As CONSOLE_SCREEN_BUFFER_INFO
      Private OriginalColors As Integer

      Private Const STD_OUTPUT_HANDLE As Integer = &HFFFFFFF5

      Private Declare Auto Function GetStdHandle Lib "kernel32.dll" (ByVal nStdHandle As Integer) As IntPtr
      Private Declare Auto Function GetConsoleScreenBufferInfo Lib "kernel32.dll" (ByVal hConsoleOutput As IntPtr, _
                                                                       ByRef lpConsoleScreenBufferInfo As CONSOLE_SCREEN_BUFFER_INFO) As Integer
      Private Declare Auto Function SetConsoleTextAttribute Lib "kernel32" (ByVal hConsoleOutput As IntPtr, ByVal wAttributes As Integer) As Long

Public Enum Foreground
		 Black =  &H0
         Blue =   &H1
         Green =  &H2
         Cyan =   &H3
         Red =    &H4
         Magenta =&H5
         Yellow = &H6
         White =  &H7            
         Intensity = &H8
      End Enum

      Public Enum Background
         Blue = &H10
         Green = &H20
         Red = &H40
         Intensity = &H80
      End Enum

      <StructLayout(LayoutKind.Sequential)> _
      Private Structure COORD
         Dim X As Short
         Dim Y As Short
      End Structure

      <StructLayout(LayoutKind.Sequential)> _
      Private Structure SMALL_RECT
         Dim Left As Short
         Dim Top As Short
         Dim Right As Short
         Dim Bottom As Short
      End Structure

      <StructLayout(LayoutKind.Sequential)> _
      Private Structure CONSOLE_SCREEN_BUFFER_INFO
         Dim dwSize As COORD
         Dim dwCursorPosition As COORD
         Dim wAttributes As Integer
         Dim srWindow As SMALL_RECT
         Dim dwMaximumWindowSize As COORD
      End Structure

      Sub New()
         hConsoleHandle = GetStdHandle(STD_OUTPUT_HANDLE)
         GetConsoleScreenBufferInfo(hConsoleHandle, ConsoleInfo)
         OriginalColors = ConsoleInfo.wAttributes
      End Sub

      Public Sub TextColor(ByVal Colors As Integer)
         ' Set the text colors.
         SetConsoleTextAttribute(hConsoleHandle, Colors)
      End Sub

      Public Sub ResetColor()
         ' Restore the original colors.
         SetConsoleTextAttribute(hConsoleHandle, OriginalColors)
      End Sub	
End Class
