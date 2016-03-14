Imports System
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Globalization

'***************************************************
Public Class WOL
'***************************************************	
'http://www.zem.fr/net-classe-wake-on-lan-en-vb-net/	
	Public Shared Sub WakeUp(ByVal MacAddress As String, Optional ByVal BroadCast As String = "255.255.255.255")
        Dim udp As New UdpClient With {.EnableBroadcast = True}
        Try
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1)
            Dim offset As Integer = 0
            Dim buf(512) As Byte

            'les 6 premiers bytes sont &HFF
            For i As Integer = 0 To 5
                buf(offset) = &HFF
                offset += 1
            Next

            'on repete l'addresse MAC 16 fois
            For i As Integer = 0 To 15
                Dim n As Integer = 0
                For j As Integer = 0 To 5
                	'try
                		buf(offset) = Byte.Parse(MacAddress.Substring(n, 2), NumberStyles.HexNumber)
                	'Catch
                		'Exit sub	
                	'End try	
                    offset += 1
                    n += 2
                Next
            Next

            'on envoie le wol
            udp.Send(buf, 512, New IPEndPoint(IPAddress.Parse(BroadCast), &H1))

        Catch ex As Exception
            udp.Close()
            Throw ex
        End Try
    End Sub
End Class
