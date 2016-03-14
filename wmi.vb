Imports System
Imports System.Management
Module Program
Public Dim _RunRemoteProcess_errorflag As Integer
Public Dim _RunRemoteProcess_errorstr As String
'**********************************************
Public Sub RunAsRemoteProcess( _ 
	ByVal Host As String, _
	ByVal Cmd As String, _
	ByVal User As String, _
	ByVal Pass As String)
 	'**************************************************************        
		_RunRemoteProcess_errorflag = 0	
		Dim retValue As String
		Dim WMIConnectionOptions As New System.Management.ConnectionOptions        
        WMIConnectionOptions.Username=User
        WMIConnectionOptions.Password=Pass
        Try
        	
        	Dim WMIpath As New System.Management.ManagementPath("\\" & Host & "\root\cimv2:Win32_Process")
        	Dim WMIscope As New System.Management.ManagementScope(WMIpath, WMIConnectionOptions)
        	WMIscope.Connect()
        	Dim opt As New System.Management.ObjectGetOptions()
	        Dim classInstance As New System.Management.ManagementClass(wmiscope, WMIpath, opt)
	
	        Dim inParams As System.Management.ManagementBaseObject = classInstance.GetMethodParameters("Create")
	        inParams("CommandLine") = Cmd
	        Dim outParams As System.Management.ManagementBaseObject = classInstance.InvokeMethod("Create", inParams, Nothing)
	        _RunRemoteProcess_errorstr=outParams("returnValue").ToString 
	        'Return "ReturnValue:" & outParams("returnValue") & " Process ID: {0}" & outParams("processId")	        
        Catch ex As Exception  
        	_RunRemoteProcess_errorflag = 1
        	_RunRemoteProcess_errorstr= Host & ": Невозможно создать процесс. " & ex.Message            
        End Try
'0 Successful completion 
'2 Access denied 
'3 Insufficient privilege 
'8 Unknown failure 
'9 Path not found 
'21 Invalid parameter
End Sub
Public Sub RunRemoteProcess( _ 
	ByVal Host As String, _
	ByVal Cmd As String, _
	ByVal User As String, _
	ByVal Pass As String)
 	'**************************************************************        
		
		Dim wmi_in As   ManagementBaseObject
		Dim	wmi_out As  ManagementBaseObject
        Dim retValue As Integer
        Dim wmi As ManagementClass        
        _RunRemoteProcess_errorflag = False	
        Try
        	
        	

            wmi_in = wmi.GetMethodParameters("Create")
            ' fill in the command line plus any command-line arguments
            ' NOTE: the command can NOT be on a network resource!
            
            'wmic /user:"deploy2\wol" /password:"A1b2c3d4"  /node:"deploy2" process call create "c:\WINDOWS\system32\ping.exe 8.8.8.8"
            ' У пользователя WOL должны быть явно заданы права на запуск ping.exe
            
            wmi_in("CommandLine") =Cmd
            ' do it!
            wmi_out = wmi.InvokeMethod("Create", wmi_in, Nothing)
            ' get the return code.  This not the return code of the
            ' application... it's a return code for the WMI method
            retValue = Convert.ToInt32(wmi_out("returnValue"))
            Select Case retValue
                Case 0
                    ' success!
                Case 2
                	'Throw New ApplicationException("Access denied")
                	Throw New ApplicationException("Отказано в доступе")
                Case 3
                    Throw New ApplicationException("Insufficient privilege")
                Case 8
                    Throw New ApplicationException("Unknown failure")
                Case 9
                    Throw New ApplicationException("Path not found")
                Case 21
                    Throw New ApplicationException("Invalid parameter")
                Case Else
                    Throw New ApplicationException("Unknown return code " & retValue)
            End Select
        Catch ex As Exception
        	_RunRemoteProcess_errorflag = True
        	_RunRemoteProcess_errorstr= Host & ": Невозможно создать процесс. " & ex.Message
            'MsgBox(Host & ": Can't create the process. " & ex.Message)
        End Try
end sub     

End Module