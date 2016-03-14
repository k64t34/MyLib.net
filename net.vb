
imports System.DirectoryServices
Module MyNet
Private Const NO_ERROR = 0

'http://www.pinvoke.net/default.aspx/iphlpapi.SendARP
Private Declare Function SendARP Lib "iphlpapi.dll" _
  (ByVal DestIP As UInt32, _
   ByVal SrcIP As UInt32, _
   ByVal pMacAddr As Byte(), _
   ByRef PhyAddrLen As Integer) As Long

	
'**********************************************
 Function GetRemoteMACAddress(Byref RemoteIP As System.Net.IPAddress, _
                                     Byref sRemoteMacAddress As String _
                                     ) As Boolean
GetRemoteMACAddress = False
'10.80.4.220=173016284 http://ip2long.ru/
'http://www.pinvoke.net/default.aspx/iphlpapi.sendarp
'http://www.vbforums.com/showthread.php?746817-Getting-MAC-Address-from-a-MAC-Client-VS2010-Silverlight
Dim mac() As Byte = New Byte(6){}
Dim len As Integer = mac.Length
	Try
		'If 
		SendARP(CType(RemoteIP.Address,UInt32),0, mac, len)
		'	=NO_ERROR Then
    	If len <> 0 Then
    		GetRemoteMACAddress = True
    		'sRemoteMacAddress=BitConverter.ToString(mac,0,len)
    		sRemoteMacAddress=""
    		Dim i As Integer
    		For i =0 To len-1
    			sRemoteMacAddress = sRemoteMacAddress +mac(i).ToString("X2")
    		next	
        End If
        'End If
    Catch     
	End Try
 End Function
 
 Function GetRemoteMACAddress(Byref sRemoteIP As String, _
                                     Byref sRemoteMacAddress As String _
                                     ) As Boolean
 	GetRemoteMACAddress=True
 	Dim RemoteIP As System.Net.IPAddress
 	Try 
 		RemoteIP=Net.IPAddress.Parse(sRemoteIP)
 	Catch	
 		GetRemoteMACAddress=False 		
 	End Try 
 	If GetRemoteMACAddress then
 		GetRemoteMACAddress=GetRemoteMACAddress(RemoteIP,sRemoteMacAddress)
 	End if	
End Function
End module